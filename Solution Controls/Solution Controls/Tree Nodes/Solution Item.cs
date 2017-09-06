namespace CustomControls
{
    public interface ISolutionItem : ISolutionTreeNode
    {
    }

    public class SolutionItem : SolutionTreeNode, ISolutionItem
    {
        #region Init
        public SolutionItem(IItemPath path, ISolutionFolder parent, IItemProperties properties)
            : base(parent, path, properties)
        {
        }
        #endregion

        #region Properties
        public override IExtendedTreeNodeCollection Children { get { return new SolutionTreeNodeCollection((ISolutionTreeNode)Parent, null); } }
        #endregion
    }
}
