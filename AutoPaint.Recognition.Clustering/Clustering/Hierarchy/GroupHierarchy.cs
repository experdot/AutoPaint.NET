using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AutoPaint.Recognition.Clustering
{
    public class GroupHierarchy : HierarchyBase
    {
        public override IHierarchy Generate()
        {
            GroupHierarchy result = new GroupHierarchy() { Rank = this.Rank + 1 };
            foreach (var SubCluster in Clusters)
            {
                Cluster similar = SubCluster.GetMostSimilar(GetNeighbours(SubCluster)).First();
                if (similar != null)
                {
                    result.AddCluster(Cluster.Combine(SubCluster, similar));
                }

                float progress = System.Convert.ToSingle((Clusters.IndexOf(SubCluster) + 1) / (double)Clusters.Count);
                Debug.WriteLine($"{progress}");
            }

            return result;
        }

        private List<Cluster> GetNeighbours(Cluster cluster)
        {
            const int MaxCount = 8;
            List<Cluster> result = new List<Cluster>();
            List<Cluster> array = new List<Cluster>();
            array.AddRange(Clusters);
            array.Sort(new ClusterPositionCompare(cluster));
            result.AddRange(array.Take(MaxCount));
            result.AddRange(array);
            return result;
        }
    }
}
