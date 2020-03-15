using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPaint.Core
{
    public interface ILine
    {
        IList<Vertex> Vertices { get; set; }
    }
}
