namespace CustomControls;

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Threading;

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

        NativeMethods.InstallHook(Owner, RecalculatePositionAndSize);
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
        NativeMethods.GetClientArea(Root, out Point WindowPosition, out int ClientAreaOffsetX, out int ClientAreaOffsetY);
        Point ControlRelativeCoordinates = new(ControlScreenCoordinates.X - WindowPosition.X + ClientAreaOffsetX, ControlScreenCoordinates.Y - WindowPosition.Y + ClientAreaOffsetY - ClientAreaOffsetX);

        const double scalingFactor = 1;

        Display.Left = Root.Left + (ControlRelativeCoordinates.X * scalingFactor);
        Display.Top = Root.Top + (ControlRelativeCoordinates.Y * scalingFactor);
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

    private readonly DisplayWindow Display;
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
}
