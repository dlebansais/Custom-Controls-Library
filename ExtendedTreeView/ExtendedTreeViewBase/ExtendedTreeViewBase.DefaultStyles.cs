namespace CustomControls;

using System.Windows;
using System.Windows.Controls.Primitives;

/// <summary>
/// Represents a control with a tree of nodes that can be moved around with Drag and Drop.
/// </summary>
public abstract partial class ExtendedTreeViewBase : MultiSelector
{
    /// <summary>
    /// Gets the default control style.
    /// </summary>
    public static Style DefaultStyle { get; private set; } = new();

    /// <summary>
    /// Gets the default control container style.
    /// </summary>
    public static Style DefaultItemContainerStyle { get; private set; } = new();
}
