namespace CustomControls;

using System.Windows.Input;

/// <summary>
/// Represents the <see cref="ActiveCommand"/> object for the Close command.
/// </summary>
public class ActiveCommandClose : ActiveCommand
{
    /// <inheritdoc />
    public override string Name => "Close";

    /// <inheritdoc />
    public override string FriendlyName => "_Close";

    /// <inheritdoc />
    public override RoutedUICommand Command => DialogValidation.DefaultCommandClose;
}
