using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using AutoPaint.Core;

namespace AutoPaint.Utilities
{
    public class BitmapHelper
    {
        public static Bitmap GetScreenImage(Rectangle rect)
        {
            Bitmap result = new Bitmap(rect.Width, rect.Height);
            using (Graphics pg = Graphics.FromImage(result))
            {
                pg.CopyFromScreen(rect.X, rect.Y, 0, 0, new Size(rect.Width, rect.Height));
            }
            return result;
        }

        public static Bitmap GetTextImage(string text, Font font, int width, int height)
        {
            Bitmap result = new Bitmap(width, height);
            using (var pg = Graphics.FromImage(result))
            {
                pg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias; // 抗锯齿
                pg.DrawString(text, font, Brushes.Black, 0, 0);
            }
            return result;
        }

        public static PixelData GetPixelDataFromBitmap(Bitmap bmp)
        {
            Color[,] colors = new Color[bmp.Width - 1 + 1, bmp.Height - 1 + 1];
            var loopTo = bmp.Width - 1;
            for (var i = 0; i <= loopTo; i++)
            {
                var loopTo1 = bmp.Height - 1;
                for (var j = 0; j <= loopTo1; j++)
                    colors[i, j] = bmp.GetPixel(i, j);
            }
            return PixelData.CreateFromColors(colors);
        }

        public static Bitmap GetBitmapFromPixelData(PixelData pixels)
        {
            return GetBitmapFromColors(pixels.Colors);
        }

        public static Color[,] GetColorsFromBitmap(Bitmap bmp)
        {
            Color[,] result = new Color[bmp.Width - 1 + 1, bmp.Height - 1 + 1];
            var loopTo = bmp.Width - 1;
            for (var i = 0; i <= loopTo; i++)
            {
                var loopTo1 = bmp.Height - 1;
                for (var j = 0; j <= loopTo1; j++)
                    result[i, j] = bmp.GetPixel(i, j);
            }
            return result;
        }

        public static Bitmap GetBitmapFromColors(Color[,] colors)
        {
            int w = colors.GetUpperBound(0) + 1;
            int h = colors.GetUpperBound(1) + 1;
            Bitmap result = new Bitmap(w, h);
            var loopTo = w - 1;
            for (var i = 0; i <= loopTo; i++)
            {
                var loopTo1 = h - 1;
                for (var j = 0; j <= loopTo1; j++)
                    result.SetPixel(i, j, colors[i, j]);
            }
            return result;
        }
    }
}
