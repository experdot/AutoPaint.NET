using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace AutoPaint.Core
{
    public struct PixelData
    {
        public Color[,] Colors;
        public int Width;
        public int Height;

        public PixelData(int w, int h)
        {
            Colors = new Color[w, h];
            this.Width = w;
            this.Height = h;
        }

        public static PixelData CreateFromColors(Color[,] colors)
        {
            int w = colors.GetUpperBound(0) + 1;
            int h = colors.GetUpperBound(1) + 1;
            PixelData result = new PixelData(w, h) { Colors = colors };
            return result;
        }

        public Color[,] GetColorsClone()
        {
            return Colors.Clone() as Color[,];
        }
    }
}
