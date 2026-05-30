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

    /// <inheritdoc />
    public FrameworkElement SourceControl { get; }

    /// <inheritdoc />
    public bool AllowDropCopy { get; private set; }

    /// <inheritdoc />
    public Guid SourceGuid { get; private set; }

    /// <inheritdoc />
    public DragActivity DragActivity { get; private set; }

    /// <inheritdoc />
    public event EventHandler<EventArgs>? DragActivityChanged;

    /// <summary>
    /// Invokes handlers of the <see cref="DragActivityChanged"/> event.
    /// </summary>
    protected void NotifyDragActivityChanged() => DragActivityChanged?.Invoke(this, EventArgs.Empty);

    /// <inheritdoc />
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

    /// <inheritdoc />
    public virtual void ClearIsDragPossible()
    {
        DraggedItemParent = null;
        AllowDropCopy = false;
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
    public virtual bool IsDraggedItemParent(object item) => DraggedItemParent == item;

    /// <inheritdoc />
    public virtual void DragAfterMouseMove(MouseEventArgs sourceLocation) => InitiateDrag();

    /// <inheritdoc />
    public virtual void CancelDrag() => DragActivity = DragActivity.Canceled;

    /// <inheritdoc />
    public virtual void SetFlatDraggedItemList(object rootItem, IList flatItemList)
    {
        RootItem = rootItem;
        FlatItemList = flatItemList;
    }

    /// <inheritdoc />
    public virtual void ClearFlatDraggedItemList()
    {
        RootItem = null;
        FlatItemList = null;
    }

    /// <inheritdoc />
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
