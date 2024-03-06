namespace CustomControls;

using System.Windows.Input;

/// <summary>
/// Represents the <see cref="ActiveCommand"/> object for the Close command.
/// </summary>
public class ActiveCommandClose : ActiveCommand
{
    /// <summary>
    /// Gets the neutral name of the <see cref="ActiveCommandClose"/> object.
    /// </summary>
    public override string Name => "Close";

    /// <summary>
    /// Gets the localized name of the <see cref="ActiveCommandClose"/> object.
    /// </summary>
    public override string FriendlyName => "_Close";

    /// <summary>
    /// Gets the routed command of the <see cref="ActiveCommandClose"/> object.
    /// </summary>
    public override RoutedUICommand Command => DialogValidation.DefaultCommandClose;
}
