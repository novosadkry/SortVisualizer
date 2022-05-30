﻿using System.Collections;
using SFML.System;
using SFML.Window;

namespace SortVisualizer
{
    public class App
    {
        public bool EnableSound { get; set; }
        public float SimulationDelay { get; set; } 

        private readonly Clock _clock;
        private readonly Canvas _canvas;
        private readonly LinkedList<IEnumerator> _queue;

        private float _delay;

        public App()
        {
            _clock = new Clock();
            _canvas = new Canvas();
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
            window.Tick += Tick;

            window.Run();
        }

        private void Init()
        {
            HandleInput();

            _canvas.Size = new Vector2i(800, 600);

            _canvas.Digits = Enumerable.Range(1, 100)
                .Select(x => (Digit)x)
                .ToArray();

            _canvas.MinValue = 0;
            _canvas.MaxValue = 100;

            _queue.AddLast(Sort.Shuffle(_canvas.Digits));
            _queue.AddLast(Sort.BubbleSort(_canvas.Digits));
            _queue.AddLast(Sort.Traverse(_canvas.Digits));
        }

        private void Tick(Window window)
        {
            _delay -= _clock.Restart().AsSeconds();

            if (_delay <= 0 && _queue.First != null)
            {
                var iterator = _queue.First.Value;

                if (!iterator.MoveNext())
                    _queue.RemoveFirst();

                else if (iterator.Current is IEnumerator e)
                    _queue.AddFirst(e);

                else
                    _delay = SimulationDelay;
            }

            window.Draw(_canvas);
        }

        private void HandleInput()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    string? input = await Console.In.ReadLineAsync();

                    if (string.IsNullOrWhiteSpace(input))
                        continue;

                    string[] commands = input.Split(' ');
                    foreach (string command in commands)
                    {
                        var sort = Sort.AvailableSorts
                            .FirstOrDefault(x => x.Name.ToLower() == command);

                        if (sort == null) 
                            continue;

                        _queue.AddLast(sort.Function(_canvas.Digits));
                    }
                }
            });
        }
    }
}
