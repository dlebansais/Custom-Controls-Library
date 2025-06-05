namespace CustomControls;

/// <summary>
/// Represents a command with information to find associated resources when a document is inactive.
/// </summary>
public class ActiveDocumentRoutedCommand : LocalizedRoutedCommand
{
    #region Properties
    /// <summary>
    /// Gets or sets the key to use to find a localized menu header in standard resource files when a document is inactive.
    /// </summary>
    public string InactiveHeaderKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the key to use to find a localized tooltip in standard resource files when a document is inactive.
    /// </summary>
    public string InactiveToolTipKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets the localized menu header when a document is inactive.
    /// </summary>
    public string InactiveMenuHeader => Reference.GetString(InactiveHeaderKey);

    /// <summary>
    /// Gets the localized tooltip when a document is inactive.
    /// </summary>
    public string InactiveButtonToolTip => Reference.GetString(InactiveToolTipKey);
    #endregion
}
