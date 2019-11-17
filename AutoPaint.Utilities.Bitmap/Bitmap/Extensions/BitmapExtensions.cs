using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using AutoPaint.Core;

namespace AutoPaint.Utilities
{
    public static class BitmapExtensions
    {
        public static PixelData GetPixelData(this Bitmap bitmap)
        {
            return BitmapHelper.GetPixelDataFromBitmap(bitmap);
        }

        public static Bitmap CreateBitmap(this PixelData bitmap)
        {
            return BitmapHelper.GetBitmapFromPixelData(bitmap);
        }
    }

}
