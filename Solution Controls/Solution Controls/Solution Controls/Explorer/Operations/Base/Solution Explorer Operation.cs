namespace CustomControls
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Windows;
    using UndoRedo;

    /// <summary>
    /// Represents an operation performed in a solution explorer.
    /// </summary>
    public abstract class SolutionExplorerOperation : IReversibleOperation
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionExplorerOperation"/> class.
        /// </summary>
        /// <param name="root">The solution root.</param>
        protected SolutionExplorerOperation(ISolutionRoot root)
        {
            Root = root;

            IsExecuted = false;
            ExpandedFolderList = new Collection<ISolutionFolder>();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the operation name.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the solution root.
        /// </summary>
        public ISolutionRoot Root { get; }

        /// <summary>
        /// Gets a value indicating whether the operatio has been executed.
        /// </summary>
        public bool IsExecuted { get; private set; }

        /// <summary>
        /// Gets the expanded list of folders of the solution.
        /// </summary>
        public Collection<ISolutionFolder> ExpandedFolderList { get; private set; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Performs the operation.
        /// </summary>
        public virtual void Redo()
        {
            if (IsExecuted)
                NotifyRedone();

            IsExecuted = true;
        }

        /// <summary>
        /// Performs the inverse operation.
        /// </summary>
        public virtual void Undo()
        {
            Debug.Assert(IsExecuted);

            NotifyUndone();
        }
        #endregion

        #region Descendant Interface
        /// <summary>
        /// Clears the list of folders.
        /// </summary>
        protected void ClearExpandedFolders()
        {
            ExpandedFolderList.Clear();
        }

        /// <summary>
        /// Adds a folder to the list.
        /// </summary>
        /// <param name="folder">The new folder.</param>
        protected void AddExpandedFolder(ISolutionFolder folder)
        {
            if (folder == null)
                throw new ArgumentNullException(nameof(folder));

            if (!ExpandedFolderList.Contains(folder))
                ExpandedFolderList.Add(folder);
        }
        #endregion

        #region Events
        /// <summary>
        /// Occurs after the operation has been performed.
        /// </summary>
        public event RoutedEventHandler? Redone;

        private void NotifyRedone()
        {
            Debug.Assert(Redone != null);
            Redone?.Invoke(this, new RoutedEventArgs());
        }

        /// <summary>
        /// Occurs after the inverse operation has been performed.
        /// </summary>
        public event RoutedEventHandler? Undone;

        private void NotifyUndone()
        {
            Debug.Assert(Undone != null);
            Undone?.Invoke(this, new RoutedEventArgs());
        }
        #endregion
    }
}
