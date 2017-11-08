''' <summary>
''' 层基类
''' </summary>
Public MustInherit Class HierarchyBase
    Implements IHierarchy

    Public Property Clusters As List(Of Cluster) = New List(Of Cluster) Implements IHierarchy.Clusters
    Public Property Rank As Integer Implements IHierarchy.Rank

    Public MustOverride Function Generate() As IHierarchy Implements IHierarchy.Generate

    ''' <summary>
    ''' 添加簇
    ''' </summary>
    Public Sub AddCluster(cluster As Cluster, Optional repeat As Boolean = False)
        If repeat OrElse Not Clusters.Contains(cluster) Then
            Clusters.Add(cluster)
        End If
    End Sub

End Class
