using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Text;

namespace AutoPaint.Core
{
    /// <summary>
    /// 表示具有位置、颜色和大小的顶点
    /// </summary>
    public class Vertex
    {
        public Vector2 Position { get; set; }
        public Color Color { get; set; }
        public float Size { get; set; }

        public int X => (int)Position.X;
        public int Y => (int)Position.Y;
    }
}
