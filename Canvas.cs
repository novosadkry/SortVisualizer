﻿using SFML.Graphics;
using SFML.System;

namespace SortVisualizer
{
    public class Canvas : Drawable
    {
        public App App { get; set; }
        public Vector2u Size { get; set; }

        private readonly RectangleShape _column = new();

        public void Draw(RenderTarget target, RenderStates states)
        {
            var digits = App.SortArray.Digits;
            int min = App.SortArray.Min;
            int max = App.SortArray.Max;

            for (int i = 0; i < digits.Length; i++)
            {
                var digit = digits[i];

                float value = digit.Value;
                var state = digit.State;

                Color color = state switch
                {
                    DigitState.Selected => Color.Red,
                    DigitState.Sorted => Color.Green,
                    DigitState.Pivot => Color.Magenta,
                    DigitState.Primary => new Color(255, 204, 204),
                    DigitState.Secondary => new Color(255, 204, 153),
                    DigitState.Tertiary => new Color(204, 255, 204),
                    _ => Color.White
                };

                var pos = new Vector2f
                {
                    X = Size.X * ((float) i / digits.Length),
                    Y = Size.Y
                };

                var size = new Vector2f
                {
                    X =  Size.X * (1.0f / digits.Length) - 1f,
                    Y = -Size.Y * (value - min) / max
                };

                _column.Position = pos;
                _column.Size = size;
                _column.FillColor = color;

                target.Draw(_column);
            }
        }
    }
}
