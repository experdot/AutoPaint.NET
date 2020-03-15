using AutoPaint.Core;
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
                Debug.WriteLine($"Total:{maxRank},Current:{i + 1},Time Consuming:{DateTime.Now - start},Hierarchy[{Hierarchies.Last().ToString()}]");
            }
            Debug.WriteLine(pixels.Colors.Length);
            for (var i = maxRank - 1; i >= 0; i += -1)
                Lines.AddRange(GenerateLines(Hierarchies[i]));
        }

        private List<ILine> GenerateLines(IHierarchy hierarchy)
        {
            List<ILine> result = new List<ILine>();
            int count = 0;
            foreach (var SubCluster in hierarchy.Clusters)
            {
                Line line = new Line();
                foreach (var SubLeaf in SubCluster.Leaves)
                {
                    Color c = SubCluster.Color;
                    Color p = Color.FromArgb(System.Convert.ToInt32(c.A / (double)(hierarchy.Rank + 1.0F)), c.R, c.G, c.B);
                    line.Vertices.Add(new Vertex() { Color = SubCluster.Color, Position = SubLeaf.Position, Size = hierarchy.Rank + 1.0F });
                }
                result.Add(line);
                count += line.Vertices.Count;
            }
            Debug.WriteLine(count);
            return result;
        }
    }

}
