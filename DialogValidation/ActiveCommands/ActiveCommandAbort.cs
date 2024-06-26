﻿namespace CustomControls;

using System.Windows.Input;

/// <summary>
/// Represents the <see cref="ActiveCommand"/> object for the Abort command.
/// </summary>
public class ActiveCommandAbort : ActiveCommand
{
    /// <summary>
    /// Gets the neutral name of the <see cref="ActiveCommandAbort"/> object.
    /// </summary>
    public override string Name => "Abort";

    /// <summary>
    /// Gets the localized name of the <see cref="ActiveCommandAbort"/> object.
    /// </summary>
    public override string FriendlyName => "_Abort";

    /// <summary>
    /// Gets the routed command of the <see cref="ActiveCommandAbort"/> object.
    /// </summary>
    public override RoutedUICommand Command => DialogValidation.DefaultCommandAbort;
}
