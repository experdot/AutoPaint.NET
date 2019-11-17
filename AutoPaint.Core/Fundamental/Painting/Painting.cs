using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPaint.Core
{
    public class Painting : IPainting
    {
        public IEnumerable<ILine> Lines { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
