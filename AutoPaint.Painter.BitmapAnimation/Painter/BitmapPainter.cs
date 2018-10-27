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
        /// <summary>
        /// 视图
        /// </summary>
        public Bitmap View { get; set; }
        /// <summary>
        /// 是否绘制原始数据
        /// </summary>
        public bool IsPaintRaw { get; set; }

        private static Random Rnd = new Random();

        /// <summary>
        /// 创建并初始化一个实例
        /// </summary>
        public BitmapPainter(Bitmap view, bool isPaintRaw = true)
        {
            this.View = view;
            this.IsPaintRaw = isPaintRaw;
        }

        public void Start(List<ILine> lines)
        {
            if (IsPaintRaw)
                PaintRaw(lines);
            else
                PaintColorful(lines);
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }
        public void Stop()
        {
            throw new NotImplementedException();

        }

        private void PaintRaw(List<ILine> lines)
        {
            using (Graphics pg = Graphics.FromImage(View))
            {
                pg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                int totalCount = lines.SelectMany(e => e.Vertices).Count();
                int current = 0;
                foreach (var SubSequence in lines)
                {
                    foreach (var SubVertex in SubSequence.Vertices)
                    {
                        float penSize = SubVertex.Size;
                        SolidBrush brush = new SolidBrush(SubVertex.Color);
                        pg.FillRectangle(brush, new RectangleF(SubVertex.X - penSize / 2, SubVertex.Y - penSize / 2, penSize, penSize));
                        // pg.DrawLine(mypen, SubPoint, SubSequence.PointList.First)
                        current += 1;
                        UpdatePaint?.Invoke(this, new UpdatePaintEventArgs(SubVertex, System.Convert.ToSingle(current / (double)totalCount)));
                    }
                }
            }
        }
        private void PaintColorful(List<ILine> lines)
        {
            Color tempColor = Color.FromArgb(255, 0, 0, 0);
            using (Graphics pg = Graphics.FromImage(View))
            {
                pg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                int totalCount = lines.SelectMany(e => e.Vertices).Count();
                int current = 0;
                foreach (var SubSequence in lines)
                {
                    tempColor = Color.FromArgb(255, System.Convert.ToInt32(Rnd.NextDouble() * 255), System.Convert.ToInt32(Rnd.NextDouble() * 255), System.Convert.ToInt32(Rnd.NextDouble() * 255));
                    foreach (var SubPoint in SubSequence.Vertices)
                    {
                        float penSize = SubPoint.Size;
                        Pen mypen = new Pen(tempColor, 1 + penSize);
                        pg.DrawEllipse(mypen, new RectangleF(SubPoint.X - penSize / 2, SubPoint.Y - penSize / 2, penSize, penSize));
                        // pg.DrawLine(mypen, SubPoint, SubSequence.PointList.First)
                        current += 1;
                        UpdatePaint?.Invoke(this, new UpdatePaintEventArgs(SubPoint, System.Convert.ToSingle(current / (double)totalCount)));
                    }
                }
            }
        }

    }
}
