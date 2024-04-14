namespace CustomControls;

using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Contracts;

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

    private static byte ToByteColor(double x)
    {
        return (byte)Math.Max(Math.Min(255, x * 255), 0);
    }

    /// <inheritdoc/>
    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);

        DrawingContext DrawingContext = Contract.AssertNotNull(drawingContext);

        double Length = Math.Min(ActualWidth, ActualHeight);
        Point Center = new(ActualWidth / 2, ActualHeight / 2);
        double RadiusX = Length / 8;
        double RadiusY = Length / 2;

        const double MaxAngle = 90;
        double Angle = (1 - (1 / (1 + (LastLag / Math.Max(DurationSensitivity, 0.01))))) * MaxAngle;

        const double BaseLength = 6.0;
        const double MaxReddening = 1.0;
        double Reddening = (1.0 - (1.0 / (1.0 + (Math.Max(0, LastQueueLength - BaseLength) / Math.Max(QueueLengthSensitivity, 1.0))))) * MaxReddening;

        double Red = 2.0 * Reddening;
        double Green = 2.0 * (1 - Reddening);
        const double Blue = 0;
        Brush Brush = new SolidColorBrush(Color.FromRgb(ToByteColor(Red), ToByteColor(Green), ToByteColor(Blue)));

        DrawingContext.PushTransform(new TranslateTransform(Center.X, Center.Y));
        DrawingContext.PushTransform(new RotateTransform(Angle));
        DrawingContext.DrawEllipse(Brush, null, new Point(0, 0), RadiusX, RadiusY);
        DrawingContext.Pop();
        DrawingContext.Pop();
    }

    private void DisplayTimerCallback()
    {
        Dispatcher.Invoke(InvalidateVisual);
    }

    private readonly ControlTimer SamplingTimer;
    private readonly ControlTimer NotificationTimer;
    private readonly ControlTimer DisplayTimer;
    private DispatcherOperation? SamplingOperation;
    private DispatcherOperation? NotificationOperation;
    private int DispatcherQueueLength;
    private int BaseQueueLength;
    private TimeSpan LastSampling = TimeSpan.Zero;
    private double LastLag;
    private double LastQueueLength;
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
