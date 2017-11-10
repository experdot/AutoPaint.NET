Imports AutomaticDrawing.Recognition.Clustering
''' <summary>
''' 位置比较器
''' </summary>
Public Class ClusterPositionCompare
    Implements IComparer(Of Cluster)
    Public Property Target As Cluster
    Public Sub New(target As Cluster)
        Me.Target = target
    End Sub
    Public Function Compare(x As Cluster, y As Cluster) As Integer Implements IComparer(Of Cluster).Compare
        If (x.Position - Target.Position).LengthSquared > (y.Position - Target.Position).LengthSquared Then
            Return 1
        Else
            Return -1
        End If
    End Function
End Class
