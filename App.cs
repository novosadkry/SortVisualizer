﻿using System.Collections;
using SFML.System;
using SFML.Window;

namespace SortVisualizer
{
    public class App
    {
        public Mixer Audio { get; }
        public Canvas Canvas { get; }
        public SortArray SortArray { get; set; }
        public float SimulationDelay { get; set; }

        private readonly LinkedList<IEnumerator> _queue;

        public App()
        {
            Audio = new Mixer();
            Canvas = new Canvas();
            SortArray = new SortArray(1, 100);
            _queue = new LinkedList<IEnumerator>();
        }

        public void Run()
        {
            var window = new Window(
                "SortVisualizer",
                new VideoMode(800, 600)
            );

            Init();

            window.Init();
            window.Tick += Draw;

            window.Run();
        }

        private void Init()
        {
            Audio.Init();

            Canvas.App = this;
            Canvas.Size = new Vector2i(800, 600);

            SortArray.App = this;

            Task.Run(HandleInput);
            Task.Run(SortEntry);
        }

        private void Draw(Window window)
        {
            window.Draw(Canvas);
        }

        private Task SortEntry()
        {
            while (true)
            {
                lock (_queue)
                {
                    if (_queue.First == null) continue;
                    var iterator = _queue.First.Value;

                    if (!iterator.MoveNext())
                        _queue.RemoveFirst();

                    else if (iterator.Current is IEnumerator e)
                        _queue.AddFirst(e);

                    else if (iterator.Current is Note n)
                        Audio.Play(n);
                }
            }
        }

        private async Task HandleInput()
        {
            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;

                lock (_queue)
                {
                    if (_queue.Count > 0)
                        Console.WriteLine("Canceled!");

                    _queue.Clear();
                }
            };

            while (true)
            {
                string? input = await Console.In.ReadLineAsync();

                if (string.IsNullOrWhiteSpace(input))
                    continue;

                string[] commands = input.Split(' ');

                foreach (string command in commands)
                    ParseCommand(command);
            }
        }

        public void ParseCommand(string cmd)
        {
            IEnumerator? action = null;

            if (cmd.StartsWith("delay"))
            {
                var split = cmd.Split(":");
                if (split.Length < 2) return;

                if (float.TryParse(split[1], out float v))
                    action = SetSimulationDelay(v);
            }

            else if (cmd.StartsWith("pause"))
            {
                var split = cmd.Split(":");
                if (split.Length < 2) return;

                if (float.TryParse(split[1], out float v))
                    action = Sort.Pause(v);
            }

            else if (cmd.StartsWith("sustain"))
            {
                var split = cmd.Split(":");
                if (split.Length < 2) return;

                if (float.TryParse(split[1], out float v))
                    action = SetSoundSustain(v);
            }

            else
            {
                // Check if command corresponds to an available sort

                var sort = Sort.AvailableSorts
                    .FirstOrDefault(x => x.Name.ToLower() == cmd);

                if (sort != null)
                    action = sort.Function(SortArray);
            }

            if (action == null) 
                return;

            lock (_queue) { _queue.AddLast(action); }
        }

        private IEnumerator SetSimulationDelay(float seconds)
        {
            SimulationDelay = seconds;
            Console.WriteLine($"[SortVisualizer] Simulation delay set to {seconds}s");

            yield return null;
        }

        private IEnumerator SetSoundSustain(float value)
        {
            Audio.SoundSustain = value;
            Console.WriteLine($"[SortVisualizer] Sound sustain set to {value}x");

            yield return null;
        }
    }
}
