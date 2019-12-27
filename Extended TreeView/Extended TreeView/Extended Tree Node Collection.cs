using System.Collections;
using System.Collections.Specialized;

namespace CustomControls
{
    public interface IExtendedTreeNodeCollection : IList, INotifyCollectionChanged
    {
        IExtendedTreeNode? Parent { get; }
        void Sort();
    }
}
