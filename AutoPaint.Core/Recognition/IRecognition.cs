using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPaint.Core
{
    /// <summary>
    /// 线条识别器接口
    /// </summary>
    public interface IRecognition
    {
        List<ILine> Recognize(PixelData pixels);
    }
}
