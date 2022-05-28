using SFML.Graphics;
using SFML.System;

namespace SortVisualizer
{
    public class Canvas : Drawable
    {
        public Vector2i Size { get; set; }
        public Digit[] Digits { get; set; }

        public int MaxValue { get; set; }
        public int MinValue { get; set; }

        public void Draw(RenderTarget target, RenderStates states)
        {
            for (int i = 0; i < Digits.Length; i++)
            {
                var digit = Digits[i];

                float value = digit.Value;
                var state = digit.State;

                Color color = state switch
                {
                    DigitState.Selected => Color.Red,
                    DigitState.Sorted => Color.Green,
                    _ => Color.White
                };

                var pos = new Vector2f
                {
                    X = Size.X * ((float) i / Digits.Length),
                    Y = Size.Y
                };

                var size = new Vector2f
                {
                    X =  Size.X * (1.0f / Digits.Length) - 0.5f,
                    Y = -Size.Y * (value - MinValue) / MaxValue
                };

                var column = new RectangleShape
                {
                    Position = pos,
                    Size = size,
                    FillColor = color
                };

                target.Draw(column);
            }
        }
    }
}
