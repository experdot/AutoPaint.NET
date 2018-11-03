using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPaint.Core
{
    public interface IRecognition
    {
        List<ILine> Recognize(PixelData pixels);
    }
}