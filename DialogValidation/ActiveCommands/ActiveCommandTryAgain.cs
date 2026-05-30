namespace CustomControls;

using System.Windows.Input;

/// <summary>
/// Represents the <see cref="ActiveCommand"/> object for the Try Again command.
/// </summary>
public class ActiveCommandTryAgain : ActiveCommand
{
    /// <inheritdoc />
    public override string Name => "TryAgain";

    /// <inheritdoc />
    public override string FriendlyName => "_Try Again";

    /// <inheritdoc />
    public override RoutedUICommand Command => DialogValidation.DefaultCommandTryAgain;
}