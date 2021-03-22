using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Text;

namespace AutoPaint.Core
{
    public class Vertex
    {
        public Vector2 Position { get; set; }
        public Color Color { get; set; }
        public float Size { get; set; }
        public int LayerIndex { get; set; }

        public float X => Position.X;
        public float Y => Position.Y;
    }
}
