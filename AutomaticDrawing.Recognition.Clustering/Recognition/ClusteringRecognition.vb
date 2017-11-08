Imports AutomaticDrawing.Core
''' <summary>
''' 使用聚类模型的线条识别器
''' </summary>
Public Class ClusteringRecognition
    Implements IRecognition
    Public Function Recognize(pixels As PixelData) As List(Of ILine) Implements IRecognition.Recognize
        Dim clusterAI As New ClusterAI(pixels)
        Return clusterAI.Lines
    End Function
End Class
