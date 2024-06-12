namespace DispatcherLagMeter.Demo;

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using CustomControls;

/// <summary>
/// Interaction logic for MainWindow.xaml.
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;

        if (TestEscape == 2)
            _ = Dispatcher.BeginInvoke(StartTest2);
        else
            Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (TestEscape == 1)
        {
            using (TestDisposeWindow Dlg = new())
            {
                _ = Dlg.ShowDialog();
            }

            await Task.Delay(TimeSpan.FromSeconds(18)).ConfigureAwait(true);
            Close();
        }
        else if (TestEscape == 3)
        {
            _ = Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, Resize3);
        }
        else
            QueueLagOperation();
    }

    private async void Resize3()
    {
        await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(true);
        SizeToContent = SizeToContent.Manual;
        ResizeMode = ResizeMode.CanResize;
        Width = ActualWidth + 10;
    }

    /// <summary>
    /// Gets or sets the duration.
    /// </summary>
    public double Duration
    {
        get => DurationField;
        set
        {
            if (DurationField != value)
            {
                DurationField = value;
            }
        }
    }

    private double DurationField;

    /// <summary>
    /// Gets or sets the count.
    /// </summary>
    public double Count
    {
        get => CountField;
        set
        {
            if (CountField != value)
            {
                CountField = value;
            }
        }
    }

    private double CountField = 1.0;

    /// <summary>
    /// Gets or sets the test escape.
    /// </summary>
    public static int TestEscape { get; set; }

    private void QueueLagOperation()
    {
        QueuedOperations++;
        _ = Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(LagGeneration));
    }

    private void LagGeneration()
    {
        QueuedOperations--;

        if (IsClosing)
        {
            if (QueuedOperations == 0)
                Close();
        }
        else
        {
            Thread.Sleep((int)Duration);

            while (QueuedOperations < Count)
                QueueLagOperation();
        }
    }

    private int QueuedOperations;

    private void OnLagMeasured(object sender, RoutedEventArgs e)
    {
        LagMeasuredEventArgs Args = (LagMeasuredEventArgs)e;
        Debug.WriteLine($"OnLagMeasured: {Math.Round(Args.DispatcherLag.Lag * 1000).ToString(CultureInfo.InvariantCulture)}, {Math.Round(Args.DispatcherLag.QueueLength, 1).ToString(CultureInfo.InvariantCulture)}");
    }

    private void OnClosing(object sender, CancelEventArgs e)
    {
        if (QueuedOperations > 0)
        {
            // Only close when no operation is pending.
            e.Cancel = true;
            IsClosing = true;
        }
    }

    private async void StartTest2()
    {
        ctrl.LagMeasured -= OnLagMeasured;

        ctrl.SamplingInterval = TimeSpan.FromSeconds(10);
        Debug.Assert(ctrl.SamplingInterval == TimeSpan.FromSeconds(10));

        ctrl.NotificationInterval = TimeSpan.FromSeconds(10);
        Debug.Assert(ctrl.NotificationInterval == TimeSpan.FromSeconds(10));

        ctrl.DurationSensitivity = 1.0;
        Debug.Assert(ctrl.DurationSensitivity == 1.0);

        ctrl.DurationFilterCutoff = 1.0;
        Debug.Assert(ctrl.DurationFilterCutoff == 1.0);

        ctrl.QueueLengthSensitivity = 1.0;
        Debug.Assert(ctrl.QueueLengthSensitivity == 1.0);

        ctrl.QueueLengthFilterCutoff = 1.0;
        Debug.Assert(ctrl.QueueLengthFilterCutoff == 1.0);

        ctrl.DisplayInterval = TimeSpan.FromSeconds(10);
        Debug.Assert(ctrl.DisplayInterval == TimeSpan.FromSeconds(10));

        await Task.Delay(TimeSpan.FromSeconds(0.1)).ConfigureAwait(true);
        ctrl.DisplayInterval = TimeSpan.FromSeconds(1);
        await Task.Delay(TimeSpan.FromSeconds(0.1)).ConfigureAwait(true);
        ctrl.DisplayInterval = TimeSpan.FromSeconds(1);
        await Task.Delay(TimeSpan.FromSeconds(0.1)).ConfigureAwait(true);
        ctrl.DisplayInterval = Timeout.InfiniteTimeSpan;
    }

    private bool IsClosing;
}
