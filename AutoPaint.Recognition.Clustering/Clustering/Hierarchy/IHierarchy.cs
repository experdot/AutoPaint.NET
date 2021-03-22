using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;
using System.Linq;

namespace AutoPaint.Recognition.Clustering
{
    public interface IHierarchy
    {
        ConcurrentBag<Cluster> Clusters { get; set; }
        int Rank { get; set; }
        IHierarchy Generate();
    }

    public abstract class HierarchyBase : IHierarchy
    {
        public ConcurrentBag<Cluster> Clusters { get; set; } = new ConcurrentBag<Cluster>();
        public int Rank { get; set; }

        public abstract IHierarchy Generate();

        public void AddCluster(Cluster cluster, bool repeat = false)
        {
            if (repeat || !Clusters.Contains(cluster))
            {
                Clusters.Add(cluster);
            }
        }
    }
}
