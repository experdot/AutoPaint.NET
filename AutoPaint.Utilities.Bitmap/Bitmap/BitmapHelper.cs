using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using AutoPaint.Core;

namespace AutoPaint.Utilities
{
    public class BitmapHelper
    {
        /// <summary>
        /// 返回指定矩形区域的屏幕图像
        /// </summary>
        public static Bitmap GetScreenImage(Rectangle rect)
        {
            Bitmap result = new Bitmap(rect.Width, rect.Height);
            using (Graphics pg = Graphics.FromImage(result))
            {
                pg.CopyFromScreen(rect.X, rect.Y, 0, 0, new Size(rect.Width, rect.Height));
            }
            return result;
        }

        /// <summary>
        /// 返回指定文字生成的位图
        /// </summary>
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

        /// <summary>
        /// 返回指定位图包含的像素数据
        /// </summary>
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
        /// <summary>
        /// 返回指定颜色数组生成的位图
        /// </summary>
        public static Bitmap GetBitmapFromPixelData(PixelData pixels)
        {
            return GetBitmapFromColors(pixels.Colors);
        }

        /// <summary>
        /// 返回指定位图包含的颜色数组
        /// </summary>
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

        /// <summary>
        /// 返回指定颜色数组生成的位图
        /// </summary>
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
