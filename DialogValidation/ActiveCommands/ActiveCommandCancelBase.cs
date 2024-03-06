namespace CustomControls;

using System.Windows.Input;

/// <summary>
/// Represents the <see cref="ActiveCommand"/> object for the Cancel command.
/// </summary>
public class ActiveCommandCancelBase : ActiveCommand
{
    /// <summary>
    /// Gets the neutral name of the <see cref="ActiveCommandCancelBase"/> object.
    /// </summary>
    public override string Name => "Cancel";

    /// <summary>
    /// Gets the localized name of the <see cref="ActiveCommandCancelBase"/> object.
    /// </summary>
    public override string FriendlyName => "Cancel";

    /// <summary>
    /// Gets the routed command of the <see cref="ActiveCommandCancelBase"/> object.
    /// </summary>
    public override RoutedUICommand Command => DialogValidation.DefaultCommandCancel;
}
