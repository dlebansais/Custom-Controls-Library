namespace CustomControls;

using System;
using System.Collections;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Contracts;

/// <summary>
/// Represents a control providing drag and drop features.
/// </summary>
public class DragSourceControl : IDragSourceControl
{
    /// <summary>
    /// Default delay for starting a drag operation.
    /// </summary>
    public static readonly TimeSpan DefaultDragDelay = TimeSpan.FromSeconds(0.4);

    /// <summary>
    /// Initializes a new instance of the <see cref="DragSourceControl"/> class.
    /// </summary>
    /// <param name="sourceControl">The control source of the drag.</param>
    public DragSourceControl(FrameworkElement sourceControl)
    {
        SourceControl = sourceControl;

        AllowDropCopy = false;
        SourceGuid = Guid.NewGuid();
        DragActivity = DragActivity.Idle;
    }

    /// <summary>
    /// Gets the control source of the dragged content.
    /// </summary>
    public FrameworkElement SourceControl { get; }

    /// <summary>
    /// Gets a value indicating whether copy is allowed on drop.
    /// </summary>
    public bool AllowDropCopy { get; private set; }

    /// <summary>
    /// Gets the GUID of the source.
    /// </summary>
    public Guid SourceGuid { get; private set; }

    /// <summary>
    /// Gets the activity state of the drag operation.
    /// </summary>
    public DragActivity DragActivity { get; private set; }

    /// <summary>
    /// Occurs when <see cref="DragActivity"/> changed.
    /// </summary>
    public event EventHandler<EventArgs>? DragActivityChanged;

    /// <summary>
    /// Invokes handlers of the <see cref="DragActivityChanged"/> event.
    /// </summary>
    protected void NotifyDragActivityChanged()
    {
        DragActivityChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Changes the drag activity.
    /// </summary>
    /// <param name="canonicSelectedItemList">The lost of selected items.</param>
    public virtual void SetIsDragPossible(CanonicSelection canonicSelectedItemList)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(canonicSelectedItemList);
#else
        if (canonicSelectedItemList is null)
            throw new ArgumentNullException(nameof(canonicSelectedItemList));
#endif

        DraggedItemParent = Contract.AssertNotNull(canonicSelectedItemList.DraggedItemParent);
        ItemList = canonicSelectedItemList.ItemList;
        AllowDropCopy = canonicSelectedItemList.AllItemsCloneable;
    }

    /// <summary>
    /// Clears the value indicating if drag is possible.
    /// </summary>
    public virtual void ClearIsDragPossible()
    {
        DraggedItemParent = null;
        AllowDropCopy = false;
    }

    /// <summary>
    /// Gets the value indicating if drag is possible.
    /// </summary>
    /// <param name="draggedItemParent">The dragged parent item upon return.</param>
    /// <param name="itemList">The list of dragged items upon return.</param>
    /// <returns>True if drag is possible; otherwise, false.</returns>
    public virtual bool IsDragPossible(out object draggedItemParent, out IList itemList)
    {
        bool IsDragPossible = true;
        IsDragPossible &= DraggedItemParent is not null;
        IsDragPossible &= ItemList is not null;

        if (IsDragPossible)
        {
            draggedItemParent = Contract.AssertNotNull(DraggedItemParent);
            itemList = Contract.AssertNotNull(ItemList);
            return true;
        }

        Contract.Unused(out draggedItemParent);
        Contract.Unused(out itemList);
        return false;
    }

    /// <summary>
    /// Checks if an item is the parent of the dragged item.
    /// </summary>
    /// <param name="item">The item to check.</param>
    /// <returns>True if the parent of the dragged item; otherwise, false.</returns>
    public virtual bool IsDraggedItemParent(object item)
    {
        return DraggedItemParent == item;
    }

    /// <summary>
    /// Called when a drag should begin after the mouse moved.
    /// </summary>
    /// <param name="sourceLocation">The source location.</param>
    public virtual void DragAfterMouseMove(MouseEventArgs sourceLocation)
    {
        InitiateDrag();
    }

    /// <summary>
    /// Cancels the drag operation.
    /// </summary>
    public virtual void CancelDrag()
    {
        DragActivity = DragActivity.Canceled;
    }

    /// <summary>
    /// Sets the flat list of dragged items.
    /// </summary>
    /// <param name="rootItem">The root item.</param>
    /// <param name="flatItemList">The flat list of dragged items.</param>
    public virtual void SetFlatDraggedItemList(object rootItem, IList flatItemList)
    {
        RootItem = rootItem;
        FlatItemList = flatItemList;
    }

    /// <summary>
    /// Clears the flat list of dragged items.
    /// </summary>
    public virtual void ClearFlatDraggedItemList()
    {
        RootItem = null;
        FlatItemList = null;
    }

    /// <summary>
    /// Checks if there are dragged items.
    /// </summary>
    /// <param name="rootItem">The root of dragged items upon return.</param>
    /// <param name="flatItemList">The flat list of dragged items upon return.</param>
    /// <returns>True if successful; otherwise, false.</returns>
    public virtual bool HasDragItemList(out object rootItem, out IList flatItemList)
    {
        if (RootItem is not null && FlatItemList is not null && FlatItemList.Count > 0)
        {
            rootItem = RootItem;
            flatItemList = FlatItemList;
            return true;
        }

        Contract.Unused(out rootItem);
        Contract.Unused(out flatItemList);
        return false;
    }

    private void InitiateDrag()
    {
        if (InitiateDragOperation is null || InitiateDragOperation.Status == DispatcherOperationStatus.Completed)
            InitiateDragOperation = SourceControl.Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new DragInitiatedHandler(OnDragInitiated));
    }

    private delegate void DragInitiatedHandler();
    private void OnDragInitiated()
    {
        DragActivity = DragActivity.Starting;
        NotifyDragActivityChanged();

        if (DragActivity != DragActivity.Canceled)
        {
            DragActivity = DragActivity.Started;
            NotifyDragActivityChanged();
        }
    }

    private DispatcherOperation? InitiateDragOperation;
    private object? DraggedItemParent;
    private object? RootItem;
    private IList? ItemList;
    private IList? FlatItemList;
}
