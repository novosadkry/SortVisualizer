using SFML.Audio;

namespace SortVisualizer
{
    public static class Oscillator
    {
        private static Sound _instance;

        public static void Init()
        {
            short[] samples = new short[44100];

            double x = 0;
            for (int i = 0; i < samples.Length; i++)
            {
                samples[i] = (short)(10000 * WaveSquare(x));
                x += 1.0 / 44100;
            }

            var buffer = new SoundBuffer(samples, 1, 44100);
            _instance = new Sound(buffer);
        }

        private static double WaveSin(double x)
        {
            return Math.Sin(440.0 * 2 * Math.PI * x);
        }

        private static double WaveSquare(double x)
        {
            return WaveSin(x) > 0.0 ? 1.0 : 0.0;
        }

        public static void Play(float pitch)
        {
            _instance.Stop();
            _instance.Pitch = pitch;
            _instance.Play();
        }

        public static void Stop()
        {
            _instance.Stop();
        }
    }
}
