using System.Collections;

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

    public static class Sort
    {
        public static IEnumerator BubbleSort(Digit[] digits)
        {
            Console.WriteLine("[SortVisualizer] Performing BubbleSort...");

            for (int i = 0; i < digits.Length; i++)
            {
                for (int j = 0; j < digits.Length - i - 1; j++)
                {
                    int a = j;
                    int b = j + 1;

                    digits[a].State = DigitState.Selected;
                    digits[b].State = DigitState.Selected;

                    yield return null;

                    if (digits[a].Value > digits[b].Value)
                        Swap(digits, a, b);

                    digits[a].State = DigitState.None;
                    digits[b].State = DigitState.Sorted;

                    yield return null;
                }
            }

            Console.WriteLine("[SortVisualizer] Done!");
        }

        public static IEnumerator Shuffle(Digit[] digits)
        {
            Console.WriteLine("[SortVisualizer] Shuffle...");

            var random = new Random();
            
            for (int i = 0; i < digits.Length; i++)
            {
                int j = random.Next(0, digits.Length);

                digits[i].State = DigitState.Selected;
                digits[j].State = DigitState.Selected;

                yield return null;

                Swap(digits, i, j);

                digits[i].State = DigitState.None;
                digits[j].State = DigitState.None;

                yield return null;
            }

            Console.WriteLine("[SortVisualizer] Done!");
        }

        public static IEnumerator Traverse(Digit[] digits)
        {
            Console.WriteLine("[SortVisualizer] Traverse...");

            for (int i = 0; i < digits.Length; i++)
                digits[i].State = DigitState.None;
            
            yield return null;

            for (int i = 0; i < digits.Length; i++)
            {
                digits[i].State = DigitState.Sorted;
                yield return Pause(5);
            }

            yield return null;
         
            for (int i = 0; i < digits.Length; i++)
                digits[i].State = DigitState.None;

            Console.WriteLine("[SortVisualizer] Done!");
        }

        public static IEnumerator Pause(int ticks)
        {
            for (int i = 0; i < ticks; i++)
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
