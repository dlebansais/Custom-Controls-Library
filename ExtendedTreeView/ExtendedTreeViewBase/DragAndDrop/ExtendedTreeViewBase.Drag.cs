namespace CustomControls;

using System;
using System.Collections;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Contracts;

/// <summary>
/// Represents a control with a tree of nodes that can be moved around with Drag and Drop.
/// </summary>
public abstract partial class ExtendedTreeViewBase : MultiSelector
{
    /// <summary>
    /// Performs a drag after the mouse has moved.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected virtual void DragAfterMouseMove(MouseEventArgs e)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(e);
#else
        if (e is null)
            throw new ArgumentNullException(nameof(e));
#endif

        if (AllowDragDrop && e.LeftButton == MouseButtonState.Pressed && (Keyboard.FocusedElement is ExtendedTreeViewItemBase))
            if (IsCopyPossible)
            {
                ExtendedTreeViewItemBase? ItemContainer = GetEventSourceItem(e);
                if (ItemContainer is not null)
                {
                    DebugMessage("Drag Started");

                    DragSource.DragAfterMouseMove(e);
                }
            }
    }

    /// <summary>
    /// Updates the drag drop allowed properties.
    /// </summary>
    protected virtual void UpdateIsDragDropPossible()
    {
        CanonicSelection CanonicSelectedItemList = new(CreateItemList());
        if (GetCanonicSelectedItemList(CanonicSelectedItemList))
            DragSource.SetIsDragPossible(CanonicSelectedItemList);
        else
        {
            DragSource.ClearIsDragPossible();
            DragSource.ClearFlatDraggedItemList();
        }
    }

    /// <summary>
    /// Occurs when activity of the current drag drop operation has changed.
    /// </summary>
    /// <param name="sender">The event source.</param>
    /// <param name="e">The event data.</param>
    protected virtual void OnDragActivityChanged(object? sender, EventArgs e)
    {
        Contract.RequireNotNull(sender, out IDragSourceControl Ctrl);

        bool IsHandled = false;

        switch (Ctrl.DragActivity)
        {
            case DragActivity.Idle:
                IsHandled = true;
                break;

            case DragActivity.Starting:
                bool IsDragPossible = DragSource.IsDragPossible(out _, out IList ItemList);
                Debug.Assert(IsDragPossible);

                SetDragItemList(DragSource, ItemList);

                CancellationToken cancellation = new();
                NotifyDragStarting(cancellation);
                if (cancellation.IsCanceled)
                    Ctrl.CancelDrag();

                IsHandled = true;
                break;

            case DragActivity.Started:
                DataObject Data = new(DragSource.GetType(), DragSource);
                DragDropEffects AllowedEffects = DragDropEffects.Move;
                if (DragSource.AllowDropCopy)
                    AllowedEffects |= DragDropEffects.Copy;
                _ = DragDrop.DoDragDrop(this, Data, AllowedEffects);
                ClearCurrentDropTarget();

                IsHandled = true;
                break;

            default:
                break;
        }

        Debug.Assert(IsHandled);
    }

    /// <summary>
    /// Called when feedback for the user is needed.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnGiveFeedback(GiveFeedbackEventArgs e)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(e);
#else
        if (e is null)
            throw new ArgumentNullException(nameof(e));
#endif

        Cursor FeedbackCursor;

        if (UseDefaultCursors)
            FeedbackCursor = Cursors.None;
        else if (e.Effects.HasFlag(DragDropEffects.Copy))
            FeedbackCursor = (CursorCopy != Cursors.None) ? CursorCopy : DefaultCursorCopy;
        else if (e.Effects.HasFlag(DragDropEffects.Move))
            FeedbackCursor = (CursorMove != Cursors.None) ? CursorMove : DefaultCursorMove;
        else
            FeedbackCursor = (CursorForbidden != Cursors.None) ? CursorForbidden : DefaultCursorForbidden;

        if (FeedbackCursor != Cursors.None)
        {
            e.UseDefaultCursors = false;
            Mouse.SetCursor(FeedbackCursor);
            e.Handled = true;
        }
        else
            base.OnGiveFeedback(e);
    }

    /// <summary>
    /// Invoked when an unhandled <see cref="UIElement.DragEnter"/> attached event reaches an element in its route that is derived from this class.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnDragEnter(DragEventArgs e)
    {
        Contract.RequireNotNull(e, out DragEventArgs Args);

        UpdateCurrentDropTarget(Args, false);

        Args.Effects = GetAllowedDropEffects(Args);
        Args.Handled = true;
    }

    /// <summary>
    /// Invoked when an unhandled <see cref="UIElement.DragLeave"/> attached event reaches an element in its route that is derived from this class.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnDragLeave(DragEventArgs e)
    {
        Contract.RequireNotNull(e, out DragEventArgs Args);

        UpdateCurrentDropTarget(Args, true);

        Args.Effects = GetAllowedDropEffects(Args);
        Args.Handled = true;
    }

    /// <summary>
    /// Invoked when an unhandled <see cref="UIElement.DragOver"/> attached event reaches an element in its route that is derived from this class.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnDragOver(DragEventArgs e)
    {
        Contract.RequireNotNull(e, out DragEventArgs Args);

        UpdateCurrentDropTarget(Args, false);

        Args.Effects = GetAllowedDropEffects(Args);
        Args.Handled = true;
    }

    /// <summary>
    /// Gets the drag source from arguments of a drag drop event.
    /// </summary>
    /// <param name="args">The event data.</param>
    /// <param name="dragSource">The drag source upon return.</param>
    /// <returns>True if successful.</returns>
    protected virtual bool GetValidDragSourceFromArgs(DragEventArgs args, out IDragSourceControl dragSource)
    {
        Contract.RequireNotNull(args, out DragEventArgs Args);

        if (Args.Data.GetDataPresent(DragSource.GetType()))
        {
            if (Args.Data.GetData(DragSource.GetType()) is IDragSourceControl AsDragSource)
            {
                if (AsDragSource.HasDragItemList(out object RootItem, out IList _) && IsSameTypeAsContent(RootItem))
                {
                    dragSource = AsDragSource;
                    return true;
                }
            }
        }

        Contract.Unused(out dragSource);
        return false;
    }
}
