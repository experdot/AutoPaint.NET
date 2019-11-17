using AutoPaint.Core;
using AutoPaint.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPaint.Recognition
{
    public class FastRecognition : IRecognition
    {
        public List<ILine> Recognize(PixelData pixels)
        {
            FastAI sequenceAI = new FastAI(ColorHelper.GetPixelDataBools(pixels));
            return sequenceAI.Lines;
        }

        IPainting IRecognition.Recognize(PixelData pixels)
        {
            FastAI sequenceAI = new FastAI(ColorHelper.GetPixelDataBools(pixels));
            return new Painting() { Lines = sequenceAI.Lines };
        }
    }
}
