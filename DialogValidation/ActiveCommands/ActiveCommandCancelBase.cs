namespace CustomControls;

using System.Windows.Input;

/// <summary>
/// Represents the <see cref="ActiveCommand"/> object for the Cancel command.
/// </summary>
public class ActiveCommandCancelBase : ActiveCommand
{
    /// <inheritdoc />
    public override string Name => "Cancel";

    /// <inheritdoc />
    public override string FriendlyName => "Cancel";

    /// <inheritdoc />
    public override RoutedUICommand Command => DialogValidation.DefaultCommandCancel;
}
