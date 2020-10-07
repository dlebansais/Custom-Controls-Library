namespace CustomControls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.IO.Compression;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Threading;
    using UndoRedo;

    /// <summary>
    /// Represents a solution explorer control.
    /// </summary>
    public partial class SolutionExplorer : UserControl, INotifyPropertyChanged
    {
        #region Custom properties and events
        #region Solution Icon
        /// <summary>
        /// Identifies the <see cref="SolutionIcon"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SolutionIconProperty = DependencyProperty.Register("SolutionIcon", typeof(ImageSource), typeof(SolutionExplorer), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the solution icon.
        /// </summary>
        public ImageSource SolutionIcon
        {
            get { return (ImageSource)GetValue(SolutionIconProperty); }
            set { SetValue(SolutionIconProperty, value); }
        }
        #endregion
        #region Root Path
        /// <summary>
        /// Identifies the <see cref="RootPath"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RootPathProperty = RootPathPropertyKey.DependencyProperty;
        private static readonly DependencyPropertyKey RootPathPropertyKey = DependencyProperty.RegisterReadOnly("RootPath", typeof(IRootPath), typeof(SolutionExplorer), new PropertyMetadata(null));

        /// <summary>
        /// Gets the root path.
        /// </summary>
        public IRootPath RootPath
        {
            get { return (IRootPath)GetValue(RootPathProperty); }
        }
        #endregion
        #region Root Properties
        /// <summary>
        /// Identifies the <see cref="RootProperties"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RootPropertiesProperty = RootPropertiesPropertyKey.DependencyProperty;
        private static readonly DependencyPropertyKey RootPropertiesPropertyKey = DependencyProperty.RegisterReadOnly("RootProperties", typeof(IRootProperties), typeof(SolutionExplorer), new PropertyMetadata(null));

        /// <summary>
        /// Gets the root properties.
        /// </summary>
        public IRootProperties RootProperties
        {
            get { return (IRootProperties)GetValue(RootPropertiesProperty); }
        }
        #endregion
        #region Tree Node Comparer
        /// <summary>
        /// Identifies the <see cref="TreeNodeComparer"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TreeNodeComparerProperty = TreeNodeComparerPropertyKey.DependencyProperty;
        private static readonly DependencyPropertyKey TreeNodeComparerPropertyKey = DependencyProperty.RegisterReadOnly("TreeNodeComparer", typeof(IComparer<ITreeNodePath>), typeof(SolutionExplorer), new PropertyMetadata(null));

        /// <summary>
        /// Gets the comparer for tree nodes.
        /// </summary>
        public IComparer<ITreeNodePath> TreeNodeComparer
        {
            get { return (IComparer<ITreeNodePath>)GetValue(TreeNodeComparerProperty); }
        }
        #endregion
        #region Undo Redo Manager
        /// <summary>
        /// Identifies the <see cref="UndoRedoManager"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty UndoRedoManagerProperty = DependencyProperty.Register("UndoRedoManager", typeof(UndoRedoManager), typeof(SolutionExplorer), new PropertyMetadata(new UndoRedoManager()));

        /// <summary>
        /// Gets or sets the undo/redo manager of the solution.
        /// </summary>
        public UndoRedoManager UndoRedoManager
        {
            get { return (UndoRedoManager)GetValue(UndoRedoManagerProperty); }
            set { SetValue(UndoRedoManagerProperty, value); }
        }
        #endregion
        #region Is Loading Tree
        /// <summary>
        /// Identifies the <see cref="IsLoadingTree"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsLoadingTreeProperty = DependencyProperty.Register("IsLoadingTree", typeof(bool), typeof(SolutionExplorer), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets a value indicating whether the folder tree is loading.
        /// </summary>
        public bool IsLoadingTree
        {
            get { return (bool)GetValue(IsLoadingTreeProperty); }
            set { SetValue(IsLoadingTreeProperty, value); }
        }
        #endregion
        #region Context Menu Loaded
        /// <summary>
        /// Identifies the <see cref="ContextMenuLoaded"/> routed event.
        /// </summary>
        public static readonly RoutedEvent ContextMenuLoadedEvent = EventManager.RegisterRoutedEvent("ContextMenuLoaded", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionExplorer));

        /// <summary>
        /// Occurs when the context menu is loaded.
        /// </summary>
        public event RoutedEventHandler ContextMenuLoaded
        {
            add { AddHandler(ContextMenuLoadedEvent, value); }
            remove { RemoveHandler(ContextMenuLoadedEvent, value); }
        }

        /// <summary>
        /// Invokes handlers of the <see cref="ContextMenuLoaded"/> event.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected virtual void NotifyContextMenuLoaded(RoutedEventArgs e)
        {
            if (e != null)
                RaiseEvent(new RoutedEventArgs(ContextMenuLoadedEvent, e.OriginalSource));
        }
        #endregion
        #region Context Menu Opened
        /// <summary>
        /// Identifies the <see cref="ContextMenuOpened"/> routed event.
        /// </summary>
        public static readonly RoutedEvent ContextMenuOpenedEvent = EventManager.RegisterRoutedEvent("ContextMenuOpened", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionExplorer));

        /// <summary>
        /// Occurs when the context menu is opened.
        /// </summary>
        public event RoutedEventHandler ContextMenuOpened
        {
            add { AddHandler(ContextMenuOpenedEvent, value); }
            remove { RemoveHandler(ContextMenuOpenedEvent, value); }
        }

        /// <summary>
        /// Invokes handlers of the <see cref="ContextMenuOpened"/> event.
        /// </summary>
        /// <param name="selectedItems">The selected items.</param>
        /// <param name="canShowCommandList">The list of commands that can be shown.</param>
        protected virtual void NotifyContextMenuOpened(IReadOnlyCollection<ITreeNodePath> selectedItems, ICollection<ExtendedRoutedCommand> canShowCommandList)
        {
            ContextMenuOpenedEventArgs Args = new ContextMenuOpenedEventArgs(ContextMenuOpenedEvent, selectedItems, canShowCommandList);
            RaiseEvent(Args);
        }
        #endregion
        #region Preview Name Changed
        /// <summary>
        /// Identifies the <see cref="PreviewNameChanged"/> routed event.
        /// </summary>
        public static readonly RoutedEvent PreviewNameChangedEvent = EventManager.RegisterRoutedEvent("PreviewNameChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionExplorer));

        /// <summary>
        /// Occurs before the name is changed.
        /// </summary>
        public event RoutedEventHandler PreviewNameChanged
        {
            add { AddHandler(PreviewNameChangedEvent, value); }
            remove { RemoveHandler(PreviewNameChangedEvent, value); }
        }

        /// <summary>
        /// Invokes handlers of the <see cref="PreviewNameChanged"/> event.
        /// </summary>
        /// <param name="path">The path to the item with name changed.</param>
        /// <param name="oldName">The old name.</param>
        /// <param name="newName">The new name.</param>
        protected virtual void NotifyPreviewNameChanged(ITreeNodePath path, string oldName, string newName)
        {
            NameChangedEventArgs Args = new NameChangedEventArgs(PreviewNameChangedEvent, path, oldName, newName, false);
            RaiseEvent(Args);
        }
        #endregion
        #region Name Changed
        /// <summary>
        /// Identifies the <see cref="NameChanged"/> routed event.
        /// </summary>
        public static readonly RoutedEvent NameChangedEvent = EventManager.RegisterRoutedEvent("NameChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionExplorer));

        /// <summary>
        /// Occurs after the name is changed, or the operation was canceled.
        /// </summary>
        public event RoutedEventHandler NameChanged
        {
            add { AddHandler(NameChangedEvent, value); }
            remove { RemoveHandler(NameChangedEvent, value); }
        }

        /// <summary>
        /// Invokes handlers of the <see cref="NameChanged"/> event.
        /// </summary>
        /// <param name="path">The path to the item with name changed.</param>
        /// <param name="oldName">The old name.</param>
        /// <param name="newName">The new name.</param>
        /// <param name="isUndoRedo">True if the operation can be undone.</param>
        /// <returns>True if the operation was canceled; otherwise, false.</returns>
        protected virtual bool NotifyNameChanged(ITreeNodePath path, string oldName, string newName, bool isUndoRedo)
        {
            NameChangedEventArgs Args = new NameChangedEventArgs(NameChangedEvent, path, oldName, newName, isUndoRedo);
            RaiseEvent(Args);

            return Args.IsCanceled;
        }
        #endregion
        #region Moved
        /// <summary>
        /// Identifies the <see cref="Moved"/> routed event.
        /// </summary>
        public static readonly RoutedEvent MovedEvent = EventManager.RegisterRoutedEvent("Moved", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionExplorer));

        /// <summary>
        /// Occurs when a folder is moved, or the operation was canceled.
        /// </summary>
        public event RoutedEventHandler Moved
        {
            add { AddHandler(MovedEvent, value); }
            remove { RemoveHandler(MovedEvent, value); }
        }

        /// <summary>
        /// Invokes handlers of the <see cref="NameChanged"/> event.
        /// </summary>
        /// <param name="path">Path to the moved folder.</param>
        /// <param name="oldParentPath">The path to the old parent.</param>
        /// <param name="newParentPath">The path to the new parent.</param>
        /// <param name="isUndoRedo">True if the operation can be undone.</param>
        /// <returns>True if the operation was canceled; otherwise, false.</returns>
        protected virtual bool NotifyMoved(ITreeNodePath path, IFolderPath oldParentPath, IFolderPath newParentPath, bool isUndoRedo)
        {
            MovedEventArgs Args = new MovedEventArgs(MovedEvent, path, oldParentPath, newParentPath, isUndoRedo);
            RaiseEvent(Args);

            return Args.IsCanceled;
        }
        #endregion
        #region Tree Changed
        /// <summary>
        /// Identifies the <see cref="TreeChanged"/> routed event.
        /// </summary>
        public static readonly RoutedEvent TreeChangedEvent = EventManager.RegisterRoutedEvent("TreeChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionExplorer));

        /// <summary>
        /// Occurs when the folder tree has changed, or the operation was canceled.
        /// </summary>
        public event RoutedEventHandler TreeChanged
        {
            add { AddHandler(TreeChangedEvent, value); }
            remove { RemoveHandler(TreeChangedEvent, value); }
        }

        /// <summary>
        /// Invokes handlers of the <see cref="TreeChanged"/> event.
        /// </summary>
        /// <param name="pathTable">The new tree.</param>
        /// <param name="isAdd">True if the operation was to add a folder.</param>
        /// <param name="isUndoRedo">True if the operation can be undone.</param>
        /// <returns>True if the operation was canceled; otherwise, false.</returns>
        protected virtual bool NotifyTreeChanged(IReadOnlyDictionary<ITreeNodePath, IPathConnection> pathTable, bool isAdd, bool isUndoRedo)
        {
            TreeChangedEventArgs Args = new TreeChangedEventArgs(TreeChangedEvent, pathTable, isAdd, isUndoRedo);
            RaiseEvent(Args);

            return Args.IsCanceled;
        }
        #endregion
        #region Selection Changed
        /// <summary>
        /// Identifies the <see cref="SelectionChanged"/> routed event.
        /// </summary>
        public static readonly RoutedEvent SelectionChangedEvent = ExtendedTreeViewBase.SelectionChangedEvent;

        /// <summary>
        /// Occurs when the selection has changed.
        /// </summary>
        public event RoutedEventHandler SelectionChanged
        {
            add { AddHandler(SelectionChangedEvent, value); }
            remove { RemoveHandler(SelectionChangedEvent, value); }
        }
        #endregion
        #region Imported
        /// <summary>
        /// Identifies the <see cref="Imported"/> routed event.
        /// </summary>
        public static readonly RoutedEvent ImportedEvent = EventManager.RegisterRoutedEvent("Imported", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionExplorer));

        /// <summary>
        /// Occurs when an object was imported.
        /// </summary>
        public event RoutedEventHandler Imported
        {
            add { AddHandler(ImportedEvent, value); }
            remove { RemoveHandler(ImportedEvent, value); }
        }

        /// <summary>
        /// Invokes handlers of the <see cref="TreeChanged"/> event.
        /// </summary>
        /// <param name="package">The solution package.</param>
        /// <param name="folderName">The folder name.</param>
        /// <returns>The imported object path.</returns>
        protected virtual IRootPath NotifyImported(SolutionPackage package, string folderName)
        {
            ImportedEventArgs Args = new ImportedEventArgs(ImportedEvent, package, folderName);
            RaiseEvent(Args);

            return Args.RootPath;
        }

        /// <summary>
        /// Invokes handlers of the <see cref="TreeChanged"/> event.
        /// </summary>
        /// <param name="package">The solution package.</param>
        /// <param name="rootPath">The root path.</param>
        /// <param name="currentFolderPath">The current folder path.</param>
        /// <param name="folderName">The folder name.</param>
        /// <returns>The imported object path.</returns>
        protected virtual IFolderPath NotifyImported(SolutionPackage package, IRootPath rootPath, IFolderPath currentFolderPath, string folderName)
        {
            ImportedEventArgs Args = new ImportedEventArgs(ImportedEvent, package, rootPath, currentFolderPath, folderName);
            RaiseEvent(Args);

            return Args.CurrentFolderPath;
        }

        /// <summary>
        /// Invokes handlers of the <see cref="TreeChanged"/> event.
        /// </summary>
        /// <param name="package">The solution package.</param>
        /// <param name="rootPath">The root path.</param>
        /// <param name="currentFolderPath">The current folder path.</param>
        /// <param name="itemName">The item name.</param>
        /// <param name="content">The item content.</param>
        protected virtual void NotifyImported(SolutionPackage package, IRootPath rootPath, IFolderPath currentFolderPath, string itemName, byte[] content)
        {
            ImportedEventArgs Args = new ImportedEventArgs(ImportedEvent, package, rootPath, currentFolderPath, itemName, content);
            RaiseEvent(Args);
        }
        #endregion
        #endregion

        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionExplorer"/> class.
        /// </summary>
        public SolutionExplorer()
        {
            RootInternal = null;
            Initialized += OnInitialized; // Dirty trick to avoid warning CA2214.
            InitializeComponent();
        }

        /// <summary>
        /// Called when the control has been initialized and before properties are set.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An object that contains no event data.</param>
        protected virtual void OnInitialized(object sender, EventArgs e)
        {
            InitializeDragDrop();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the solution root.
        /// </summary>
        public ISolutionRoot Root
        {
            get
            {
                if (RootInternal != null)
                    return RootInternal;
                else
                    throw new InvalidOperationException();
            }
            set
            {
                if (RootInternal != value)
                {
                    RootInternal = value;
                    NotifyThisPropertyChanged();
                }
            }
        }
        private ISolutionRoot? RootInternal;

        /// <summary>
        /// Gets the selected nodes.
        /// </summary>
        public IReadOnlyDictionary<ITreeNodePath, IPathConnection> SelectedNodes
        {
            get
            {
                Dictionary<ITreeNodePath, IPathConnection> PathTable = new Dictionary<ITreeNodePath, IPathConnection>();
                foreach (ISolutionTreeNode Child in treeviewSolutionExplorer.SelectedItems)
                {
                    if (PathTable.ContainsKey(Child.Path))
                        continue;

                    ISolutionFolder? ParentFolder = Child.Parent as ISolutionFolder;
                    IFolderPath? ParentPath = ParentFolder != null ? (IFolderPath)ParentFolder.Path : null;
                    PathTable.Add(Child.Path, new PathConnection(ParentPath, Child.Properties, treeviewSolutionExplorer.IsExpanded(Child)));
                }

                return PathTable;
            }
        }

        /// <summary>
        /// Gets the selected tree of nodes.
        /// </summary>
        public IReadOnlyDictionary<ITreeNodePath, IPathConnection> SelectedTree
        {
            get
            {
                Dictionary<ITreeNodePath, IPathConnection> PathConnectionTable = new Dictionary<ITreeNodePath, IPathConnection>();
                RecursiveGetTree(treeviewSolutionExplorer.SelectedItems, PathConnectionTable);

                return PathConnectionTable;
            }
        }

        private void RecursiveGetTree(IList items, Dictionary<ITreeNodePath, IPathConnection> pathConnectionTable)
        {
            foreach (ISolutionTreeNode Child in items)
            {
                if (pathConnectionTable.ContainsKey(Child.Path))
                    continue;

                ISolutionFolder? ParentFolder = Child.Parent as ISolutionFolder;
                IFolderPath? ParentPath = ParentFolder != null ? (IFolderPath)ParentFolder.Path : null;

                pathConnectionTable.Add(Child.Path, new PathConnection(ParentPath, Child.Properties, treeviewSolutionExplorer.IsExpanded(Child)));

                if (Child is ISolutionFolder AsFolder)
                    RecursiveGetTree(AsFolder.Children, pathConnectionTable);
            }
        }

        /// <summary>
        /// Gets the list of modified items.
        /// </summary>
        public ICollection<ITreeNodePath> DirtyItems
        {
            get
            {
                if (Root != null)
                    return GetDirtyItems(Root);
                else
                    return new List<ITreeNodePath>();
            }
        }

        private ICollection<ITreeNodePath> GetDirtyItems(ISolutionTreeNode baseNode)
        {
            List<ITreeNodePath> DirtyItemList = new List<ITreeNodePath>();

            if (baseNode.IsDirty)
                DirtyItemList.Add(baseNode.Path);

            foreach (ISolutionTreeNode Child in baseNode.Children)
                DirtyItemList.AddRange(GetDirtyItems(Child));

            return DirtyItemList;
        }

        /// <summary>
        /// Gets the list of items with modified properties.
        /// </summary>
        public ICollection<ITreeNodePath> DirtyProperties
        {
            get
            {
                if (Root != null)
                    return GetDirtyProperties(Root);
                else
                    return new List<ITreeNodePath>();
            }
        }

        private ICollection<ITreeNodePath> GetDirtyProperties(ISolutionTreeNode baseNode)
        {
            List<ITreeNodePath> DirtyPropertiesList = new List<ITreeNodePath>();

            if (baseNode.Properties != null && baseNode.Properties.IsDirty)
                DirtyPropertiesList.Add(baseNode.Path);

            foreach (ISolutionTreeNode Child in baseNode.Children)
                DirtyPropertiesList.AddRange(GetDirtyProperties(Child));

            return DirtyPropertiesList;
        }

        /// <summary>
        /// Gets the valid edit operation.
        /// </summary>
        public ValidEditOperations ValidEditOperations
        {
            get
            {
                bool IsSolutionSelected = false;
                bool IsFolderSelected = false;
                bool IsDocumentSelected = false;
                bool IsSingleTarget = treeviewSolutionExplorer.SelectedItems.Count == 1;

                foreach (ISolutionTreeNode Item in treeviewSolutionExplorer.SelectedItems)
                {
                    if (Item is ISolutionRoot)
                        IsSolutionSelected = true;
                    else if (Item is ISolutionFolder)
                        IsFolderSelected = true;
                    else if (Item is ISolutionItem)
                        IsDocumentSelected = true;
                }

                bool CanCutOrCopy = treeviewSolutionExplorer.IsCopyPossible;

                bool CanPaste;
                IDataObject? DataObject = Clipboard.GetDataObject();
                if (DataObject != null)
                {
                    ClipboardPathData? Data = DataObject.GetData(ClipboardPathData.SolutionExplorerClipboardPathFormat) as ClipboardPathData;
                    CanPaste = Data != null;
                }
                else
                    CanPaste = false;

                return new ValidEditOperations(IsSolutionSelected, IsFolderSelected, IsDocumentSelected, IsSingleTarget, CanCutOrCopy, CanPaste);
            }
        }

        /// <summary>
        /// Gets the expanded list of folders.
        /// </summary>
        public IList<IFolderPath> ExpandedFolderList
        {
            get
            {
                List<IFolderPath> Result = new List<IFolderPath>();

                IList VisibleItems = treeviewSolutionExplorer.VisibleItems;
                foreach (ISolutionTreeNode Item in VisibleItems)
                {
                    if (Item == Root)
                        continue;

                    if (Item.Path is IFolderPath AsFolderPath)
                        if (treeviewSolutionExplorer.IsExpanded(Item))
                            Result.Add(AsFolderPath);
                }

                return Result;
            }
        }

        /// <summary>
        /// Gets the item following the last selected item.
        /// </summary>
        public ITreeNodePath? ItemAfterLastSelected
        {
            get
            {
                IList SelectedItems = treeviewSolutionExplorer.SelectedItems;
                if (SelectedItems.Count > 0)
                {
                    IList VisibleItems = treeviewSolutionExplorer.VisibleItems;
                    int Index = VisibleItems.IndexOf(SelectedItems[SelectedItems.Count - 1]);
                    if (Index + 1 < VisibleItems.Count)
                        Index++;
                    return ((ISolutionTreeNode)VisibleItems[Index]).Path;
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// Gets the selected folder.
        /// </summary>
        public IFolderPath? SelectedFolder
        {
            get
            {
                if (treeviewSolutionExplorer.SelectedItems.Count == 1)
                    if (treeviewSolutionExplorer.SelectedItems[0] is ISolutionFolder AsFolder)
                        return (IFolderPath)AsFolder.Path;

                return null;
            }
        }

        /// <summary>
        /// Gets the list of items in the solution.
        /// </summary>
        public ICollection<IItemPath> SolutionItems
        {
            get { return GetFlatChildrenItems(Root); }
        }

        private List<IItemPath> GetFlatChildrenItems(ISolutionFolder folder)
        {
            List<IItemPath> Result = new List<IItemPath>();

            foreach (ISolutionTreeNode Child in folder.Children)
            {
                switch (Child)
                {
                    case ISolutionFolder AsFolder:
                        Result.AddRange(GetFlatChildrenItems(AsFolder));
                        break;

                    case ISolutionItem AsItem:
                        Result.Add((IItemPath)AsItem.Path);
                        break;
                }
            }

            return Result;
        }

        /// <summary>
        /// Gets the menu separator in document menu.
        /// </summary>
        public Separator DocumentMenuSeparator
        {
            get { return separatorAddDocumentStart; }
        }

        /// <summary>
        /// Gets the menu separator in item menu.
        /// </summary>
        public Separator ContextMenuSeparator
        {
            get { return separatorContextMenuItem; }
        }

        /// <summary>
        /// Gets a value indicating whether the last operation can be undone.
        /// </summary>
        public bool CanUndo
        {
            get { return UndoRedoManager.CanUndo; }
        }

        /// <summary>
        /// Gets a value indicating whether the last undone operation can be redone.
        /// </summary>
        public bool CanRedo
        {
            get { return UndoRedoManager.CanRedo; }
        }
        #endregion

        #region Client Interface
        /// <summary>
        /// Resets the root.
        /// </summary>
        public void ResetRoot()
        {
            SetValue(RootPathPropertyKey, null);
            SetValue(RootPropertiesPropertyKey, null);
            SetValue(TreeNodeComparerPropertyKey, null);

            RootInternal = null;
            NotifyThisPropertyChanged();

            UndoRedoManager.Reset();
        }

        /// <summary>
        /// Sets the solution root.
        /// </summary>
        /// <param name="newRootPath">Path to the new root.</param>
        /// <param name="newRootProperties">New root properties.</param>
        /// <param name="newComparer">New comparer to use for nodes.</param>
        public void SetRoot(IRootPath newRootPath, IRootProperties newRootProperties, IComparer<ITreeNodePath> newComparer)
        {
            SetValue(RootPathPropertyKey, newRootPath);
            SetValue(RootPropertiesPropertyKey, newRootProperties);
            SetValue(TreeNodeComparerPropertyKey, newComparer);

            Root = CreateSolutionRoot(newRootPath, newRootProperties, newComparer);
            UndoRedoManager.Reset();
        }

        /// <summary>
        /// Creates the solution root from a path.
        /// </summary>
        /// <param name="path">The root path.</param>
        /// <param name="properties">The root properties.</param>
        /// <param name="comparer">The comparer to use for nodes.</param>
        /// <returns>The created solutuon root.</returns>
        protected virtual ISolutionRoot CreateSolutionRoot(IRootPath path, IRootProperties properties, IComparer<ITreeNodePath> comparer)
        {
            return new SolutionRoot(path, properties, comparer);
        }

        /// <summary>
        /// Sets the focus to this control.
        /// </summary>
        public new virtual void Focus()
        {
            treeviewSolutionExplorer.Focus();
        }

        /// <summary>
        /// Changes a name.
        /// </summary>
        /// <param name="path">The path to the item to change.</param>
        /// <param name="newName">The new name.</param>
        public void ChangeName(ITreeNodePath path, string newName)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            RenameOperation Operation = new RenameOperation(Root, path, newName);
            UndoRedoManager.AddAndExecuteOperation(Operation);
            Operation.Redone += OnRenameRedone;
            Operation.Undone += OnRenameUndone;
        }

        private void OnRenameRedone(object sender, RoutedEventArgs e)
        {
            RenameOperation Operation = (RenameOperation)sender;
            NotifyNameChanged(Operation.Path, Operation.OldName, Operation.NewName, true);
        }

        private void OnRenameUndone(object sender, RoutedEventArgs e)
        {
            RenameOperation Operation = (RenameOperation)sender;
            NotifyNameChanged(Operation.Path, Operation.NewName, Operation.OldName, true);
        }

        /// <summary>
        /// Moves nodes.
        /// </summary>
        /// <param name="path">The source path.</param>
        /// <param name="destinationPath">The destination path.</param>
        public void Move(ITreeNodePath path, IFolderPath destinationPath)
        {
            ISolutionTreeNode? Node = Root.FindTreeNode(path);
            ISolutionFolder? OldParent = Node?.Parent as ISolutionFolder;

            IFolderPath newParentPath = destinationPath;
            ISolutionFolder? NewParent = Root.FindTreeNode(newParentPath) as ISolutionFolder;
            if (Node != null && OldParent != null && NewParent != null)
            {
                MoveOperation Operation = new MoveOperation(Root, path, OldParent, NewParent);
                UndoRedoManager.AddAndExecuteOperation(Operation);
                Operation.Redone += OnMoveRedone;
                Operation.Undone += OnMoveUndone;
            }
        }

        private void OnMoveUndone(object sender, RoutedEventArgs e)
        {
            MoveOperation Operation = (MoveOperation)sender;
            NotifyMoved(Operation.Path, (IFolderPath)Operation.NewParent.Path, (IFolderPath)Operation.OldParent.Path, true);
        }

        private void OnMoveRedone(object sender, RoutedEventArgs e)
        {
            MoveOperation Operation = (MoveOperation)sender;
            NotifyMoved(Operation.Path, (IFolderPath)Operation.OldParent.Path, (IFolderPath)Operation.NewParent.Path, true);
        }

        /// <summary>
        /// Expands a folder.
        /// </summary>
        /// <param name="folder">The folder to expand.</param>
        public void ExpandFolder(IFolderPath folder)
        {
            treeviewSolutionExplorer.Expand(folder);
        }

        /// <summary>
        /// Copy the solution to the clipboard.
        /// </summary>
        public void Copy()
        {
            IReadOnlyDictionary<ITreeNodePath, IPathConnection> PathTable = SelectedTree;
            if (PathGroup.HasCommonParent(PathTable))
            {
                ClipboardPathData Data = new ClipboardPathData(new PathGroup(PathTable));
                DataObject CopiedDataObject = new DataObject(ClipboardPathData.SolutionExplorerClipboardPathFormat, Data);
                Clipboard.SetDataObject(CopiedDataObject);
            }
        }

        /// <summary>
        /// Reads the clipboard to get the content of a solution.
        /// </summary>
        /// <returns>The solution data.</returns>
        public static ClipboardPathData? ReadClipboard()
        {
            IDataObject DataObject = Clipboard.GetDataObject();
            if (DataObject != null)
                return DataObject.GetData(ClipboardPathData.SolutionExplorerClipboardPathFormat) as ClipboardPathData;
            else
                return null;
        }

        /// <summary>
        /// Undoes the last operation.
        /// </summary>
        public void Undo()
        {
            UndoRedoManager.Undo();
            ExpandNewNodes((SolutionExplorerOperation)UndoRedoManager.LastOperation);
        }

        /// <summary>
        /// Redoes the last undone operation.
        /// </summary>
        public void Redo()
        {
            UndoRedoManager.Redo();
            ExpandNewNodes((SolutionExplorerOperation)UndoRedoManager.LastOperation);
        }

        /// <summary>
        /// Select all items in the solution.
        /// </summary>
        public void SelectAll()
        {
            treeviewSolutionExplorer.SelectAll();
        }

        /// <summary>
        /// Adds a folder to the solution.
        /// </summary>
        /// <param name="destinationFolderPath">The path to the destination.</param>
        /// <param name="newFolderPath">The folder to add.</param>
        /// <param name="newFolderProperties">The folder properties.</param>
        public void AddFolder(IFolderPath destinationFolderPath, IFolderPath newFolderPath, IFolderProperties newFolderProperties)
        {
            AddFolderOperation Operation = new AddFolderOperation(Root, destinationFolderPath, newFolderPath, newFolderProperties);
            UndoRedoManager.AddAndExecuteOperation(Operation);
            Operation.Redone += OnAddRemoveRedone;
            Operation.Undone += OnAddRemoveUndone;

            ExpandNewNodes(Operation);
        }

        /// <summary>
        /// Adds an item to the solution.
        /// </summary>
        /// <param name="destinationFolderPath">The path to the destination.</param>
        /// <param name="newItemPath">The item to add.</param>
        /// <param name="newItemProperties">The item properties.</param>
        public void AddItem(IFolderPath destinationFolderPath, IItemPath newItemPath, IItemProperties newItemProperties)
        {
            AddItemOperation Operation = new AddItemOperation(Root, destinationFolderPath, newItemPath, newItemProperties);
            UndoRedoManager.AddAndExecuteOperation(Operation);
            Operation.Redone += OnAddRemoveRedone;
            Operation.Undone += OnAddRemoveUndone;

            ExpandNewNodes(Operation);
        }

        /// <summary>
        /// Adds a tree to the solution.
        /// </summary>
        /// <param name="pathTable">The table of path to add.</param>
        public void AddTree(IReadOnlyDictionary<ITreeNodePath, IPathConnection> pathTable)
        {
            AddTreeOperation Operation = new AddTreeOperation(Root, pathTable);
            UndoRedoManager.AddAndExecuteOperation(Operation);
            Operation.Redone += OnAddRemoveRedone;
            Operation.Undone += OnAddRemoveUndone;

            ExpandNewNodes(Operation);
        }

        /// <summary>
        /// Deletes a tree from the solution.
        /// </summary>
        /// <param name="pathTable">The table of path to delete.</param>
        public void DeleteTree(IReadOnlyDictionary<ITreeNodePath, IPathConnection> pathTable)
        {
            RemoveTreeOperation Operation = new RemoveTreeOperation(Root, pathTable);
            UndoRedoManager.AddAndExecuteOperation(Operation);
            Operation.Redone += OnAddRemoveRedone;
            Operation.Undone += OnAddRemoveUndone;
        }

        private void OnAddRemoveRedone(object sender, RoutedEventArgs e)
        {
            AddRemoveOperation Operation = (AddRemoveOperation)sender;
            NotifyTreeChanged(Operation.PathTable, Operation.IsAdd, true);
        }

        private void OnAddRemoveUndone(object sender, RoutedEventArgs e)
        {
            AddRemoveOperation Operation = (AddRemoveOperation)sender;
            NotifyTreeChanged(Operation.PathTable, !Operation.IsAdd, true);
        }

        /// <summary>
        /// Selects a node of the solution.
        /// </summary>
        /// <param name="path">The node to select.</param>
        public void SetSelected(ITreeNodePath path)
        {
            treeviewSolutionExplorer.UnselectAll();

            ISolutionTreeNode? TreeNode = Root.FindTreeNode(path);
            if (TreeNode != null)
            {
                treeviewSolutionExplorer.SetSelected(TreeNode);
                treeviewSolutionExplorer.Focus();
            }
        }

        /// <summary>
        /// Clears flags indicating whether nodes are modified.
        /// </summary>
        public void ClearDirtyItemsAndProperties()
        {
            if (Root != null)
                ClearDirtyItemsAndProperties(Root);
        }

        /// <summary>
        /// Clears flags indicating whether nodes are modified in a folder.
        /// </summary>
        /// <param name="folder">The folder.</param>
        protected virtual void ClearDirtyItemsAndProperties(ISolutionFolder folder)
        {
            ClearNodeDirtyItemsAndProperties(folder);

            if (folder != null)
                foreach (ISolutionTreeNode Child in folder.Children)
                {
                    if (Child is ISolutionFolder AsFolder)
                        ClearDirtyItemsAndProperties(AsFolder);
                    else
                        ClearNodeDirtyItemsAndProperties(Child);
                }
        }

        /// <summary>
        /// Clears flags indicating whether nodes are modified in a node.
        /// </summary>
        /// <param name="node">The node.</param>
        protected virtual void ClearNodeDirtyItemsAndProperties(ISolutionTreeNode node)
        {
            if (node != null)
            {
                if (node.IsDirty)
                    node.ClearIsDirty();

                if (node.Properties != null && node.Properties.IsDirty)
                    node.Properties.ClearIsDirty();
            }
        }

        /// <summary>
        /// Gets a table of items by their path.
        /// </summary>
        /// <param name="documentPathList">The table of paths to search.</param>
        /// <returns>The table of items.</returns>
        public IReadOnlyDictionary<ITreeNodePath, IPathConnection> FindItemsByDocumentPath(IReadOnlyCollection<IDocumentPath> documentPathList)
        {
            if (documentPathList == null)
                throw new ArgumentNullException(nameof(documentPathList));

            return FindItemsByDocumentPath(Root, documentPathList);
        }

        private IReadOnlyDictionary<ITreeNodePath, IPathConnection> FindItemsByDocumentPath(ISolutionFolder folder, IReadOnlyCollection<IDocumentPath> documentPathList)
        {
            Dictionary<ITreeNodePath, IPathConnection> Result = new Dictionary<ITreeNodePath, IPathConnection>();

            foreach (ISolutionTreeNode Child in folder.Children)
                switch (Child)
                {
                    case ISolutionFolder AsFolder:
                        IReadOnlyDictionary<ITreeNodePath, IPathConnection> InnerTree = FindItemsByDocumentPath(AsFolder, documentPathList);
                        foreach (KeyValuePair<ITreeNodePath, IPathConnection> Entry in InnerTree)
                            Result.Add(Entry.Key, Entry.Value);
                        break;

                    case ISolutionItem AsItem:
                        IItemPath ItemPath = (IItemPath)AsItem.Path;
                        foreach (IDocumentPath DocumentPath in documentPathList)
                            if (ItemPath.DocumentPath.IsEqual(DocumentPath))
                            {
                                if (Child.Parent is ISolutionFolder ParentFolder)
                                    Result.Add(Child.Path, new PathConnection((IFolderPath)ParentFolder.Path, Child.Properties, treeviewSolutionExplorer.IsExpanded(Child)));
                                break;
                            }
                        break;
                }

            return Result;
        }

        /// <summary>
        /// Gets the properties of an item.
        /// </summary>
        /// <param name="path">The path to the item.</param>
        /// <returns>The properties.</returns>
        public IItemProperties? GetItemProperties(IItemPath path)
        {
            if (Root.FindTreeNode(path) is ISolutionItem Item)
                return Item.Properties as IItemProperties;
            else
                return null;
        }

        /// <summary>
        /// Gets children of a folder.
        /// </summary>
        /// <param name="folderPath">The folder path.</param>
        /// <returns>The list of children.</returns>
        public IReadOnlyCollection<ITreeNodePath>? GetChildren(IFolderPath folderPath)
        {
            if (Root.FindTreeNode(folderPath) is ISolutionFolder ParentFolder)
            {
                Collection<ITreeNodePath> Result = new Collection<ITreeNodePath>();

                foreach (SolutionTreeNode Child in ParentFolder.Children)
                    Result.Add(Child.Path);

                return Result;
            }
            else
                return null;
        }

        /// <summary>
        /// Gets the tree at a folder.
        /// </summary>
        /// <param name="folderPath">The path to the folder.</param>
        /// <returns>The tree of nodes.</returns>
        public IReadOnlyCollection<ITreeNodePath>? GetTree(IFolderPath folderPath)
        {
            if (Root.FindTreeNode(folderPath) is ISolutionFolder ParentFolder)
                return GetTree(ParentFolder);
            else
                return null;
        }

        private IReadOnlyCollection<ITreeNodePath> GetTree(ISolutionFolder parentFolder)
        {
            Collection<ITreeNodePath> Result = new Collection<ITreeNodePath>();
            Result.Add(parentFolder.Path);

            foreach (SolutionTreeNode Child in parentFolder.Children)
                if (Child is ISolutionFolder AsFolder)
                {
                    IReadOnlyCollection<ITreeNodePath> InnerTree = GetTree(AsFolder);
                    foreach (ITreeNodePath Path in InnerTree)
                        Result.Add(Path);
                }
                else
                    Result.Add(Child.Path);

            return Result;
        }

        /// <summary>
        /// Gets the source of an event.
        /// </summary>
        /// <param name="e">The event data.</param>
        /// <returns>The event source.</returns>
        public static ITreeNodePath? GetEventSource(RoutedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (e.OriginalSource is FrameworkElement SourceElement)
                if (SourceElement.DataContext is ISolutionTreeNode AsTreeNode)
                    return AsTreeNode.Path;

            return null;
        }

        /// <summary>
        /// Triggers a rename operation of the focused item.
        /// </summary>
        public void TriggerRename()
        {
            if (Keyboard.FocusedElement is ExtendedTreeViewItemBase AsItemBase)
            {
                DependencyObject RootObject = VisualTreeHelper.GetChild(AsItemBase, 0);
                if (FindEditableTextBlock(RootObject) is EditableTextBlock Ctrl)
                    Ctrl.IsEditing = true;
            }
        }

        private EditableTextBlock? FindEditableTextBlock(DependencyObject rootObject)
        {
            if (rootObject is EditableTextBlock AsEditableTextBlock)
                return AsEditableTextBlock;

            int Count = VisualTreeHelper.GetChildrenCount(rootObject);
            for (int i = 0; i < Count; i++)
            {
                if (FindEditableTextBlock(VisualTreeHelper.GetChild(rootObject, i)) is EditableTextBlock ChildEdit)
                    return ChildEdit;
            }

            return null;
        }

        /// <summary>
        /// Resets the undo redo manager.
        /// </summary>
        public void ResetUndoRedo()
        {
            UndoRedoManager.Reset();
        }

        /// <summary>
        /// Creates a package for the solution.
        /// </summary>
        /// <param name="destinationPath">Path to the package file.</param>
        /// <param name="contentTable">table of object content.</param>
        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "If you think it's ok to dispose of my object, then I think it's ok to dispose of it twice, so FO")]
        public void CreateExportedSolutionPackage(string destinationPath, Dictionary<IDocumentPath, byte[]> contentTable)
        {
            using (FileStream fs = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (ZipArchive Archive = new ZipArchive(fs, ZipArchiveMode.Create))
                {
                    InsertNodeContent(Archive, string.Empty, Root, contentTable);
                }
            }
        }

        private static void InsertNodeContent(ZipArchive archive, string folderPathIn, ISolutionFolder parentFolder, Dictionary<IDocumentPath, byte[]> contentTable)
        {
            foreach (SolutionTreeNode Child in parentFolder.Children)
            {
                switch (Child)
                {
                    case ISolutionFolder AsFolder:
                        string InnerFolderPath = folderPathIn + AsFolder.Name;

                        InsertNodeContent(archive, InnerFolderPath + @"\", AsFolder, contentTable);
                        break;

                    case ISolutionItem AsItem:
                        InsertItemContent(archive, folderPathIn, AsItem, contentTable);
                        break;
                }
            }
        }

        private static void InsertItemContent(ZipArchive archive, string folderPathIn, ISolutionItem asItem, Dictionary<IDocumentPath, byte[]> contentTable)
        {
            IItemPath Path = (IItemPath)asItem.Path;
            IDocumentPath ArchivedDocumentPath = Path.DocumentPath;

            foreach (KeyValuePair<IDocumentPath, byte[]> Entry in contentTable)
                if (ArchivedDocumentPath.IsEqual(Entry.Key))
                {
                    byte[] Content = Entry.Value;

                    using (MemoryStream ms = new MemoryStream(Content))
                    {
                        string ExportId = ArchivedDocumentPath.ExportId;

                        ZipArchiveEntry ArchiveEntry = archive.CreateEntry(folderPathIn + ExportId);
                        using (Stream ArchiveStream = ArchiveEntry.Open())
                        {
                            ms.CopyTo(ArchiveStream);
                        }
                    }
                }
        }

        /// <summary>
        /// Reads a solution package.
        /// </summary>
        /// <param name="package">The solution package.</param>
        /// <param name="destinationPath">Path to the package file.</param>
        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "If you think it's ok to dispose of my object, then I think it's ok to dispose of it twice, so FO")]
        public void ReadImportedSolutionPackage(SolutionPackage package, string destinationPath)
        {
            using (FileStream fs = new FileStream(destinationPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (ZipArchive Archive = new ZipArchive(fs, ZipArchiveMode.Read))
                {
                    List<string> EntryList = new List<string>();
                    foreach (ZipArchiveEntry Entry in Archive.Entries)
                    {
                        string EntryName = Entry.FullName;
                        EntryList.Add(EntryName);
                    }

                    EntryList.Sort();

                    ReadNodeContent(package, EntryList, Archive, string.Empty, new EmptyPath());
                }
            }
        }

        private void ReadNodeContent(SolutionPackage package, List<string> entryList, ZipArchive archive, string folderPathIn, IFolderPath currentFolderPath)
        {
            while (entryList.Count > 0)
            {
                string NextEntry = entryList[0];
                if (!NextEntry.StartsWith(folderPathIn, StringComparison.Ordinal))
                    break;

                int NextFolderIndex = NextEntry.IndexOf('\\', folderPathIn.Length);
                if (NextFolderIndex >= 0)
                {
                    IFolderPath CurrentPath = currentFolderPath;
                    string NewFolderName = NextEntry.Substring(folderPathIn.Length, NextFolderIndex - folderPathIn.Length);
                    IFolderPath NewFolderPath = NotifyImported(package, RootPath, currentFolderPath, NewFolderName);

                    ReadNodeContent(package, entryList, archive, folderPathIn + NewFolderName + "\\", NewFolderPath);
                    currentFolderPath = CurrentPath;
                }
                else
                {
                    entryList.RemoveAt(0);

                    foreach (ZipArchiveEntry Entry in archive.Entries)
                        if (Entry.FullName == NextEntry)
                        {
                            ReadArchivedItem(package, Entry, currentFolderPath);
                            break;
                        }
                }
            }
        }

        private void ReadArchivedItem(SolutionPackage package, ZipArchiveEntry entry, IFolderPath currentFolderPath)
        {
            byte[] Content;

            using (Stream ArchiveStream = entry.Open())
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    ArchiveStream.CopyTo(ms);
                    Content = ms.ToArray();
                }
            }

            NotifyImported(package, RootPath, currentFolderPath, entry.Name, Content);
        }
        #endregion

        #region Menu
        /// <summary>
        /// Calleds when a context menu is loaded.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnContextMenuLoaded(object sender, RoutedEventArgs e)
        {
            NotifyContextMenuLoaded(e);
        }

        /// <summary>
        /// Calleds when a context submenu is opened.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnSubmenuOpened(object sender, RoutedEventArgs e)
        {
            PrettyItemsControl.MakeMenuPretty((ItemsControl)sender);
        }

        /// <summary>
        /// Calleds when a context menu is opened.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnContextMenuOpened(object sender, RoutedEventArgs e)
        {
            if (sender == null)
                throw new ArgumentNullException(nameof(sender));

            ContextMenu ExplorerContextMenu = (ContextMenu)sender;

            ValidEditOperations ValidOperations = ValidEditOperations;

            menuitemDeleteSolution.CanShow = ValidOperations.DeleteSolution;
            menuitemCut.CanShow = ValidOperations.Cut;
            menuitemCopy.CanShow = ValidOperations.Copy;
            menuitemPaste.CanShow = ValidOperations.Paste;
            menuitemDelete.CanShow = ValidOperations.Delete;
            menuitemAddExistingItem.CanShow = ValidOperations.Add;

            List<ExtendedToolBarMenuItem> AddDocumentMenuItemList = GetAddDocumentMenuItemList(ExplorerContextMenu);
            foreach (ExtendedToolBarMenuItem Item in AddDocumentMenuItemList)
                Item.CanShow = ValidOperations.Add;

            menuitemAddNewFolder.CanShow = ValidOperations.AddFolder;
            menuitemOpen.CanShow = ValidOperations.Open;
            menuitemRename.CanShow = ValidOperations.Rename;
            menuitemProperties.CanShow = ValidOperations.Properties;

            ShowCustomMenus(ExplorerContextMenu);

            List<ITreeNodePath> SelectedItems = new List<ITreeNodePath>();
            foreach (ISolutionTreeNode Item in treeviewSolutionExplorer.SelectedItems)
                SelectedItems.Add(Item.Path);

            IList<ExtendedRoutedCommand> CanShowCommandList = GetCanShowCommandList(ExplorerContextMenu);
            NotifyContextMenuOpened(SelectedItems, CanShowCommandList);
            HideMenuItems(ExplorerContextMenu, CanShowCommandList);

            PrettyItemsControl.MakeMenuPretty(ExplorerContextMenu);
        }

        /// <summary>
        /// Shows a custom menu in a context menu.
        /// </summary>
        /// <param name="explorerContextMenu">The context menu.</param>
        protected virtual void ShowCustomMenus(ContextMenu explorerContextMenu)
        {
            if (explorerContextMenu != null)
            {
                Separator InsertionSeparator = ContextMenuSeparator;
                ItemCollection ContextMenuItems = explorerContextMenu.Items;
                int LastIndex = ContextMenuItems.IndexOf(InsertionSeparator);
                int FirstIndex = LastIndex;

                while (FirstIndex > 0 && !(ContextMenuItems[FirstIndex - 1] is Separator))
                    FirstIndex--;

                for (int i = FirstIndex; i < LastIndex; i++)
                    if (ContextMenuItems[i] is ExtendedToolBarMenuItem AsExtendedToolBarMenuItem)
                        AsExtendedToolBarMenuItem.CanShow = true;
            }
        }

        private List<ExtendedToolBarMenuItem> GetAddDocumentMenuItemList(ItemsControl itemsCollection)
        {
            List<ExtendedToolBarMenuItem> Result = new List<ExtendedToolBarMenuItem>();
            foreach (object Item in itemsCollection.Items)
            {
                if (Item is ExtendedToolBarMenuItem AsExtendedToolBarMenuItem)
                    if (AsExtendedToolBarMenuItem.Command is DocumentRoutedCommand)
                        Result.Add(AsExtendedToolBarMenuItem);

                if (Item is ItemsControl AsItemsCollection)
                    Result.AddRange(GetAddDocumentMenuItemList(AsItemsCollection));
            }

            return Result;
        }

        private IList<ExtendedRoutedCommand> GetCanShowCommandList(ItemsControl itemsCollection)
        {
            List<ExtendedRoutedCommand> Result = new List<ExtendedRoutedCommand>();
            foreach (object Item in itemsCollection.Items)
            {
                if (Item is ExtendedToolBarMenuItem AsExtendedToolBarMenuItem)
                    if (AsExtendedToolBarMenuItem.CanShow)
                        if (AsExtendedToolBarMenuItem.Command is ExtendedRoutedCommand AsExtendedCommand)
                            Result.Add(AsExtendedCommand);

                if (Item is ItemsControl AsItemsCollection)
                    Result.AddRange(GetCanShowCommandList(AsItemsCollection));
            }

            return Result;
        }

        private void HideMenuItems(ItemsControl itemsCollection, IList<ExtendedRoutedCommand> canShowCommandList)
        {
            foreach (object Item in itemsCollection.Items)
            {
                if (Item is ExtendedToolBarMenuItem AsExtendedToolBarMenuItem)
                    if (AsExtendedToolBarMenuItem.CanShow)
                        if (AsExtendedToolBarMenuItem.Command is ExtendedRoutedCommand AsExtendedCommand)
                            if (!canShowCommandList.Contains(AsExtendedCommand))
                                AsExtendedToolBarMenuItem.CanShow = false;

                if (Item is ItemsControl AsItemsCollection)
                    HideMenuItems(AsItemsCollection, canShowCommandList);
            }
        }
        #endregion

        #region Rename
        private void OnEditEnter(object sender, RoutedEventArgs e)
        {
            EditableTextBlockEventArgs Args = (EditableTextBlockEventArgs)e;

            if (FindResource("RenameCommand") is RoutedCommand RenameCommand)
                if (!RenameCommand.CanExecute(null, this))
                    Args.Cancel();
        }

        private void OnEditLeave(object sender, RoutedEventArgs e)
        {
            EditableTextBlock Ctrl = (EditableTextBlock)sender;
            EditLeaveEventArgs Args = (EditLeaveEventArgs)e;
            ISolutionTreeNode Node = (ISolutionTreeNode)Ctrl.DataContext;

            if (!Args.IsEditCanceled)
            {
                bool IsCanceled = NotifyNameChanged(Node.Path, Ctrl.Text, Args.Text, false);

                if (IsCanceled)
                    Args.Cancel();
            }
        }
        #endregion

        #region Drag & Drop
        /// <summary>
        /// Initializes drag and drop.
        /// </summary>
        protected virtual void InitializeDragDrop()
        {
        }

        private void OnSolutionDragStarting(object sender, RoutedEventArgs e)
        {
            DragStartingEventArgs Args = (DragStartingEventArgs)e;

            ValidEditOperations ValidOperations = ValidEditOperations;
            if (!ValidOperations.Copy)
                Args.Cancel();
        }

        private void OnSolutionDropCheck(object sender, RoutedEventArgs e)
        {
            DropCheckEventArgs Args = (DropCheckEventArgs)e;

            if (!(Args.DropDestinationItem is ISolutionFolder))
                Args.Deny();
        }

        private void OnSolutionPreviewDropCompleted(object sender, RoutedEventArgs e)
        {
            DropCompletedEventArgs Args = (DropCompletedEventArgs)e;

            if (Args.DropDestinationItem is ISolutionFolder AsFolder)
                if (!Args.IsCopy && Args.ItemList != null)
                {
                    foreach (ISolutionTreeNode Item in Args.ItemList)
                    {
                        ITreeNodePath ItemPath = Item.Path;
                        ISolutionFolder? ItemParent = Item.Parent as ISolutionFolder;

                        if (ItemParent != null)
                        {
                            NotifyMoved(ItemPath, (IFolderPath)ItemParent.Path, (IFolderPath)AsFolder.Path, true);

                            MoveOperation Operation = new MoveOperation(Root, ItemPath, ItemParent, AsFolder);
                            UndoRedoManager.AddOperation(Operation);
                            Operation.Redone += OnMoveRedone;
                            Operation.Undone += OnMoveUndone;
                        }
                    }
                }
        }
        #endregion

        #region Mouse
        private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement Ctrl = (FrameworkElement)sender;
            bool IsHandled = false;

            switch (Ctrl.DataContext)
            {
                case ISolutionFolder AsSolutionFolder:
                    treeviewSolutionExplorer.ToggleIsExpanded(AsSolutionFolder);
                    IsHandled = true;
                    break;

                case ISolutionItem AsSolutionItem:
                    RoutedUICommand OpenCommand = ApplicationCommands.Open;
                    if (OpenCommand.CanExecute(this, null))
                        OpenCommand.Execute(this, null);

                    IsHandled = true;
                    break;
            }

            Debug.Assert(IsHandled);
        }
        #endregion

        #region Undo/Redo
        private void ExpandNewNodes(SolutionExplorerOperation operation)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));

            Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new FollowOperationWithExpandHandler(OnFollowOperationWithExpand), operation.ExpandedFolderList);
        }

        private delegate void FollowOperationWithExpandHandler(IReadOnlyCollection<ISolutionFolder> expandedFolderList);
        private void OnFollowOperationWithExpand(IReadOnlyCollection<ISolutionFolder> expandedFolderList)
        {
            foreach (ISolutionFolder ExpandedFolder in expandedFolderList)
                treeviewSolutionExplorer.Expand(ExpandedFolder);
        }
        #endregion

        #region Implementation of INotifyPropertyChanged
        /// <summary>
        /// Implements the PropertyChanged event.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Invoke handlers of the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">Name of the property that changed.</param>
        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Invoke handlers of the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">Name of the property that changed.</param>
        protected void NotifyThisPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
