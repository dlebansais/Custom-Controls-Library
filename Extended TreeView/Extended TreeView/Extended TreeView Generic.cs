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
        protected override object GetParent(object item)
        {
            return ((TItem)item).Parent;
        }

        protected override int GetChildrenCount(object item)
        {
            return ((TItem)item).Children.Count;
        }

        protected override IList GetChildren(object item)
        {
            return (IList)((TItem)item).Children;
        }

        protected override object GetChild(object item, int index)
        {
            return ((TItem)item).Children[index];
        }

        protected override void InstallHandlers(object item)
        {
            ((TItem)item).Children.CollectionChanged += OnItemChildrenChanged;
        }

        protected override void UninstallHandlers(object item)
        {
            ((TItem)item).Children.CollectionChanged -= OnItemChildrenChanged;
        }

        protected override void DragDropMove(object sourceItem, object destinationItem, IList itemList)
        {
            TCollection SourceCollection = (TCollection)((TItem)sourceItem).Children;
            TCollection DestinationCollection = (TCollection)((TItem)destinationItem).Children;

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
            TCollection ItemCollection = (TCollection)sender;
            object Item = ItemCollection.Parent;

            HandleChildrenChanged(Item, e);
        }
        #endregion
    }
}
