Imports AutomaticDrawing.Recognition.Clustering

Public Class ClusterCompare
    Implements IComparer(Of Cluster)

    Public Property Target As Cluster
    Public Sub New(target As Cluster)
        Me.Target = target
    End Sub

    Public Function Compare(x As Cluster, y As Cluster) As Integer Implements IComparer(Of Cluster).Compare
        If (x.AveragePostion - Target.AveragePostion).LengthSquared > (y.AveragePostion - Target.AveragePostion).LengthSquared Then
            Return 1
        Else
            Return -1
        End If
    End Function
End Class
