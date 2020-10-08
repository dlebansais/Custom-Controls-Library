namespace CustomControls
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a context for the <see cref="SolutionDeletedEventArgs"/> event data.
    /// </summary>
    public class SolutionDeletedEventContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionDeletedEventContext"/> class.
        /// </summary>
        /// <param name="deletedRootPath">The path to the deleted solution.</param>
        /// <param name="deletedTree">The tree of deleted nodes.</param>
        public SolutionDeletedEventContext(IRootPath deletedRootPath, IReadOnlyCollection<ITreeNodePath>? deletedTree)
        {
            DeletedRootPath = deletedRootPath;
            DeletedTree = deletedTree;
        }

        /// <summary>
        /// Gets the path to the deleted solution.
        /// </summary>
        public IRootPath DeletedRootPath { get; private set; }

        /// <summary>
        /// Gets the tree of deleted nodes.
        /// </summary>
        public IReadOnlyCollection<ITreeNodePath>? DeletedTree { get; private set; }
    }
}
