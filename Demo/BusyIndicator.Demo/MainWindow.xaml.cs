namespace TestBusyIndicator;

using System;
using System.Threading;
using System.Windows;

/// <summary>
/// Main window of the BusyIndicatorDemo program.
/// </summary>
public partial class MainWindow : Window, IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;

        StopTimer = new Timer(new TimerCallback(StopTimerCallback), this, TimeSpan.FromSeconds(5), Timeout.InfiniteTimeSpan);
    }

    private void StopTimerCallback(object? parameter)
    {
        _ = Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(OnStopTimer));
    }

    private void OnStopTimer()
    {
        Close();
    }

    /// <summary>
    /// Disposes of managed and unmanaged resources.
    /// </summary>
    /// <param name="disposing"><see langword="True"/> if the method should dispose of resources; Otherwise, <see langword="false"/>.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                StopTimer.Dispose();
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

    private readonly Timer StopTimer;
    private bool disposedValue;
}
