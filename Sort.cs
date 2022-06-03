using System.Collections;
using SFML.System;

namespace SortVisualizer
{
    public enum DigitState
    {
        None,
        Selected,
        Sorted,
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
            new() { Name = "BubbleSort", Function = BubbleSort },
            new() { Name = "SelectionSort", Function = SelectionSort }
        };

        public static IEnumerator BubbleSort(SortArray array)
        {
            Console.Write("[SortVisualizer] Performing BubbleSort... ");

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

                    digits[a].State = DigitState.None;
                    digits[b].State = DigitState.Sorted;

                    yield return Pause(delay);
                }
            }

            Console.WriteLine("Done!");
        }

        public static IEnumerator SelectionSort(SortArray array)
        {
            Console.Write("[SortVisualizer] Performing SelectionSort... ");

            var digits = array.Digits;
            var delay = array.App.SimulationDelay;

            for (int i = 0; i < digits.Length; i++)
            {
                int m = i;

                for (int j = i + 1; j < digits.Length; j++)
                {
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

                yield return Swap(array, m, i);
                digits[i].State = DigitState.Sorted;
            }

            Console.WriteLine("Done!");
        }

        public static IEnumerator Shuffle(SortArray array)
        {
            Console.Write("[SortVisualizer] Shuffling... ");

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
                yield return Pause(delay * 20.0f);

                digits[i].State = DigitState.None;
                digits[j].State = DigitState.None;

                yield return Pause(delay);
            }

            Console.WriteLine("Done!");
        }

        public static IEnumerator Traverse(SortArray array)
        {
            Console.Write("[SortVisualizer] Traversing... ");

            var digits = array.Digits;
            var delay = array.App.SimulationDelay;

            for (int i = 0; i < digits.Length; i++)
                digits[i].State = DigitState.None;

            yield return Pause(delay);

            for (int i = 0; i < digits.Length; i++)
            {
                digits[i].State = DigitState.Sorted;
                yield return MakeSound(array, i);
                yield return Pause(delay * 20.0f);
            }

            yield return Pause(delay);

            for (int i = 0; i < digits.Length; i++)
                digits[i].State = DigitState.None;

            Console.WriteLine("Done!");
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
