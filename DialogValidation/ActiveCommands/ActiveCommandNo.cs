namespace CustomControls;

using System.Windows.Input;

/// <summary>
/// Represents the <see cref="ActiveCommand"/> object for the No command.
/// </summary>
public class ActiveCommandNo : ActiveCommand
{
    /// <inheritdoc />
    public override string Name => "No";

    /// <inheritdoc />
    public override string FriendlyName => "_No";

    /// <inheritdoc />
    public override RoutedUICommand Command => DialogValidation.DefaultCommandNo;
}
