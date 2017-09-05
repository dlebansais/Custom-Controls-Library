using SolutionControlsInternal.Properties;
using System.Windows;
using Verification;

namespace CustomControls
{
    internal class RenameOperation : SolutionExplorerOperation
    {
        #region Init
        public RenameOperation(ISolutionRoot root, ITreeNodePath path, string newName)
            : base(root)
        {
            Assert.ValidateReference(path);
            Assert.ValidateReference(newName);

            this.Path = path;
            this.OldName = Path.FriendlyName;
            this.NewName = newName;
        }
        #endregion

        #region Properties
        public override string Name { get { return Resources.Rename; } }
        public ITreeNodePath Path { get; private set; }
        public string OldName { get; private set; }
        public string NewName { get; private set; }
        #endregion

        #region Client Interface
        public override void Redo()
        {
            ChangeName(NewName);
            base.Redo();
        }

        public override void Undo()
        {
            ChangeName(OldName);
            base.Undo();
        }
        #endregion

        #region Implementation
        private void ChangeName(string Name)
        {
            ISolutionTreeNode Node = Root.FindTreeNode(Path);
            Assert.CheckCondition(Node != null);

            Node.ChangeName(Name);
        }
        #endregion
    }
}
