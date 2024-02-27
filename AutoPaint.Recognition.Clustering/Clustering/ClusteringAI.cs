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

            // Paint lines
            Lines.AddRange(GenerateLines1(Hierarchies[9].Clusters.ToList()));
            Lines.AddRange(GenerateLines1(Hierarchies[8].Clusters.ToList()));
            Lines.AddRange(GenerateLines1(Hierarchies[7].Clusters.ToList()));
            Lines.AddRange(GenerateLines1(Hierarchies[6].Clusters.ToList()));
            Lines.AddRange(GenerateLines2(Hierarchies[7].Clusters.ToList()));


            // Paint pixels
            foreach (var item in Hierarchies[9].Clusters)
            {
                BuildLeafResult(Lines, item);
            }
        }


        private List<ILine> GenerateLines1(List<Cluster> clusters)
        {
            List<ILine> result = new List<ILine>();

            //clusters.Sort((a, b) => Math.Sign(a.LeavesCount - b.LeavesCount));

            var averagePosition = VectorHelper.GetAveragePosition(clusters.Select(v => v.Position));
            clusters.Sort((a, b) => Math.Sign((a.Position - averagePosition).LengthSquared() - (b.Position - averagePosition).LengthSquared()));

            for (int i = 0; i < clusters.Count; i++)
            {
                BuildResult(result, clusters[i]);
            }

            return result;
        }

        private List<ILine> GenerateLines2(List<Cluster> clusters)
        {
            List<ILine> result = new List<ILine>();

            //clusters.Sort((a, b) => -Math.Sign(a.LeavesCount - b.LeavesCount));

            var averagePosition = VectorHelper.GetAveragePosition(clusters.Select(v => v.Position));
            clusters.Sort((a, b) => Math.Sign((a.Position - averagePosition).LengthSquared() - (b.Position - averagePosition).LengthSquared()));

            for (int i = 0; i < clusters.Count; i++)
            {
                BuildResult(result, clusters[i]);
                result.AddRange(GenerateLines2(clusters[i].Children.ToList()));
            }

            return result;
        }

        private static void BuildResult(List<ILine> result, Cluster cluster)
        {
            Line line = new Line();
            var leaves = cluster.Leaves;
            var step = cluster.LayerIndex * 8 + 1;
            for (int i = 0; i < leaves.Count; i += step)
            {
                var leaf = leaves[i];
                Color c = cluster.Color;
                Color p = Color.FromArgb((int)(0 + 255 / (double)(cluster.LayerIndex * 4 + 2F)), c.R, c.G, c.B);
                if (cluster.LayerIndex == 0)
                {
                    p = cluster.Color;
                }
                line.Vertices.Add(new Vertex() { Color = p, Position = leaf.Position, Size = cluster.LayerIndex * 16f + 1.0F, LayerIndex = cluster.LayerIndex });
            }
            result.Add(line);
        }

        private static void BuildClusterResult(List<ILine> result, Cluster cluster)
        {
            Line line = new Line();
            var leaves = cluster.Leaves.ToList();
            for (int i = 0; i < leaves.Count; i += 1)
            {
                var leaf = leaves[i];
                line.Vertices.Add(new Vertex() { Color = cluster.Color, Position = leaf.Position, Size = 1.0F, LayerIndex = 0 });
            }
            result.Add(line);
        }

        private static void BuildLeafResult(List<ILine> result, Cluster cluster)
        {
            Line line = new Line();
            var leaves = cluster.Leaves.ToList();
            for (int i = 0; i < leaves.Count; i += 1)
            {
                var leaf = leaves[i];
                line.Vertices.Add(new Vertex() { Color = leaf.Color, Position = leaf.Position, Size = 1.0F, LayerIndex = 0 });
            }
            result.Add(line);
        }
    }
}
