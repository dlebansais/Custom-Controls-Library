namespace CustomControls;

using System.Windows.Input;

/// <summary>
/// Represents the <see cref="ActiveCommand"/> object for the Abort command.
/// </summary>
public class ActiveCommandAbort : ActiveCommand
{
    /// <inheritdoc />
    public override string Name => "Abort";

    /// <inheritdoc />
    public override string FriendlyName => "_Abort";

    /// <inheritdoc />
    public override RoutedUICommand Command => DialogValidation.DefaultCommandAbort;
}
