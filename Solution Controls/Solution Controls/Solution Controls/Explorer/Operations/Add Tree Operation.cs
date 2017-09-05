using System.Collections.Generic;

namespace CustomControls
{
    internal class AddTreeOperation : AddRemoveTreeOperation
    {
        #region Init
        public AddTreeOperation(ISolutionRoot root, IReadOnlyDictionary<ITreeNodePath, IPathConnection> pathTable)
            : base(root, pathTable)
        {
        }
        #endregion

        #region Properties
        public override string Name { get { return SolutionControlsInternal.Properties.Resources.AddTree; } }
        public override bool IsAdd { get { return true; } }
        #endregion

        #region Client Interface
        public override void Redo()
        {
            Add(PathTable);
            base.Redo();
        }

        public override void Undo()
        {
            Remove(PathTable);
            base.Undo();
        }
        #endregion
    }
}
