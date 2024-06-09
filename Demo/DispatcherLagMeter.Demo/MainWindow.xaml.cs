namespace DispatcherLagMeter.Demo;

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
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

        QueueLagOperation();
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
        Debug.WriteLine($"OnLagMeasured: {Math.Round(Args.DispatcherLag.Lag * 1000)}, {Math.Round(Args.DispatcherLag.QueueLength, 1)}");
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

    private bool IsClosing;
}
