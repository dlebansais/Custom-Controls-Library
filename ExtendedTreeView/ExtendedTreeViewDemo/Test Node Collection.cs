using CustomControls;
using System.Collections.ObjectModel;

namespace ExtendedTreeViewDemo
{
    public class TestNodeCollection : ObservableCollection<TestNode>, IExtendedTreeNodeCollection
    {
        public TestNodeCollection(TestNode parent)
        {
            this.Parent = parent;
        }

        public IExtendedTreeNode Parent { get; private set; }

        public void Sort()
        {
        }
    }
}
