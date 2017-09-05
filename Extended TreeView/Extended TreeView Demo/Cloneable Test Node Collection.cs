using CustomControls;
using System.Collections.ObjectModel;

namespace ExtendedTreeViewDemo
{
    public class CloneableTestNodeCollection : ObservableCollection<CloneableTestNode>, IExtendedTreeNodeCollection
    {
        public CloneableTestNodeCollection(CloneableTestNode parent)
        {
            this.Parent = parent;
        }

        public IExtendedTreeNode Parent { get; private set; }

        public void Sort()
        {
        }
    }
}
