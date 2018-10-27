Imports AutoPaint.Core
Imports AutoPaint.Utilities
''' <summary>
''' 使用快速模型的线条识别器
''' </summary>
Public Class FastRecognition
    Implements IRecognition
    Public Function Recognize(pixels As PixelData) As List(Of ILine) Implements IRecognition.Recognize
        Dim sequenceAI As New SequenceAI(ColorHelper.GetPixelDataBools(pixels))
        Return sequenceAI.Lines
    End Function
End Class
