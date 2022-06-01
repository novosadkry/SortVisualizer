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

        public Canvas Canvas { get; }
        public bool EnableSound { get; set; }
        public float SimulationDelay { get; set; } 
        public float SoundSustain { get; set; }

        private readonly Emitter _emitter;
        private readonly LinkedList<IEnumerator> _queue;

        private App()
        {
            Canvas = new Canvas();
            _emitter = new Emitter();
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
            if (EnableSound)
                _emitter.Play();

            Canvas.Size = new Vector2i(800, 600);

            Canvas.Digits = Enumerable.Range(1, 100)
                .Select(x => (Digit)x)
                .ToArray();

            Canvas.MinValue = 0;
            Canvas.MaxValue = 100;

            _queue.AddLast(Sort.Shuffle(Canvas.Digits));
            _queue.AddLast(Sort.BubbleSort(Canvas.Digits));
            _queue.AddLast(Sort.Traverse(Canvas.Digits));

            Task.Run(HandleInput);
            Task.Run(SortEntry);
        }

        private void Draw(Window window)
        {
            window.Draw(Canvas);
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

                    else if (iterator.Current is Note n)
                        _emitter.AddNote(n);
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
                        _queue.AddLast(sort.Function(Canvas.Digits));
                    }
                }
            }
        }
    }
}
