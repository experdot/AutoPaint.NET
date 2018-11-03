using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPaint.Core
{
    public class Line : ILine
    {
        public List<Vertex> Vertices { get; set; } = new List<Vertex>();
    }
}
