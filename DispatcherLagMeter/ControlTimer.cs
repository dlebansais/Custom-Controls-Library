namespace CustomControls;

using System;
using System.Threading;

/// <summary>
/// Represents a timer.
/// </summary>
internal class ControlTimer : IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ControlTimer"/> class.
    /// </summary>
    /// <param name="timerCallback">The callback to execute.</param>
    public ControlTimer(Action timerCallback)
    {
        Timer = new(new TimerCallback((object? parameter) => timerCallback()));
    }

    /// <summary>
    /// Starts the timer.
    /// </summary>
    /// <param name="dueTime">The due time.</param>
    /// <param name="period">The period.</param>
    public void Start(TimeSpan dueTime, TimeSpan period)
    {
        _ = Timer.Change(dueTime, period);
    }

    /// <summary>
    /// Stops the timer.
    /// </summary>
    public void Stop()
    {
        _ = Timer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
    }

    /// <summary>
    /// Restarts the timer.
    /// </summary>
    /// <param name="oldInterval">The old interval.</param>
    /// <param name="newInterval">The new interval.</param>
    public void Restart(TimeSpan oldInterval, TimeSpan newInterval)
    {
        TimeSpan FirstSampling;
        if (newInterval < oldInterval)
        {
            FirstSampling = newInterval;
        }
        else if (newInterval == Timeout.InfiniteTimeSpan)
        {
            FirstSampling = newInterval;
        }
        else if (newInterval > oldInterval)
        {
            FirstSampling = newInterval - oldInterval;
        }
        else
            return;

        Stop();
        Start(FirstSampling, newInterval);
    }

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
                Timer.Dispose();
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

    private readonly Timer Timer;
    private bool disposedValue;
}
