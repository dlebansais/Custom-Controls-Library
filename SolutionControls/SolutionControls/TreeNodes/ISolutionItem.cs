namespace CustomControls
{
    /// <summary>
    /// Represents an item in a solution.
    /// </summary>
    public interface ISolutionItem : ISolutionTreeNode
    {
    }

    /// <summary>
    /// Represents an item in a solution.
    /// </summary>
    public class SolutionItem : SolutionTreeNode, ISolutionItem
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionItem"/> class.
        /// </summary>
        /// <param name="path">The item path.</param>
        /// <param name="parent">The item parent.</param>
        /// <param name="properties">The item properties.</param>
        public SolutionItem(IItemPath path, ISolutionFolder parent, IItemProperties properties)
            : base(parent, path, properties)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the item children.
        /// </summary>
        public override IExtendedTreeNodeCollection Children { get { return new SolutionTreeNodeCollection(Parent as ISolutionTreeNode); } }
        #endregion
    }
}
