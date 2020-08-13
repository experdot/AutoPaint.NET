using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Numerics;

namespace AutoPaint.Utilities
{
    public class VectorHelper
    {
        /// <summary>
        /// 返回向量集合的平均位置
        /// </summary>
        public static Vector2 GetAveragePosition(IEnumerable<Vector2> positions)
        {
            Vector2 result;
            float x = positions.Sum(p => p.X) / positions.Count();
            float y = positions.Sum(p => p.Y) / positions.Count();
            result = new Vector2(x, y);
            return result;
        }

        /// <summary>
        /// 返回3D向量集合的平均位置
        /// </summary>
        public static Vector3 GetAveragePosition(IEnumerable<Vector3> positions)
        {
            Vector3 result;
            float x = positions.Sum(p => p.X) / positions.Count();
            float y = positions.Sum(p => p.Y) / positions.Count();
            float z = positions.Sum(p => p.Z) / positions.Count();
            result = new Vector3(x, y, z);
            return result;
        }
    }
}
