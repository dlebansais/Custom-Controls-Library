namespace CustomControls;

using System.Windows.Input;

/// <summary>
/// Represents the <see cref="ActiveCommand"/> object for the Help command.
/// </summary>
public class ActiveCommandHelp : ActiveCommand
{
    /// <inheritdoc />
    public override string Name => "Help";

    /// <inheritdoc />
    public override string FriendlyName => "Help";

    /// <inheritdoc />
    public override RoutedUICommand Command => DialogValidation.DefaultCommandHelp;
}
