using CustomControls;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CustomControls
{
    public interface ISolutionTreeNodeCollection : IList<ISolutionTreeNode>, IReadOnlyList<ISolutionTreeNode>, IExtendedTreeNodeCollection
    {
        IComparer<ITreeNodePath> NodeComparer { get; }
    }

    public class SolutionTreeNodeCollection : ObservableCollection<ISolutionTreeNode>, ISolutionTreeNodeCollection
    {
        #region Init
        public SolutionTreeNodeCollection(ISolutionTreeNode parent, IComparer<ITreeNodePath> nodeComparer)
            : base()
        {
            this.Parent = parent;
            this.NodeComparer = nodeComparer;
        }
        #endregion

        #region Init
        public IExtendedTreeNode Parent { get; private set; }
        public IComparer<ITreeNodePath> NodeComparer { get; private set; }
        #endregion

        #region Sorting
        public virtual void Sort()
        {
            if (NodeComparer == null)
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
                {
                    //string Name1 = this[i].Name;
                    //string Name2 = this[SortedIndex].Name;

                    Move(i, SortedIndex);
                }
                else
                    i++;
            }
        }
        #endregion
    }
}
