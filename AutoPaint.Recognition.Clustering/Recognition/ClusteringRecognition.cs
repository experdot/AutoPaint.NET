using AutoPaint.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPaint.Recognition.Clustering.Recognition
{
    public class ClusteringRecognition : IRecognition
    {
        public IPainting Recognize(PixelData pixels)
        {
            ClusteringAI clusteringAI = new ClusteringAI(pixels);
            return new Painting() { Lines = clusteringAI.Lines };
        }
    }
}
