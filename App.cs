using System.Collections;
using SFML.System;
using SFML.Window;

namespace SortVisualizer
{
    public class App
    {
        public bool EnableSound { get; set; }
        public float SimulationDelay { get; set; } 

        private readonly Clock _clock = new();
        private readonly Canvas _canvas = new();
        private readonly LinkedList<IEnumerator> _queue = new();

        private float _delay;

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
            Oscillator.Init();

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

                else if (EnableSound && iterator.Current is int i)
                    Oscillator.Play((float)i / _canvas.Digits.Length);

                else
                    _delay = SimulationDelay;
            }

            window.Draw(_canvas);
        }
    }
}
