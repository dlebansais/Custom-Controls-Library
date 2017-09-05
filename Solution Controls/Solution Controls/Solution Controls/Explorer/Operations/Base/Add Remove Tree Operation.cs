using System.Collections.Generic;

namespace CustomControls
{
    internal abstract class AddRemoveTreeOperation : AddRemoveOperation
    {
        #region Init
        protected AddRemoveTreeOperation(ISolutionRoot root, IReadOnlyDictionary<ITreeNodePath, IPathConnection> pathTable)
            : base(root, pathTable)
        {
        }
        #endregion
    }
}
