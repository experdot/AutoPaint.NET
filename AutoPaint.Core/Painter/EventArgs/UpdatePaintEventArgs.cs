using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPaint.Core
{
    public class UpdatePaintEventArgs
    {
        /// <summary>
        /// 新的绘制点
        /// </summary>
        public Vertex Point { get; set; }
        /// <summary>
        ///  绘制进度
        /// </summary>
        public float Percent { get; set; }

        public UpdatePaintEventArgs(Vertex point, float percent)
        {
            this.Point = point;
            this.Percent = percent;
        }
    }
}
