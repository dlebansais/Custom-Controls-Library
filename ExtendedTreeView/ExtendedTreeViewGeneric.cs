namespace CustomControls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;

    /// <summary>
    /// Represents a tree view control for a generic type of items.
    /// </summary>
    /// <typeparam name="TItem">The type of items.</typeparam>
    /// <typeparam name="TCollection">The type of collection of items.</typeparam>
    public class ExtendedTreeViewGeneric<TItem, TCollection> : ExtendedTreeViewBase
        where TItem : IExtendedTreeNode
        where TCollection : IExtendedTreeNodeCollection
    {
        #region Ancestor Interface
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
                return ((IList)AsItem.Children)[index];
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
                foreach (TItem ChildItem in itemList)
                    SourceCollection.Remove(ChildItem);

                foreach (TItem ChildItem in itemList)
                {
                    ChildItem.ChangeParent((TItem)destinationItem);
                    DestinationCollection.Add(ChildItem);
                }
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
        protected override void DragDropCopy(object sourceItem, object destinationItem, IList? itemList, IList cloneList)
        {
            if (!(destinationItem is TItem AsDestinationItem))
                throw new ArgumentNullException(nameof(destinationItem));

            TCollection DestinationCollection = (TCollection)AsDestinationItem.Children;

            if (itemList != null && cloneList != null)
                foreach (ICloneable ChildItem in itemList)
                {
                    TItem Clone = (TItem)ChildItem.Clone();
                    Clone.ChangeParent((TItem)destinationItem);
                    DestinationCollection.Add(Clone);

                    cloneList.Add(Clone);
                }

            DestinationCollection.Sort();
        }

        /// <summary>
        /// Creates a list of items.
        /// </summary>
        /// <returns>The created list of items.</returns>
        protected override IList CreateItemList()
        {
            return new List<TItem>();
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Called when children of an item have changed.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnItemChildrenChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (sender is TCollection ItemCollection && ItemCollection.Parent is object Item)
                HandleChildrenChanged(Item, e);
            else
                throw new ArgumentOutOfRangeException(nameof(sender));
        }
        #endregion
    }
}
