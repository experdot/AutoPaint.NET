using AutoPaint.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;

namespace AutoPaint.Utilities
{
    static class Thin
    {
        static int[] ThinMap = {
            0,0,1,1,0,0,1,1,1,1,0,1,1,1,0,1,
            1,1,0,0,1,1,1,1,0,0,0,0,0,0,0,1,
            0,0,1,1,0,0,1,1,1,1,0,1,1,1,0,1,
            1,1,0,0,1,1,1,1,0,0,0,0,0,0,0,1,
            1,1,0,0,1,1,0,0,0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
            1,1,0,0,1,1,0,0,1,1,0,1,1,1,0,1,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
            0,0,1,1,0,0,1,1,1,1,0,1,1,1,0,1,
            1,1,0,0,1,1,1,1,0,0,0,0,0,0,0,1,
            0,0,1,1,0,0,1,1,1,1,0,1,1,1,0,1,
            1,1,0,0,1,1,1,1,0,0,0,0,0,0,0,0,
            1,1,0,0,1,1,0,0,0,0,0,0,0,0,0,0,
            1,1,0,0,1,1,1,1,0,0,0,0,0,0,0,0,
            1,1,0,0,1,1,0,0,1,1,0,1,1,1,0,0,
            1,1,0,0,1,1,1,0,1,1,0,0,1,0,0,0};

        public static PixelData Solve(PixelData pixels)
        {
            int[,] bools = ColorHelper.GetPixelDataBools(pixels);
            int[,] iThin = bools.Clone() as int[,];
            Color[,] colors = pixels.GetColorsClone();

            int w = pixels.Width;
            int h = pixels.Height;

            for (var i = 0; i < h; i++)
            {
                for (var j = 0; j < w; j++)
                {
                    if (bools[j, i] == 1)
                    {
                        int sum = 0;
                        int[] a = { 1, 1, 1, 1, 1, 1, 1, 1, 1 };
                        for (int m = 0; m < 3; m++)
                        {
                            for (int n = 0; n < 3; n++)
                            {
                                int ry = i - 1 + m;
                                int rx = j - 1 + n;
                                if (-1 < rx && rx < w && -1 < ry && ry < h && iThin[rx, ry] == 1)
                                {
                                    a[m * 3 + n] = 0;
                                }
                            }
                        }
                        sum = a[0] * 1 + a[1] * 2 + a[2] * 4 + a[3] * 8 + a[5] * 16 + a[6] * 32 + a[7] * 64 + a[8] * 128;
                        iThin[j, i] = 1 - ThinMap[sum];
                        colors[j, i] = iThin[j, i] == 1 ? Color.Black : Color.White;
                    }
                }
            }

            return PixelData.CreateFromColors(colors);
        }

        public static PixelData Solve2(PixelData pixels)
        {
            PixelData p = pixels;
            int[,] bools = ColorHelper.GetPixelDataBools(pixels);
            int[,] iThin = bools.Clone() as int[,];
            int w = pixels.Width;
            int h = pixels.Height;

            for (int i = 0; i < 100; i++)
            {
                iThin = VThin(iThin, w, h);
                iThin = HThin(iThin, w, h);
            }

            for (var i = 0; i <= pixels.Width - 1; i++)
            {
                for (var j = 0; j <= pixels.Height - 1; j++)
                {
                    pixels.Colors[i, j] = iThin[i, j] == 1 ? Color.Black : Color.White;
                }
            }

            return pixels;
        }

        public static int[,] VThin(int[,] bools, int w, int h)
        {
            int[,] iThin = bools.Clone() as int[,];
            int NEXT = 1;

            for (var i = 0; i < h; i++)
            {
                for (var j = 0; j < w; j++)
                {
                    if (NEXT == 0)
                    {
                        NEXT = 1;
                        continue;
                    }
                    int M = (j > 0 && j < w - 1) ? 3 - bools[j - 1, i] - bools[j, i] - bools[j + 1, i] : 1;
                    if (bools[j, i] == 1 && M != 0)
                    {
                        int sum = 0;
                        int[] a = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                        for (int m = 0; m < 3; m++)
                        {
                            for (int n = 0; n < 3; n++)
                            {
                                int ry = i - 1 + m;
                                int rx = j - 1 + n;
                                if (-1 < rx && rx < w && -1 < ry && ry < h && iThin[rx, ry] == 0)
                                {
                                    a[m * 3 + n] = 1;
                                }
                            }
                        }
                        sum = a[0] * 1 + a[1] * 2 + a[2] * 4 + a[3] * 8 + a[5] * 16 + a[6] * 32 + a[7] * 64 + a[8] * 128;
                        iThin[j, i] = 1 - ThinMap[sum];
                        if (ThinMap[sum] == 1)
                        {
                            NEXT = 0;
                        }
                    }
                }
            }

            return iThin;
        }

        public static int[,] HThin(int[,] bools, int w, int h)
        {
            int[,] iThin = bools.Clone() as int[,];
            int NEXT = 1;

            for (var j = 0; j < w; j++)
            {
                for (var i = 0; i < h; i++)
                {
                    if (NEXT == 0)
                    {
                        NEXT = 1;
                        continue;
                    }
                    int M = (i > 0 && i < h - 1) ? 3 - bools[j, i - 1] - bools[j, i] - bools[j, i + 1] : 1;
                    if (bools[j, i] == 1 && M != 0)
                    {
                        int sum = 0;
                        int[] a = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                        for (int m = 0; m < 3; m++)
                        {
                            for (int n = 0; n < 3; n++)
                            {
                                int ry = i - 1 + m;
                                int rx = j - 1 + n;
                                if (-1 < rx && rx < w && -1 < ry && ry < h && iThin[rx, ry] == 0)
                                {
                                    a[m * 3 + n] = 1;
                                }
                            }
                        }
                        sum = a[0] * 1 + a[1] * 2 + a[2] * 4 + a[3] * 8 + a[5] * 16 + a[6] * 32 + a[7] * 64 + a[8] * 128;
                        iThin[j, i] = 1 - ThinMap[sum];
                        if (ThinMap[sum] == 1)
                        {
                            NEXT = 0;
                        }
                    }
                }
            }

            return iThin;
        }


    }
}
