namespace CustomControls;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows;
using Contracts;

/// <summary>
/// Represents a tree view control for a generic type of items.
/// </summary>
/// <typeparam name="TItem">The type of items.</typeparam>
/// <typeparam name="TCollection">The type of collection of items.</typeparam>
public partial class ExtendedTreeViewGeneric<TItem, TCollection> : ExtendedTreeViewBase
    where TItem : class, IExtendedTreeNode
    where TCollection : class, IExtendedTreeNodeCollection
{
    #region Content
    /// <summary>
    /// Identifies the <see cref="Content"/> attached property.
    /// </summary>
    public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(nameof(Content), typeof(TItem), typeof(ExtendedTreeViewGeneric<TItem, TCollection>), new FrameworkPropertyMetadata(new EmptyExtendedTreeNode<TItem, TCollection>(), OnContentChanged));

    /// <summary>
    /// Gets or sets the control content.
    /// </summary>
    public TItem Content
    {
        get => (TItem)GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    /// <summary>
    /// Handles changes of the <see cref="Content"/> property.
    /// </summary>
    /// <param name="control">The modified control.</param>
    /// <param name="args">An object that contains event data.</param>
    [Access("protected", "static")]
    [RequireNotNull(nameof(control), Type = "DependencyObject", Name = "modifiedObject")]
    private static void OnContentChangedVerified(ExtendedTreeViewGeneric<TItem, TCollection> control, DependencyPropertyChangedEventArgs args) => control.OnContentChanged(args);

    /// <summary>
    /// Handles changes of the <see cref="Content"/> property.
    /// </summary>
    /// <param name="args">An object that contains event data.</param>
    protected virtual void OnContentChanged(DependencyPropertyChangedEventArgs args) => BuildFlatChildrenTables();
    #endregion

    #region Ancestor Interface
    /// <inheritdoc />
    protected override bool IsContent(object item) => item is TItem AsItem && AsItem == Content;

    /// <inheritdoc />
    protected override bool IsSameTypeAsContent(object item)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(item);
#else
        if (item is null)
            throw new ArgumentNullException(nameof(item));
#endif

        if (item.GetType() != Content.GetType())
            return false;

        return true;
    }

    /// <inheritdoc />
    protected override object? GetItemParent(object item)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(item);
#else
        if (item is null)
            throw new ArgumentNullException(nameof(item));
#endif

        TItem Item = (TItem)item;
        return Item.Parent;
    }

    /// <inheritdoc />
    protected override int GetItemChildrenCount(object item)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(item);
#else
        if (item is null)
            throw new ArgumentNullException(nameof(item));
#endif

        TItem Item = (TItem)item;
        return ((IList)Item.Children).Count;
    }

    /// <inheritdoc />
    protected override IList GetItemChildren(object item)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(item);
#else
        if (item is null)
            throw new ArgumentNullException(nameof(item));
#endif

        TItem Item = (TItem)item;
        return Item.Children;
    }

    /// <inheritdoc />
    protected override object GetItemChild(object item, int index)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(item);
#else
        if (item is null)
            throw new ArgumentNullException(nameof(item));
#endif

        TItem Item = (TItem)item;
        return Contract.AssertNotNull(((IList)Item.Children)[index]);
    }

    /// <inheritdoc />
    protected override void InstallHandlers(object item)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(item);
#else
        if (item is null)
            throw new ArgumentNullException(nameof(item));
#endif

        TItem Item = (TItem)item;
        Item.Children.CollectionChanged += OnItemChildrenChanged;
    }

    /// <inheritdoc />
    protected override void UninstallHandlers(object item)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(item);
#else
        if (item is null)
            throw new ArgumentNullException(nameof(item));
#endif

        TItem Item = (TItem)item;
        Item.Children.CollectionChanged -= OnItemChildrenChanged;
    }

    /// <inheritdoc />
    protected override void DragDropMove(object sourceItem, object destinationItem, IList itemList)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(sourceItem);
        ArgumentNullException.ThrowIfNull(destinationItem);
        ArgumentNullException.ThrowIfNull(itemList);
#else
        if (sourceItem is null)
            throw new ArgumentNullException(nameof(sourceItem));
        if (destinationItem is null)
            throw new ArgumentNullException(nameof(destinationItem));
        if (itemList is null)
            throw new ArgumentNullException(nameof(itemList));
#endif

        TItem SourceItem = (TItem)sourceItem;
        TItem DestinationItem = (TItem)destinationItem;
        TCollection SourceCollection = (TCollection)SourceItem.Children;
        TCollection DestinationCollection = (TCollection)DestinationItem.Children;

#if NETCOREAPP3_1
        foreach (TItem? ChildItem in itemList)
            if (ChildItem is not null)
                SourceCollection.Remove(ChildItem);

        foreach (TItem? ChildItem in itemList)
            if (ChildItem is not null)
            {
                ChildItem.ChangeParent((TItem)destinationItem);
                DestinationCollection.Add(ChildItem);
            }
#else
        foreach (TItem ChildItem in itemList)
            SourceCollection.Remove(ChildItem);

        foreach (TItem ChildItem in itemList)
        {
            ChildItem.ChangeParent((TItem)destinationItem);
            DestinationCollection.Add(ChildItem);
        }
#endif

        DestinationCollection.Sort();
    }

    /// <inheritdoc />
    protected override void DragDropCopy(object sourceItem, object destinationItem, IList itemList, IList cloneList)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(destinationItem);
        ArgumentNullException.ThrowIfNull(itemList);
        ArgumentNullException.ThrowIfNull(cloneList);
#else
        if (destinationItem is null)
            throw new ArgumentNullException(nameof(destinationItem));
        if (itemList is null)
            throw new ArgumentNullException(nameof(itemList));
        if (cloneList is null)
            throw new ArgumentNullException(nameof(cloneList));
#endif

        Debug.Assert(itemList.Count > 0);

        TItem DestinationItem = (TItem)destinationItem;
        TCollection DestinationCollection = (TCollection)DestinationItem.Children;

#if NETCOREAPP3_1
        foreach (ICloneable? ChildItem in itemList)
            if (ChildItem is not null)
            {
                TItem Clone = (TItem)ChildItem.Clone();
                Clone.ChangeParent(DestinationItem);
                DestinationCollection.Add(Clone);

                cloneList.Add(Clone);
            }
#else
        foreach (ICloneable ChildItem in itemList)
        {
            TItem Clone = (TItem)ChildItem.Clone();
            Clone.ChangeParent(DestinationItem);
            DestinationCollection.Add(Clone);

            cloneList.Add(Clone);
        }
#endif

        DestinationCollection.Sort();
    }

    /// <inheritdoc />
    protected override void InsertChildrenFromRootDontNotify()
    {
        if (Content is TItem Item)
        {
            IInsertItemContext Context = CreateInsertItemContext(Item, 0);
            Context.Start();

            InsertChildren(Context, Item, null);

            Context.Complete();
            Context.Close();
        }
    }

    /// <inheritdoc />
    protected override IList CreateItemList() => new List<TItem>();

    /// <inheritdoc />
    protected override void SetDragItemList(IDragSourceControl dragSource, IList itemList)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(dragSource);
#else
        if (dragSource is null)
            throw new ArgumentNullException(nameof(dragSource));
#endif

        dragSource.SetFlatDraggedItemList(Content, FlatItemList(itemList));
    }

    /// <inheritdoc />
    protected override void ClearDragItemList(IDragSourceControl dragSource)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(dragSource);
#else
        if (dragSource is null)
            throw new ArgumentNullException(nameof(dragSource));
#endif

        dragSource.ClearFlatDraggedItemList();
    }

#if NET5_0
    /// <summary>
    /// Gets the list of visible items.
    /// </summary>
    public override List<TItem> VisibleItems()
    {
        List<TItem> Result = new List<TItem>();
        IList BaseItemList = base.VisibleItems();

        foreach (object item in BaseItemList)
            Result.Add((TItem)item);

        return Result;
    }
#elif NETCOREAPP3_1
    /// <inheritdoc />
    public override IList VisibleItems()
    {
        List<TItem?> Result = new();
        IList BaseItemList = base.VisibleItems();

        foreach (object? item in BaseItemList)
            Result.Add(item as TItem);

        return Result;
    }
#else
    /// <inheritdoc />
    public override IList VisibleItems()
    {
        List<TItem> Result = new();
        IList BaseItemList = base.VisibleItems();

        foreach (object item in BaseItemList)
            Result.Add((TItem)item);

        return Result;
    }
#endif
    #endregion

    #region Implementation
    /// <summary>
    /// Called when children of an item have changed.
    /// </summary>
    /// <param name="itemCollection">The event source.</param>
    /// <param name="e">The event data.</param>
    [Access("protected", "virtual")]
    [RequireNotNull(nameof(itemCollection), Type = "object?", Name = "sender", AliasName = "ItemCollection")]
    [Require("ItemCollection.Parent is not null")]
    private void OnItemChildrenChangedVerified(TCollection itemCollection, NotifyCollectionChangedEventArgs e)
    {
        object Item = Contract.AssertNotNull(itemCollection.Parent);

        HandleChildrenChanged(Item, e);
    }
    #endregion
}
