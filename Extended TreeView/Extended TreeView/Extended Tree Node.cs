using System.Collections.ObjectModel;

namespace CustomControls
{
    public interface IExtendedTreeNode
    {
        IExtendedTreeNode Parent { get; }
        IExtendedTreeNodeCollection Children { get; }
        void ChangeParent(IExtendedTreeNode newParent);
    }
}
