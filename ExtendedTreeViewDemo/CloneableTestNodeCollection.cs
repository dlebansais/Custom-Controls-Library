namespace ExtendedTreeViewDemo
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using CustomControls;

    internal class CloneableTestNodeCollection : ObservableCollection<CloneableTestNode>, IExtendedTreeNodeCollection
    {
        public CloneableTestNodeCollection(CloneableTestNode parent)
        {
            this.Parent = parent;
        }

        public IExtendedTreeNode Parent { get; private set; }

        public void Sort()
        {
        }

        int IList<IExtendedTreeNode>.IndexOf(IExtendedTreeNode item)
        {
            return IndexOf((CloneableTestNode)item);
        }

        void IList<IExtendedTreeNode>.Insert(int index, IExtendedTreeNode item)
        {
            Insert(index, (CloneableTestNode)item);
        }

        IExtendedTreeNode IList<IExtendedTreeNode>.this[int index]
        {
            get { return this[index]; }
            set { this[index] = (CloneableTestNode)value; }
        }

        void ICollection<IExtendedTreeNode>.Add(IExtendedTreeNode item)
        {
            Add((CloneableTestNode)item);
        }

        bool ICollection<IExtendedTreeNode>.Contains(IExtendedTreeNode item)
        {
            return Contains((CloneableTestNode)item);
        }
        void ICollection<IExtendedTreeNode>.CopyTo(IExtendedTreeNode[] array, int arrayIndex)
        {
            CopyTo(array as CloneableTestNode[], arrayIndex);
        }

        bool ICollection<IExtendedTreeNode>.Remove(IExtendedTreeNode item)
        {
            return Remove((CloneableTestNode)item);
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        IEnumerator<IExtendedTreeNode> IEnumerable<IExtendedTreeNode>.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
