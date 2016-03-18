Public Class Form1

    Dim ImageProcessing As ImageProcessClass '图像处理
    Dim MyPainter As PainterClass '绘图操作
    Dim OriginalBitmap As Bitmap '初始图像
    Dim CurrentBitmap As Bitmap '当前图像
    '窗体加载
    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ImageProcessing = New ImageProcessClass
        MyPainter = New PainterClass
        Me.TopMost = True
    End Sub
    '复制屏幕
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        TabControl1.SelectedIndex = 0
        OriginalBitmap = ImageProcessing.GetScreenImage(TabPage1.PointToScreen(New Point(0, 0)).X + 3, TabPage1.PointToScreen(New Point(0, 0)).Y + 3, TabPage1.Width - 6, TabPage1.Height - 6)
        'CurrentBitmap = New Bitmap(OriginalBitmap)
        'ImageChanged()
        TabControl1.SelectedIndex = 1
        TrackBar1_MouseUp(sender, e)
    End Sub
    '绘制图像
    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        If Not CurrentBitmap Is Nothing Then
            If MsgBox("请确保你已经打开画板程序！", MsgBoxStyle.OkCancel, "提示") = MsgBoxResult.Ok Then
                TabControl1.SelectedIndex = 0
                MyPainter.StartPaint(CurrentBitmap, TabPage1.PointToScreen(New Point(0, 0)).X + 3, TabPage1.PointToScreen(New Point(0, 0)).Y + 3)
            End If
        Else
                MsgBox("请先复制屏幕")
        End If
    End Sub
    Private Sub TrackBar1_MouseUp(sender As Object, e As MouseEventArgs) Handles TrackBar1.MouseUp
        If Not OriginalBitmap Is Nothing Then
            CurrentBitmap = ImageProcessing.GetThresholdImage(OriginalBitmap, TrackBar1.Value)
            ImageChanged()
        Else
            MsgBox("请先复制屏幕")
        End If
    End Sub
    Private Sub TrackBar2_MouseUp(sender As Object, e As MouseEventArgs) Handles TrackBar2.MouseUp
        If Not OriginalBitmap Is Nothing Then
            CurrentBitmap = ImageProcessing.GetOutLineImage(OriginalBitmap, TrackBar2.Value)
            ImageChanged()
        Else
            MsgBox("请先复制屏幕")
        End If
    End Sub
    ''' <summary>
    ''' 对图像更改时调用该方法
    ''' </summary>
    Private Sub ImageChanged()
        PictureBox1.Image = CurrentBitmap
    End Sub
End Class
