namespace CustomControls;

using System.Windows.Input;

/// <summary>
/// Represents the <see cref="ActiveCommand"/> object for the Yes command.
/// </summary>
public class ActiveCommandYes : ActiveCommand
{
    /// <inheritdoc />
    public override string Name => "Yes";

    /// <inheritdoc />
    public override string FriendlyName => "_Yes";

    /// <inheritdoc />
    public override RoutedUICommand Command => DialogValidation.DefaultCommandYes;
}