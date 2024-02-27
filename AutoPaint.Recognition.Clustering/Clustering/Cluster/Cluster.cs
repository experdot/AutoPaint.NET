﻿using AutoPaint.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Collections.Concurrent;

namespace AutoPaint.Recognition.Clustering
{
    public class Cluster
    {
        public Cluster Parent { get; set; }
        public ConcurrentBag<Cluster> Children { get; set; } = new ConcurrentBag<Cluster>();
        public Vector2 Position { get; set; }
        public Color Color { get; set; }
        public int LayerIndex { get; set; }

        public Vector3 ColorVector
        {
            get
            {
                return new Vector3(Color.R, Color.G, Color.B);
            }
        }

        public Vector4 EdgeVector { get; set; }

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

        public float Percent
        {
            get
            {
                var counts = Children.Select(v => v.Children.Count() + 1.0f).ToList();

                if (counts.Count <= 1)
                {
                    return 0;
                }

                counts.Sort();
                var value = counts.Aggregate((v1, v2) => v1 / v2);
                return value;
            }
        }

        private int _leavesCountCache;
        public int LeavesCount
        {
            get
            {
                if (_leavesCountCache > 0)
                {
                    return _leavesCountCache;
                }

                if (Children.Count == 0)
                {
                    _leavesCountCache = 1;
                }
                else
                {
                    _leavesCountCache = Children.Sum(v => v.LeavesCount);
                }

                return _leavesCountCache;
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
                result = new Cluster() { LayerIndex = cluster1.LayerIndex + 1 };
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

        public List<Cluster> GetMostSimilar(List<Cluster> clusters)
        {
            List<Cluster> result = new List<Cluster>();
            float maxValue = 0.001f;
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
            result.Add(temp);
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
                return ColorHelper.FindClosestColor(GetAColorsOfChildren());
        }

        private float Compare(Cluster cluster1, Cluster cluster2)
        {
            float result;

            Vector3 color1 = cluster1.ColorVector;
            Vector3 color2 = cluster2.ColorVector;

            Vector4 edge1 = cluster1.EdgeVector;
            Vector4 edge2 = cluster2.EdgeVector;

            float colorDistance = 1 / (float)(1 + (color1 - color2).LengthSquared());
            float edgeDistance = 1 / (float)(1 + (edge1 - edge2).LengthSquared());
            result = colorDistance * edgeDistance;
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
