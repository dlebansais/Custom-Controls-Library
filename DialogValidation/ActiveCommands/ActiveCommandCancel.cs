namespace CustomControls;

/// <summary>
/// Represents the <see cref="ActiveCommand"/> object for the Cancel command with a flag to indicate a button associated to this command should be a cancel button.
/// </summary>
public class ActiveCommandCancel : ActiveCommandCancelBase
{
    /// <inheritdoc />
    public override bool IsCancel => true;
}
