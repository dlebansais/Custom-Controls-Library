namespace CustomControls;

using System.Windows.Input;

/// <summary>
/// Represents the <see cref="ActiveCommand"/> object for the No command.
/// </summary>
public class ActiveCommandNo : ActiveCommand
{
    /// <summary>
    /// Gets the neutral name of the <see cref="ActiveCommandNo"/> object.
    /// </summary>
    public override string Name => "No";

    /// <summary>
    /// Gets the localized name of the <see cref="ActiveCommandNo"/> object.
    /// </summary>
    public override string FriendlyName => "_No";

    /// <summary>
    /// Gets the routed command of the <see cref="ActiveCommandNo"/> object.
    /// </summary>
    public override RoutedUICommand Command => DialogValidation.DefaultCommandNo;
}
