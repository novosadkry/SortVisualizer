using System.Collections;
using SFML.System;
using SFML.Window;

namespace SortVisualizer
{
    public class App
    {
        private static readonly Lazy<App> Lazy 
            = new(() => new App());

        public static App Instance => Lazy.Value;

        public bool EnableSound { get; set; }
        public float SimulationDelay { get; set; } 

        private readonly Canvas _canvas;
        private readonly LinkedList<IEnumerator> _queue;

        private App()
        {
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
            window.Tick += Draw;

            window.Run();
        }

        private void Init()
        {
            _canvas.Size = new Vector2i(800, 600);

            _canvas.Digits = Enumerable.Range(1, 100)
                .Select(x => (Digit)x)
                .ToArray();

            _canvas.MinValue = 0;
            _canvas.MaxValue = 100;

            _queue.AddLast(Sort.Shuffle(_canvas.Digits));
            _queue.AddLast(Sort.BubbleSort(_canvas.Digits));
            _queue.AddLast(Sort.Traverse(_canvas.Digits));

            Task.Run(HandleInput);
            Task.Run(SortEntry);
        }

        private void Draw(Window window)
        {
            window.Draw(_canvas);
        }

        private void SortEntry()
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
                }
            }
        }

        private async Task HandleInput()
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

                    lock (_queue)
                    {
                        _queue.AddLast(sort.Function(_canvas.Digits));
                    }
                }
            }
        }
    }
}
