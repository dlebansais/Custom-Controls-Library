namespace CustomControls
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents the event data for a solution opened completion event.
    /// </summary>
    internal interface ISolutionOpenedCompletionArgs
    {
        /// <summary>
        /// Gets the root properties of the opened solution.
        /// </summary>
        IRootProperties? OpenedRootProperties { get; }

        /// <summary>
        /// Gets the comparer of the opened solution.
        /// </summary>
        IComparer<ITreeNodePath>? OpenedRootComparer { get; }

        /// <summary>
        /// Gets the expanded folder list.
        /// </summary>
        IList<IFolderPath>? ExpandedFolderList { get; }

        /// <summary>
        /// Gets the operation context.
        /// </summary>
        object? Context { get; }
    }

    /// <summary>
    /// Represents the event data for a solution opened completion event.
    /// </summary>
    internal class SolutionOpenedCompletionArgs : ISolutionOpenedCompletionArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionOpenedCompletionArgs"/> class.
        /// </summary>
        public SolutionOpenedCompletionArgs()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionOpenedCompletionArgs"/> class.
        /// </summary>
        /// <param name="openedRootProperties">The root properties of the opened solution.</param>
        /// <param name="openedRootComparer">The comparer of the opened solution.</param>
        /// <param name="expandedFolderList">The expanded folder list.</param>
        /// <param name="context">The operation context.</param>
        public SolutionOpenedCompletionArgs(IRootProperties openedRootProperties, IComparer<ITreeNodePath> openedRootComparer, IList<IFolderPath> expandedFolderList, object context)
        {
            OpenedRootProperties = openedRootProperties;
            OpenedRootComparer = openedRootComparer;
            ExpandedFolderList = expandedFolderList;
            Context = context;
        }

        /// <summary>
        /// Gets the root properties of the opened solution.
        /// </summary>
        public IRootProperties? OpenedRootProperties { get; }

        /// <summary>
        /// Gets the comparer of the opened solution.
        /// </summary>
        public IComparer<ITreeNodePath>? OpenedRootComparer { get; }

        /// <summary>
        /// Gets the expanded folder list.
        /// </summary>
        public IList<IFolderPath>? ExpandedFolderList { get; }

        /// <summary>
        /// Gets the operation context.
        /// </summary>
        public object? Context { get; }
    }
}
