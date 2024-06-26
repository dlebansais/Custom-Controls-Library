﻿namespace CustomControls;

using System.Windows.Input;

/// <summary>
/// Represents the <see cref="ActiveCommand"/> object for the Ignore command.
/// </summary>
public class ActiveCommandIgnore : ActiveCommand
{
    /// <summary>
    /// Gets the neutral name of the <see cref="ActiveCommandIgnore"/> object.
    /// </summary>
    public override string Name => "Ignore";

    /// <summary>
    /// Gets the localized name of the <see cref="ActiveCommandIgnore"/> object.
    /// </summary>
    public override string FriendlyName => "_Ignore";

    /// <summary>
    /// Gets the routed command of the <see cref="ActiveCommandIgnore"/> object.
    /// </summary>
    public override RoutedUICommand Command => DialogValidation.DefaultCommandIgnore;
}
