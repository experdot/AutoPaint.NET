Imports AutomaticDrawing.Recognition.Clustering
''' <summary>
''' 由组表示的层
''' </summary>
Public Class GroupHierarchy
    Inherits HierarchyBase

    Public Overrides Function Generate() As IHierarchy
        Dim result As New GroupHierarchy With {.Rank = Me.Rank + 1}
        Dim start As DateTime = DateTime.Now
        For Each SubCluster In Clusters
            Dim similar As Cluster = SubCluster.GetMostSimilar(GetNeighbours(SubCluster)).First
            If similar IsNot Nothing Then
                result.AddCluster(Cluster.Combine(SubCluster, similar))
            End If
            Dim progress As Single = CSng((Clusters.IndexOf(SubCluster) + 1) / Clusters.Count)
            'Dim total As Long = (DateTime.Now - start).TotalMilliseconds
            Debug.WriteLine($"{progress}")
        Next

        Return result
    End Function


    Private Function GetNeighbours(cluster As Cluster) As List(Of Cluster)
        Const MaxCount As Integer = 8
        Dim result As New List(Of Cluster)
        Dim array As New List(Of Cluster)
        array.AddRange(Clusters)
        array.Sort(New ClusterPositionCompare(cluster))
        result.AddRange(array.Take(MaxCount))
        result.AddRange(array)
        Return result
    End Function
End Class
