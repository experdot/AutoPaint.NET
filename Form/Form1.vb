Public Class Form1

    Dim ImageProcessing As ImageProcessClass '图像处理
    Dim MyPainter As PainterClass '绘图操作
    Dim OriginalBitmap As Bitmap '初始图像
    Dim CurrentBitmap As Bitmap '当前图像
    Dim FinalBitmap As Bitmap '最终绘制图像
    '窗体加载
    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ImageProcessing = New ImageProcessClass
        MyPainter = New PainterClass
        AddHandler MyPainter.UpdatePreviewImage, AddressOf RefreshPicturebox2
        Me.TopMost = True
    End Sub
    '复制屏幕
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        TabControl1.SelectedIndex = 0
        OriginalBitmap = ImageProcessing.GetScreenImage(TabPage1.PointToScreen(New Point(0, 0)).X + 3, TabPage1.PointToScreen(New Point(0, 0)).Y + 3, TabPage1.Width - 6, TabPage1.Height - 6)
        TabControl1.SelectedIndex = 1
        TrackBar_MouseUp(TrackBar1, e)
    End Sub
    '绘制图像
    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        If Not FinalBitmap Is Nothing Then
            If MsgBox("请确保你已经打开画板程序！", MsgBoxStyle.OkCancel, "提示") = MsgBoxResult.Ok Then
                Me.Hide()
                TabControl1.SelectedIndex = 0
                MyPainter.StartPaint(FinalBitmap, TabPage1.PointToScreen(New Point(0, 0)).X + 3, TabPage1.PointToScreen(New Point(0, 0)).Y + 3)
                If CheckBox3.Checked Then
                    Dim SignatureBitmap As Bitmap = ImageProcessing.GetTextImage(TextBox1.Text, New Font("Letter Gothic Std"， 18)， 200, 36)
                    MyPainter.StartPaint(SignatureBitmap, TabPage1.PointToScreen(New Point(0, 0)).X + 3, TabPage1.PointToScreen(New Point(0, 0)).Y + 3)
                End If
                Me.Show()
            End If
        Else
            MsgBox("请先复制屏幕")
        End If
    End Sub
    '预览绘制
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        If Not FinalBitmap Is Nothing Then
            TabControl1.SelectedIndex = 2
            PictureBox2.Image = New Bitmap(FinalBitmap.Width, FinalBitmap.Height)
            MyPainter.StartPreview(FinalBitmap, PictureBox2.Image）
        Else
            MsgBox("请先复制屏幕")
        End If
    End Sub
    Private Sub TrackBar_MouseUp(sender As Object, e As MouseEventArgs) Handles TrackBar1.MouseUp, TrackBar2.MouseUp
        If Not OriginalBitmap Is Nothing Then
            If sender Is TrackBar1 Then
                CurrentBitmap = ImageProcessing.GetThresholdImage(OriginalBitmap, sender.Value)
                RadioButton1.Checked = True
            Else
                CurrentBitmap = ImageProcessing.GetOutLineImage(OriginalBitmap, sender.Value)
                RadioButton2.Checked = True
            End If
            ImageChanged()
        Else
            MsgBox("请先复制屏幕")
        End If
    End Sub
    ''' <summary>
    ''' 对图像更改时调用该方法
    ''' </summary>
    Private Sub ImageChanged()
        FinalBitmap = New Bitmap(CurrentBitmap)
        If CheckBox1.Checked Then
            FinalBitmap = ImageProcessing.GetAroundImage(CurrentBitmap)
        End If
        If CheckBox2.Checked Then
            FinalBitmap = ImageProcessing.GetInvertImage(FinalBitmap)
        End If
        PictureBox1.Image = FinalBitmap
    End Sub
    Private Sub CheckBox_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged， CheckBox2.CheckedChanged
        ImageChanged()
    End Sub
    Dim IsMouseDown As Boolean
    Dim OldPoint As New PointF
    Private Sub PictureBox1_MouseDown(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseDown
        IsMouseDown = True
        OldPoint = New Point(e.X, e.Y)
        ImageChanged()
    End Sub
    Private Sub PictureBox1_MouseMove(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseMove
        If IsMouseDown Then
            paintPoint(e.X, e.Y)
            ImageChanged()
        End If
    End Sub
    Private Sub paintPoint(dX As Integer, dY As Integer)
        Using pg = Graphics.FromImage(CurrentBitmap)
            pg.DrawLine(Pens.Black, New Point(dX, dY), OldPoint)
        End Using
        OldPoint = New Point(dX, dY)
    End Sub
    Private Sub PictureBox1_MouseUp(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseUp
        IsMouseDown = False
    End Sub
    Private Sub RefreshPicturebox2()
        PictureBox2.Refresh()
        Application.DoEvents()
    End Sub
End Class
