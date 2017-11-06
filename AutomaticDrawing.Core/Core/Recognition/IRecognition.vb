''' <summary>
''' 线条识别器接口
''' </summary>
Public Interface IRecognition
    ''' <summary>
    ''' 识别
    ''' </summary>
    Function Recognize(pixels As PixelData) As List(Of ILine)
End Interface
