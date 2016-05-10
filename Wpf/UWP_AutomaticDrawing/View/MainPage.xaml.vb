'“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍
Imports System.Numerics
''' <summary>
''' 可用于自身或导航至 Frame 内部的空白页。
''' </summary>
Public NotInheritable Class MainPage
    Inherits Page
    Private Sub button_Click(sender As Object, e As RoutedEventArgs) Handles button.Click

        Dim tempImage As New BitmapImage(New Uri("ms-appx:///Images/sample_01.jpg", UriKind.Absolute))
        image1.Source = tempImage

    End Sub
End Class
