Imports System.Numerics
Imports AutoPaint.Core
Imports AutoPaint.Painter
Imports AutomaticDrawing.Recognition.Clustering
Imports AutomaticDrawing.Recognition.FastAI
Imports AutoPaint.Utilities

Public Class Form1
    ''' <summary>
    ''' 绘图机器
    ''' </summary>
    Private Machine As Machine

    Private PreviewSpeed As Integer = 1

    ''' <summary>
    ''' 窗体加载时
    ''' </summary>
    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.TopMost = True
        Machine = New Machine(Nothing, Nothing, True)
    End Sub
    ''' <summary>
    ''' 复制屏幕
    ''' </summary>
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        TabControl1.SelectedIndex = 0
        Machine.CopyScreen(New Rectangle(TabPage1.PointToScreen(New Drawing.Point(0, 0)).X + 3, TabPage1.PointToScreen(New Drawing.Point(0, 0)).Y + 3, TabPage1.Width - 6, TabPage1.Height - 6))
        TabControl1.SelectedIndex = 1
        TrackBar_MouseUp(TrackBar1, e)
        Panel4.Enabled = True
    End Sub
    ''' <summary>
    ''' 鼠标绘制
    ''' </summary>t
    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        If MessageBox.Show("Are you sure the window is on microsoft paint application?" + vbCrLf +
                           "(Please remember the hot keys,it's useful.)", "Warning", MessageBoxButtons.YesNo) = DialogResult.Yes Then
            Me.Hide()
            Machine.ResetReconition(New FastRecognition)
            Machine.ResetPainter(New MousePainter(New Vector2(TabPage1.PointToScreen(New Point(0, 0)).X + 3, TabPage1.PointToScreen(New Point(0, 0)).Y + 3)))
            Machine.IsUseOriginal = False
            AddHandler Machine.Painter.UpdatePaint, AddressOf RefreshPicturebox2
            Machine.Run()
            Me.Show()
        End If
    End Sub
    ''' <summary>
    ''' 预览绘制
    ''' </summary>
    Private Sub Button3_Click(sender As System.Object, e As System.EventArgs) Handles Button3.Click
        Static HasShownBox As Boolean = False
        If Not HasShownBox Then
            MessageBox.Show("The clustering algorithm may take serveral or dozens of seconds, please wait patiently.", "Info")
        End If
        If Machine.Final IsNot Nothing Then
            TabControl1.SelectedIndex = 2
            Machine.Preview = New Drawing.Bitmap(Machine.Final.Width, Machine.Final.Height)
            PictureBox2.Image = Machine.Preview
            Machine.ResetReconition(New ClusteringRecognition)
            Machine.ResetPainter(New BitmapPainter(Machine.Preview, True))
            Machine.IsUseOriginal = True
            AddHandler Machine.Painter.UpdatePaint, AddressOf RefreshPicturebox2
            Machine.Run()
        Else
            MsgBox("Please press Screenshot button.(请先复制屏幕)")
        End If
    End Sub
    ''' <summary>
    ''' 滑动按钮松开时
    ''' </summary>
    Private Sub TrackBar_MouseUp(sender As Object, e As MouseEventArgs) Handles TrackBar1.MouseUp
        If Not Machine.Original Is Nothing Then
            Dim thresold As Single = sender.value
            If RadioButton1.Checked Then
                Machine.Current = ColorHelper.GetThresholdPixelData(Machine.Original.GetPixelData, thresold).CreateBitmap
            Else
                Machine.Current = ColorHelper.GetOutLinePixelData(Machine.Original.GetPixelData, thresold).CreateBitmap
            End If
            ImageChanged()
        Else
            MsgBox("Please press Screenshot button.(请先复制屏幕)")
        End If
    End Sub
    ''' <summary>
    ''' 选择按钮状态改变时
    ''' </summary>
    Private Sub CheckBox_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox2.CheckedChanged， CheckBox3.CheckedChanged
        ImageChanged()
    End Sub
    ''' <summary>
    ''' 单选按钮状态改变时
    ''' </summary>
    Private Sub RadioButton_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton1.CheckedChanged
        ImageChanged()
    End Sub
    ''' <summary>
    ''' 画布大小改变时
    ''' </summary>
    Private Sub PictureBox1_SizeChanged(sender As Object, e As EventArgs) Handles PictureBox1.SizeChanged
        ToolStripStatusLabel3.Text = "Size:" & PictureBox1.Width & "*" & PictureBox1.Height & "Pixel"
    End Sub
    ''' <summary>
    ''' 选项卡切换时
    ''' </summary>
    Private Sub TabControl1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles TabControl1.SelectedIndexChanged
        If TabControl1.SelectedIndex = 2 Then
            Panel1.Hide()
        Else
            Panel1.Show()
        End If
    End Sub
    Private Sub TrackBar4_Scroll(sender As Object, e As EventArgs) Handles TrackBar4.Scroll
        PreviewSpeed = TrackBar4.Value
    End Sub

    ''' <summary>
    ''' 更新预览画布
    ''' </summary>
    Private Sub RefreshPicturebox2(sender As Object, e As UpdatePaintEventArgs)
        Static Count As Integer = 0
        Count += 1
        If Count Mod PreviewSpeed = 0 Then
            Task.Run(Sub()
                         Me.Invoke(Sub()
                                       ToolStripStatusLabel2.Text = $"Position:[{e.Point.X},{e.Point.Y}]"
                                       ToolStripStatusLabel4.Text = $"Preview:{(e.Percent * 100).ToString("F2")}%"
                                       PictureBox2.Refresh()
                                   End Sub)
                     End Sub)
            If Count > 10000 Then Count = 0
        End If
        Application.DoEvents()
    End Sub
    ''' <summary>
    ''' 对图像更改时调用该方法
    ''' </summary>
    Private Sub ImageChanged()
        If Machine?.Current IsNot Nothing Then
            Machine.Final = New Drawing.Bitmap(Machine.Current)
            If CheckBox2.Checked Then
                Machine.Final = ColorHelper.GetHollowPixelData(Machine.Current.GetPixelData).CreateBitmap
            End If
            If CheckBox3.Checked Then
                Machine.Final = ColorHelper.GetInvertPixelData(Machine.Final.GetPixelData).CreateBitmap
            End If
            PictureBox1.Image = Machine.Final
        End If
    End Sub
End Class
