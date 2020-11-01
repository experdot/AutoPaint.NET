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
            PaintLines(painting.Lines);
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

        private void PaintLines(IEnumerable<ILine> lines)
        {
            using (Graphics pg = Graphics.FromImage(SourceBitmap))
            {
                pg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                int totalCount = lines.Count();
                int current = 0;
                foreach (Line line in lines)
                {
                    if (line.IsOutline)
                    {
                        PaintOutline(pg, line);
                    }
                    else
                    {
                        PaintLine(pg, line);
                    }
                    current += 1;
                    OnPaintingUpdated?.Invoke(this, new OnPaintingUpdatedEventArgs(line.Vertices.FirstOrDefault(), current / (float)totalCount));
                }
            }
        }

        private static void PaintLine(Graphics pg, ILine line)
        {
            foreach (var vertex in line.Vertices)
            {
                float penSize = vertex.Size;
                float halfSize = penSize / 2;
                SolidBrush brush = new SolidBrush(vertex.Color);
                pg.FillRectangle(brush, new RectangleF(vertex.X - halfSize, vertex.Y - halfSize, penSize, penSize));
            }
        }

        private static void PaintOutline(Graphics pg, ILine line)
        {
            var vertex = line.Vertices[0];
            float penSize = 1;
            //float penSize = vertex.Size;
            SolidBrush brush = new SolidBrush(vertex.Color);
            var points = line.Vertices.Select(v => new PointF(v.X, v.Y)).ToArray();
            pg.DrawLines(new Pen(brush, penSize), points);
        }
    }
}
