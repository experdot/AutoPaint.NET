using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using AutoPaint.Core;
using System.Drawing;

namespace AutoPaint.Painter
{
    public class BitmapPainter : IPainter
    {
        public event EventHandler<OnPaintingUpdatedEventArgs> OnPaintingUpdated;
        public Bitmap SourceBitmap { get; set; }

        public BitmapPainter(Bitmap targetBitmap)
        {
            this.SourceBitmap = targetBitmap;
        }

        public void Start(IPainting painting)
        {
            PaintRaw(painting.Lines);
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }
        public void Resume()
        {
            throw new NotImplementedException();
        }
        public void Stop()
        {
            throw new NotImplementedException();
        }

        private void PaintRaw(IEnumerable<ILine> lines)
        {
            using (Graphics pg = Graphics.FromImage(SourceBitmap))
            {
                pg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                int totalCount = lines.SelectMany(e => e.Vertices).Count();
                int current = 0;
                foreach (var sequence in lines)
                {
                    foreach (var vertex in sequence.Vertices)
                    {
                        float penSize = vertex.Size;
                        float halfSize = penSize / 2;
                        SolidBrush brush = new SolidBrush(vertex.Color);
                        pg.FillRectangle(brush, new RectangleF(vertex.X - halfSize, vertex.Y - halfSize, penSize, penSize));
                        current += 1;
                        OnPaintingUpdated?.Invoke(this, new OnPaintingUpdatedEventArgs(vertex, current / (float)totalCount));
                    }
                }
            }
        }
    }
}
