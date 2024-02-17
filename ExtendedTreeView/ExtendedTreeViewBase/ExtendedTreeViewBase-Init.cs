namespace CustomControls;

using System;
using System.Windows;
using System.Windows.Controls.Primitives;

/// <summary>
/// Represents a control with a tree of nodes that can be moved around with Drag and Drop.
/// </summary>
public abstract partial class ExtendedTreeViewBase : MultiSelector
{
    static ExtendedTreeViewBase()
    {
        OverrideAncestorMetadata();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtendedTreeViewBase"/> class.
    /// </summary>
    protected ExtendedTreeViewBase()
    {
        if (TryFindResource(typeof(ExtendedTreeViewBase)) is Style DirectDefaultStyle && TryFindResource(typeof(ExtendedTreeViewItemBase)) is Style DirectDefaultItemContainerStyle)
        {
            DefaultStyle = DirectDefaultStyle;
            DefaultItemContainerStyle = DirectDefaultItemContainerStyle;
        }
        else
        {
            Resources.MergedDictionaries.Add(SharedResourceDictionaryManager.SharedDictionary);

            DefaultStyle = (Style)FindResource(typeof(ExtendedTreeViewBase));
            DefaultItemContainerStyle = (Style)FindResource(typeof(ExtendedTreeViewItemBase));
        }

        InitAncestor();
        InitializeImplementation();
        InitializeContextMenu();
        InitializeDragAndDrop();
    }
}
