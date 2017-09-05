using SolutionControlsInternal.Properties;
using System.Collections.Generic;

namespace CustomControls
{
    internal class RemoveTreeOperation : AddRemoveTreeOperation
    {
        #region Init
        public RemoveTreeOperation(ISolutionRoot root, IReadOnlyDictionary<ITreeNodePath, IPathConnection> pathTable)
            : base(root, pathTable)
        {
        }
        #endregion

        #region Properties
        public override string Name { get { return Resources.RemoveTree; } }
        public override bool IsAdd { get { return false; } }
        #endregion

        #region Client Interface
        public override void Redo()
        {
            Remove(PathTable);
            base.Redo();
        }

        public override void Undo()
        {
            Add(PathTable);
            base.Undo();
        }
        #endregion
    }
}
