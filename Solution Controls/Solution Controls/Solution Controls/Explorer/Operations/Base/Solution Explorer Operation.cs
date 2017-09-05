using System.Collections.ObjectModel;
using System.Windows;
using UndoRedo;
using Verification;

namespace CustomControls
{
    public abstract class SolutionExplorerOperation : IReversibleOperation
    {
        #region Init
        protected SolutionExplorerOperation(ISolutionRoot root)
        {
            Assert.ValidateReference(root);

            this.Root = root;

            IsExecuted = false;
            ExpandedFolderList = new Collection<ISolutionFolder>();
        }
        #endregion

        #region Properties
        public abstract string Name { get; }
        public ISolutionRoot Root { get; private set; }
        public bool IsExecuted { get; private set; }
        public Collection<ISolutionFolder> ExpandedFolderList { get; private set; }
        #endregion

        #region Client Interface
        public virtual void Redo()
        {
            if (IsExecuted)
                NotifyRedone();

            IsExecuted = true;
        }

        public virtual void Undo()
        {
            Assert.CheckCondition(IsExecuted);

            NotifyUndone();
        }
        #endregion

        #region Descendant Interface
        protected void ClearExpandedFolders()
        {
            ExpandedFolderList.Clear();
        }

        protected void AddExpandedFolder(ISolutionFolder folder)
        {
            Assert.ValidateReference(folder);

            if (!ExpandedFolderList.Contains(folder))
                ExpandedFolderList.Add(folder);
        }
        #endregion

        #region Events
        public event RoutedEventHandler Redone;

        private void NotifyRedone()
        {
            Assert.CheckCondition(Redone != null);
            Redone(this, new RoutedEventArgs());
        }

        public event RoutedEventHandler Undone;

        private void NotifyUndone()
        {
            Assert.CheckCondition(Undone != null);
            Undone(this, new RoutedEventArgs());
        }
        #endregion
    }
}
