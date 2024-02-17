namespace CustomControls;

using System.Windows.Input;

/// <summary>Represents the <see cref="ActiveCommand"/> object for the Cancel command.</summary>
public class ActiveCommandCancelBase : ActiveCommand
{
    /// <summary>Gets the neutral name of the <see cref="ActiveCommandCancelBase"/> object.</summary>
    public override string Name { get { return "Cancel"; } }

    /// <summary>Gets the localized name of the <see cref="ActiveCommandCancelBase"/> object.</summary>
    public override string FriendlyName { get { return "Cancel"; } }

    /// <summary>Gets the routed command of the <see cref="ActiveCommandCancelBase"/> object.</summary>
    public override RoutedUICommand Command { get { return DialogValidation.DefaultCommandCancel; } }
}
