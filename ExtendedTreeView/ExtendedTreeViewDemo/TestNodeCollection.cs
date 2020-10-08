namespace ExtendedTreeViewDemo
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using CustomControls;

    internal class TestNodeCollection : ObservableCollection<TestNode>, IExtendedTreeNodeCollection
    {
        public TestNodeCollection(TestNode parent)
        {
            this.Parent = parent;
        }

        public IExtendedTreeNode Parent { get; private set; }

        public void Sort()
        {
        }

        int IList<IExtendedTreeNode>.IndexOf(IExtendedTreeNode item)
        {
            return IndexOf((TestNode)item);
        }

        void IList<IExtendedTreeNode>.Insert(int index, IExtendedTreeNode item)
        {
            Insert(index, (TestNode)item);
        }

        IExtendedTreeNode IList<IExtendedTreeNode>.this[int index]
        {
            get { return this[index]; }
            set { this[index] = (TestNode)value; }
        }

        void ICollection<IExtendedTreeNode>.Add(IExtendedTreeNode item)
        {
            Add((TestNode)item);
        }

        bool ICollection<IExtendedTreeNode>.Contains(IExtendedTreeNode item)
        {
            return Contains((TestNode)item);
        }
        void ICollection<IExtendedTreeNode>.CopyTo(IExtendedTreeNode[] array, int arrayIndex)
        {
            CopyTo(array as TestNode[], arrayIndex);
        }

        bool ICollection<IExtendedTreeNode>.Remove(IExtendedTreeNode item)
        {
            return Remove((TestNode)item);
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
