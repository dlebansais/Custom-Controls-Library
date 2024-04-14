namespace CustomControls;

using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;

/// <summary>
/// Represents a control that displays the dispatcher lag.
/// </summary>
public partial class DispatcherLagMeter
{
    #region SamplingInterval
    /// <summary>
    /// Identifies the <see cref="SamplingInterval"/> dependency property.
    /// </summary>
    /// <returns>The identifier for the <see cref="SamplingInterval"/> dependency property.</returns>
    public static readonly DependencyProperty SamplingIntervalProperty = DependencyProperty.Register(nameof(SamplingInterval), typeof(TimeSpan), typeof(DispatcherLagMeter), new FrameworkPropertyMetadata(Timeout.InfiniteTimeSpan, OnSamplingIntervalChanged));

    /// <summary>
    /// Gets or sets the sampling interval. The default value is <see cref="Timeout.InfiniteTimeSpan"/>.
    /// </summary>
    [Bindable(true)]
    public TimeSpan SamplingInterval
    {
        get { return (TimeSpan)GetValue(SamplingIntervalProperty); }
        set { SetValue(SamplingIntervalProperty, value); }
    }

    private static void OnSamplingIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
    {
        DispatcherLagMeter Ctrl = (DispatcherLagMeter)d;
        Ctrl.OnSamplingIntervalChanged(args);
    }

    private void OnSamplingIntervalChanged(DependencyPropertyChangedEventArgs args)
    {
        TimeSpan OldValue = (TimeSpan)args.OldValue;
        TimeSpan NewValue = (TimeSpan)args.NewValue;

        SamplingTimer.Restart(OldValue, NewValue);
    }
    #endregion

    #region NotificationInterval
    /// <summary>
    /// Identifies the <see cref="NotificationInterval"/> dependency property.
    /// </summary>
    /// <returns>The identifier for the <see cref="NotificationInterval"/> dependency property.</returns>
    public static readonly DependencyProperty NotificationIntervalProperty = DependencyProperty.Register(nameof(NotificationInterval), typeof(TimeSpan), typeof(DispatcherLagMeter), new FrameworkPropertyMetadata(Timeout.InfiniteTimeSpan, OnNotificationIntervalChanged));

    /// <summary>
    /// Gets or sets the notification interval. The default value is <see cref="Timeout.InfiniteTimeSpan"/>. The actual interval will be greater than or equal to <see cref="SamplingInterval"/>.
    /// </summary>
    [Bindable(true)]
    public TimeSpan NotificationInterval
    {
        get { return (TimeSpan)GetValue(NotificationIntervalProperty); }
        set { SetValue(NotificationIntervalProperty, value); }
    }

    private static void OnNotificationIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
    {
        DispatcherLagMeter Ctrl = (DispatcherLagMeter)d;
        Ctrl.OnNotificationIntervalChanged(args);
    }

    private void OnNotificationIntervalChanged(DependencyPropertyChangedEventArgs args)
    {
        TimeSpan OldValue = (TimeSpan)args.OldValue;
        TimeSpan NewValue = (TimeSpan)args.NewValue;

        NotificationTimer.Restart(OldValue, NewValue);
    }
    #endregion

    #region Edit Enter event
    /// <summary>
    /// Identifies the <see cref="LagMeasured"/> routed event.
    /// </summary>
    /// <returns>
    /// The identifier for the <see cref="LagMeasured"/> routed event.
    /// </returns>
    public static readonly RoutedEvent LagMeasuredEvent = EventManager.RegisterRoutedEvent(nameof(LagMeasured), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DispatcherLagMeter));

    /// <summary>
    /// Sent when the control has measured lag.
    /// </summary>
    public event RoutedEventHandler LagMeasured
    {
        add { AddHandler(LagMeasuredEvent, value); }
        remove { RemoveHandler(LagMeasuredEvent, value); }
    }

    /// <summary>
    /// Sends a <see cref="LagMeasured"/> event.
    /// </summary>
    /// <param name="lag">The dispatcher lag.</param>
    /// <param name="queueLength">The dispatcher queue length.</param>
    protected virtual void NotifyLagMeasured(double lag, double queueLength)
    {
        LagMeasuredEventArgs Args = CreateLagMeasuredEvent(lag, queueLength);
        Dispatcher.Invoke(() => RaiseEvent(Args));
    }

    /// <summary>
    /// Creates arguments for the <see cref="LagMeasured"/> routed event.
    /// </summary>
    /// <param name="lag">The dispatcher lag.</param>
    /// <param name="queueLength">The dispatcher queue length.</param>
    /// <returns>The <see cref="LagMeasuredEventArgs"/> object created.</returns>
    protected virtual LagMeasuredEventArgs CreateLagMeasuredEvent(double lag, double queueLength)
    {
        return new LagMeasuredEventArgs(LagMeasuredEvent, this, new DispatcherLag(DateTime.UtcNow, DateTime.Now, Clock.Elapsed, lag, queueLength));
    }
    #endregion

    #region DisplayInterval
    /// <summary>
    /// Identifies the <see cref="DisplayInterval"/> dependency property.
    /// </summary>
    /// <returns>The identifier for the <see cref="DisplayInterval"/> dependency property.</returns>
    public static readonly DependencyProperty DisplayIntervalProperty = DependencyProperty.Register(nameof(DisplayInterval), typeof(TimeSpan), typeof(DispatcherLagMeter), new FrameworkPropertyMetadata(Timeout.InfiniteTimeSpan, OnDisplayIntervalChanged));

    /// <summary>
    /// Gets or sets the display interval. The default value is <see cref="Timeout.InfiniteTimeSpan"/>. The actual interval will be greater than or equal to <see cref="SamplingInterval"/>.
    /// </summary>
    [Bindable(true)]
    public TimeSpan DisplayInterval
    {
        get { return (TimeSpan)GetValue(DisplayIntervalProperty); }
        set { SetValue(DisplayIntervalProperty, value); }
    }

    private static void OnDisplayIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
    {
        DispatcherLagMeter Ctrl = (DispatcherLagMeter)d;
        Ctrl.OnDisplayIntervalChanged(args);
    }

    private void OnDisplayIntervalChanged(DependencyPropertyChangedEventArgs args)
    {
        TimeSpan OldValue = (TimeSpan)args.OldValue;
        TimeSpan NewValue = (TimeSpan)args.NewValue;

        DisplayTimer.Restart(OldValue, NewValue);
    }
    #endregion

    #region DurationSensitivity
    /// <summary>
    /// Identifies the <see cref="DurationSensitivity"/> dependency property.
    /// </summary>
    /// <returns>The identifier for the <see cref="DurationSensitivity"/> dependency property.</returns>
    public static readonly DependencyProperty DurationSensitivityProperty = DependencyProperty.Register(nameof(DurationSensitivity), typeof(double), typeof(DispatcherLagMeter), new FrameworkPropertyMetadata(0.02));

    /// <summary>
    /// Gets or sets the duration sensitivity, in seconds. The minimum effective value is 0.01.
    /// </summary>
    [Bindable(true)]
    public double DurationSensitivity
    {
        get { return (double)GetValue(DurationSensitivityProperty); }
        set { SetValue(DurationSensitivityProperty, value); }
    }
    #endregion

    #region DurationFilterCutoff
    /// <summary>
    /// Identifies the <see cref="DurationFilterCutoff"/> dependency property.
    /// </summary>
    /// <returns>The identifier for the <see cref="DurationFilterCutoff"/> dependency property.</returns>
    public static readonly DependencyProperty DurationFilterCutoffProperty = DependencyProperty.Register(nameof(DurationFilterCutoff), typeof(double), typeof(DispatcherLagMeter), new FrameworkPropertyMetadata(10.0));

    /// <summary>
    /// Gets or sets the duration filtering cutoff. The minimum effective value is 1.0.
    /// </summary>
    [Bindable(true)]
    public double DurationFilterCutoff
    {
        get { return (double)GetValue(DurationFilterCutoffProperty); }
        set { SetValue(DurationFilterCutoffProperty, value); }
    }
    #endregion

    #region QueueLengthSensitivity
    /// <summary>
    /// Identifies the <see cref="QueueLengthSensitivity"/> dependency property.
    /// </summary>
    /// <returns>The identifier for the <see cref="QueueLengthSensitivity"/> dependency property.</returns>
    public static readonly DependencyProperty QueueLengthSensitivityProperty = DependencyProperty.Register(nameof(QueueLengthSensitivity), typeof(double), typeof(DispatcherLagMeter), new FrameworkPropertyMetadata(2.0));

    /// <summary>
    /// Gets or sets the queue length sensitivity. The minimum effective value is 1.0.
    /// </summary>
    [Bindable(true)]
    public double QueueLengthSensitivity
    {
        get { return (double)GetValue(QueueLengthSensitivityProperty); }
        set { SetValue(QueueLengthSensitivityProperty, value); }
    }
    #endregion

    #region QueueLengthFilterCutoff
    /// <summary>
    /// Identifies the <see cref="QueueLengthFilterCutoff"/> dependency property.
    /// </summary>
    /// <returns>The identifier for the <see cref="QueueLengthFilterCutoff"/> dependency property.</returns>
    public static readonly DependencyProperty QueueLengthFilterCutoffProperty = DependencyProperty.Register(nameof(QueueLengthFilterCutoff), typeof(double), typeof(DispatcherLagMeter), new FrameworkPropertyMetadata(10.0));

    /// <summary>
    /// Gets or sets the queue length filtering cutoff. The minimum effective value is 1.0.
    /// </summary>
    [Bindable(true)]
    public double QueueLengthFilterCutoff
    {
        get { return (double)GetValue(QueueLengthFilterCutoffProperty); }
        set { SetValue(QueueLengthFilterCutoffProperty, value); }
    }
    #endregion
}
