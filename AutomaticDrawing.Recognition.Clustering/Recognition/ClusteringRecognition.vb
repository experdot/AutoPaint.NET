Imports AutoPaint.Core
''' <summary>
''' 使用聚类模型的线条识别器
''' </summary>
Public Class ClusteringRecognition
    Implements IRecognition

    Private Function IRecognition_Recognize(pixels As PixelData) As IPainting Implements IRecognition.Recognize
        Dim clusterAI As New ClusterAI(pixels)
        Return New Painting() With {.Lines = clusterAI.Lines}
    End Function
End Class
