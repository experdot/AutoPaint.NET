using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPaint.Core
{
    /// <summary>
    /// 绘制器接口
    /// </summary>
    public interface IPainter
    {
        /// <summary>
        /// 绘制点更新时发生的事件
        /// </summary>
        event EventHandler<UpdatePaintEventArgs> UpdatePaint;

        void Start(List<ILine> lines);
        void Pause();
        void Stop();
    }
}
