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

        #region Implementation of ISolutionTreeNodeCollection
        /// <summary>
        /// Determines the index of a specific item in the <see cref="IList{IExtendedTreeNode}"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="IList{IExtendedTreeNode}"/>.</param>
        /// <returns>The index of item if found in the list; otherwise, -1.</returns>
        int IList<IExtendedTreeNode>.IndexOf(IExtendedTreeNode item)
        {
            return IndexOf((ISolutionTreeNode)item);
        }

        /// <summary>
        /// Inserts an item to the <see cref="IList{IExtendedTreeNode}"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="IList{IExtendedTreeNode}"/>.</param>
        void IList<IExtendedTreeNode>.Insert(int index, IExtendedTreeNode item)
        {
            Insert(index, (ISolutionTreeNode)item);
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        IExtendedTreeNode IList<IExtendedTreeNode>.this[int index]
        {
            get { return this[index]; }
            set { this[index] = (ISolutionTreeNode)value; }
        }

        /// <summary>
        /// Adds an item to the <see cref="ICollection{IExtendedTreeNode}"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="ICollection{IExtendedTreeNode}"/>.</param>
        void ICollection<IExtendedTreeNode>.Add(IExtendedTreeNode item)
        {
            Add((ISolutionTreeNode)item);
        }

        /// <summary>
        /// Determines whether the <see cref="ICollection{IExtendedTreeNode}"/> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="ICollection{IExtendedTreeNode}"/>.</param>
        /// <returns>True if item is found in the <see cref="ICollection{IExtendedTreeNode}"/>; otherwise, False.</returns>
        bool ICollection<IExtendedTreeNode>.Contains(IExtendedTreeNode item)
        {
            return Contains((ISolutionTreeNode)item);
        }

        /// <summary>
        /// Copies the elements of the <see cref="ICollection{IExtendedTreeNode}"/> to an System.Array, starting at a particular <see cref="System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements copied from <see cref="ICollection{IExtendedTreeNode}"/>. The <see cref="System.Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        void ICollection<IExtendedTreeNode>.CopyTo(IExtendedTreeNode[] array, int arrayIndex)
        {
            CopyTo(array as ISolutionTreeNode[], arrayIndex);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="ICollection{IExtendedTreeNode}"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="ICollection{IExtendedTreeNode}"/>.</param>
        /// <returns>True if item was successfully removed from the <see cref="ICollection{IExtendedTreeNode}"/>; otherwise, false. This method also returns false if item is not found in the original <see cref="ICollection{IExtendedTreeNode}"/>.</returns>
        bool ICollection<IExtendedTreeNode>.Remove(IExtendedTreeNode item)
        {
            return Remove((ISolutionTreeNode)item);
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="ICollection{IExtendedTreeNode}"/> is read-only.
        /// </summary>
        /// <return>True if the <see cref="ICollection{IExtendedTreeNode}"/> is read-only; otherwise, False.</return>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        IEnumerator<IExtendedTreeNode> IEnumerable<IExtendedTreeNode>.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
}
