using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Drawing;
using System.Numerics;
using AutoPaint.Core;

namespace AutoPaint.Utilities
{
    public class ColorHelper
    {
        public static int CalculateDistance(Color color1, Color color2)
        {
            int deltaR = color1.R - color2.R;
            int deltaG = color1.G - color2.G;
            int deltaB = color1.B - color2.B;

            return deltaR * deltaR + deltaG * deltaG + deltaB * deltaB;
        }

        public static Color FindClosestColor(IEnumerable<Color> colors)
        {
            int minDistance = int.MaxValue;
            Color closestColor = Color.Empty;

            var targetColor = GetAverageColor(colors);

            foreach (Color color in colors)
            {
                int distance = CalculateDistance(color, targetColor);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestColor = color;
                }
            }

            return closestColor;
        }

        public static Color GetAverageColor(IEnumerable<Color> colors)
        {
            Color result;
            int a = colors.Sum(c => c.A) / colors.Count();
            int r = colors.Sum(c => c.R) / colors.Count();
            int g = colors.Sum(c => c.G) / colors.Count();
            int b = colors.Sum(c => c.B) / colors.Count();
            result = Color.FromArgb(a, r, g, b);
            return result;
        }

        public static Color GetAverageColor(Color color1, Color color2)
        {
            Color result;
            int a = (System.Convert.ToInt32(color1.A) + System.Convert.ToInt32(color2.A)) / 2;
            int r = (System.Convert.ToInt32(color1.R) + System.Convert.ToInt32(color2.R)) / 2;
            int g = (System.Convert.ToInt32(color1.G) + System.Convert.ToInt32(color2.G)) / 2;
            int b = (System.Convert.ToInt32(color1.B) + System.Convert.ToInt32(color2.B)) / 2;
            result = Color.FromArgb(a, r, g, b);
            return result;
        }

        public static float GetColorSimilarity(Color color1, Color color2)
        {
            float result;
            Vector3 vec1 = new Vector3(color1.R, color1.G, color1.B);
            Vector3 vec2 = new Vector3(color2.R, color2.G, color2.B);
            result = 1 / (1 + (vec1 - vec2).LengthSquared());
            return result;
        }

        public static bool CompareBaseRGB(Color color1, Color color2, float distance)
        {
            int r = System.Convert.ToInt32(color1.R) - System.Convert.ToInt32(color2.R);
            int g = System.Convert.ToInt32(color1.G) - System.Convert.ToInt32(color2.G);
            int b = System.Convert.ToInt32(color1.B) - System.Convert.ToInt32(color2.B);
            int temp = (int)Math.Sqrt(r * r + g * g + b * b);
            if (temp < distance)
                return true;
            else
                return false;
        }

        public static bool CompareBaseHSB(Color color1, Color color2, float distance)
        {
            // Vector distance
            // Dim h As Single = (Color1.GetHue - Color2.GetHue) / 360
            // Dim s As Single = Color1.GetSaturation - Color2.GetSaturation
            // Dim b As Single = Color1.GetBrightness - Color2.GetBrightness
            // Dim absDis As Single = Math.Sqrt(h * h + s * s + b * b)
            // If absDis < Distance Then
            // Return True
            // Else
            // Return False
            // End If
            // Vector Angle
            float h1 = color1.GetHue() / 360;
            float s1 = color1.GetSaturation();
            float b1 = color1.GetBrightness();
            float h2 = color2.GetHue() / 360;
            float s2 = color2.GetSaturation();
            float b2 = color2.GetBrightness();
            float absDis = (float)((h1 * h2 + s1 * s2 + b1 * b2) / (Math.Sqrt(h1 * h1 + s1 * s1 + b1 * b1) * Math.Sqrt(h2 * h2 + s2 * s2 + b2 * b2)));
            if (absDis > distance / (double)5 + 0.8)
                return true;
            else
                return false;
        }

        public static int GetGrayOfColor(Color color)
        {
            int mid, r, g, b;
            r = color.R;
            g = color.G;
            b = color.B;
            mid = (r + g + b) / 3;
            return mid;
        }

        public static PixelData GetLumpPixelData(PixelData pixels, int range = 10)
        {
            Color[,] colors = pixels.GetColorsClone();
            for (var i = 0; i <= pixels.Width - 1; i++)
            {
                for (var j = 0; j <= pixels.Height - 1; j++)
                {
                    var r = (colors[i, j].R / range) * range;
                    var g = (colors[i, j].G / range) * range;
                    var b = (colors[i, j].B / range) * range;
                    colors[i, j] = Color.FromArgb(r, g, b);
                }
            }
            return PixelData.CreateFromColors(colors);
        }

        public static PixelData GetThresholdPixelData(PixelData pixels, float threshold, bool isHSB = false)
        {
            Color[,] colors = pixels.GetColorsClone();
            Func<Color, bool> IsOverThreshold = (Color color) =>
             {
                 return isHSB ? (color.GetHue() / 360 + color.GetBrightness() + color.GetSaturation()) / (double)3 < threshold : GetGrayOfColor(color) < threshold;
             };
            for (var i = 0; i <= pixels.Width - 1; i++)
            {
                for (var j = 0; j <= pixels.Height - 1; j++)
                    colors[i, j] = IsOverThreshold(colors[i, j]) ? Color.Black : Color.White;
            }
            return PixelData.CreateFromColors(colors);
        }

        public static PixelData GetOutLinePixelData(PixelData pixels, float distance, bool isHSB = false)
        {
            Func<Color, Color, float, bool> CompareColor = (Color c1, Color c2, float d) =>
              {
                  return isHSB ? CompareBaseHSB(c1, c2, d) : CompareBaseRGB(c1, c2, d);
              };
            Func<Color, Color, bool> CompareColorExtra = (Color c1, Color c2) =>
            {
                return isHSB ? c1.GetBrightness() - c2.GetBrightness() > 0 : GetGrayOfColor(c1) - GetGrayOfColor(c2) > 0;
            };
            var colors = pixels.GetColorsClone();
            Color color1, color2;
            for (var i = 1; i <= pixels.Width - 2; i++)
            {
                for (var j = 1; j <= pixels.Height - 2; j++)
                {
                    color1 = pixels.Colors[i, j];
                    for (var p = 0; p <= 3; p++)
                    {
                        color2 = pixels.Colors[i + OffsetX[p], j + OffsetY[p]];
                        if (!CompareColor(color1, color2, distance) && CompareColorExtra(color1, color2))
                            colors[i, j] = Color.Black;
                        else
                            colors[i, j] = Color.White;
                    }
                }
            }
            return PixelData.CreateFromColors(colors);
        }

        public static PixelData GetHollowPixelData(PixelData pixels)
        {
            var colors = pixels.GetColorsClone();
            int[,] bools = GetPixelDataBools(pixels);
            for (var i = 0; i <= pixels.Width - 1; i++)
            {
                for (var j = 0; j <= pixels.Height - 1; j++)
                {
                    if (bools[i, j] == 1 && IsBesieged(bools, i, j) == false)
                        colors[i, j] = Color.Black;
                    else
                        colors[i, j] = Color.White;
                }
            }
            return PixelData.CreateFromColors(colors);
        }

        public static PixelData GetThinPixelData(PixelData pixels)
        {
            return Thin.Solve2(pixels);
        }

        public static PixelData GetInvertPixelData(PixelData pixels)
        {
            var colors = pixels.GetColorsClone();
            int[,] bools = GetPixelDataBools(pixels);
            for (var i = 0; i <= pixels.Width - 1; i++)
            {
                for (var j = 0; j <= pixels.Height - 1; j++)
                {
                    if (bools[i, j] == 0)
                        colors[i, j] = Color.Black;
                    else
                        colors[i, j] = Color.White;
                }
            }
            return PixelData.CreateFromColors(colors);
        }

        public static int[,] GetPixelDataBools(PixelData pixels)
        {
            int[,] result = new int[pixels.Width - 1 + 1, pixels.Height - 1 + 1];
            Color[,] colors = pixels.GetColorsClone();
            for (var i = 0; i <= pixels.Width - 1; i++)
            {
                for (var j = 0; j <= pixels.Height - 1; j++)
                {
                    if (colors[i, j].Equals(Color.FromArgb(255, 255, 255)))
                        result[i, j] = 0;
                    else
                        result[i, j] = 1;
                }
            }
            return result;
        }

        public static int[] OffsetX = new[] { 0, 1, 0, -1 };
        public static int[] OffsetY = new[] { -1, 0, 1, 0 };

        private static bool IsBesieged(int[,] bools, int x, int y)
        {
            for (var i = 0; i <= 3; i++)
            {
                int dx = x + OffsetX[i];
                int dy = y + OffsetY[i];
                if (IsIndexOverFlowArrayBound(bools, dx, dy) || bools[dx, dy] == 0)
                    return false;
            }
            return true;
        }

        private static bool IsIndexOverFlowArrayBound(Array array, int x, int y)
        {
            int w = array.GetUpperBound(0);
            int h = array.GetUpperBound(1);
            return !(x >= 0 && y >= 0 && x <= w && y <= h);
        }
    }
}
