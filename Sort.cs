using System.Collections;
using SFML.System;

namespace SortVisualizer
{
    public enum DigitState
    {
        None,
        Selected,
        Sorted,
        Pivot,
        Primary,
        Secondary,
        Tertiary
    }

    public struct Digit
    {
        public int Value { get; set; }
        public DigitState State { get; set; }

        public static implicit operator Digit(int value)
        {
            return new Digit { Value = value };
        }
    }

    public class SortArray
    {
        public App App { get; set; }

        public Digit[] Digits { get; }
        public int Min { get; }
        public int Max { get; }

        public SortArray(int min, int max)
        {
            Digits = Enumerable.Range(min + 1, max)
                .Select(x => (Digit)x)
                .ToArray();

            Min = min;
            Max = max;
        }
    }

    public class Sort
    {
        public string Name { get; set; }
        public Func<SortArray, IEnumerator> Function;

        public static readonly Sort[] AvailableSorts =
        {
            new() { Name = "Shuffle", Function = Shuffle },
            new() { Name = "Traverse", Function = Traverse },
            new() { Name = "HeapSort", Function = HeapSort },
            new() { Name = "QuickSort", Function = QuickSort },
            new() { Name = "BubbleSort", Function = BubbleSort },
            new() { Name = "SelectionSort", Function = SelectionSort },
            new() { Name = "InsertionSort", Function = InsertionSort }
        };

        public static IEnumerator BubbleSort(SortArray array)
        {
            Console.Write("[SortVisualizer] Performing BubbleSort... ");
            var clock = new Clock();

            var digits = array.Digits;
            var delay = array.App.SimulationDelay;

            for (int i = 0; i < digits.Length; i++)
            {
                for (int j = 0; j < digits.Length - i - 1; j++)
                {
                    int a = j;
                    int b = j + 1;

                    digits[a].State = DigitState.Selected;
                    digits[b].State = DigitState.Selected;

                    yield return Pause(delay);

                    if (digits[a].Value > digits[b].Value)
                        yield return Swap(array, a, b);

                    yield return Pause(delay);

                    digits[a].State = DigitState.None;
                    digits[b].State = DigitState.None;
                }
            }

            float elapsed = clock.ElapsedTime.AsSeconds();
            Console.WriteLine($"Done! (took {elapsed:F2}s)");
        }

        public static IEnumerator SelectionSort(SortArray array)
        {
            Console.Write("[SortVisualizer] Performing SelectionSort... ");
            var clock = new Clock();

            var digits = array.Digits;
            var delay = array.App.SimulationDelay;

            for (int i = 0; i < digits.Length; i++)
            {
                int m = i;

                for (int j = i + 1; j < digits.Length; j++)
                {
                    digits[i].State = DigitState.Sorted;
                    digits[j].State = DigitState.Selected;

                    yield return Pause(delay);

                    digits[j].State = DigitState.None;

                    if (digits[m].Value > digits[j].Value)
                    {
                        digits[m].State = DigitState.None;
                        digits[j].State = DigitState.Selected;
                        m = j;
                    }

                    yield return MakeSound(array, m);
                    yield return MakeSound(array, j);

                    yield return Pause(delay);
                }

                digits[i].State = DigitState.None;
                digits[m].State = DigitState.None;
                yield return Swap(array, m, i);
            }

            float elapsed = clock.ElapsedTime.AsSeconds();
            Console.WriteLine($"Done! (took {elapsed:F2}s)");
        }

        public static IEnumerator InsertionSort(SortArray array)
        {
            Console.Write("[SortVisualizer] Performing InsertionSort... ");
            var clock = new Clock();

            var digits = array.Digits;
            var delay = array.App.SimulationDelay;

            for (int i = 1; i < digits.Length; i++)
            {
                for (int j = i; j > 0; j--)
                {
                    if (digits[j].Value > digits[j - 1].Value)
                        break;

                    digits[i].State = DigitState.Sorted;
                    digits[j].State = DigitState.Selected;
                    digits[j - 1].State = DigitState.Selected;

                    yield return Pause(delay);
                    yield return Swap(array, j, j - 1);
                    yield return Pause(delay);

                    digits[j].State = DigitState.None;
                    digits[j - 1].State = DigitState.None;
                }

                digits[i].State = DigitState.None;
            }

            float elapsed = clock.ElapsedTime.AsSeconds();
            Console.WriteLine($"Done! (took {elapsed:F2}s)");
        }

        public static IEnumerator QuickSort(SortArray array)
        {
            Console.Write("[SortVisualizer] Performing QuickSort... ");
            var clock = new Clock();

            var digits = array.Digits;
            var delay = array.App.SimulationDelay;

            var stack = new Stack<int>();

            stack.Push(0);
            stack.Push(digits.Length - 1);

            while (stack.Count > 0)
            {
                int r = stack.Pop();
                int l = stack.Pop();

                int p = l;
                int m = digits[p].Value;

                digits[l].State = DigitState.Sorted;
                digits[r].State = DigitState.Sorted;

                for (int i = l; i <= r; i++)
                {
                    digits[i].State = DigitState.Selected;
                    digits[p].State = DigitState.Pivot;

                    yield return Pause(delay);
                    yield return MakeSound(array, i);

                    if (digits[i].Value < m)
                    {
                        digits[p].State = DigitState.None;
                        digits[i].State = DigitState.None;

                        yield return Swap(array, i, ++p);
                    }

                    digits[l].State = DigitState.Sorted;
                    digits[r].State = DigitState.Sorted;

                    yield return Pause(delay);

                    digits[i].State = DigitState.None;
                }

                digits[p].State = DigitState.Pivot;
                yield return Pause(delay);
                yield return Swap(array, l, p);

                digits[l].State = DigitState.Sorted;
                digits[p].State = DigitState.Selected;
                yield return Pause(delay);

                digits[l].State = DigitState.None;
                digits[r].State = DigitState.None;
                digits[p].State = DigitState.None;

                if (p - 1 > l)
                {
                    stack.Push(l);
                    stack.Push(p - 1);
                }

                if (p + 1 < r)
                {
                    stack.Push(p + 1);
                    stack.Push(r);
                }
            }

            float elapsed = clock.ElapsedTime.AsSeconds();
            Console.WriteLine($"Done! (took {elapsed:F2}s)");
        }

        public static IEnumerator HeapSort(SortArray array)
        {
            Console.Write("[SortVisualizer] Performing HeapSort... ");
            var clock = new Clock();

            var digits = array.Digits;
            var delay = array.App.SimulationDelay;

            // Helper functions to calculate node indices
            static int Left(int i) => i * 2 + 1;
            static int Right(int i) => i * 2 + 2;
            static int Parent(int i) => (i - 1) / 2;

            // Choose digit state based on level of depth
            static DigitState Exp(int i) => (DigitState)((int)DigitState.Primary + (int)Math.Log2(i + 1) % 3);

            // Heapify up
            for (int i = 1; i < digits.Length; i++)
            {
                digits[i].State = Exp(i);

                int h = i;
                int child = digits[h].Value;

                while (h > 0 && child > digits[Parent(h)].Value)
                {
                    yield return Pause(delay);
                    yield return Swap(array, h, Parent(h));
                    yield return Pause(delay);

                    digits[h].State = Exp(h);
                    digits[Parent(h)].State = Exp(Parent(h));

                    h = Parent(h);
                }
            }

            // Swap first element with last to remove it
            // then heapify down
            for (int i = digits.Length - 1; i > 0; i--)
            {
                yield return Swap(array, i, 0);
                digits[i].State = DigitState.Sorted;

                int h = 0;
                while (Left(h) < i && digits[Left(h)].Value > digits[h].Value ||
                       Right(h) < i && digits[Right(h)].Value > digits[h].Value)
                {
                    if (Right(h) < i && digits[Left(h)].Value < digits[Right(h)].Value)
                    {
                        yield return Pause(delay);
                        yield return Swap(array, h, Right(h));
                        yield return Pause(delay);

                        h = Right(h);
                    }

                    else
                    {
                        yield return Pause(delay);
                        yield return Swap(array, h, Left(h));
                        yield return Pause(delay);

                        h = Left(h);
                    }
                }

                digits[i].State = DigitState.None;
            }

            float elapsed = clock.ElapsedTime.AsSeconds();
            Console.WriteLine($"Done! (took {elapsed:F2}s)");
        }

        public static IEnumerator Shuffle(SortArray array)
        {
            Console.Write("[SortVisualizer] Shuffling... ");
            var clock = new Clock();

            var random = new Random();
            var digits = array.Digits;
            var delay = array.App.SimulationDelay;

            for (int i = 0; i < digits.Length; i++)
            {
                int j = random.Next(0, digits.Length);

                digits[i].State = DigitState.Selected;
                digits[j].State = DigitState.Selected;

                yield return Pause(delay);

                yield return Swap(array, i, j);
                yield return Pause(delay);

                digits[i].State = DigitState.None;
                digits[j].State = DigitState.None;

                yield return Pause(delay);
            }

            float elapsed = clock.ElapsedTime.AsSeconds();
            Console.WriteLine($"Done! (took {elapsed:F2}s)");
        }

        public static IEnumerator Traverse(SortArray array)
        {
            Console.Write("[SortVisualizer] Traversing... ");
            var clock = new Clock();

            var digits = array.Digits;
            var delay = array.App.SimulationDelay;

            for (int i = 0; i < digits.Length; i++)
                digits[i].State = DigitState.None;

            yield return Pause(delay);

            for (int i = 0; i < digits.Length; i++)
            {
                digits[i].State = DigitState.Sorted;
                yield return MakeSound(array, i);
                yield return Pause(delay);
            }

            yield return Pause(delay);

            for (int i = 0; i < digits.Length; i++)
                digits[i].State = DigitState.None;

            float elapsed = clock.ElapsedTime.AsSeconds();
            Console.WriteLine($"Done! (took {elapsed:F2}s)");
        }

        public static IEnumerator Pause(float seconds)
        {
            var clock = new Clock();

            while (clock.ElapsedTime.AsSeconds() < seconds)
                yield return null;
        }

        private static Note MakeSound(SortArray array, int i)
        {
            var digits = array.Digits;

            float min = array.App.SortArray.Min;
            float max = array.App.SortArray.Max;

            float n = (digits[i].Value - min) / max;
            float d = array.App.SimulationDelay;
            float s = array.App.Audio.SoundSustain;

            return new Note(d * s, 120 + 1200 * n * n);
        }

        private static IEnumerator Swap(SortArray array, int i, int j)
        {
            var digits = array.Digits;

            (digits[i], digits[j]) = (digits[j], digits[i]);

            yield return MakeSound(array, i);
            yield return MakeSound(array, j);
        }
    }
}
