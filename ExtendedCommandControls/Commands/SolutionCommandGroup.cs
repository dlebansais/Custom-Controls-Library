﻿namespace CustomControls;

/// <summary>
/// Represents an object that indicates if a feature is enabled, and therefore if controls associated to this feature should be shown.
/// </summary>
public class SolutionCommandGroup
{
    /// <summary>
    /// Gets or sets a value indicating whether the feature is enabled.
    /// True, the feature is enabled and commands associated to that feature should be used.
    /// False, the feature is disabled and commands associated to that feature should not be used.
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// Gets the default <see cref="SolutionCommandGroup"/>.
    /// </summary>
    public static SolutionCommandGroup Default { get; } = new();
}
