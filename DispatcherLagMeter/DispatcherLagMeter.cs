namespace CustomControls;

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using Contracts;

/// <summary>
/// Misc.
/// </summary>
internal enum WindowsMessage
{
    /// <summary>Sent after a window has been moved.</summary>
    WM_MOVE = 0x0003,
    /// <summary>
    /// Sent to a window when the size or position of the window is about to change.
    /// An application can use this message to override the window's default maximized size and position,
    /// or its default minimum or maximum tracking size.
    /// </summary>
    WM_GETMINMAXINFO = 0x0024,
    /// <summary>
    /// Sent to a window whose size, position, or place in the Z order is about to change as a result
    /// of a call to the SetWindowPos function or another window-management function.
    /// </summary>
    WM_WINDOWPOSCHANGING = 0x0046,
    /// <summary>
    /// Sent to a window whose size, position, or place in the Z order has changed as a result of a
    /// call to the SetWindowPos function or another window-management function.
    /// </summary>
    WM_WINDOWPOSCHANGED = 0x0047,
    /// <summary>
    /// Sent to a window that the user is moving. By processing this message, an application can monitor
    /// the position of the drag rectangle and, if needed, change its position.
    /// </summary>
    WM_MOVING = 0x0216,
    /// <summary>
    /// Sent once to a window after it enters the moving or sizing modal loop. The window enters the
    /// moving or sizing modal loop when the user clicks the window's title bar or sizing border, or
    /// when the window passes the WM_SYSCOMMAND message to the DefWindowProc function and the wParam
    /// parameter of the message specifies the SC_MOVE or SC_SIZE value. The operation is complete
    /// when DefWindowProc returns.
    /// <para />
    /// The system sends the WM_ENTERSIZEMOVE message regardless of whether the dragging of full windows
    /// is enabled.
    /// </summary>
    WM_ENTERSIZEMOVE = 0x0231,
    /// <summary>
    /// Sent once to a window once it has exited moving or sizing modal loop. The window enters the
    /// moving or sizing modal loop when the user clicks the window's title bar or sizing border, or
    /// when the window passes the WM_SYSCOMMAND message to the DefWindowProc function and the
    /// wParam parameter of the message specifies the SC_MOVE or SC_SIZE value. The operation is
    /// complete when DefWindowProc returns.
    /// </summary>
    WM_EXITSIZEMOVE = 0x0232,
}

/// <summary>
/// Represents a control that displays the dispatcher lag.
/// </summary>
public partial class DispatcherLagMeter : UserControl, IDisposable
{
    #region Init
    /// <summary>
    /// Initializes a new instance of the <see cref="DispatcherLagMeter"/> class.
    /// </summary>
    public DispatcherLagMeter()
    {
        SamplingTimer = new(SamplingTimerCallback);
        NotificationTimer = new(NotificationTimerCallback);
        DisplayTimer = new(DisplayTimerCallback);

        Dispatcher.Hooks.OperationPosted += (s, e) => Interlocked.Increment(ref DispatcherQueueLength);
        Dispatcher.Hooks.OperationCompleted += (s, e) => Interlocked.Decrement(ref DispatcherQueueLength);
        Dispatcher.Hooks.OperationAborted += (s, e) => Interlocked.Decrement(ref DispatcherQueueLength);

        Clock.Start();

        Dispatcher.ShutdownStarted += OnShutdownStarted;

        Display = new(this);
        Display.Width = 0;
        Display.Height = 0;
        Display.Show();

        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, EventArgs e)
    {
        Window Owner = Window.GetWindow(this);
        Display.Owner = Owner;

        IntPtr mainWindowPtr = new WindowInteropHelper(Owner).Handle;
        HwndSource mainWindowSrc = HwndSource.FromHwnd(mainWindowPtr);
        mainWindowSrc.AddHook(WndProc);

        SizeChanged += OnSizeChanged;

        _ = Dispatcher.BeginInvoke(RecalculatePositionAndSize);
    }

    private static double FromBool(bool value)
    {
        return value ? 1.0 : 0.0;
    }

    private void RecalculatePositionAndSize()
    {
        double Visibility = FromBool(true) * FromBool(!DesignerProperties.GetIsInDesignMode(this));
        Display.Width = ActualWidth * Visibility;
        Display.Height = ActualHeight * Visibility;

        Point ControlScreenCoordinates = PointToScreen(new Point(0, 0));
        Window Root = Window.GetWindow(this);
        IntPtr mainWindowPtr = new WindowInteropHelper(Root).Handle;
        HwndSource mainWindowSrc = HwndSource.FromHwnd(mainWindowPtr);
        NativePoint lpPoint = default;
        ClientToScreen(mainWindowSrc.Handle, ref lpPoint);
        NativeRect rcRect = default;
        GetClientRect(mainWindowSrc.Handle, ref rcRect);
        NativeRect rcWinRect = default;
        GetWindowRect(mainWindowSrc.Handle, ref rcWinRect);

        int ClientX = ((rcWinRect.Right - rcWinRect.Left) - (rcRect.Right - rcRect.Left)) / 2;
        int ClientY = (rcWinRect.Bottom - rcWinRect.Top) - (rcRect.Bottom - rcRect.Top);

        Point ControlRelativeCoordinates = new(ControlScreenCoordinates.X - lpPoint.X + ClientX, ControlScreenCoordinates.Y - lpPoint.Y + ClientY - ClientX);

        const double scalingFactor = 1;

        Display.Left = Root.Left + (ControlRelativeCoordinates.X * scalingFactor);
        Display.Top = Root.Top + (ControlRelativeCoordinates.Y * scalingFactor);

        // Debug.WriteLine($"Left: {Display.Left}, Top: {Display.Top}, Width: {Display.Width}, Height: {Display.Height}");
    }

    private void OnSizeChanged(object sender, RoutedEventArgs e)
    {
        _ = Dispatcher.BeginInvoke(RecalculatePositionAndSize);
    }

    private void OnShutdownStarted(object? sender, EventArgs e)
    {
        SamplingTimer.Stop();
        NotificationTimer.Stop();
        DisplayTimer.Stop();

        Display.Close();
    }
    #endregion

    #region Timer
    private void SamplingTimerCallback()
    {
        if (SamplingOperation is null || SamplingOperation.Status == DispatcherOperationStatus.Completed)
        {
            LastSampling = Clock.Elapsed;
            SamplingOperation = Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new Action(OnSampleTimerCalled));
        }
    }

    private void OnSampleTimerCalled()
    {
        double Elapsed = (Clock.Elapsed - LastSampling).TotalSeconds;

        double Cutoff = Math.Max(1.0, DurationFilterCutoff);
        LastLag = ((LastLag * (Cutoff - 1.0)) + Elapsed) / Cutoff;
    }

    private void NotificationTimerCallback()
    {
        if (NotificationOperation is null || NotificationOperation.Status == DispatcherOperationStatus.Completed)
        {
            // Takes into account existing queued operations when this instance was created: DispatcherQueueLength can be negative.
            int LastDispatcherQueueLength = DispatcherQueueLength;
            if (BaseQueueLength > LastDispatcherQueueLength)
                BaseQueueLength = LastDispatcherQueueLength;

            int QueueLength = LastDispatcherQueueLength - BaseQueueLength;
            NotificationOperation = Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new Action<int>(OnNotificationTimerCalled), QueueLength);
        }
    }

    private void OnNotificationTimerCalled(int queueLength)
    {
        double Cutoff = Math.Max(1.0, QueueLengthFilterCutoff);
        LastQueueLength = ((LastQueueLength * (Cutoff - 1.0)) + queueLength) / Cutoff;

        NotifyLagMeasured(LastLag, LastQueueLength);
    }

    private void DisplayTimerCallback()
    {
        // Dispatcher.Invoke(InvalidateVisual);
        Display.Update();
    }

    /// <summary>
    /// Gets the last lag value.
    /// </summary>
    internal double LastLag { get; private set; }

    /// <summary>
    /// Gets the last queue length value.
    /// </summary>
    internal double LastQueueLength { get; private set; }

    private readonly ControlTimer SamplingTimer;
    private readonly ControlTimer NotificationTimer;
    private readonly ControlTimer DisplayTimer;
    private DispatcherOperation? SamplingOperation;
    private DispatcherOperation? NotificationOperation;
    private int DispatcherQueueLength;
    private int BaseQueueLength;
    private TimeSpan LastSampling = TimeSpan.Zero;
    private readonly Stopwatch Clock = new();
    #endregion

    #region Implementation of IDisposable
    private bool disposedValue;

    /// <summary>
    /// Disposes of resources.
    /// </summary>
    /// <param name="disposing"><see langword="true"/> to dispose of resources now.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                Dispatcher.ShutdownStarted -= OnShutdownStarted;

                SamplingTimer.Dispose();
                NotificationTimer.Dispose();
                DisplayTimer.Dispose();
            }

            disposedValue = true;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion

    private readonly DisplayWindow Display;

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    private static extern IntPtr DefWindowProc(
        IntPtr hWnd,
        int msg,
        IntPtr wParam,
        IntPtr lParam);

    [StructLayout(LayoutKind.Sequential)]
    private struct NativePoint
    {
        public int X;
        public int Y;
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    private static extern void ClientToScreen(IntPtr hWnd, ref NativePoint lpPoint);

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeRect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    private static extern void GetClientRect(IntPtr hWnd, ref NativeRect lpRect);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    private static extern void GetWindowRect(IntPtr hWnd, ref NativeRect lpRect);

    [StructLayout(LayoutKind.Sequential)]
    private struct WINDOWPOS
    {
        public IntPtr Hwnd;
        public IntPtr HwndInsertAfter;
        public int X;
        public int Y;
        public int CX;
        public int CY;
        public int Flags;
    }

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == (uint)WindowsMessage.WM_WINDOWPOSCHANGED)
        {
            _ = DefWindowProc(hwnd, msg, wParam, lParam);

            WINDOWPOS wp = (WINDOWPOS)System.Runtime.InteropServices.Marshal.PtrToStructure(lParam, typeof(WINDOWPOS))!;
            const int SWP_NOMOVE = 0x0002;

            if ((wp.Flags & SWP_NOMOVE) == 0)
                RecalculatePositionAndSize();
        }

        return IntPtr.Zero;
    }
}
