﻿namespace CustomControls;

using System.Windows.Input;

/// <summary>
/// Represents the <see cref="ActiveCommand"/> object for the Help command.
/// </summary>
public class ActiveCommandHelp : ActiveCommand
{
    /// <summary>
    /// Gets the neutral name of the <see cref="ActiveCommandHelp"/> object.
    /// </summary>
    public override string Name => "Help";

    /// <summary>
    /// Gets the localized name of the <see cref="ActiveCommandHelp"/> object.
    /// </summary>
    public override string FriendlyName => "Help";

    /// <summary>
    /// Gets the routed command of the <see cref="ActiveCommandHelp"/> object.
    /// </summary>
    public override RoutedUICommand Command => DialogValidation.DefaultCommandHelp;
}
