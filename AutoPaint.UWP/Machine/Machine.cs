using AutoPaint.Core;
using AutoPaint.Painter.Win2DAnimation;
using AutoPaint.Recognition.Clustering.Recognition;
using AutoPaint.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPaint.UWP
{
    public class Machine
    {
        public Win2DPainter Painter { get; set; } = new Win2DPainter();
        public ClusteringRecognition Recognition { get; set; } = new ClusteringRecognition();

        public void Run(PixelData data)
        {
            var painting = Recognition.Recognize(data);
            Painter.Start(painting);
        }
    }
}
