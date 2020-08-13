using AutoPaint.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;

namespace AutoPaint.Recognition.Clustering
{
    public class Cluster
    {
        public Cluster Parent { get; set; }
        public List<Cluster> Children { get; set; } = new List<Cluster>();
        public Vector2 Position { get; set; }
        public Color Color { get; set; }
        public Vector3 ColorDirection { get; set; }

        public Vector3 ColorVector
        {
            get
            {
                return new Vector3(Color.R, Color.G, Color.B);
            }
        }

        public List<Cluster> Leaves
        {
            get
            {
                if (Children.Count == 0)
                {
                    return new List<Cluster>() { this };
                }
                else
                {
                    return GetLeavesOfChildren();
                }
            }
        }

        public static Cluster Combine(Cluster cluster1, Cluster cluster2)
        {
            Cluster result;

            if (cluster1.Parent != null)
            {
                result = cluster1.Parent;
            }
            else if (cluster2.Parent != null)
            {
                result = cluster2.Parent;
            }
            else
            {
                result = new Cluster();
            }

            result.AddChild(cluster1, false);
            result.AddChild(cluster2, false);

            return result;
        }

        public void AddChild(Cluster child, bool repeat = false)
        {
            if (repeat || !Children.Contains(child))
            {
                Children.Add(child);
                child.Parent = this;
            }
        }

        public Queue<Cluster> GetMostSimilar(List<Cluster> clusters)
        {
            Queue<Cluster> result = new Queue<Cluster>();
            float maxValue = float.MinValue;
            Cluster temp = null;
            if (clusters.Count > 0)
            {
                for (var i = 0; i <= clusters.Count - 1; i++)
                {
                    var cluster = clusters[i];
                    if (cluster != this)
                    {
                        float value = Compare(this, cluster);
                        if (value > maxValue)
                        {
                            maxValue = value;
                            temp = cluster;
                        }
                    }
                }
            }
            result.Enqueue(temp);
            return result;
        }

        public Vector2 GetAveragePosition()
        {
            if (Children.Count == 0)
                return Position;
            else
                return VectorHelper.GetAveragePosition(GetPostionsOfChidren());
        }

        public Color GetAverageColor()
        {
            if (Children.Count == 0)
                return Color;
            else
                return ColorHelper.GetAverageColor(GetAColorsOfChildren());
        }

        public Vector3 GetAverageColorDirection()
        {
            if (Children.Count == 0)
                return ColorDirection;
            else
                return VectorHelper.GetAveragePosition(GetColorDirectionOfChidren());
        }

        private float Compare(Cluster cluster1, Cluster cluster2)
        {
            float result;

            Color color1 = cluster1.Color;
            Color color2 = cluster2.Color;
            Vector3 vec1 = new Vector3(color1.R, color1.G, color1.B);
            Vector3 vec2 = new Vector3(color2.R, color2.G, color2.B);

            // TODO
            float colorDistance = 1 / (float)(1 + (vec1 - vec2).LengthSquared());
            float directionDistance = 1 / (float)(1 + (cluster1.ColorDirection - cluster2.ColorDirection).LengthSquared());

            result = directionDistance * colorDistance;
            return result;
        }

        private List<Cluster> GetLeavesOfChildren()
        {
            return (Children.SelectMany(c => c.Leaves)).ToList();
        }

        private IEnumerable<Vector2> GetPostionsOfChidren()
        {
            return from c in Children select c.Position;
        }

        private IEnumerable<Color> GetAColorsOfChildren()
        {
            return from c in Children select c.Color;
        }

        private IEnumerable<Vector3> GetColorDirectionOfChidren()
        {
            return from c in Children select c.ColorDirection;
        }
    }


    public class ClusterColorCompare : IComparer<Cluster>
    {
        public Cluster Target { get; set; }
        public ClusterColorCompare(Cluster target)
        {
            this.Target = target;
        }
        public int Compare(Cluster x, Cluster y)
        {
            if (ColorHelper.GetColorSimilarity(Target.Color, x.Color) < ColorHelper.GetColorSimilarity(Target.Color, y.Color))
                return 1;
            else
                return -1;
        }
    }

    public class ClusterPositionCompare : IComparer<Cluster>
    {
        public Cluster Target { get; set; }
        public ClusterPositionCompare(Cluster target)
        {
            this.Target = target;
        }
        public int Compare(Cluster x, Cluster y)
        {
            if ((x.Position - Target.Position).LengthSquared() > (y.Position - Target.Position).LengthSquared())
                return 1;
            else
                return -1;
        }
    }
}
