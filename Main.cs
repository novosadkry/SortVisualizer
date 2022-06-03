﻿namespace SortVisualizer
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();
            app.SimulationDelay = 0.1f;
            app.SoundSustain = 1.0f;
            app.EnableSound = true;

            app.Run();
        }
    }
}