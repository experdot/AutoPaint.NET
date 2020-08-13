using AutoPaint.Core;
using AutoPaint.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
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

            int mid = 6;
            for (var i = maxRank - 1; i >= mid; i += -1)
                Lines.AddRange(GenerateLines(Hierarchies[i]));

            //Lines.AddRange(DeepGenerateLines(Hierarchies[mid].Clusters, Hierarchies[mid].Rank));
        }

        private List<ILine> GenerateLines(IHierarchy hierarchy)
        {
            List<ILine> result = new List<ILine>();
            int count = 0;

            //var averagePosition = VectorHelper.GetAveragePosition(hierarchy.Clusters.Select(v => v.Position));
            //hierarchy.Clusters.Sort((a, b) => -Math.Sign((a.Position - averagePosition).LengthSquared() - (b.Position - averagePosition).LengthSquared()));
            foreach (var cluster in hierarchy.Clusters)
            {
                Line line = new Line();
                foreach (var leaf in cluster.Leaves)
                {
                    Color c = cluster.Color;
                    Color p = Color.FromArgb(System.Convert.ToInt32(c.A / (double)(hierarchy.Rank + 1.0F)), c.R, c.G, c.B);
                    line.Vertices.Add(new Vertex() { Color = p, Position = leaf.Position, Size = hierarchy.Rank + 1.0F });
                }
                result.Add(line);
                count += line.Vertices.Count;
            }
            return result;
        }


        private List<ILine> DeepGenerateLines(List<Cluster> clusters, int rank)
        {
            List<ILine> result = new List<ILine>();
            foreach (var cluster in clusters)
            {
                Line line = new Line();
                foreach (var leaf in cluster.Leaves)
                {
                    line.Vertices.Add(new Vertex() { Color = cluster.Color, Position = leaf.Position, Size = 1 });
                }
                result.Add(line);
                result.AddRange(DeepGenerateLines(cluster.Children, rank - 1));
            }
            return result;
        }
    }

}
