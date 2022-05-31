using SFML.Audio;
using SFML.System;

namespace SortVisualizer
{
    public class Emitter : SoundStream
    {
        private const int BufferSize = 4096;
        private const int MaxOscillators = 128;

        public Emitter()
        {
            Initialize(1, 44100);
            Oscillators = new List<Oscillator>(MaxOscillators);
        }

        private List<Oscillator> Oscillators { get; }
        private readonly short[] _samples = new short[BufferSize];

        protected override bool OnGetData(out short[] samples)
        {
            Array.Clear(_samples, 0, _samples.Length);

            lock (Oscillators)
            {
                double step = 1.0 / SampleRate;
                double amp = 1.0 / Oscillators.Count;

                foreach (var osc in Oscillators)
                {
                    if (osc.Ended)
                        continue;

                    var env = osc.Note.Envelope;

                    double x = osc.TimeSpent;
                    double d = osc.Note.Duration;

                    for (int i = 0; i < _samples.Length; i++)
                    {
                        _samples[i] += (short)(osc.Wave(x) * env.ADSR(x / d) * amp);
                        x += step;
                    }

                    osc.TimeSpent = x;
                }
            }

            samples = _samples;
            return true;
        }

        public void AddNote(Note note)
        {
            lock (Oscillators)
            {
                if (Oscillators.Count < MaxOscillators)
                {
                    Oscillators.Add(new Oscillator(note));
                    return;
                }

                var oldest = Oscillators
                    .MaxBy(x => x.TimeSpent);

                if (oldest == null)
                    return;

                oldest.Note = note;
                oldest.TimeSpent = 0.0;
            }
        }

        protected override void OnSeek(Time timeOffset)
        {
            throw new NotImplementedException();
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

            Amplitude = 6000;
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
