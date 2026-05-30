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
    /// <inheritdoc />
    protected override void OnMouseEnter(MouseEventArgs e) => DebugCall();

    /// <inheritdoc />
    protected override void OnMouseLeave(MouseEventArgs e) => DebugCall();

    /// <inheritdoc />
    protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        DebugCall();
        base.OnPreviewMouseLeftButtonDown(e);
    }

    /// <inheritdoc />
    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        DebugCall();
        SelectItemOnLeftButtonDown();
        base.OnMouseLeftButtonDown(e);
    }

    /// <inheritdoc />
    protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        DebugCall();
        base.OnPreviewMouseLeftButtonUp(e);
    }

    /// <inheritdoc />
    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        DebugCall();
        UnselectItemOnLeftButtonUp();
        base.OnMouseLeftButtonUp(e);
    }

    /// <inheritdoc />
    protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
    {
        DebugCall();
        base.OnPreviewMouseRightButtonDown(e);
    }

    /// <inheritdoc />
    protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
    {
        DebugCall();
        SelectItemOnRightButtonDown();
        base.OnMouseRightButtonDown(e);
    }

    /// <inheritdoc />
    protected override void OnPreviewMouseRightButtonUp(MouseButtonEventArgs e)
    {
        DebugCall();
        base.OnPreviewMouseRightButtonUp(e);
    }

    /// <inheritdoc />
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
