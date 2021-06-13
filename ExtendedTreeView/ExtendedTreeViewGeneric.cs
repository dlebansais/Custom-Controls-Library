namespace CustomControls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Windows;

    /// <summary>
    /// Represents a tree view control for a generic type of items.
    /// </summary>
    /// <typeparam name="TItem">The type of items.</typeparam>
    /// <typeparam name="TCollection">The type of collection of items.</typeparam>
    public class ExtendedTreeViewGeneric<TItem, TCollection> : ExtendedTreeViewBase
        where TItem : class, IExtendedTreeNode
        where TCollection : IExtendedTreeNodeCollection
    {
        #region Content
        /// <summary>
        /// Identifies the <see cref="Content"/> attached property.
        /// </summary>
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(TItem), typeof(ExtendedTreeViewGeneric<TItem, TCollection>), new FrameworkPropertyMetadata(new EmptyExtendedTreeNode<TItem, TCollection>(), OnContentChanged));

        /// <summary>
        /// Gets or sets the control content.
        /// </summary>
        public TItem Content
        {
            get { return (TItem)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        /// <summary>
        /// Handles changes of the <see cref="Content"/> property.
        /// </summary>
        /// <param name="modifiedObject">The modified object.</param>
        /// <param name="e">An object that contains event data.</param>
        protected static void OnContentChanged(DependencyObject modifiedObject, DependencyPropertyChangedEventArgs e)
        {
            if (modifiedObject == null)
                throw new ArgumentNullException(nameof(modifiedObject));

            ExtendedTreeViewGeneric<TItem, TCollection> ctrl = (ExtendedTreeViewGeneric<TItem, TCollection>)modifiedObject;
            ctrl.OnContentChanged(e);
        }

        /// <summary>
        /// Handles changes of the <see cref="Content"/> property.
        /// </summary>
        /// <param name="e">An object that contains event data.</param>
        protected virtual void OnContentChanged(DependencyPropertyChangedEventArgs e)
        {
            BuildFlatChildrenTables();
        }
        #endregion

        #region Ancestor Interface
        /// <summary>
        /// Checks if an item is the current content.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>True if equal to the current content; otherwise, false.</returns>
        protected override bool IsContent(object item)
        {
            return item is TItem AsItem && AsItem == Content;
        }

        /// <summary>
        /// Checks if an item is of the same type as the current content.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>True if of the same type as the current content; otherwise, false.</returns>
        protected override bool IsSameTypeAsContent(object? item)
        {
            if (item == null || Content == null)
                return false;

            if (item.GetType() != Content.GetType())
                return false;

            return true;
        }

        /// <summary>
        /// Gets the parent of an item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The parent item.</returns>
        protected override object? GetItemParent(object item)
        {
            if (item is TItem AsItem)
                return AsItem.Parent;
            else
                throw new ArgumentNullException(nameof(item));
        }

        /// <summary>
        /// Gets the number of children of an item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The number of children.</returns>
        protected override int GetItemChildrenCount(object item)
        {
            if (item is TItem AsItem)
                return ((IList)AsItem.Children).Count;
            else
                throw new ArgumentNullException(nameof(item));
        }

        /// <summary>
        /// Gets children of an item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The children.</returns>
        protected override IList GetItemChildren(object item)
        {
            if (item is TItem AsItem)
                return AsItem.Children;
            else
                throw new ArgumentNullException(nameof(item));
        }

        /// <summary>
        /// Gets the child of an item at the provided index.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="index">The child index.</param>
        /// <returns>The child at the provided index.</returns>
        protected override object GetItemChild(object item, int index)
        {
            if (item is TItem AsItem)
                return ((IList)AsItem.Children)[index]!;
            else
                throw new ArgumentNullException(nameof(item));
        }

        /// <summary>
        /// Installs event handlers on an item.
        /// </summary>
        /// <param name="item">The item.</param>
        protected override void InstallHandlers(object item)
        {
            if (item is TItem AsItem)
                AsItem.Children.CollectionChanged += OnItemChildrenChanged;
            else
                throw new ArgumentNullException(nameof(item));
        }

        /// <summary>
        /// Uninstalls event handlers from an item.
        /// </summary>
        /// <param name="item">The item.</param>
        protected override void UninstallHandlers(object item)
        {
            if (item is TItem AsItem)
                AsItem.Children.CollectionChanged -= OnItemChildrenChanged;
            else
                throw new ArgumentNullException(nameof(item));
        }

        /// <summary>
        /// Moves items from source to destination.
        /// </summary>
        /// <param name="sourceItem">The source item.</param>
        /// <param name="destinationItem">The destination item.</param>
        /// <param name="itemList">Moved children.</param>
        protected override void DragDropMove(object sourceItem, object destinationItem, IList? itemList)
        {
            if (!(sourceItem is TItem AsSourceItem))
                throw new ArgumentNullException(nameof(sourceItem));
            if (!(destinationItem is TItem AsDestinationItem))
                throw new ArgumentNullException(nameof(destinationItem));

            TCollection SourceCollection = (TCollection)AsSourceItem.Children;
            TCollection DestinationCollection = (TCollection)AsDestinationItem.Children;

            if (itemList != null)
            {
#if NETCOREAPP3_1
                foreach (TItem? ChildItem in itemList)
                    if (ChildItem != null)
                        SourceCollection.Remove(ChildItem);

                foreach (TItem? ChildItem in itemList)
                    if (ChildItem != null)
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
            }

            DestinationCollection.Sort();
        }

        /// <summary>
        /// Copy items from source to destination.
        /// </summary>
        /// <param name="sourceItem">The source item.</param>
        /// <param name="destinationItem">The destination item.</param>
        /// <param name="itemList">Children at the source.</param>
        /// <param name="cloneList">Cloned children at the destination.</param>
        protected override void DragDropCopy(object sourceItem, object destinationItem, IList itemList, IList cloneList)
        {
            Debug.Assert(itemList.Count > 0);

            TItem DestinationItem = (TItem)destinationItem;
            TCollection DestinationCollection = (TCollection)DestinationItem.Children;

#if NETCOREAPP3_1
            foreach (ICloneable? ChildItem in itemList)
                if (ChildItem != null)
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

        /// <summary>
        /// Inserts child items starting from the content root.
        /// </summary>
        protected override void InsertChildrenFromRootDontNotify()
        {
            if (Content != null)
            {
                IInsertItemContext Context = CreateInsertItemContext(Content, 0);
                Context.Start();

                InsertChildren(Context, Content, null);

                Context.Complete();
                Context.Close();
            }
        }

        /// <summary>
        /// Creates a list of items.
        /// </summary>
        /// <returns>The created list of items.</returns>
        protected override IList CreateItemList()
        {
            return new List<TItem>();
        }

        /// <summary>
        /// Sets the dragged items.
        /// </summary>
        /// <param name="dragSource">The drag source.</param>
        /// <param name="itemList">The list of dragged items.</param>
        protected override void SetDragItemList(IDragSourceControl dragSource, IList itemList)
        {
            dragSource.SetFlatDraggedItemList(Content, FlatItemList(itemList));
        }

        /// <summary>
        /// Clears the dragged items.
        /// </summary>
        /// <param name="dragSource">The drag source.</param>
        protected override void ClearDragItemList(IDragSourceControl dragSource)
        {
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
        /// <summary>
        /// Gets the list of visible items.
        /// </summary>
        public override IList VisibleItems()
        {
            List<TItem?> Result = new List<TItem?>();
            IList BaseItemList = base.VisibleItems();

            foreach (object? item in BaseItemList)
                Result.Add(item as TItem);

            return Result;
        }
#else
        /// <summary>
        /// Gets the list of visible items.
        /// </summary>
        public override IList VisibleItems()
        {
            List<TItem> Result = new List<TItem>();
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
        /// <param name="sender">The event source.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnItemChildrenChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (sender is TCollection ItemCollection && ItemCollection.Parent is object Item)
                HandleChildrenChanged(Item, e);
            else
                throw new ArgumentOutOfRangeException(nameof(sender));
        }
#endregion
    }
}
