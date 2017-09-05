using System.Collections.Generic;

namespace CustomControls
{
    internal abstract class AddRemoveSingleOperation : AddRemoveOperation
    {
        #region Init
        protected AddRemoveSingleOperation(ISolutionRoot root, IFolderPath parentPath, ITreeNodePath path, ITreeNodeProperties properties)
            : base(root, CreateSinglePathTable(parentPath, path, properties))
        {
        }

        private static IReadOnlyDictionary<ITreeNodePath, IPathConnection> CreateSinglePathTable(IFolderPath parentPath, ITreeNodePath path, ITreeNodeProperties properties)
        {
            return new Dictionary<ITreeNodePath, IPathConnection>() { { path, new PathConnection(parentPath, properties, false) } };
        }
        #endregion
    }
}
