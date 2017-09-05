using System.Collections.Generic;

namespace CustomControls
{
    public class SolutionDeletedEventContext
    {
        public SolutionDeletedEventContext(IRootPath deletedRootPath, IReadOnlyCollection<ITreeNodePath> deletedTree)
        {
            this.DeletedRootPath = deletedRootPath;
            this.DeletedTree = deletedTree;
        }

        public IRootPath DeletedRootPath { get; private set; }
        public IReadOnlyCollection<ITreeNodePath> DeletedTree { get; private set; }
    }
}
