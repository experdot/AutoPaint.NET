using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPaint.Core
{
    /// <summary>
    /// 线条接口
    /// </summary>
    public interface ILine
    {
        /// <summary>
        /// 顶点集
        /// </summary>
        List<Vertex> Vertices { get; set; }
    }
}
