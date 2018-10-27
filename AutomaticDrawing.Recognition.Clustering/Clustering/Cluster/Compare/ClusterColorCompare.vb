Imports AutomaticDrawing.Recognition.Clustering
Imports AutoPaint.Utilities
''' <summary>
''' 颜色比较器
''' </summary>
Public Class ClusterColorCompare
    Implements IComparer(Of Cluster)
    Public Property Target As Cluster
    Public Sub New(target As Cluster)
        Me.Target = target
    End Sub
    Public Function Compare(x As Cluster, y As Cluster) As Integer Implements IComparer(Of Cluster).Compare
        If ColorHelper.GetColorSimilarity(Target.Color, x.Color) < ColorHelper.GetColorSimilarity(Target.Color, y.Color) Then
            Return 1
        Else
            Return -1
        End If
    End Function
End Class
