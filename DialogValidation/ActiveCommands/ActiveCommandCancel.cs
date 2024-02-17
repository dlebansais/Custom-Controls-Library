namespace CustomControls;

/// <summary>
/// Represents the <see cref="ActiveCommand"/> object for the Cancel command with a flag to indicate a button associated to this command should be a cancel button.
/// </summary>
public class ActiveCommandCancel : ActiveCommandCancelBase
{
    /// <summary>
    /// Gets a value indicating whether a button using this command should be a cancel button.
    /// </summary>
    public override bool IsCancel { get { return true; } }
}