using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace CustomControls
{
    public class ExtendedTreeViewGeneric<TItem, TCollection> : ExtendedTreeViewBase
        where TItem : IExtendedTreeNode
        where TCollection : IExtendedTreeNodeCollection
    {
        #region Ancestor Interface
        protected override object GetItemParent(object item)
        {
            if (item is TItem AsItem)
                return AsItem.Parent;
            else
                throw new ArgumentNullException(nameof(item));
        }

        protected override int GetItemChildrenCount(object item)
        {
            if (item is TItem AsItem)
                return AsItem.Children.Count;
            else
                throw new ArgumentNullException(nameof(item));
        }

        protected override IList GetItemChildren(object item)
        {
            if (item is TItem AsItem)
                return AsItem.Children;
            else
                throw new ArgumentNullException(nameof(item));
        }

        protected override object GetItemChild(object item, int index)
        {
            if (item is TItem AsItem)
                return AsItem.Children[index];
            else
                throw new ArgumentNullException(nameof(item));
        }

        protected override void InstallHandlers(object item)
        {
            if (item is TItem AsItem)
                AsItem.Children.CollectionChanged += OnItemChildrenChanged;
            else
                throw new ArgumentNullException(nameof(item));
        }

        protected override void UninstallHandlers(object item)
        {
            if (item is TItem AsItem)
                AsItem.Children.CollectionChanged -= OnItemChildrenChanged;
            else
                throw new ArgumentNullException(nameof(item));
        }

        protected override void DragDropMove(object sourceItem, object destinationItem, IList itemList)
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

        protected override void DragDropCopy(object sourceItem, object destinationItem, IList itemList, IList cloneList)
        {
            if (destinationItem == null)
                throw new ArgumentNullException(nameof(destinationItem));

            TCollection DestinationCollection = (TCollection)((TItem)destinationItem).Children;

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

        protected override IList CreateItemList()
        {
            return new List<TItem>();
        }
        #endregion

        #region Implementation
        protected virtual void OnItemChildrenChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (sender == null)
                throw new ArgumentNullException(nameof(sender));

            TCollection ItemCollection = (TCollection)sender;
            object Item = ItemCollection.Parent;

            HandleChildrenChanged(Item, e);
        }
        #endregion
    }
}
