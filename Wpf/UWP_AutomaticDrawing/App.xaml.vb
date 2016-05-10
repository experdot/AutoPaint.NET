''' <summary>
''' 提供特定于应用程序的行为，以补充默认的应用程序类。
''' </summary>
NotInheritable Class App
    Inherits Application

    ''' <summary>
    ''' 初始化应用类的新实例。
    ''' </summary>
    Public Sub New()
        Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync(
            Microsoft.ApplicationInsights.WindowsCollectors.Metadata Or
            Microsoft.ApplicationInsights.WindowsCollectors.Session)
        InitializeComponent()
    End Sub

    ''' <summary>
    ''' 在应用程序由最终用户正常启动时进行调用。
    ''' 当启动应用程序以打开特定的文件或显示时使用
    ''' 搜索结果等
    ''' </summary>
    ''' <param name="e">有关启动请求和过程的详细信息。</param>
    Protected Overrides Sub OnLaunched(e As Windows.ApplicationModel.Activation.LaunchActivatedEventArgs)
#If DEBUG Then
        ' 调试时显示图形分析信息。
        If System.Diagnostics.Debugger.IsAttached Then
            ' 显示当前帧速率计数器
            Me.DebugSettings.EnableFrameRateCounter = True
        End If
#End If

        Dim rootFrame As Frame = TryCast(Window.Current.Content, Frame)

        ' 不要在窗口已包含内容时重复应用程序初始化，
        ' 只需确保窗口处于活动状态

        If rootFrame Is Nothing Then
            ' 创建要充当导航上下文的框架，并导航到第一页
            rootFrame = New Frame()

            AddHandler rootFrame.NavigationFailed, AddressOf OnNavigationFailed

            If e.PreviousExecutionState = ApplicationExecutionState.Terminated Then
                ' TODO: 从之前挂起的应用程序加载状态
            End If
            ' 将框架放在当前窗口中
            Window.Current.Content = rootFrame
        End If

        If e.PrelaunchActivated = False Then
            If rootFrame.Content Is Nothing Then
                ' 当导航堆栈尚未还原时，导航到第一页，
                ' 并通过将所需信息作为导航参数传入来配置
                ' 参数
                rootFrame.Navigate(GetType(MainPage), e.Arguments)
            End If

            ' 确保当前窗口处于活动状态
            Window.Current.Activate()
        End If
    End Sub

    ''' <summary>
    ''' 导航到特定页失败时调用
    ''' </summary>
    '''<param name="sender">导航失败的框架</param>
    '''<param name="e">有关导航失败的详细信息</param>
    Private Sub OnNavigationFailed(sender As Object, e As NavigationFailedEventArgs)
        Throw New Exception("Failed to load Page " + e.SourcePageType.FullName)
    End Sub

    ''' <summary>
    ''' 在将要挂起应用程序执行时调用。  在不知道应用程序
    ''' 无需知道应用程序会被终止还是会恢复，
    ''' 并让内存内容保持不变。
    ''' </summary>
    ''' <param name="sender">挂起的请求的源。</param>
    ''' <param name="e">有关挂起请求的详细信息。</param>
    Private Sub OnSuspending(sender As Object, e As SuspendingEventArgs) Handles Me.Suspending
        Dim deferral As SuspendingDeferral = e.SuspendingOperation.GetDeferral()
        ' TODO: 保存应用程序状态并停止任何后台活动
        deferral.Complete()
    End Sub

End Class
