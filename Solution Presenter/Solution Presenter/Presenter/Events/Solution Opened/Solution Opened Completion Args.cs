using System.Collections.Generic;

namespace CustomControls
{
    internal interface ISolutionOpenedCompletionArgs
    {
        IRootProperties? OpenedRootProperties { get; }
        IComparer<ITreeNodePath>? OpenedRootComparer { get; }
        IList<IFolderPath>? ExpandedFolderList { get; }
        object? Context { get; }
    }

    internal class SolutionOpenedCompletionArgs : ISolutionOpenedCompletionArgs
    {
        public SolutionOpenedCompletionArgs()
        {
        }

        public SolutionOpenedCompletionArgs(IRootProperties openedRootProperties, IComparer<ITreeNodePath> openedRootComparer, IList<IFolderPath> expandedFolderList, object context)
        {
            OpenedRootProperties = openedRootProperties;
            OpenedRootComparer = openedRootComparer;
            ExpandedFolderList = expandedFolderList;
            Context = context;
        }

        public IRootProperties? OpenedRootProperties { get; }
        public IComparer<ITreeNodePath>? OpenedRootComparer { get; }
        public IList<IFolderPath>? ExpandedFolderList { get; }
        public object? Context { get; }
    }
}
