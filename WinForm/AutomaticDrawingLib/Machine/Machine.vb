Imports System.Drawing
''' <summary>
''' 绘图机器
''' </summary>
Public Class Machine
    Public Event Notify(sender As Object, e As NotifyEventArgs)
    ''' <summary>
    ''' 绘制器
    ''' </summary>
    Public Property Painter As Painter
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
    ''' 创建并初始化一个实例
    ''' </summary>
    Public Sub New()
        Painter = New Painter()
    End Sub

    Public Sub Start(x As Integer, y As Integer)
        If Not Final Is Nothing Then
            If MsgBox("请确保你已经打开画板程序！", MsgBoxStyle.OkCancel, "提示") = MsgBoxResult.Ok Then
                Painter.StartPaint(Final, x, y)
                If IsPaintSignature Then
                    Dim SignatureBitmap As Bitmap = BitmapHelper.GetTextImage(Signature, New Font("Letter Gothic Std"， 18)， 200, 36)
                    Painter.StartPaint(SignatureBitmap, x, y)
                End If
            End If
        Else
            RaiseEvent Notify(Me, New NotifyEventArgs("请先复制屏幕"))
        End If
    End Sub
    ''' <summary>
    ''' 截屏
    ''' </summary>
    Public Sub CopyScreen(rect As Rectangle)
        Original = BitmapHelper.GetScreenImage(rect)
    End Sub

End Class
