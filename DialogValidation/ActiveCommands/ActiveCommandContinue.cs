namespace CustomControls;

using System.Windows.Input;

/// <summary>
/// Represents the <see cref="ActiveCommand"/> object for the Continue command.
/// </summary>
public class ActiveCommandContinue : ActiveCommand
{
    /// <inheritdoc />
    public override string Name => "Continue";

    /// <inheritdoc />
    public override string FriendlyName => "_Continue";

    /// <inheritdoc />
    public override RoutedUICommand Command => DialogValidation.DefaultCommandContinue;
}
