using AutoPaint.Core;
using AutoPaint.Utilities;
using MIConvexHull;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;

namespace AutoPaint.Recognition.Clustering
{
    public class ClusteringAI
    {
        public List<ILine> Lines { get; set; } = new List<ILine>();
        public List<IHierarchy> Hierarchies { get; set; } = new List<IHierarchy>();

        public ClusteringAI(PixelData pixels, int maxRank = 10)
        {
            DateTime start = DateTime.Now;
            Debug.WriteLine("Initialize Start");
            Hierarchies.Add(GridHierarchy.CreateFromPixels(pixels));
            Debug.WriteLine($"Initialize Over,TimeConsuming:{DateTime.Now - start}");
            Debug.WriteLine($"Generation Start");
            for (var i = 0; i <= maxRank - 1; i++)
            {
                start = DateTime.Now;
                Hierarchies.Add(Hierarchies.Last().Generate());
                Debug.WriteLine($"Total:{maxRank},Current:{i + 1},Time Consuming:{DateTime.Now - start},Hierarchy[{Hierarchies.Last()}]");
            }
            Debug.WriteLine(pixels.Colors.Length);

            //int mid = 3;
            //for (var i = maxRank - 1; i >= mid; i += -3)
            //{
            //    //Lines.AddRange(GenerateOutlines(Hierarchies[i]));
            //    Lines.AddRange(GenerateLines(Hierarchies[i].Clusters.ToList()));
            //    //Lines.AddRange(DeepGenerateLines(Hierarchies[i].Clusters));
            //}

            Lines.AddRange(GenerateLines(Hierarchies[9].Clusters.ToList()));
            //Lines.AddRange(DeepGenerateLines(Hierarchies[0].Clusters));
        }

        private List<ILine> GenerateLines(List<Cluster> clusters)
        {
            List<ILine> result = new List<ILine>();

            clusters.Sort((a, b) => -Math.Sign(a.LeavesCount - b.LeavesCount));

            for (int i = 0; i < clusters.Count; i++)
            {
                BuildResult(result, clusters[i]);
                if (i >= 0)
                {
                    continue;
                }
                for (int j = i + 1; j < clusters.Count; j++)
                {
                    BuildResult2(result, clusters[j], clusters[i].Color);
                }
            }

            for (int i = 0; i < clusters.Count; i++)
            {
                result.AddRange(GenerateLines(clusters[i].Children.ToList()));
            }

            //var averagePosition = VectorHelper.GetAveragePosition(hierarchy.Clusters.Select(v => v.Position));
            //hierarchy.Clusters.Sort((a, b) => -Math.Sign((a.Position - averagePosition).LengthSquared() - (b.Position - averagePosition).LengthSquared()));

            return result;
        }

        private static void BuildResult(List<ILine> result, Cluster cluster)
        {
            Line line = new Line();
            var leaves = cluster.Leaves.Select(v => v.Position).ToList();
            foreach (var leaf in leaves)
            {
                Color c = cluster.Color;
                Color p = Color.FromArgb((int)(c.A / (double)(cluster.LayerIndex * 2 + 1.0F)), c.R, c.G, c.B);
                line.Vertices.Add(new Vertex() { Color = p, Position = leaf, Size = cluster.LayerIndex + 1.0F, LayerIndex = cluster.LayerIndex });
            }
            result.Add(line);
        }

        private static void BuildResult2(List<ILine> result, Cluster cluster, Color color)
        {
            Line line = new Line();
            var leaves = cluster.Leaves.Select(v => v.Position).ToList();
            foreach (var leaf in leaves)
            {
                Color c = color;
                Color p = Color.FromArgb((int)(c.A / (double)(cluster.LayerIndex * 2 + 1.0F)), c.R, c.G, c.B);
                line.Vertices.Add(new Vertex() { Color = p, Position = leaf, Size = cluster.LayerIndex + 1.0F, LayerIndex = cluster.LayerIndex });
            }
            result.Add(line);
        }

        private List<ILine> DeepGenerateLines(IEnumerable<Cluster> clusters)
        {
            List<ILine> result = new List<ILine>();
            foreach (var cluster in clusters)
            {
                Line line = new Line();
                foreach (var leaf in cluster.Leaves)
                {
                    Color c = cluster.Color;
                    //Color p = Color.FromArgb(System.Convert.ToInt32(c.A / (double)(cluster.LayerIndex * 2 + 1.0F)), c.R, c.G, c.B);
                    line.Vertices.Add(new Vertex() { Color = c, Position = leaf.Position, Size = 1, LayerIndex = 0 });
                }
                result.Add(line);
                //result.AddRange(DeepGenerateLines(cluster.Children));
            }
            return result;
        }

        private List<ILine> GenerateOutlines(IEnumerable<Cluster> clusters, int rank)
        {
            List<ILine> result = new List<ILine>();

            if (clusters.Count() <= 1)
            {
                return result;
            }

            //clusters.Sort((a, b) => -Math.Sign(a.Leaves.Count - b.Leaves.Count));


            foreach (var cluster in clusters)
            {
                Line line = new Line() { IsOutline = true };
                var leaves = cluster.Leaves.Select(v => v.Position).ToList();
                var outlines = leaves;
                if (leaves.Count > 5)
                {
                    var doubles = leaves.Select(v => new double[] { v.X, v.Y }).ToArray();
                    var hull = MIConvexHull.ConvexHull.Create2D(doubles).Result;
                    outlines = hull.Select(v => new Vector2((float)v.X, (float)v.Y)).ToList();
                }
                outlines.Add(outlines[0]);

                foreach (var outline in outlines)
                {
                    Color c = cluster.Color;
                    Color p = Color.FromArgb(System.Convert.ToInt32(c.A / (double)(rank + 1.0F)), 150, 150, 150);
                    line.Vertices.Add(new Vertex() { Color = p, Position = outline, Size = rank + 1.0F });
                }
                result.Add(line);
            }
            return result;
        }
    }

}
