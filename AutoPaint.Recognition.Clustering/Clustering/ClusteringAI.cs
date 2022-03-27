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

            Lines.AddRange(GenerateLines(Hierarchies[9].Clusters.ToList()));
        }

        private List<ILine> GenerateLines(List<Cluster> clusters)
        {
            List<ILine> result = new List<ILine>();

            clusters.Sort((a, b) => -Math.Sign(a.LeavesCount - b.LeavesCount));

            for (int i = 0; i < clusters.Count; i++)
            {
                BuildResult(result, clusters[i]);
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
            var step = cluster.LayerIndex * 8 + 1;
            for (int i = 0; i < leaves.Count; i += step)
            {
                var leaf = leaves[i];
                Color c = cluster.Color;
                Color p = Color.FromArgb((int)(0 + 255 / (double)(cluster.LayerIndex * 8 + 2F)), c.R, c.G, c.B);
                line.Vertices.Add(new Vertex() { Color = p, Position = leaf, Size = cluster.LayerIndex * 16f + 1.0F, LayerIndex = cluster.LayerIndex });
            }
            result.Add(line);
        }
    }
}
