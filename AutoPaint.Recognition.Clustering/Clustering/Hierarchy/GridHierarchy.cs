﻿using AutoPaint.Core;
using AutoPaint.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AutoPaint.Recognition.Clustering
{
    public class GridHierarchy : HierarchyBase
    {
        public ConcurrentBag<Cluster>[,] Grid { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }
        public float Size { get; set; }

        private static readonly int[] OffsetX9 = new[] { 0, -1, 1, 0, 0, -1, 1, -1, 1 };
        private static readonly int[] OffsetY9 = new[] { 0, 0, 0, -1, 1, -1, 1, 1, -1 };

        private static readonly int[] OffsetX4 = new[] { 0, 1, 0, -1 };
        private static readonly int[] OffsetY4 = new[] { -1, 0, 1, 0 };

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

            Grid = new ConcurrentBag<Cluster>[w - 1 + 1, h - 1 + 1];
            for (var i = 0; i <= w - 1; i++)
            {
                for (var j = 0; j <= h - 1; j++)
                    Grid[i, j] = new ConcurrentBag<Cluster>();
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
            //foreach (var cluster in Clusters)
            //{
            //    Cluster similar = cluster.GetMostSimilar(GetNeighbours(cluster)).FirstOrDefault();
            //    if (similar != null)
            //    {
            //        if (cluster.Parent == null && similar.Parent == null)
            //            result.Clusters.Add(Cluster.Combine(cluster, similar));
            //        else
            //            Cluster.Combine(cluster, similar);
            //    }
            //    else
            //    {
            //        result.Clusters.Add(cluster);
            //    }
            //}

            Parallel.ForEach(Clusters, cluster =>
            {
                Cluster similar = cluster.GetMostSimilar(GetNeighbours(cluster)).FirstOrDefault();
                if (similar != null)
                {
                    if (cluster.Parent == null && similar.Parent == null)
                        result.Clusters.Add(Cluster.Combine(cluster, similar));
                    else
                        Cluster.Combine(cluster, similar);
                }
                else
                {
                    result.Clusters.Add(cluster);
                }
            });

            // Initialize properties
            Parallel.ForEach(result.Clusters, cluster =>
            {
                cluster.Position = cluster.GetAveragePosition();
                cluster.Color = cluster.GetAverageColor();
            });

            // Locate to cell
            Parallel.ForEach(result.Clusters, cluster =>
            {
                Vector2 p = cluster.Position;
                int x = System.Convert.ToInt32(p.X / (double)result.Size);
                int y = System.Convert.ToInt32(p.Y / (double)result.Size);
                result.Grid[x, y].Add(cluster);
            });

            // Calculate edge vector
            Parallel.ForEach(result.Clusters, cluster =>
            {
                cluster.EdgeVector = result.GetNeighboursEdgeVector(cluster); ;
            });

            return result;
        }

        public override string ToString()
        {
            return $"Rank:{Rank}Clusters.Count:{Clusters.Count}";
        }

        public List<Cluster> GetNeighbours(Cluster cluster)
        {
            List<Cluster> result = new List<Cluster>(9);
            int xBound = Grid.GetUpperBound(0);
            int yBound = Grid.GetUpperBound(1);
            int dx, dy;
            int x = System.Convert.ToInt32(cluster.Position.X / (double)Size);
            int y = System.Convert.ToInt32(cluster.Position.Y / (double)Size);
            for (var i = 0; i < 9; i++)
            {
                dx = x + OffsetX9[i];
                dy = y + OffsetY9[i];
                if ((dx >= 0 && dy >= 0 && dx <= xBound && dy <= yBound))
                    result.AddRange(Grid[dx, dy]);
                else
                    continue;
            }
            result.Remove(cluster);
            return result;
        }

        public Vector4 GetNeighboursEdgeVector(Cluster cluster)
        {
            float[] floats = new float[4];
            int xBound = Grid.GetUpperBound(0);
            int yBound = Grid.GetUpperBound(1);
            int dx, dy;
            int x = System.Convert.ToInt32(cluster.Position.X / (double)Size);
            int y = System.Convert.ToInt32(cluster.Position.Y / (double)Size);

            for (var i = 0; i < 4; i++)
            {
                dx = x + OffsetX4[i];
                dy = y + OffsetY4[i];
                if ((dx >= 0 && dy >= 0 && dx <= xBound && dy <= yBound))
                {
                    if (Grid[dx, dy].Count == 0)
                    {
                        floats[i] = 0.0f;
                    }
                    else
                    {
                        var neighbourColor = ColorHelper.FindClosestColor(Grid[dx, dy].Select(v => v.Color));
                        floats[i] = ColorHelper.GetColorSimilarity(cluster.Color, neighbourColor);
                    }
                }
                else
                {
                    floats[i] = 0.0f;
                }
            }

            return new Vector4(floats[0], floats[1], floats[2], floats[3]);
        }
    }
}
