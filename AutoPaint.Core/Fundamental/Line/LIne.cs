using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPaint.Core
{
    public class Line : ILine
    {
        public IList<Vertex> Vertices { get; set; } = new List<Vertex>();
    }
}
