using System.Collections;
using SFML.Graphics;
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

            SortArray.App = this;
            Canvas.App = this;
            Canvas.Size = window.Size;

            Audio.Init();

            using var cts = new CancellationTokenSource();
            ThreadPool.QueueUserWorkItem(Thread_HandleSort, cts.Token, false);
            ThreadPool.QueueUserWorkItem(Thread_HandleInput, cts.Token, false);
            
            window.Init();
            window.Tick += Draw;
            window.Resized += Resize;

            window.Run();
            cts.Cancel();
        }

        private void Draw(Window window)
        {
            window.Draw(Canvas);
        }

        private void Thread_HandleSort(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
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

        private void Thread_HandleInput(CancellationToken token)
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

            while (!token.IsCancellationRequested)
            {
                string? input = Console.In.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                    continue;

                string[] commands = input.Split(' ');

                try
                {
                    foreach (var command in commands)
                        ParseCommand(command);
                }
                catch (Exception e)
                {
                    Console.WriteLine("[SortVisualizer] " + e.Message);
                }
            }
        }

        public void ParseCommand(string cmd)
        {
            IEnumerator? action = null;

            if (cmd.StartsWith("delay"))
            {
                var split = cmd.Split(":");
                if (split.Length < 2) return;

                if (!float.TryParse(split[1], out float v))
                    throw new FormatException("Invalid command arguments");
                
                action = SetSimulationDelay(v);
            }

            else if (cmd.StartsWith("pause"))
            {
                var split = cmd.Split(":");
                if (split.Length < 2) return;

                if (!float.TryParse(split[1], out float v))
                    throw new FormatException("Invalid command arguments");

                action = Sort.Pause(v);
            }

            else if (cmd.StartsWith("sustain"))
            {
                var split = cmd.Split(":");
                if (split.Length < 2) return;

                if (!float.TryParse(split[1], out float v))
                    throw new FormatException("Invalid command arguments");
                
                action = SetSoundSustain(v);
            }

            else if (cmd.StartsWith("size"))
            {
                var split = cmd.Split(":");
                if (split.Length < 2) return;

                if (!int.TryParse(split[1], out int v))
                    throw new FormatException("Invalid command arguments");
                
                action = SetArraySize(v);
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
                throw new ArgumentException("Invalid command", nameof(cmd));

            lock (_queue) { _queue.AddLast(action); }
        }

        private void Resize(object? sender, SizeEventArgs e)
        {
            var window = sender as Window;
            window?.SetView(new View(new FloatRect(0, 0, e.Width, e.Height)));

            Canvas.Size = new Vector2u(e.Width, e.Height);
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

        private IEnumerator SetArraySize(int size)
        {
            SortArray = new SortArray(0, size) { App = this };
            Console.WriteLine($"[SortVisualizer] Array size set to {size}");

            yield return null;
        }
    }
}
