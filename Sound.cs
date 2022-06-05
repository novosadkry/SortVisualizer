using SFML.Audio;
using SFML.System;

namespace SortVisualizer
{
    public class Mixer
    {
        public bool Enabled { get; set; }
        public int MaxEmitters { get; set; }
        public float SoundSustain { get; set; }

        private readonly List<Emitter> _emitters;

        public Mixer()
        {
            Enabled = true;
            MaxEmitters = 128;
            SoundSustain = 2f;

            _emitters = new List<Emitter>(MaxEmitters);
        }

        public void Init()
        {
            for (int i = 0; i < MaxEmitters; i++)
            {
                _emitters.Add(new Emitter(
                    new Oscillator(new Note(0, 0))
                ));
            }

            _emitters.ForEach(x => x.Play());
        }

        public void Play(Note note)
        {
            if (!Enabled)
                return;

            var oldest = _emitters
                .OrderByDescending(x => x.Oscillator.Ended)
                .ThenByDescending(x => x.Oscillator.TimeSpent)
                .FirstOrDefault();

            if (oldest == null) return;
            oldest.Oscillator = new Oscillator(note);
        }
    }

    public class Emitter : SoundStream
    {
        public const int BufferSize = 4096;
        public Oscillator Oscillator { get; set; }

        public Emitter(Oscillator oscillator)
        {
            Initialize(1, 44100);
            Oscillator = oscillator;
        }

        private readonly short[] _samples = new short[BufferSize];

        protected override bool OnGetData(out short[] samples)
        {
            Array.Clear(_samples, 0, _samples.Length);
            samples = _samples;

            var osc = Oscillator;

            if (osc.Ended) return true;
            var env = osc.Note.Envelope;

            double x = osc.TimeSpent;
            double d = osc.Note.Duration;
            double step = 1.0 / SampleRate;

            for (int i = 0; i < _samples.Length; i++)
            {
                _samples[i] += (short)(osc.Wave(x) * env.ADSR(x / d));
                x += step;
            }

            osc.TimeSpent = x;
            return true;
        }

        protected override void OnSeek(Time timeOffset)
        {
            Oscillator.TimeSpent = timeOffset.AsSeconds();
        }
    }

    public class Note
    {
        public Envelope Envelope { get; set; }

        public double Duration { get; set; }
        public double Amplitude { get; set; }
        public double Frequency { get; set; }

        public Note(double duration, double frequency)
        {
            Envelope = new Envelope();

            Amplitude = 3000;
            Duration = duration;
            Frequency = frequency;
        }
    }

    public class Oscillator
    {
        public Note Note { get; set; }

        public double TimeSpent { get; set; }
        public bool Ended => TimeSpent > Note.Duration;

        public Oscillator(Note note)
        {
            Note = note;
            TimeSpent = 0.0;
        }

        public double Wave(double x)
        {
            return WaveTriangle(x * Note.Frequency) * Note.Amplitude;
        }

        private static double WaveSin(double x)
        {
            return Math.Sin(2 * Math.PI * x);
        }

        private static double WaveSquare(double x)
        {
            return WaveSin(x) > 0.0 ? 1.0 : -1.0;
        }

        private static double WaveTriangle(double x)
        {
            x %= 1.0;

            if (x <= 0.25) return 4.0 * x;
            if (x <= 0.75) return 2.0 - 4.0 * x;
            return 4.0 * x - 4.0;
        }
    }

    public class Envelope
    {
        /* Percentage of each segment */
        public double Attack  { get; set; }
        public double Decay   { get; set; }
        public double Release { get; set; }
        public double Sustain { get; set; }

        public Envelope()
        {
            Attack  = 0.1;
            Decay   = 0.1;
            Release = 0.3;
            Sustain = 0.9;
        }

        public double ADSR(double x)
        {
            if (x > 1.0)
                x = 1.0;

            if (x < Attack)
                return 1.0 / Attack * x;

            if (x < Attack + Decay)
                return 1.0 - (x - Attack) / Decay * (1.0 - Sustain);

            if (x < 1.0 - Release)
                return Sustain;

            return Sustain * (1.0 - x) / Release;
        }
    }
}
