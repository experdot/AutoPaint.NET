using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPaint.Core
{
    public class OnPaintingUpdatedEventArgs
    {
        /// <summary>
        /// Gets the vertex being draw.
        /// </summary>
        public Vertex NewVertex { get; }

        /// <summary>
        /// Gets the progress of painting.
        /// </summary>
        public float Percent { get; }

        public OnPaintingUpdatedEventArgs(Vertex point, float percent)
        {
            this.NewVertex = point;
            this.Percent = percent;
        }
    }
}
