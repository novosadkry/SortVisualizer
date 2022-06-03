namespace SortVisualizer
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var app = new App
            {
                SimulationDelay = 0.001f,
                Audio = {
                    Enabled = true,
                    MaxEmitters = 128,
                    SoundSustain = 20.0f
                }
            };

            app.Run();
        }
    }
}