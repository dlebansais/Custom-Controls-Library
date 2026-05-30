namespace CustomControls;

using System.Windows.Input;

/// <summary>
/// Represents the <see cref="ActiveCommand"/> object for the Ignore command.
/// </summary>
public class ActiveCommandIgnore : ActiveCommand
{
    /// <inheritdoc />
    public override string Name => "Ignore";

    /// <inheritdoc />
    public override string FriendlyName => "_Ignore";

    /// <inheritdoc />
    public override RoutedUICommand Command => DialogValidation.DefaultCommandIgnore;
}
