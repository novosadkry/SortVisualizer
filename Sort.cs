﻿using System.Collections;
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

            var delay = App.Instance.SimulationDelay;

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
                        yield return Swap(digits, a, b);

                    digits[a].State = DigitState.None;
                    digits[b].State = DigitState.Sorted;

                    yield return Pause(delay);
                }
            }

            Console.WriteLine("Done!");
        }

        public static IEnumerator SelectionSort(Digit[] digits)
        {
            Console.Write("[SortVisualizer] Performing SelectionSort... ");

            var delay = App.Instance.SimulationDelay;

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

                    yield return MakeSound(digits, m);
                    yield return MakeSound(digits, j);

                    yield return Pause(delay);
                }

                yield return Swap(digits, m, i);
                digits[i].State = DigitState.Sorted;
            }

            Console.WriteLine("Done!");
        }

        public static IEnumerator Shuffle(Digit[] digits)
        {
            Console.Write("[SortVisualizer] Shuffling... ");

            var random = new Random();
            var delay = App.Instance.SimulationDelay;

            for (int i = 0; i < digits.Length; i++)
            {
                int j = random.Next(0, digits.Length);

                digits[i].State = DigitState.Selected;
                digits[j].State = DigitState.Selected;

                yield return Pause(delay);

                yield return Swap(digits, i, j);
                yield return Pause(delay);

                digits[i].State = DigitState.None;
                digits[j].State = DigitState.None;

                yield return Pause(delay);
            }

            Console.WriteLine("Done!");
        }

        public static IEnumerator Traverse(Digit[] digits)
        {
            Console.Write("[SortVisualizer] Traversing... ");

            var delay = App.Instance.SimulationDelay;

            for (int i = 0; i < digits.Length; i++)
                digits[i].State = DigitState.None;

            yield return Pause(delay);

            for (int i = 0; i < digits.Length; i++)
            {
                digits[i].State = DigitState.Sorted;
                yield return Pause(delay);
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

        private static Note MakeSound(Digit[] digits, int i)
        {
            float min = App.Instance.Canvas.MinValue;
            float max = App.Instance.Canvas.MaxValue;

            float n = (digits[i].Value - min) / max;
            float d = App.Instance.SimulationDelay;
            float s = App.Instance.SoundSustain;

            return new Note(d * s, 120 + 1200 * n * n);
        }

        private static IEnumerator Swap(Digit[] digits, int i, int j)
        {
            (digits[i], digits[j]) = (digits[j], digits[i]);

            yield return MakeSound(digits, i);
            yield return MakeSound(digits, j);
        }
    }
}
