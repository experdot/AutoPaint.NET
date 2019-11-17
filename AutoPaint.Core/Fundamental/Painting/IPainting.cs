using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPaint.Core
{
    public interface IPainting
    {
        IEnumerable<ILine> Lines { get; set; }
    }
}
