using AutoPaint.Core;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public void FillCircle(Vertex point, int layerIndex)
        {
            Sessions[layerIndex].FillCircle(point.Position, point.Size, point.Color.ToUIColor());
            ForegroundSession.FillCircle(point.Position, point.Size, Colors.Black);
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
