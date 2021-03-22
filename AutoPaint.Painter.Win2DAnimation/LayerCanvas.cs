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
    public class LayerCanvas
    {
        public CanvasRenderTarget[] Canvas { get; set; }
        public CanvasRenderTarget Foreground { get; set; }

        public LayerCanvas(ICanvasResourceCreatorWithDpi resourceCreator, Size size, int count)
        {
            Canvas = new CanvasRenderTarget[count];
            for (int i = 0; i < Canvas.Length; i++)
            {
                Canvas[i] = new CanvasRenderTarget(resourceCreator, size);
                using (var ds = Canvas[i].CreateDrawingSession())
                {
                    ds.Clear(Colors.Transparent);
                }
            }

            Foreground = new CanvasRenderTarget(resourceCreator, size);
        }

        public LayerRender CreateLayerRender()
        {
            return new LayerRender(this);
        }

        public void OnDraw(CanvasDrawingSession session)
        {
            for (int i = 0; i < Canvas.Length; i++)
            {
                session.DrawImage(Canvas[i]);
            }

            using (var shadow = new Microsoft.Graphics.Canvas.Effects.ShadowEffect() { Source = Foreground })
            {
                session.DrawImage(shadow);
            }
        }
    }
}
