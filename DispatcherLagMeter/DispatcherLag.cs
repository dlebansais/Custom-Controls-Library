namespace CustomControls;

using System;

/// <summary>
/// Represents the lag measured lag of the dispatcher.
/// </summary>
/// <param name="UtcTime">The time when the event was sent (UTC).</param>
/// <param name="LocalTime">The time when the event was sent (local).</param>
/// <param name="ElapsedSinceLastNotification">The time elapsed since the last notification, monotonic.</param>
/// <param name="Lag">The average last measured lag, in seconds.</param>
/// <param name="QueueLength">The average number of pending dispatcher operations (always 0 or greater).</param>
public record DispatcherLag(DateTime UtcTime, DateTime LocalTime, TimeSpan ElapsedSinceLastNotification, double Lag, double QueueLength)
{
}
