using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPaint.Core
{
    public class Line : ILine
    {
        /// <summary>
        ///   顶点集
        ///  </summary>
        public List<Vertex> Vertices { get; set; } = new List<Vertex>();
    }
}
