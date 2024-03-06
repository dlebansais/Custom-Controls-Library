namespace CustomControls;

using System.Windows.Input;

/// <summary>
/// Represents the <see cref="ActiveCommand"/> object for the Yes command.
/// </summary>
public class ActiveCommandYes : ActiveCommand
{
    /// <summary>
    /// Gets the neutral name of the <see cref="ActiveCommandYes"/> object.
    /// </summary>
    public override string Name => "Yes";

    /// <summary>
    /// Gets the localized name of the <see cref="ActiveCommandYes"/> object.
    /// </summary>
    public override string FriendlyName => "_Yes";

    /// <summary>
    /// Gets the routed command of the <see cref="ActiveCommandYes"/> object.
    /// </summary>
    public override RoutedUICommand Command => DialogValidation.DefaultCommandYes;
}