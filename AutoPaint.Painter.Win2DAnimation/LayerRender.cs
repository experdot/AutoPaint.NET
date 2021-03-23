using AutoPaint.Core;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;


namespace AutoPaint.Painter.Win2DAnimation
{
    public class LayerRender : IDisposable
    {
        public CanvasDrawingSession[] Sessions { get; set; }
        public CanvasDrawingSession ForegroundSession { get; set; }

        public LayerRender(LayerCanvas paint)
        {
            Sessions = new CanvasDrawingSession[paint.Canvas.Length];
            for (int i = 0, loopTo = Sessions.Length - 1; i <= loopTo; i++)
            {
                Sessions[i] = paint.Canvas[i].CreateDrawingSession();
            }
            ForegroundSession = paint.Foreground.CreateDrawingSession();
            ForegroundSession.Clear(Colors.Transparent);
        }

        public Vertex LastVertex { get; set; }

        public void FillCircle(Vertex vertex)
        {
            var index = Sessions.Length - 1 - vertex.LayerIndex;
            if (vertex.Size == 1000)
            {

                Sessions[index].FillRectangle(new Rect(vertex.Position.X, vertex.Position.Y, 1, 1), vertex.Color.ToUIColor());
            }
            else
            {
                Sessions[index].DrawLine((LastVertex ?? vertex).Position, vertex.Position, vertex.Color.ToUIColor(), vertex.Size);
                LastVertex = vertex;
                //Sessions[index].FillCircle(vertex.Position, vertex.Size, vertex.Color.ToUIColor());
            }
            ForegroundSession.FillCircle(vertex.Position, vertex.Size, Colors.Black);
        }

        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    for (int i = 0, loopTo = Sessions.Length - 1; i <= loopTo; i++)
                    {
                        Sessions[i].Dispose();
                    }
                }
                ForegroundSession.Dispose();
            }

            disposedValue = true;
        }
        public void Dispose()
        {
            Dispose(true);
        }
    }
}
