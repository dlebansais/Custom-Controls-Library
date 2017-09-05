using System.Collections.Generic;

namespace CustomControls
{
    internal interface ISolutionOpenedCompletionArgs
    {
        IRootProperties OpenedRootProperties { get; }
        IComparer<ITreeNodePath> OpenedRootComparer { get; }
        IList<IFolderPath> ExpandedFolderList { get; }
        object Context { get; }
    }

    internal class SolutionOpenedCompletionArgs : ISolutionOpenedCompletionArgs
    {
        public SolutionOpenedCompletionArgs()
        {
            this.OpenedRootProperties = null;
            this.ExpandedFolderList = null;
            this.Context = null;
        }

        public SolutionOpenedCompletionArgs(IRootProperties OpenedRootProperties, IComparer<ITreeNodePath> OpenedRootComparer, IList<IFolderPath> ExpandedFolderList, object Context)
        {
            this.OpenedRootProperties = OpenedRootProperties;
            this.OpenedRootComparer = OpenedRootComparer;
            this.ExpandedFolderList = ExpandedFolderList;
            this.Context = Context;
        }

        public IRootProperties OpenedRootProperties { get; private set; }
        public IComparer<ITreeNodePath> OpenedRootComparer { get; private set; }
        public IList<IFolderPath> ExpandedFolderList { get; private set; }
        public object Context { get; private set; }
    }
}
