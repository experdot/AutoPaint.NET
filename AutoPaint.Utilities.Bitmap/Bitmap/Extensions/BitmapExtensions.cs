using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using AutoPaint.Core;

namespace AutoPaint.Utilities
{
    public static class BitmapExtensions
    {
        /// <summary>
        /// 返回当前位图的像素数据
        /// </summary>
        public static PixelData GetPixelData(this Bitmap bitmap)
        {
            return BitmapHelper.GetPixelDataFromBitmap(bitmap);
        }

        /// <summary>
        /// 从当前像素数据生成位图
        /// </summary>
        public static Bitmap CreateBitmap(this PixelData bitmap)
        {
            return BitmapHelper.GetBitmapFromPixelData(bitmap);
        }
    }

}
