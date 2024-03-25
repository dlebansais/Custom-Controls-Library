namespace CustomControls;

using System.Windows;
using System.Windows.Controls;

/// <summary>
/// Represents a tool bar with a localized name.
/// </summary>
public partial class ExtendedToolBar : ToolBar
{
    #region ToolBar Name
    /// <summary>
    /// Identifies the <see cref="ToolBarName"/> dependency property.
    /// </summary>
    /// <returns>
    /// The identifier for the <see cref="ToolBarName"/> dependency property.
    /// </returns>
    public static readonly DependencyProperty ToolBarNameProperty = DependencyProperty.Register(nameof(ToolBarName), typeof(string), typeof(ExtendedToolBar), new FrameworkPropertyMetadata(null));

    /// <summary>
    /// Gets or sets the localized name of the toolbar. Can be null.
    /// </summary>
    public string ToolBarName
    {
        get { return (string)GetValue(ToolBarNameProperty); }
        set { SetValue(ToolBarNameProperty, value); }
    }
    #endregion
}
