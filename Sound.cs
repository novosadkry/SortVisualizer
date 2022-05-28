using SFML.Audio;

namespace SortVisualizer
{
    public static class SoundFactory
    {
        private static SoundBuffer? _buffer;

        public static void Init()
        {
            short[] samples = new short[44100];

            double x = 0;
            for (int i = 0; i < samples.Length; i++)
            {
                samples[i] = (short)(10000 * Math.Sin(440.0 * 2 * Math.PI * x));
                x += 1.0 / 44100;
            }

            _buffer = new SoundBuffer(samples, 1, 44100);
        }

        public static Sound Get(float pitch)
        {
            if (_buffer == null) Init();
            return new Sound(_buffer) { Pitch = pitch };
        }
    }
}
