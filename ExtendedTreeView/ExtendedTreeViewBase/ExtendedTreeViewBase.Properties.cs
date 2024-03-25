namespace CustomControls;

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;

/// <summary>
/// Represents a control with a tree of nodes that can be moved around with Drag and Drop.
/// </summary>
public abstract partial class ExtendedTreeViewBase : MultiSelector
{
    /// <summary>
    /// Gets the list of child items visible in the scroll view.
    /// </summary>
    protected ObservableCollection<object> VisibleChildren { get; } = new();

    /// <summary>
    /// Gets a list of expanded items.
    /// </summary>
    protected Dictionary<object, IList> ExpandedChildren { get; } = new();
}
