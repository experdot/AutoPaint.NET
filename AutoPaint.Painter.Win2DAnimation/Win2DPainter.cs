using AutoPaint.Core;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPaint.Painter.Win2DAnimation
{
    public class Win2DPainter : IPainter
    {
        public event EventHandler<OnPaintingUpdatedEventArgs> OnPaintingUpdated;

        public LayerCanvas Canvas { get; set; }

        public ConcurrentQueue<Vertex> CurrentPoints { get; set; } = new ConcurrentQueue<Vertex>();

        public void Pause()
        {
            throw new NotImplementedException();
        }

        public void Resume()
        {
            throw new NotImplementedException();
        }

        public void Start(IPainting painting)
        {
            CurrentPoints = new ConcurrentQueue<Vertex>(painting.Lines.SelectMany(v => v.Vertices));
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void Draw(CanvasDrawingSession session)
        {
            using (var render = new LayerRender(Canvas))
            {
                for (int i = 0; i < 300; i++)
                {
                    if (CurrentPoints.TryDequeue(out Vertex result))
                    {
                        render.FillCircle(result);
                    }
                }
                Canvas.OnDraw(session);
            }

        }
    }
}
