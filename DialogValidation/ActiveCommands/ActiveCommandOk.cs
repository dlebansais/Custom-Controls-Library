namespace CustomControls;

/// <summary>
/// Represents the <see cref="ActiveCommand"/> object for the OK command with a flag to indicate a button associated to this command should be a default button.
/// </summary>
public class ActiveCommandOk : ActiveCommandOkBase
{
    /// <inheritdoc />
    public override bool IsDefault => true;
}