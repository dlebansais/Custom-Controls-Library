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

namespace CustomControls
{
    public partial class SolutionExplorer : UserControl, INotifyPropertyChanged
    {
        #region Custom properties and events
        #region Solution Icon
        public static readonly DependencyProperty SolutionIconProperty = DependencyProperty.Register("SolutionIcon", typeof(ImageSource), typeof(SolutionExplorer), new PropertyMetadata(null));

        public ImageSource SolutionIcon
        {
            get { return (ImageSource)GetValue(SolutionIconProperty); }
            set { SetValue(SolutionIconProperty, value); }
        }
        #endregion
        #region Root Path
        private static readonly DependencyPropertyKey RootPathPropertyKey = DependencyProperty.RegisterReadOnly("RootPath", typeof(IRootPath), typeof(SolutionExplorer), new PropertyMetadata(null));
        public static readonly DependencyProperty RootPathProperty = RootPathPropertyKey.DependencyProperty;

        public IRootPath RootPath
        {
            get { return (IRootPath)GetValue(RootPathProperty); }
        }
        #endregion
        #region Root Properties
        private static readonly DependencyPropertyKey RootPropertiesPropertyKey = DependencyProperty.RegisterReadOnly("RootProperties", typeof(IRootProperties), typeof(SolutionExplorer), new PropertyMetadata(null));
        public static readonly DependencyProperty RootPropertiesProperty = RootPropertiesPropertyKey.DependencyProperty;

        public IRootProperties RootProperties
        {
            get { return (IRootProperties)GetValue(RootPropertiesProperty); }
        }
        #endregion
        #region Tree Node Comparer
        private static readonly DependencyPropertyKey TreeNodeComparerPropertyKey = DependencyProperty.RegisterReadOnly("TreeNodeComparer", typeof(IComparer<ITreeNodePath>), typeof(SolutionExplorer), new PropertyMetadata(null));
        public static readonly DependencyProperty TreeNodeComparerProperty = TreeNodeComparerPropertyKey.DependencyProperty;

        public IComparer<ITreeNodePath> TreeNodeComparer
        {
            get { return (IComparer<ITreeNodePath>)GetValue(TreeNodeComparerProperty); }
        }
        #endregion
        #region Undo Redo Manager
        public static readonly DependencyProperty UndoRedoManagerProperty = DependencyProperty.Register("UndoRedoManager", typeof(UndoRedoManager), typeof(SolutionExplorer), new PropertyMetadata(null));

        public UndoRedoManager UndoRedoManager
        {
            get { return (UndoRedoManager)GetValue(UndoRedoManagerProperty); }
            set { SetValue(UndoRedoManagerProperty, value); }
        }
        #endregion
        #region Is Loading Tree
        public static readonly DependencyProperty IsLoadingTreeProperty = DependencyProperty.Register("IsLoadingTree", typeof(bool), typeof(SolutionExplorer), new PropertyMetadata(null));

        public bool IsLoadingTree
        {
            get { return (bool)GetValue(IsLoadingTreeProperty); }
            set { SetValue(IsLoadingTreeProperty, value); }
        }
        #endregion
        #region Context Menu Loaded
        public static readonly RoutedEvent ContextMenuLoadedEvent = EventManager.RegisterRoutedEvent("ContextMenuLoaded", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionExplorer));

        public event RoutedEventHandler ContextMenuLoaded
        {
            add { AddHandler(ContextMenuLoadedEvent, value); }
            remove { RemoveHandler(ContextMenuLoadedEvent, value); }
        }

        protected virtual void NotifyContextMenuLoaded(RoutedEventArgs e)
        {
            if (e != null)
                RaiseEvent(new RoutedEventArgs(ContextMenuLoadedEvent, e.OriginalSource));
        }
        #endregion
        #region Context Menu Opened
        public static readonly RoutedEvent ContextMenuOpenedEvent = EventManager.RegisterRoutedEvent("ContextMenuOpened", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionExplorer));

        public event RoutedEventHandler ContextMenuOpened
        {
            add { AddHandler(ContextMenuOpenedEvent, value); }
            remove { RemoveHandler(ContextMenuOpenedEvent, value); }
        }

        protected virtual void NotifyContextMenuOpened(IReadOnlyCollection<ITreeNodePath> selectedItems, ICollection<ExtendedRoutedCommand> canShowCommandList)
        {
            ContextMenuOpenedEventArgs Args = new ContextMenuOpenedEventArgs(ContextMenuOpenedEvent, selectedItems, canShowCommandList);
            RaiseEvent(Args);
        }
        #endregion
        #region Preview Name Changed
        public static readonly RoutedEvent PreviewNameChangedEvent = EventManager.RegisterRoutedEvent("PreviewNameChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionExplorer));

        public event RoutedEventHandler PreviewNameChanged
        {
            add { AddHandler(PreviewNameChangedEvent, value); }
            remove { RemoveHandler(PreviewNameChangedEvent, value); }
        }

        protected virtual void NotifyPreviewNameChanged(ITreeNodePath path, string oldName, string newName)
        {
            NameChangedEventArgs Args = new NameChangedEventArgs(PreviewNameChangedEvent, path, oldName, newName, false);
            RaiseEvent(Args);
        }
        #endregion
        #region Name Changed
        public static readonly RoutedEvent NameChangedEvent = EventManager.RegisterRoutedEvent("NameChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionExplorer));

        public event RoutedEventHandler NameChanged
        {
            add { AddHandler(NameChangedEvent, value); }
            remove { RemoveHandler(NameChangedEvent, value); }
        }

        protected virtual bool NotifyNameChanged(ITreeNodePath path, string oldName, string newName, bool isUndoRedo)
        {
            NameChangedEventArgs Args = new NameChangedEventArgs(NameChangedEvent, path, oldName, newName, isUndoRedo);
            RaiseEvent(Args);

            return Args.IsCanceled;
        }
        #endregion
        #region Moved
        public static readonly RoutedEvent MovedEvent = EventManager.RegisterRoutedEvent("Moved", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionExplorer));

        public event RoutedEventHandler Moved
        {
            add { AddHandler(MovedEvent, value); }
            remove { RemoveHandler(MovedEvent, value); }
        }

        protected virtual bool NotifyMoved(ITreeNodePath path, IFolderPath oldParentPath, IFolderPath newParentPath, bool isUndoRedo)
        {
            MovedEventArgs Args = new MovedEventArgs(MovedEvent, path, oldParentPath, newParentPath, isUndoRedo);
            RaiseEvent(Args);

            return Args.IsCanceled;
        }
        #endregion
        #region Tree Changed
        public static readonly RoutedEvent TreeChangedEvent = EventManager.RegisterRoutedEvent("TreeChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionExplorer));

        public event RoutedEventHandler TreeChanged
        {
            add { AddHandler(TreeChangedEvent, value); }
            remove { RemoveHandler(TreeChangedEvent, value); }
        }

        protected virtual bool NotifyTreeChanged(IReadOnlyDictionary<ITreeNodePath, IPathConnection> pathTable, bool isAdd, bool isUndoRedo)
        {
            TreeChangedEventArgs Args = new TreeChangedEventArgs(TreeChangedEvent, pathTable, isAdd, isUndoRedo);
            RaiseEvent(Args);

            return Args.IsCanceled;
        }
        #endregion
        #region Selection Changed
        public static readonly RoutedEvent SelectionChangedEvent = ExtendedTreeViewBase.SelectionChangedEvent;

        public event RoutedEventHandler SelectionChanged
        {
            add { AddHandler(SelectionChangedEvent, value); }
            remove { RemoveHandler(SelectionChangedEvent, value); }
        }
        #endregion
        #region Imported
        public static readonly RoutedEvent ImportedEvent = EventManager.RegisterRoutedEvent("Imported", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionExplorer));

        public event RoutedEventHandler Imported
        {
            add { AddHandler(ImportedEvent, value); }
            remove { RemoveHandler(ImportedEvent, value); }
        }

        protected virtual IRootPath NotifyImported(SolutionPackage package, string folderName)
        {
            ImportedEventArgs Args = new ImportedEventArgs(ImportedEvent, package, folderName);
            RaiseEvent(Args);

            return Args.RootPath;
        }

        protected virtual IFolderPath NotifyImported(SolutionPackage package, IRootPath rootPath, IFolderPath currentFolderPath, string folderName)
        {
            ImportedEventArgs Args = new ImportedEventArgs(ImportedEvent, package, rootPath, currentFolderPath, folderName);
            RaiseEvent(Args);

            return Args.CurrentFolderPath;
        }

        protected virtual void NotifyImported(SolutionPackage package, IRootPath rootPath, IFolderPath currentFolderPath, string itemName, byte[] content)
        {
            ImportedEventArgs Args = new ImportedEventArgs(ImportedEvent, package, rootPath, currentFolderPath, itemName, content);
            RaiseEvent(Args);
        }
        #endregion
        #endregion

        #region Init
        public SolutionExplorer()
        {
            _Root = null;
            Initialized += OnInitialized; // Dirty trick to avoid warning CA2214.
            InitializeComponent();
        }

        /// <summary>
        ///     Called when the control has been initialized and before properties are set.
        /// </summary>
        /// <parameters>
        /// <param name="sender">This parameter is not used.</param>
        /// <param name="e">This parameter is not used.</param>
        /// </parameters>
        protected virtual void OnInitialized(object sender, EventArgs e)
        {
            InitializeDragDrop();
        }
        #endregion

        #region Properties
        public ISolutionRoot Root
        {
            get { return _Root; }
            set 
            {
                if (_Root != value)
                {
                    _Root = value;
                    NotifyThisPropertyChanged();
                }
            }
        }
        private ISolutionRoot _Root;

        public IReadOnlyDictionary<ITreeNodePath, IPathConnection> SelectedNodes
        {
            get
            {
                Dictionary<ITreeNodePath, IPathConnection> PathTable = new Dictionary<ITreeNodePath, IPathConnection>();
                foreach (ISolutionTreeNode Child in treeviewSolutionExplorer.SelectedItems)
                {
                    if (PathTable.ContainsKey(Child.Path))
                        continue;

                    ISolutionFolder ParentFolder = Child.Parent as ISolutionFolder;
                    IFolderPath ParentPath = ParentFolder != null ? (IFolderPath)ParentFolder.Path : null;
                    PathTable.Add(Child.Path, new PathConnection(ParentPath, Child.Properties, treeviewSolutionExplorer.IsExpanded(Child)));
                }

                return PathTable;
            }
        }

        public IReadOnlyDictionary<ITreeNodePath, IPathConnection> SelectedTree
        {
            get 
            {
                Dictionary<ITreeNodePath, IPathConnection> PathConnectionTable = new Dictionary<ITreeNodePath, IPathConnection>();
                RecursiveGetTree(treeviewSolutionExplorer.SelectedItems, PathConnectionTable);

                return PathConnectionTable;
            }
        }

        private void RecursiveGetTree(IList Items, Dictionary<ITreeNodePath, IPathConnection> PathConnectionTable)
        {
            foreach (ISolutionTreeNode Child in Items)
            {
                if (PathConnectionTable.ContainsKey(Child.Path))
                    continue;

                ISolutionFolder ParentFolder = Child.Parent as ISolutionFolder;
                IFolderPath ParentPath = ParentFolder != null ? (IFolderPath)ParentFolder.Path : null;

                PathConnectionTable.Add(Child.Path, new PathConnection(ParentPath, Child.Properties, treeviewSolutionExplorer.IsExpanded(Child)));

                ISolutionFolder AsFolder;
                if ((AsFolder = Child as ISolutionFolder) != null)
                    RecursiveGetTree(AsFolder.Children, PathConnectionTable);
            }
        }

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

        private ICollection<ITreeNodePath> GetDirtyItems(ISolutionTreeNode Base)
        {
            List<ITreeNodePath> DirtyItemList = new List<ITreeNodePath>();

            if (Base.IsDirty)
                DirtyItemList.Add(Base.Path);

            foreach (ISolutionTreeNode Child in Base.Children)
                DirtyItemList.AddRange(GetDirtyItems(Child));

            return DirtyItemList;
        }

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

        private ICollection<ITreeNodePath> GetDirtyProperties(ISolutionTreeNode Base)
        {
            List<ITreeNodePath> DirtyPropertiesList = new List<ITreeNodePath>();

            if (Base.Properties != null && Base.Properties.IsDirty)
                DirtyPropertiesList.Add(Base.Path);

            foreach (ISolutionTreeNode Child in Base.Children)
                DirtyPropertiesList.AddRange(GetDirtyProperties(Child));

            return DirtyPropertiesList;
        }

        public ValidEditOperations ValidEditOperations
        {
            get
            {
                bool IsSolutionSelected = false;
                bool IsFolderSelected = false;
                bool IsDocumentSelected = false;
                bool IsSingleTarget = (treeviewSolutionExplorer.SelectedItems.Count == 1);

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
                IDataObject DataObject = Clipboard.GetDataObject();
                if (DataObject != null)
                {
                    ClipboardPathData Data = DataObject.GetData(ClipboardPathData.SolutionExplorerClipboardPathFormat) as ClipboardPathData;
                    CanPaste = (Data != null);
                }
                else
                    CanPaste = false;

                return new ValidEditOperations(IsSolutionSelected, IsFolderSelected, IsDocumentSelected, IsSingleTarget, CanCutOrCopy, CanPaste);
            }
        }

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

                    IFolderPath AsFolderPath;
                    if ((AsFolderPath = Item.Path as IFolderPath) != null)
                        if (treeviewSolutionExplorer.IsExpanded(Item))
                            Result.Add(AsFolderPath);
                }

                return Result;
            }
        }

        public ITreeNodePath ItemAfterLastSelected
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

        public IFolderPath SelectedFolder
        {
            get
            {
                if (treeviewSolutionExplorer.SelectedItems.Count == 1)
                {
                    ISolutionFolder AsFolder;
                    if ((AsFolder = treeviewSolutionExplorer.SelectedItems[0] as ISolutionFolder) != null)
                    {
                        return (IFolderPath)AsFolder.Path;
                    }
                }

                return null;
            }
        }

        public ICollection<IItemPath> SolutionItems
        {
            get { return GetFlatChildrenItems(Root); }
        }

        private List<IItemPath> GetFlatChildrenItems(ISolutionFolder Folder)
        {
            List<IItemPath> Result = new List<IItemPath>();

            foreach (ISolutionTreeNode Child in Folder.Children)
            {
                ISolutionFolder AsFolder;
                ISolutionItem AsItem;

                if ((AsFolder = Child as ISolutionFolder) != null)
                    Result.AddRange(GetFlatChildrenItems(AsFolder));

                else if ((AsItem = Child as ISolutionItem) != null)
                    Result.Add(AsItem.Path as IItemPath);
            }

            return Result;
        }

        public Separator DocumentMenuSeparator
        {
            get { return separatorAddDocumentStart; }
        }

        public Separator ContextMenuSeparator
        {
            get { return separatorContextMenuItem; }
        }

        public bool CanUndo
        {
            get { return UndoRedoManager != null && UndoRedoManager.CanUndo; }
        }

        public bool CanRedo
        {
            get { return UndoRedoManager != null && UndoRedoManager.CanRedo; }
        }
        #endregion

        #region Client Interface
        public void ResetRoot()
        {
            SetValue(RootPathPropertyKey, null);
            SetValue(RootPropertiesPropertyKey, null);
            SetValue(TreeNodeComparerPropertyKey, null);

            Root = null;
            if (UndoRedoManager != null)
                UndoRedoManager.Reset();
        }

        public void SetRoot(IRootPath newRootPath, IRootProperties newRootProperties, IComparer<ITreeNodePath> newComparer)
        {
            SetValue(RootPathPropertyKey, newRootPath);
            SetValue(RootPropertiesPropertyKey, newRootProperties);
            SetValue(TreeNodeComparerPropertyKey, newComparer);

            Root = CreateSolutionRoot(newRootPath, newRootProperties, newComparer);
            if (UndoRedoManager != null)
                UndoRedoManager.Reset();
        }

        protected virtual ISolutionRoot CreateSolutionRoot(IRootPath path, IRootProperties properties, IComparer<ITreeNodePath> comparer)
        {
            return new SolutionRoot(path, properties, comparer);
        }

        public new virtual void Focus()
        {
            treeviewSolutionExplorer.Focus();
        }

        public void ChangeName(ITreeNodePath path, string newName)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            RenameOperation Operation = new RenameOperation(Root, path, newName);
            if (UndoRedoManager != null)
            {
                UndoRedoManager.AddAndExecuteOperation(Operation);
                Operation.Redone += OnRenameRedone;
                Operation.Undone += OnRenameUndone;
            }
            else
                Operation.Redo();
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

        public void Move(ITreeNodePath path, IFolderPath destinationPath)
        {
            ISolutionTreeNode Node = Root.FindTreeNode(path);
            ISolutionFolder OldParent = Node.Parent as ISolutionFolder;

            IFolderPath newParentPath = destinationPath;
            ISolutionFolder NewParent = Root.FindTreeNode(newParentPath) as ISolutionFolder;
            if (Node != null && OldParent != null && NewParent != null)
            {
                MoveOperation Operation = new MoveOperation(Root, path, OldParent, NewParent);
                if (UndoRedoManager != null)
                {
                    UndoRedoManager.AddAndExecuteOperation(Operation);
                    Operation.Redone += OnMoveRedone;
                    Operation.Undone += OnMoveUndone;
                }
                else
                    UndoRedoManager.Redo();
            }
        }

        private void OnMoveUndone(object sender, RoutedEventArgs e)
        {
            MoveOperation Operation = (MoveOperation)sender;
            NotifyMoved(Operation.Path, Operation.NewParent.Path as IFolderPath, Operation.OldParent.Path as IFolderPath, true);
        }

        private void OnMoveRedone(object sender, RoutedEventArgs e)
        {
            MoveOperation Operation = (MoveOperation)sender;
            NotifyMoved(Operation.Path, Operation.OldParent.Path as IFolderPath, Operation.NewParent.Path as IFolderPath, true);
        }

        public void ExpandFolder(IFolderPath folder)
        {
            treeviewSolutionExplorer.Expand(folder);
        }

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

        public static ClipboardPathData ReadClipboard()
        {
            IDataObject DataObject = Clipboard.GetDataObject();
            if (DataObject != null)
                return DataObject.GetData(ClipboardPathData.SolutionExplorerClipboardPathFormat) as ClipboardPathData;
            else
                return null;
        }

        public void Undo()
        {
            if (UndoRedoManager != null)
            {
                UndoRedoManager.Undo();
                ExpandNewNodes(UndoRedoManager.LastOperation as SolutionExplorerOperation);
            }
        }

        public void Redo()
        {
            if (UndoRedoManager != null)
            {
                UndoRedoManager.Redo();
                ExpandNewNodes(UndoRedoManager.LastOperation as SolutionExplorerOperation);
            }
        }

        public void SelectAll()
        {
            treeviewSolutionExplorer.SelectAll();
        }

        public void AddFolder(IFolderPath destinationFolderPath, IFolderPath newFolderPath, IFolderProperties newFolderProperties)
        {
            AddFolderOperation Operation = new AddFolderOperation(Root, destinationFolderPath, newFolderPath, newFolderProperties);
            if (UndoRedoManager != null)
            {
                UndoRedoManager.AddAndExecuteOperation(Operation);
                Operation.Redone += OnAddRemoveRedone;
                Operation.Undone += OnAddRemoveUndone;
            }
            else
                Operation.Redo();

            ExpandNewNodes(Operation);
        }

        public void AddItem(IFolderPath destinationFolderPath, IItemPath newItemPath, IItemProperties newItemProperties)
        {
            AddItemOperation Operation = new AddItemOperation(Root, destinationFolderPath, newItemPath, newItemProperties);
            if (UndoRedoManager != null)
            {
                UndoRedoManager.AddAndExecuteOperation(Operation);
                Operation.Redone += OnAddRemoveRedone;
                Operation.Undone += OnAddRemoveUndone;
            }
            else
                Operation.Redo();

            ExpandNewNodes(Operation);
        }

        public void AddTree(IReadOnlyDictionary<ITreeNodePath, IPathConnection> pathTable)
        {
            AddTreeOperation Operation = new AddTreeOperation(Root, pathTable);
            if (UndoRedoManager != null)
            {
                UndoRedoManager.AddAndExecuteOperation(Operation);
                Operation.Redone += OnAddRemoveRedone;
                Operation.Undone += OnAddRemoveUndone;
            }
            else
                Operation.Redo();

            ExpandNewNodes(Operation);
        }

        public void DeleteTree(IReadOnlyDictionary<ITreeNodePath, IPathConnection> pathTable)
        {
            RemoveTreeOperation Operation = new RemoveTreeOperation(Root, pathTable);
            if (UndoRedoManager != null)
            {
                UndoRedoManager.AddAndExecuteOperation(Operation);
                Operation.Redone += OnAddRemoveRedone;
                Operation.Undone += OnAddRemoveUndone;
            }
            else
                Operation.Redo();
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

        public void SetSelected(ITreeNodePath path)
        {
            treeviewSolutionExplorer.UnselectAll();

            ISolutionTreeNode TreeNode = Root.FindTreeNode(path);
            if (TreeNode != null)
            {
                treeviewSolutionExplorer.SetSelected(TreeNode);
                treeviewSolutionExplorer.Focus();
            }
        }

        public void ClearDirtyItemsAndProperties()
        {
            if (Root != null)
                ClearDirtyItemsAndProperties(Root);
        }

        protected virtual void ClearDirtyItemsAndProperties(ISolutionFolder folder)
        {
            ClearNodeDirtyItemsAndProperties(folder);

            if (folder != null)
                foreach (ISolutionTreeNode Child in folder.Children)
                {
                    ISolutionFolder AsFolder;
                    if ((AsFolder = Child as ISolutionFolder) != null)
                        ClearDirtyItemsAndProperties(AsFolder);
                    else
                        ClearNodeDirtyItemsAndProperties(Child);
                }
        }

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

        public IReadOnlyDictionary<ITreeNodePath, IPathConnection> FindItemsByDocumentPath(IReadOnlyCollection<IDocumentPath> documentPathList)
        {
            if (documentPathList == null)
                throw new ArgumentNullException(nameof(documentPathList));

            return FindItemsByDocumentPath(Root, documentPathList);
        }

        private IReadOnlyDictionary<ITreeNodePath, IPathConnection> FindItemsByDocumentPath(ISolutionFolder Folder, IReadOnlyCollection<IDocumentPath> DocumentPathList)
        {
            Dictionary<ITreeNodePath, IPathConnection> Result = new Dictionary<ITreeNodePath, IPathConnection>();

            foreach (ISolutionTreeNode Child in Folder.Children)
            {
                ISolutionFolder AsFolder;
                ISolutionItem AsItem;

                if ((AsFolder = Child as ISolutionFolder) != null)
                {
                    IReadOnlyDictionary<ITreeNodePath, IPathConnection> InnerTree = FindItemsByDocumentPath(AsFolder, DocumentPathList);
                    foreach (KeyValuePair<ITreeNodePath, IPathConnection> Entry in InnerTree)
                        Result.Add(Entry.Key, Entry.Value);
                }

                else if ((AsItem = Child as ISolutionItem) != null)
                {
                    IItemPath ItemPath = (IItemPath)AsItem.Path;
                    foreach (IDocumentPath DocumentPath in DocumentPathList)
                        if (ItemPath.DocumentPath.IsEqual(DocumentPath))
                        {
                            ISolutionFolder ParentFolder = Child.Parent as ISolutionFolder;
                            Result.Add(Child.Path, new PathConnection((IFolderPath)ParentFolder.Path, Child.Properties, treeviewSolutionExplorer.IsExpanded(Child)));
                            break;
                        }
                }
            }

            return Result;
        }

        public IItemProperties GetItemProperties(IItemPath path)
        {
            IItemPath ItemPath = path;
            ISolutionItem Item = Root.FindTreeNode(ItemPath) as ISolutionItem;
            if (Item != null)
                return Item.Properties as IItemProperties;
            else
                return null;
        }

        public IReadOnlyCollection<ITreeNodePath> GetChildren(IFolderPath folderPath)
        {
            IFolderPath ParentFolderPath = folderPath;
            ISolutionFolder ParentFolder = Root.FindTreeNode(ParentFolderPath) as ISolutionFolder;
            if (ParentFolder != null)
            {
                Collection<ITreeNodePath> Result = new Collection<ITreeNodePath>();

                foreach (SolutionTreeNode Child in ParentFolder.Children)
                    Result.Add(Child.Path);

                return Result;
            }
            else
                return null;
        }

        public IReadOnlyCollection<ITreeNodePath> GetTree(IFolderPath folderPath)
        {
            IFolderPath ParentFolderPath = folderPath;
            ISolutionFolder ParentFolder = Root.FindTreeNode(ParentFolderPath) as ISolutionFolder;
            if (ParentFolder != null)
                return GetTree(ParentFolder);
            else
                return null;
        }

        private IReadOnlyCollection<ITreeNodePath> GetTree(ISolutionFolder ParentFolder)
        {
            Collection<ITreeNodePath> Result = new Collection<ITreeNodePath>();
            Result.Add(ParentFolder.Path);

            foreach (SolutionTreeNode Child in ParentFolder.Children)
            {
                ISolutionFolder AsFolder;
                if ((AsFolder = Child as ISolutionFolder) != null)
                {
                    IReadOnlyCollection<ITreeNodePath> InnerTree = GetTree(AsFolder);
                    foreach (ITreeNodePath Path in InnerTree)
                        Result.Add(Path);
                }
                else
                    Result.Add(Child.Path);
            }

            return Result;
        }

        public ITreeNodePath GetEventSource(object sender, RoutedEventArgs e)
        {
            if (e != null)
            {
                FrameworkElement SourceElement;
                if ((SourceElement = e.OriginalSource as FrameworkElement) != null)
                {
                    ISolutionTreeNode AsTreeNode;
                    if ((AsTreeNode = SourceElement.DataContext as ISolutionTreeNode) != null)
                        return AsTreeNode.Path;
                }
            }

            return null;
        }

        public void TriggerRename()
        {
            ExtendedTreeViewItemBase AsItemBase;
            if ((AsItemBase = Keyboard.FocusedElement as ExtendedTreeViewItemBase) != null)
            {
                DependencyObject RootObject = VisualTreeHelper.GetChild(AsItemBase, 0);
                EditableTextBlock Ctrl = FindEditableTextBlock(RootObject);
                if (Ctrl != null)
                    Ctrl.IsEditing = true;
            }
        }

        private EditableTextBlock FindEditableTextBlock(DependencyObject RootObject)
        {
            EditableTextBlock AsEditableTextBlock;
            if ((AsEditableTextBlock = RootObject as EditableTextBlock) != null)
                return AsEditableTextBlock;

            int Count = VisualTreeHelper.GetChildrenCount(RootObject);
            for (int i = 0; i < Count; i++)
            {
                EditableTextBlock ChildEdit = FindEditableTextBlock(VisualTreeHelper.GetChild(RootObject, i));
                if (ChildEdit != null)
                    return ChildEdit;
            }

            return null;
        }

        public void ResetUndoRedo()
        {
            if (UndoRedoManager != null)
                UndoRedoManager.Reset();
        }

        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "If you think it's ok to dispose of my object, then I think it's ok to dispose of it twice, so FO")]
        public void CreateExportedSolutionPackage(string destinationPath, Dictionary<IDocumentPath, byte[]> contentTable)
        {
            using (FileStream fs = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (ZipArchive Archive = new ZipArchive(fs, ZipArchiveMode.Create))
                {
                    InsertNodeContent(Archive, "", Root, contentTable);
                }
            }
        }

        private static void InsertNodeContent(ZipArchive Archive, string FolderPathIn, ISolutionFolder ParentFolder, Dictionary<IDocumentPath, byte[]> ContentTable)
        {
            foreach (SolutionTreeNode Child in ParentFolder.Children)
            {
                ISolutionFolder AsFolder;
                ISolutionItem AsItem;

                if ((AsFolder = Child as ISolutionFolder) != null)
                {
                    string InnerFolderPath = FolderPathIn + AsFolder.Name;
                    //Archive.CreateEntry(InnerFolderPath);

                    InsertNodeContent(Archive, InnerFolderPath + @"\", AsFolder, ContentTable);
                }

                else if ((AsItem = Child as ISolutionItem) != null)
                    InsertItemContent(Archive, FolderPathIn, AsItem, ContentTable);
            }
        }

        private static void InsertItemContent(ZipArchive Archive, string FolderPathIn, ISolutionItem AsItem, Dictionary<IDocumentPath, byte[]> ContentTable)
        {
            IItemPath Path = (IItemPath)AsItem.Path;
            IDocumentPath ArchivedDocumentPath = Path.DocumentPath;

            foreach (KeyValuePair<IDocumentPath, byte[]> Entry in ContentTable)
                if (ArchivedDocumentPath.IsEqual(Entry.Key))
                {
                    byte[] Content = Entry.Value;

                    using (MemoryStream ms = new MemoryStream(Content))
                    {
                        string ExportId = ArchivedDocumentPath.ExportId;

                        ZipArchiveEntry ArchiveEntry = Archive.CreateEntry(FolderPathIn + ExportId);
                        using (Stream ArchiveStream = ArchiveEntry.Open())
                        {
                            ms.CopyTo(ArchiveStream);
                        }
                    }
                }
        }

        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "If you think it's ok to dispose of my object, then I think it's ok to dispose of it twice, so FO")]
        public void ReadImportedSolutionPackage(SolutionPackage package, string destinationPath)
        {
            using (FileStream fs = new FileStream(destinationPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (ZipArchive Archive = new ZipArchive(fs, ZipArchiveMode.Read))
                {
                    IRootPath RootPath = NotifyImported(package, Path.GetFileNameWithoutExtension(destinationPath));

                    List<string> EntryList = new List<string>();
                    foreach (ZipArchiveEntry Entry in Archive.Entries)
                    {
                        string EntryName = Entry.FullName;
                        EntryList.Add(EntryName);
                    }

                    EntryList.Sort();

                    ReadNodeContent(package, EntryList, Archive, "", RootPath, null);
                }
            }
        }

        private void ReadNodeContent(SolutionPackage Package, List<string> EntryList, ZipArchive Archive, string FolderPathIn, IRootPath RootPath, IFolderPath CurrentFolderPath)
        {
            while (EntryList.Count > 0)
            {
                string NextEntry = EntryList[0];
                if (!NextEntry.StartsWith(FolderPathIn, StringComparison.Ordinal))
                    break;

                int NextFolderIndex = NextEntry.IndexOf('\\', FolderPathIn.Length);
                if (NextFolderIndex >= 0)
                {
                    IFolderPath CurrentPath = CurrentFolderPath;
                    string NewFolderName = NextEntry.Substring(FolderPathIn.Length, NextFolderIndex - FolderPathIn.Length);
                    IFolderPath NewFolderPath = NotifyImported(Package, RootPath, CurrentFolderPath, NewFolderName);

                    ReadNodeContent(Package, EntryList, Archive, FolderPathIn + NewFolderName + "\\", RootPath, NewFolderPath);
                    CurrentFolderPath = CurrentPath;
                }
                else
                {
                    EntryList.RemoveAt(0);

                    foreach (ZipArchiveEntry Entry in Archive.Entries)
                        if (Entry.FullName == NextEntry)
                        {
                            ReadArchivedItem(Package, Entry, CurrentFolderPath);
                            break;
                        }
                }
            }
        }

        private void ReadArchivedItem(SolutionPackage Package, ZipArchiveEntry Entry, IFolderPath CurrentFolderPath)
        {
            byte[] Content;

            using (Stream ArchiveStream = Entry.Open())
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    ArchiveStream.CopyTo(ms);
                    Content = ms.ToArray();
                }
            }

            NotifyImported(Package, RootPath, CurrentFolderPath, Entry.Name, Content);
        }
        #endregion

        #region Menu
        protected virtual void OnContextMenuLoaded(object sender, RoutedEventArgs e)
        {
            NotifyContextMenuLoaded(e);
        }

        protected virtual void OnSubmenuOpened(object sender, RoutedEventArgs e)
        {
            PrettyItemsControl.MakeMenuPretty((ItemsControl)sender);
        }

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
                {
                    ExtendedToolBarMenuItem AsExtendedToolBarMenuItem;
                    if ((AsExtendedToolBarMenuItem = ContextMenuItems[i] as ExtendedToolBarMenuItem) != null)
                        AsExtendedToolBarMenuItem.CanShow = true;
                }
            }
        }

        private List<ExtendedToolBarMenuItem> GetAddDocumentMenuItemList(ItemsControl ItemsCollection)
        {
            List<ExtendedToolBarMenuItem> Result = new List<ExtendedToolBarMenuItem>();
            foreach (object Item in ItemsCollection.Items)
            {
                ExtendedToolBarMenuItem AsExtendedToolBarMenuItem;
                if ((AsExtendedToolBarMenuItem = Item as ExtendedToolBarMenuItem) != null)
                {
                    if (AsExtendedToolBarMenuItem.Command is DocumentRoutedCommand)
                        Result.Add(AsExtendedToolBarMenuItem);
                }

                ItemsControl AsItemsCollection;
                if ((AsItemsCollection = Item as ItemsControl) != null)
                    Result.AddRange(GetAddDocumentMenuItemList(AsItemsCollection));
            }

            return Result;
        }

        private IList<ExtendedRoutedCommand> GetCanShowCommandList(ItemsControl ItemsCollection)
        {
            List<ExtendedRoutedCommand> Result = new List<ExtendedRoutedCommand>();
            foreach (object Item in ItemsCollection.Items)
            {
                ExtendedToolBarMenuItem AsExtendedToolBarMenuItem;
                if ((AsExtendedToolBarMenuItem = Item as ExtendedToolBarMenuItem) != null)
                    if (AsExtendedToolBarMenuItem.CanShow)
                    {
                        ExtendedRoutedCommand AsExtendedCommand;
                        if ((AsExtendedCommand = AsExtendedToolBarMenuItem.Command as ExtendedRoutedCommand) != null)
                            Result.Add(AsExtendedCommand);
                    }

                ItemsControl AsItemsCollection;
                if ((AsItemsCollection = Item as ItemsControl) != null)
                    Result.AddRange(GetCanShowCommandList(AsItemsCollection));
            }

            return Result;
        }

        private void HideMenuItems(ItemsControl ItemsCollection, IList<ExtendedRoutedCommand> CanShowCommandList)
        {
            foreach (object Item in ItemsCollection.Items)
            {
                ExtendedToolBarMenuItem AsExtendedToolBarMenuItem;
                if ((AsExtendedToolBarMenuItem = Item as ExtendedToolBarMenuItem) != null)
                    if (AsExtendedToolBarMenuItem.CanShow)
                    {
                        ExtendedRoutedCommand AsExtendedCommand;
                        if ((AsExtendedCommand = AsExtendedToolBarMenuItem.Command as ExtendedRoutedCommand) != null)
                            if (!CanShowCommandList.Contains(AsExtendedCommand))
                                AsExtendedToolBarMenuItem.CanShow = false;
                    }

                ItemsControl AsItemsCollection;
                if ((AsItemsCollection = Item as ItemsControl) != null)
                    HideMenuItems(AsItemsCollection, CanShowCommandList);
            }
        }
        #endregion

        #region Rename
        private void OnEditEnter(object sender, RoutedEventArgs e)
        {
            EditableTextBlockEventArgs Args = e as EditableTextBlockEventArgs;

            RoutedCommand RenameCommand = FindResource("RenameCommand") as RoutedCommand;
            if (RenameCommand != null)
            {
                if (!RenameCommand.CanExecute(null, this))
                    Args.Cancel();
            }
        }

        private void OnEditLeave(object sender, RoutedEventArgs e)
        {
            EditableTextBlock Ctrl = sender as EditableTextBlock;
            EditLeaveEventArgs Args = e as EditLeaveEventArgs;
            ISolutionTreeNode Node = Ctrl.DataContext as ISolutionTreeNode;

            if (!Args.IsEditCanceled)
            {
                bool IsCanceled = NotifyNameChanged(Node.Path, Ctrl.Text, Args.Text, false);

                if (IsCanceled)
                    Args.Cancel();
            }
        }
        #endregion

        #region Drag & Drop
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

            ISolutionFolder AsFolder;
            if ((AsFolder = Args.DropDestinationItem as ISolutionFolder) != null)
            {
                if (!Args.IsCopy)
                {
                    foreach (ISolutionTreeNode Item in Args.ItemList)
                    {
                        ITreeNodePath ItemPath = Item.Path;
                        ISolutionFolder ItemParent = Item.Parent as ISolutionFolder;

                        NotifyMoved(ItemPath, ItemParent.Path as IFolderPath, AsFolder.Path as IFolderPath, true);

                        MoveOperation Operation = new MoveOperation(Root, ItemPath, ItemParent, AsFolder);
                        if (UndoRedoManager != null)
                        {
                            UndoRedoManager.AddOperation(Operation);
                            Operation.Redone += OnMoveRedone;
                            Operation.Undone += OnMoveUndone;
                        }
                    }
                }
            }
        }
        #endregion

        #region Mouse
        private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement Ctrl = (FrameworkElement)sender;

            ISolutionFolder AsFolder;
            bool IsHandled = false;

            if ((AsFolder = Ctrl.DataContext as ISolutionFolder) != null)
            {
                treeviewSolutionExplorer.ToggleIsExpanded(AsFolder);
                IsHandled = true;
            }

            else if (Ctrl.DataContext is ISolutionItem)
            {
                RoutedUICommand OpenCommand = ApplicationCommands.Open;
                if (OpenCommand.CanExecute(this, null))
                    OpenCommand.Execute(this, null);

                IsHandled = true;
            }

            Debug.Assert(IsHandled);
        }
        #endregion

        #region Undo/Redo
        private void ExpandNewNodes(SolutionExplorerOperation Operation)
        {
            if (Operation == null)
                throw new ArgumentNullException(nameof(Operation));

            Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new FollowOperationWithExpandHandler(OnFollowOperationWithExpand), Operation.ExpandedFolderList);
        }

        private delegate void FollowOperationWithExpandHandler(IReadOnlyCollection<ISolutionFolder> ExpandedFolderList);
        private void OnFollowOperationWithExpand(IReadOnlyCollection<ISolutionFolder> ExpandedFolderList)
        {
            foreach (ISolutionFolder ExpandedFolder in ExpandedFolderList)
                treeviewSolutionExplorer.Expand(ExpandedFolder);
        }
        #endregion

        #region Implementation of INotifyPropertyChanged
        /// <summary>
        ///     Implements the PropertyChanged event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        internal void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameter is mandatory with [CallerMemberName]")]
        internal void NotifyThisPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
