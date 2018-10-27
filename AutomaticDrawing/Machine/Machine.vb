Imports AutoPaint.Core
Imports AutoPaint.Utilities
''' <summary>
''' 绘图机器
''' </summary>
Public Class Machine

    ''' <summary>
    ''' 为外部提供消息通知
    ''' </summary>
    Public Event Notify(sender As Object, e As NotifyEventArgs)

    ''' <summary>
    ''' 识别器
    ''' </summary>
    Public Property Reconition As IRecognition
    ''' <summary>
    ''' 绘制器
    ''' </summary>
    Public Property Painter As IPainter
    ''' <summary>
    ''' 原始图像
    ''' </summary>
    Public Property Original As Bitmap
    ''' <summary>
    ''' 当前图像
    ''' </summary>
    Public Property Current As Bitmap
    ''' <summary>
    ''' 最终图像
    ''' </summary>
    Public Property Final As Bitmap
    ''' <summary>
    ''' 预览图像
    ''' </summary>
    Public Property Preview As Bitmap
    ''' <summary>
    ''' 签名
    ''' </summary>
    Public Property Signature As String
    ''' <summary>
    ''' 是否绘制签名
    ''' </summary>
    Public Property IsPaintSignature As Boolean
    ''' <summary>
    ''' 是否使用原始图像
    ''' </summary>
    Public Property IsUseOriginal As Boolean = False

    ''' <summary>
    ''' 创建并初始化一个实例
    ''' </summary>
    Public Sub New(reconition As IRecognition, painter As IPainter, Optional isUseOriginal As Boolean = False)
        Me.Reconition = reconition
        Me.Painter = painter
        Me.IsUseOriginal = isUseOriginal
    End Sub

    ''' <summary>
    ''' 重置绘图器
    ''' </summary>
    Public Sub ResetPainter(painter As IPainter)
        Me.Painter = painter
    End Sub
    ''' <summary>
    ''' 重置识别器
    ''' </summary>
    Public Sub ResetReconition(reconition As IRecognition)
        Me.Reconition = reconition
    End Sub
    ''' <summary>
    ''' 启动
    ''' </summary>
    Public Sub Run()
        Dim pixels As PixelData = BitmapHelper.GetPixelDataFromBitmap(If(IsUseOriginal, Original, Final))
        Painter.Start(Reconition.Recognize(pixels))
    End Sub
    ''' <summary>
    ''' 截屏
    ''' </summary>
    Public Sub CopyScreen(rect As Rectangle)
        Original = BitmapHelper.GetScreenImage(rect)
    End Sub
End Class
