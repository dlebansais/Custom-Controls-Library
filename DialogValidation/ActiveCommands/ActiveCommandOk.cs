namespace CustomControls;

/// <summary>
/// Represents the <see cref="ActiveCommand"/> object for the OK command with a flag to indicate a button associated to this command should be a default button.
/// </summary>
public class ActiveCommandOk : ActiveCommandOkBase
{
    /// <summary>
    /// Gets a value indicating whether a button using this command should be a default button.
    /// </summary>
    public override bool IsDefault { get { return true; } }
}