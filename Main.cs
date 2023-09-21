namespace SortVisualizer
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var app = new App
            {
                Audio = {
                    Enabled = true,
                    MaxEmitters = 128,
                    SoundSustain = 20.0f
                },
                SimulationDelay = 0.001f,
                SortArray = new SortArray(0, 100)
            };

            try
            {
                foreach (var command in args)
                    app.ParseCommand(command);
            }
            catch (Exception e)
            {
                Console.WriteLine("[SortVisualizer] " + e.Message);
            }

            app.Run();
        }
    }
}