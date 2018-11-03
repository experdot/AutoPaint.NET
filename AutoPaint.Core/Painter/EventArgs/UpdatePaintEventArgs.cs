using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPaint.Core
{
    public class UpdatePaintEventArgs
    {
        /// <summary>
        /// Gets the vertex being draw.
        /// </summary>
        public Vertex NewVertex { get; }

        /// <summary>
        /// Gets the progress of painting.
        /// </summary>
        public float Percent { get; }

        public UpdatePaintEventArgs(Vertex point, float percent)
        {
            this.NewVertex = point;
            this.Percent = percent;
        }
    }
}
