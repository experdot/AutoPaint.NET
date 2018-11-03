using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Text;

namespace AutoPaint.Core
{
    /// <summary>
    /// Represents a vertex with position, color, and size properties.
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
