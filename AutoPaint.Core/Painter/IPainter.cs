using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPaint.Core
{
    public interface IPainter
    {
        /// <summary>
        /// Gets the event that occur when a painting happens to update.
        /// </summary>
        event EventHandler<OnPaintingUpdatedEventArgs> OnPaintingUpdated;

        void Start(IPainting painting);
        void Pause();
        void Resume();
        void Stop();
    }
}
