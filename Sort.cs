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

    public class Sort
    {
        public string Name { get; set; }
        public Func<Digit[], IEnumerator> Function;

        public static readonly Sort[] AvailableSorts =
        {
            new() { Name = "Shuffle", Function = Shuffle },
            new() { Name = "Traverse", Function = Traverse },
            new() { Name = "BubbleSort", Function = BubbleSort },
            new() { Name = "SelectionSort", Function = SelectionSort }
        };

        public static IEnumerator BubbleSort(Digit[] digits)
        {
            Console.Write("[SortVisualizer] Performing BubbleSort... ");

            for (int i = 0; i < digits.Length; i++)
            {
                for (int j = 0; j < digits.Length - i - 1; j++)
                {
                    int a = j;
                    int b = j + 1;

                    digits[a].State = DigitState.Selected;
                    digits[b].State = DigitState.Selected;

                    yield return j;

                    if (digits[a].Value > digits[b].Value)
                        Swap(digits, a, b);

                    digits[a].State = DigitState.None;
                    digits[b].State = DigitState.Sorted;

                    yield return j;
                }
            }

            Console.WriteLine("Done!");
        }

        public static IEnumerator SelectionSort(Digit[] digits)
        {
            Console.Write("[SortVisualizer] Performing SelectionSort... ");

            for (int i = 0; i < digits.Length; i++)
            {
                int m = i;

                for (int j = i + 1; j < digits.Length; j++)
                {
                    digits[j].State = DigitState.Selected;

                    yield return j;

                    digits[j].State = DigitState.None;

                    if (digits[m].Value > digits[j].Value)
                    {
                        digits[m].State = DigitState.None;
                        digits[j].State = DigitState.Selected;
                        m = j;
                    }

                    yield return j;
                }

                Swap(digits, m, i);
                digits[i].State = DigitState.Sorted;
            }

            Console.WriteLine("Done!");
        }

        public static IEnumerator Shuffle(Digit[] digits)
        {
            Console.Write("[SortVisualizer] Shuffling... ");

            var random = new Random();

            for (int i = 0; i < digits.Length; i++)
            {
                int j = random.Next(0, digits.Length);

                digits[i].State = DigitState.Selected;
                digits[j].State = DigitState.Selected;

                yield return j;

                Swap(digits, i, j);
                yield return Pause(.01f);

                digits[i].State = DigitState.None;
                digits[j].State = DigitState.None;

                yield return j;
            }

            Console.WriteLine("Done!");
        }

        public static IEnumerator Traverse(Digit[] digits)
        {
            Console.Write("[SortVisualizer] Traversing... ");

            for (int i = 0; i < digits.Length; i++)
                digits[i].State = DigitState.None;

            yield return null;

            for (int i = 0; i < digits.Length; i++)
            {
                digits[i].State = DigitState.Sorted;
                yield return Pause(.01f);
                yield return i;
            }

            yield return null;

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

        private static void Swap(Digit[] digits, int i, int j)
        {
            ref var a = ref digits[i];
            ref var b = ref digits[j];

            (a, b) = (b, a);
        }
    }
}
