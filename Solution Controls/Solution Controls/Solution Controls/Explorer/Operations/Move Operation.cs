using SolutionControlsInternal.Properties;
using Verification;

namespace CustomControls
{
    internal class MoveOperation : SolutionExplorerOperation
    {
        #region Init
        public MoveOperation(ISolutionRoot root, ITreeNodePath path, ISolutionFolder oldParent, ISolutionFolder newParent)
            : base(root)
        {
            Assert.ValidateReference(path);
            Assert.ValidateReference(oldParent);
            Assert.ValidateReference(newParent);

            this.Path = path;
            this.OldParent = oldParent;
            this.NewParent = newParent;
        }
        #endregion

        #region Properties
        public override string Name { get { return Resources.Move; } }
        public ITreeNodePath Path { get; private set; }
        public ISolutionFolder OldParent { get; private set; }
        public ISolutionFolder NewParent { get; private set; }
        #endregion

        #region Client Interface
        public override void Redo()
        {
            ChangeParent(OldParent, NewParent);
            base.Redo();
        }

        public override void Undo()
        {
            ChangeParent(NewParent, OldParent);
            base.Undo();
        }
        #endregion

        #region Implementation
        private void ChangeParent(ISolutionFolder oldParent, ISolutionFolder newParent)
        {
            ISolutionTreeNode Node = Root.FindTreeNode(Path);
            Assert.CheckCondition(Node != null);

            ISolutionTreeNodeCollection OldChildrenCollection = (ISolutionTreeNodeCollection)oldParent.Children;
            OldChildrenCollection.Remove(Node);

            ISolutionTreeNodeCollection NewChildrenCollection = (ISolutionTreeNodeCollection)newParent.Children;
            NewChildrenCollection.Add(Node);

            NewChildrenCollection.Sort();
        }
        #endregion
    }
}
