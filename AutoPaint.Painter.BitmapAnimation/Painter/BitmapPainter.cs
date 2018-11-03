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
        public event EventHandler<UpdatePaintEventArgs> UpdatePaint;
        public Bitmap TargetBitmap { get; set; }

        public BitmapPainter(Bitmap targetBitmap)
        {
            this.TargetBitmap = targetBitmap;
        }

        public void Start(List<ILine> lines)
        {
            PaintRaw(lines);
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

        private void PaintRaw(List<ILine> lines)
        {
            using (Graphics pg = Graphics.FromImage(TargetBitmap))
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
                        UpdatePaint?.Invoke(this, new UpdatePaintEventArgs(vertex, current / (float)totalCount));
                    }
                }
            }
        }
    }
}
