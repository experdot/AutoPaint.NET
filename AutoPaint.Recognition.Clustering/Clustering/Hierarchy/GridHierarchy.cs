using AutoPaint.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace AutoPaint.Recognition.Clustering
{
    public class GridHierarchy : HierarchyBase
    {
        public List<Cluster>[,] Grid { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }
        public float Size { get; set; }

        private static readonly int[] OffsetX = new[] { 0, -1, 1, 0, 0, -1, 1, -1, 1 };
        private static readonly int[] OffsetY = new[] { 0, 0, 0, -1, 1, -1, 1, 1, -1 };

        public GridHierarchy(int w, int h, float size)
        {
            if (w < 1)
            {
                w = 1;
            }

            if (h < 1)
            {
                h = 1;
            }

            this.Width = w;
            this.Height = h;
            this.Size = size;

            Grid = new List<Cluster>[w - 1 + 1, h - 1 + 1];
            for (var i = 0; i <= w - 1; i++)
            {
                for (var j = 0; j <= h - 1; j++)
                    Grid[i, j] = new List<Cluster>();
            }
        }

        public static GridHierarchy CreateFromPixels(PixelData pixels)
        {
            GridHierarchy result = new GridHierarchy(pixels.Width, pixels.Height, 1);
            for (var i = 0; i <= pixels.Width - 1; i++)
            {
                for (var j = 0; j <= pixels.Height - 1; j++)
                {
                    Cluster cluster = new Cluster()
                    {
                        Position = new Vector2(i, j),
                        Color = pixels.Colors[i, j]
                    };
                    result.Grid[i, j].Add(cluster);
                    result.Clusters.Add(cluster);
                }
            }
            return result;
        }

        public override IHierarchy Generate()
        {
            float rate = 2.0F;
            float newSize = this.Size * rate;
            GridHierarchy result = new GridHierarchy(System.Convert.ToInt32(Math.Ceiling(this.Width / (double)rate) + 1), System.Convert.ToInt32(Math.Ceiling(this.Height / (double)rate) + 1), newSize) { Rank = this.Rank + 1 };

            // Combine to a new cluster
            foreach (var SubCluster in Clusters)
            {
                Cluster similar = SubCluster.GetMostSimilar(GetNeighbours(SubCluster)).FirstOrDefault();
                if (similar != null)
                {
                    if (SubCluster.Parent == null && similar.Parent == null)
                        result.Clusters.Add(Cluster.Combine(SubCluster, similar));
                    else
                        Cluster.Combine(SubCluster, similar);
                }
            }
            // Initialize properties
            foreach (var SubCluster in result.Clusters)
            {
                SubCluster.Position = SubCluster.GetAveragePosition();
                SubCluster.Color = SubCluster.GetAverageColor();
            }
            // Locate to cell
            foreach (var SubCluster in result.Clusters)
            {
                Vector2 p = SubCluster.Position;
                int x = System.Convert.ToInt32(p.X / (double)result.Size);
                int y = System.Convert.ToInt32(p.Y / (double)result.Size);
                result.Grid[x, y].Add(SubCluster);
            }
            return result;
        }

        public override string ToString()
        {
            return $"Rank:{Rank}Clusters.Count:{Clusters.Count}";
        }

        private List<Cluster> GetNeighbours(Cluster cluster)
        {
            List<Cluster> result = new List<Cluster>();
            int xBound = Grid.GetUpperBound(0);
            int yBound = Grid.GetUpperBound(1);
            int dx, dy;
            int x = System.Convert.ToInt32(cluster.Position.X / (double)Size);
            int y = System.Convert.ToInt32(cluster.Position.Y / (double)Size);
            for (var i = 0; i <= 8; i++)
            {
                dx = x + OffsetX[i];
                dy = y + OffsetY[i];
                if ((dx >= 0 && dy >= 0 && dx <= xBound && dy <= yBound))
                    result.AddRange(Grid[dx, dy]);
                else
                    continue;
            }
            result.Remove(cluster);
            return result;
        }
    }
}
