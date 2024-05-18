namespace CustomControls;

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Contracts;

/// <summary>
/// Represents an item in a tree view control.
/// </summary>
public partial class ExtendedTreeViewItemBase : ContentControl, INotifyPropertyChanged
{
    #region Mouse Events
    /// <summary>
    /// Invoked when an unhandled <see cref="Mouse.MouseEnterEvent"/> attached event is raised on this element.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnMouseEnter(MouseEventArgs e)
    {
        DebugCall();
    }

    /// <summary>
    /// Invoked when an unhandled <see cref="Mouse.MouseLeaveEvent"/> attached event is raised on this element.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnMouseLeave(MouseEventArgs e)
    {
        DebugCall();
    }

    /// <summary>
    /// Invoked when an unhandled <see cref="UIElement.PreviewMouseLeftButtonDown"/> attached event is raised on this element.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        DebugCall();
        base.OnPreviewMouseLeftButtonDown(e);
    }

    /// <summary>
    /// Invoked when an unhandled <see cref="UIElement.MouseLeftButtonDown"/> attached event is raised on this element.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        DebugCall();
        SelectItemOnLeftButtonDown();
        base.OnMouseLeftButtonDown(e);
    }

    /// <summary>
    /// Invoked when an unhandled <see cref="UIElement.PreviewMouseLeftButtonUp"/> attached event is raised on this element.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        DebugCall();
        base.OnPreviewMouseLeftButtonUp(e);
    }

    /// <summary>
    /// Invoked when an unhandled <see cref="UIElement.MouseLeftButtonUp"/> attached event is raised on this element.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        DebugCall();
        UnselectItemOnLeftButtonUp();
        base.OnMouseLeftButtonUp(e);
    }

    /// <summary>
    /// Invoked when an unhandled <see cref="UIElement.PreviewMouseRightButtonDown"/> attached event is raised on this element.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
    {
        DebugCall();
        base.OnPreviewMouseRightButtonDown(e);
    }

    /// <summary>
    /// Invoked when an unhandled <see cref="UIElement.MouseRightButtonDown"/> attached event is raised on this element.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
    {
        DebugCall();
        SelectItemOnRightButtonDown();
        base.OnMouseRightButtonDown(e);
    }

    /// <summary>
    /// Invoked when an unhandled <see cref="UIElement.PreviewMouseRightButtonUp"/> attached event is raised on this element.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnPreviewMouseRightButtonUp(MouseButtonEventArgs e)
    {
        DebugCall();
        base.OnPreviewMouseRightButtonUp(e);
    }

    /// <summary>
    /// Invoked when an unhandled <see cref="UIElement.MouseRightButtonUp"/> attached event is raised on this element.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
    {
        DebugCall();
        UnselectItemOnRightButtonUp();
        base.OnMouseRightButtonUp(e);
    }
    #endregion

    #region Keyboard Events
    /// <summary>
    /// Invoked when an unhandled <see cref="Keyboard.KeyDownEvent"/> attached event reaches an element in its route that is derived from this class.
    /// </summary>
    /// <param name="args">The event data.</param>
    [Access("protected", "override")]
    [RequireNotNull(nameof(args))]
    private void OnKeyDownVerified(KeyEventArgs args)
    {
        switch (args.Key)
        {
            case Key.Left:
                if (IsExpanded && Host.IsItemCollapsible(Content))
                {
                    IsExpanded = false;
                    args.Handled = true;
                    return;
                }

                break;

            case Key.Right:
                if (!IsExpanded && Host.IsItemExpandable(Content))
                {
                    IsExpanded = true;
                    args.Handled = true;
                    return;
                }

                break;

            case Key.Up:
                Host.ClickPreviousItem(Content);
                break;

            case Key.Down:
                Host.ClickNextItem(Content);
                break;

            default:
                break;
        }

        base.OnKeyDown(args);
    }
    #endregion
}
