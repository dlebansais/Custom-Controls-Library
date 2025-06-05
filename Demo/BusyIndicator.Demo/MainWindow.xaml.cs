namespace BusyIndicatorDemo;

using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

/// <summary>
/// Main window of the BusyIndicator demo program.
/// </summary>
internal partial class MainWindow : Window, IDisposable
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

    private void StopTimerCallback(object? parameter) => _ = Dispatcher.BeginInvoke(OnStopTimer);

    private void OnStopTimer() => Close();

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
