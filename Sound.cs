using SFML.Audio;
using SFML.System;

namespace SortVisualizer
{
    public class Emitter : SoundStream
    {
        private const int BufferSize = 4096;

        public Emitter()
        {
            Notes = new List<Note>();
            Initialize(1, 44100);
        }

        private List<Note> Notes { get; }

        protected override bool OnGetData(out short[] samples)
        {
            samples = new short[BufferSize];

            lock (Notes)
            {
                foreach (var note in Notes)
                {
                    var env = note.Envelope;
                    var osc = note.Oscillator;

                    double x = note.TimeSpent;
                    double d = note.Duration;

                    for (int i = 0; i < samples.Length; i++)
                    {
                        samples[i] += (short)(osc.Wave(x) * env.ADSR(x / d));
                        x += 1.0 / SampleRate;
                    }

                    note.TimeSpent = x;
                }

                Notes.RemoveAll(x => x.Ended);
            }

            return true;
        }

        public void AddNote(Note note)
        {
            lock (Notes) Notes.Add(note);
        }

        protected override void OnSeek(Time timeOffset)
        {
            throw new NotImplementedException();
        }
    }

    public class Note
    {
        public Oscillator Oscillator { get; set; }
        public Envelope   Envelope   { get; set; }

        public double Duration  { get; set; }
        public double TimeSpent { get; set; }

        public bool Ended => TimeSpent > Duration;

        public Note(double duration, double frequency)
        {
            Oscillator = new Oscillator();
            Envelope = new Envelope();

            Duration = duration;
            TimeSpent = 0.0;

            Oscillator.Frequency = frequency;
        }
    }

    public class Oscillator
    {
        public double Amplitude { get; set; }
        public double Frequency { get; set; }

        public Oscillator()
        {
            Amplitude = 6000;
            Frequency = 440;
        }

        public double Wave(double x)
        {
            return WaveSin(x * Frequency) * Amplitude;
        }

        private static double WaveSin(double x)
        {
            return Math.Sin(2 * Math.PI * x);
        }

        private static double WaveSquare(double x)
        {
            return WaveSin(x) > 0.0 ? 1.0 : -1.0;
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
