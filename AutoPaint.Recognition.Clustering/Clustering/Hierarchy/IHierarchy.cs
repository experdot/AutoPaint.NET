using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPaint.Recognition.Clustering
{
    public interface IHierarchy
    {
        List<Cluster> Clusters { get; set; }
        int Rank { get; set; }
        IHierarchy Generate();
    }

    public abstract class HierarchyBase : IHierarchy
    {
        public List<Cluster> Clusters { get; set; } = new List<Cluster>();
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
