using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPaint.Core
{
    public interface IPainter
    {
        /// <summary>
        /// Gets the event that occur when a drawing point happens to update.
        /// </summary>
        event EventHandler<UpdatePaintEventArgs> UpdatePaint;

        void Start(List<ILine> lines);
        void Pause();
        void Resume();
        void Stop();
    }
}
