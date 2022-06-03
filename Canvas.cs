using SFML.Graphics;
using SFML.System;

namespace SortVisualizer
{
    public class Canvas : Drawable
    {
        public App App { get; set; }
        public Vector2i Size { get; set; }

        public int MaxValue { get; set; }
        public int MinValue { get; set; }

        private readonly RectangleShape _column = new();

        public void Draw(RenderTarget target, RenderStates states)
        {
            var digits = App.SortArray.Digits;

            for (int i = 0; i < digits.Length; i++)
            {
                var digit = digits[i];

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
                    X = Size.X * ((float) i / digits.Length),
                    Y = Size.Y
                };

                var size = new Vector2f
                {
                    X =  Size.X * (1.0f / digits.Length) - 0.5f,
                    Y = -Size.Y * (value - MinValue) / MaxValue
                };

                _column.Position = pos;
                _column.Size = size;
                _column.FillColor = color;

                target.Draw(_column);
            }
        }
    }
}
