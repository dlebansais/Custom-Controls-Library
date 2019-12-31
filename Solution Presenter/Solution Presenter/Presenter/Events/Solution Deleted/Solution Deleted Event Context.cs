namespace CustomControls
{
    using System.Collections.Generic;

    public class SolutionDeletedEventContext
    {
        public SolutionDeletedEventContext(IRootPath deletedRootPath, IReadOnlyCollection<ITreeNodePath>? deletedTree)
        {
            DeletedRootPath = deletedRootPath;
            DeletedTree = deletedTree;
        }

        public IRootPath DeletedRootPath { get; private set; }
        public IReadOnlyCollection<ITreeNodePath>? DeletedTree { get; private set; }
    }
}
