namespace SortVisualizer
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var app = App.Instance;
            app.SimulationDelay = 0.1f;

            app.Run();
        }
    }
}