namespace CustomControls
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Represents a collection of tree nodes in a solution.
    /// </summary>
    public interface ISolutionTreeNodeCollection : IList<ISolutionTreeNode>, IReadOnlyList<ISolutionTreeNode>, IExtendedTreeNodeCollection
    {
        /// <summary>
        /// Gets the comparer for tree nodes.
        /// </summary>
        IComparer<ITreeNodePath> NodeComparer { get; }
    }

    /// <summary>
    /// Represents a collection of tree nodes in a solution.
    /// </summary>
    public class SolutionTreeNodeCollection : ObservableCollection<ISolutionTreeNode>, ISolutionTreeNodeCollection
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionTreeNodeCollection"/> class.
        /// </summary>
        public SolutionTreeNodeCollection()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionTreeNodeCollection"/> class.
        /// </summary>
        /// <param name="parent">The parent node.</param>
        public SolutionTreeNodeCollection(ISolutionTreeNode? parent)
            : base()
        {
            Parent = parent;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionTreeNodeCollection"/> class.
        /// </summary>
        /// <param name="parent">The parent node.</param>
        /// <param name="nodeComparer">The node comparer.</param>
        public SolutionTreeNodeCollection(ISolutionTreeNode? parent, IComparer<ITreeNodePath> nodeComparer)
            : base()
        {
            Parent = parent;
            NodeComparer = nodeComparer;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the parent node.
        /// </summary>
        public IExtendedTreeNode? Parent { get; }

        /// <summary>
        /// Gets the comparer for tree nodes.
        /// </summary>
        public IComparer<ITreeNodePath> NodeComparer { get; } = Comparer<ITreeNodePath>.Default;
        #endregion

        #region Sorting
        /// <summary>
        /// Sorts the collection.
        /// </summary>
        public virtual void Sort()
        {
            if (NodeComparer == Comparer<ITreeNodePath>.Default)
                return;

            bool SortByHighest = true;

            for (int i = 0; i < Count;)
            {
                int HighestI = i;
                int LowestI = i;

                for (int j = i + 1; j < Count; j++)
                {
                    int n = NodeComparer.Compare(this[i].Path, this[j].Path);
                    if (n > 0)
                        HighestI = j;
                    else if (n < 0)
                        LowestI = j;
                }

                int SortedIndex = SortByHighest ? HighestI : LowestI;

                if (SortedIndex != i)
                    Move(i, SortedIndex);
                else
                    i++;
            }
        }
        #endregion
    }
}
