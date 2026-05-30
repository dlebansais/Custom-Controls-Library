namespace CustomControls;

using System.Windows.Input;

/// <summary>
/// Represents the <see cref="ActiveCommand"/> object for the Retry command.
/// </summary>
public class ActiveCommandRetry : ActiveCommand
{
    /// <inheritdoc />
    public override string Name => "Retry";

    /// <inheritdoc />
    public override string FriendlyName => "_Retry";

    /// <inheritdoc />
    public override RoutedUICommand Command => DialogValidation.DefaultCommandRetry;
}