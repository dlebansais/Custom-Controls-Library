namespace CustomControls;

using System.Windows.Input;

/// <summary>
/// Represents the <see cref="ActiveCommand"/> object for the OK command.
/// </summary>
public class ActiveCommandOkBase : ActiveCommand
{
    /// <inheritdoc />
    public override string Name => "Ok";

    /// <inheritdoc />
    public override string FriendlyName => "OK";

    /// <inheritdoc />
    public override RoutedUICommand Command => DialogValidation.DefaultCommandOk;
}
