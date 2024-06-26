﻿namespace CustomControls;

using System.Windows.Input;

/// <summary>
/// Represents the <see cref="ActiveCommand"/> object for the Try Again command.
/// </summary>
public class ActiveCommandTryAgain : ActiveCommand
{
    /// <summary>
    /// Gets the neutral name of the <see cref="ActiveCommandTryAgain"/> object.
    /// </summary>
    public override string Name => "TryAgain";

    /// <summary>
    /// Gets the localized name of the <see cref="ActiveCommandTryAgain"/> object.
    /// </summary>
    public override string FriendlyName => "_Try Again";

    /// <summary>
    /// Gets the routed command of the <see cref="ActiveCommandTryAgain"/> object.
    /// </summary>
    public override RoutedUICommand Command => DialogValidation.DefaultCommandTryAgain;
}