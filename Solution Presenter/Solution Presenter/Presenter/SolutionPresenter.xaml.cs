using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using UndoRedo;
using Verification;
using Xceed.Wpf.AvalonDock.Controls;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Layout.Serialization;
using Xceed.Wpf.AvalonDock.Themes;

namespace CustomControls
{
    public partial class SolutionPresenter : UserControl, IGestureSource, IActiveDocumentSource
    {
        #region Custom properties and events
        #region Application Name
        public static readonly DependencyProperty ApplicationNameProperty = DependencyProperty.Register("ApplicationName", typeof(string), typeof(SolutionPresenter), new PropertyMetadata(null));

        public string ApplicationName
        {
            get { return (string)GetValue(ApplicationNameProperty); }
            set { SetValue(ApplicationNameProperty, value); }
        }
        #endregion
        #region Document Types
        private static readonly DependencyPropertyKey DocumentTypesPropertyKey = DependencyProperty.RegisterReadOnly("DocumentTypes", typeof(DocumentTypeCollection), typeof(SolutionPresenter), new PropertyMetadata(null));
        public static readonly DependencyProperty DocumentTypesProperty = DocumentTypesPropertyKey.DependencyProperty;

        public DocumentTypeCollection DocumentTypes
        {
            get { return (DocumentTypeCollection)GetValue(DocumentTypesProperty); }
        }

        public virtual void SetDocumentTypes(DocumentTypeCollection documentTypes)
        {
            SetValue(DocumentTypesPropertyKey, documentTypes);
            UpdateDocumentTypeCommands();
        }
        #endregion
        #region Open Documents
        private static readonly DependencyPropertyKey OpenDocumentsPropertyKey = DependencyProperty.RegisterReadOnly("OpenDocuments", typeof(ICollection<IDocument>), typeof(SolutionPresenter), new PropertyMetadata(null));
        public static readonly DependencyProperty OpenDocumentsProperty = OpenDocumentsPropertyKey.DependencyProperty;

        public ICollection<IDocument> OpenDocuments
        {
            get { return (ICollection<IDocument>)GetValue(OpenDocumentsProperty); }
        }
        #endregion
        #region Active Document
        public static readonly DependencyProperty ActiveDocumentProperty = DependencyProperty.Register("ActiveDocument", typeof(IDocument), typeof(SolutionPresenter), new PropertyMetadata(null, OnActiveDocumentChanged));

        public IDocument ActiveDocument
        {
            get { return (IDocument)GetValue(ActiveDocumentProperty); }
            set { SetValue(ActiveDocumentProperty, value); }
        }

        protected static void OnActiveDocumentChanged(DependencyObject modifiedObject, DependencyPropertyChangedEventArgs e)
        {
            SolutionPresenter ctrl = (SolutionPresenter)modifiedObject;
            ctrl.OnActiveDocumentChanged(e);
        }

        protected virtual void OnActiveDocumentChanged(DependencyPropertyChangedEventArgs e)
        {
            if (OpenDocuments.Contains(ActiveDocument) && !IsActiveDocumentChanging)
                UserActivateDocument(ActiveDocument);
        }
        #endregion
        #region Solution Icon
        public static readonly DependencyProperty SolutionIconProperty = DependencyProperty.Register("SolutionIcon", typeof(ImageSource), typeof(SolutionPresenter), new PropertyMetadata(null));

        public ImageSource SolutionIcon
        {
            get { return (ImageSource)GetValue(SolutionIconProperty); }
            set { SetValue(SolutionIconProperty, value); }
        }
        #endregion
        #region Solution Extension
        public static readonly DependencyProperty SolutionExtensionProperty = DependencyProperty.Register("SolutionExtension", typeof(string), typeof(SolutionPresenter), new PropertyMetadata(null));

        public string SolutionExtension
        {
            get { return (string)GetValue(SolutionExtensionProperty); }
            set { SetValue(SolutionExtensionProperty, value); }
        }
        #endregion
        #region Solution Extension Filter
        public static readonly DependencyProperty SolutionExtensionFilterProperty = DependencyProperty.Register("SolutionExtensionFilter", typeof(string), typeof(SolutionPresenter), new PropertyMetadata(null));

        public string SolutionExtensionFilter
        {
            get { return (string)GetValue(SolutionExtensionFilterProperty); }
            set { SetValue(SolutionExtensionFilterProperty, value); }
        }
        #endregion
        #region Theme Option
        public static readonly DependencyProperty ThemeOptionProperty = DependencyProperty.Register("ThemeOption", typeof(ThemeOption), typeof(SolutionPresenter), new PropertyMetadata(ThemeOption.Aero, OnThemeOptionChanged));

        public ThemeOption ThemeOption
        {
            get { return (ThemeOption)GetValue(ThemeOptionProperty); }
            set { SetValue(ThemeOptionProperty, value); }
        }

        protected static void OnThemeOptionChanged(DependencyObject modifiedObject, DependencyPropertyChangedEventArgs e)
        {
            SolutionPresenter ctrl = (SolutionPresenter)modifiedObject;
            ctrl.OnThemeOptionChanged(e);
        }

        protected virtual void OnThemeOptionChanged(DependencyPropertyChangedEventArgs e)
        {
            UpdateThemeOption();
        }
        #endregion
        #region SaveBeforeCompiling
        public static readonly DependencyProperty SaveBeforeCompilingProperty = DependencyProperty.Register("SaveBeforeCompiling", typeof(bool), typeof(SolutionPresenter), new PropertyMetadata(true));

        public bool SaveBeforeCompiling
        {
            get { return (bool)GetValue(SaveBeforeCompilingProperty); }
            set { SetValue(SaveBeforeCompilingProperty, value); }
        }
        #endregion
        #region Gesture Translator
        public static readonly DependencyProperty GestureTranslatorProperty = DependencyProperty.Register("GestureTranslator", typeof(IGestureTranslator), typeof(SolutionPresenter), new PropertyMetadata(new GestureTranslator()));

        public IGestureTranslator GestureTranslator
        {
            get { return (IGestureTranslator)GetValue(GestureTranslatorProperty); }
            set { SetValue(GestureTranslatorProperty, value); }
        }
        #endregion
        #region Root Path
        private static readonly DependencyPropertyKey RootPathPropertyKey = DependencyProperty.RegisterReadOnly("RootPath", typeof(IRootPath), typeof(SolutionPresenter), new PropertyMetadata(null));
        public static readonly DependencyProperty RootPathProperty = RootPathPropertyKey.DependencyProperty;

        public IRootPath RootPath
        {
            get { return (IRootPath)GetValue(RootPathProperty); }
        }
        #endregion
        #region Root Properties
        private static readonly DependencyPropertyKey RootPropertiesPropertyKey = DependencyProperty.RegisterReadOnly("RootProperties", typeof(IRootProperties), typeof(SolutionPresenter), new PropertyMetadata(null));
        public static readonly DependencyProperty RootPropertiesProperty = RootPropertiesPropertyKey.DependencyProperty;

        public IRootProperties RootProperties
        {
            get { return (IRootProperties)GetValue(RootPropertiesProperty); }
        }
        #endregion
        #region Option Pages
        private static readonly DependencyPropertyKey OptionPagesPropertyKey = DependencyProperty.RegisterReadOnly("OptionPages", typeof(ICollection<TabItem>), typeof(SolutionPresenter), new PropertyMetadata(null));
        public static readonly DependencyProperty OptionPagesProperty = OptionPagesPropertyKey.DependencyProperty;

        public ICollection<TabItem> OptionPages
        {
            get { return (ICollection<TabItem>)GetValue(OptionPagesProperty); }
        }

        public virtual void SetOptionPages(ICollection<TabItem> optionPages)
        {
            SetValue(OptionPagesPropertyKey, optionPages);
        }
        #endregion
        #region Is Loading Tree
        private static readonly DependencyPropertyKey IsLoadingTreePropertyKey = DependencyProperty.RegisterReadOnly("IsLoadingTree", typeof(bool), typeof(SolutionPresenter), new PropertyMetadata(false));
        public static readonly DependencyProperty IsLoadingTreeProperty = IsLoadingTreePropertyKey.DependencyProperty;

        public bool IsLoadingTree
        {
            get { return (bool)GetValue(IsLoadingTreeProperty); }
        }
        #endregion
        #region Tree Node Comparer
        private static readonly DependencyPropertyKey TreeNodeComparerPropertyKey = DependencyProperty.RegisterReadOnly("TreeNodeComparer", typeof(IComparer<ITreeNodePath>), typeof(SolutionPresenter), new PropertyMetadata(null));
        public static readonly DependencyProperty TreeNodeComparerProperty = TreeNodeComparerPropertyKey.DependencyProperty;

        public IComparer<ITreeNodePath> TreeNodeComparer
        {
            get { return (IComparer<ITreeNodePath>)GetValue(TreeNodeComparerProperty); }
        }
        #endregion
        #region Main Menu Loaded
        public static readonly RoutedEvent MainMenuLoadedEvent = EventManager.RegisterRoutedEvent("MainMenuLoaded", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        public event RoutedEventHandler MainMenuLoaded
        {
            add { AddHandler(MainMenuLoadedEvent, value); }
            remove { RemoveHandler(MainMenuLoadedEvent, value); }
        }

        protected virtual void NotifyMainMenuLoaded(RoutedEventArgs e)
        {
            Assert.ValidateReference(e);

            RaiseEvent(new RoutedEventArgs(MainMenuLoadedEvent, e.OriginalSource));
        }
        #endregion
        #region Main ToolBar Loaded
        public static readonly RoutedEvent MainToolBarLoadedEvent = EventManager.RegisterRoutedEvent("MainToolBarLoaded", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        public event RoutedEventHandler MainToolBarLoaded
        {
            add { AddHandler(MainToolBarLoadedEvent, value); }
            remove { RemoveHandler(MainToolBarLoadedEvent, value); }
        }

        protected virtual void NotifyMainToolBarLoaded(RoutedEventArgs e)
        {
            Assert.ValidateReference(e);

            RaiseEvent(new RoutedEventArgs(MainToolBarLoadedEvent, e.OriginalSource));
        }
        #endregion
        #region Context Menu Loaded
        public static readonly RoutedEvent ContextMenuLoadedEvent = EventManager.RegisterRoutedEvent("ContextMenuLoaded", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        public event RoutedEventHandler ContextMenuLoaded
        {
            add { AddHandler(ContextMenuLoadedEvent, value); }
            remove { RemoveHandler(ContextMenuLoadedEvent, value); }
        }

        protected virtual void NotifyContextMenuLoaded(RoutedEventArgs e)
        {
            Assert.ValidateReference(e);

            RaiseEvent(new RoutedEventArgs(ContextMenuLoadedEvent, e.OriginalSource));
        }
        #endregion
        #region Context Menu Opened
        public static readonly RoutedEvent ContextMenuOpenedEvent = SolutionExplorer.ContextMenuOpenedEvent;

        public event RoutedEventHandler ContextMenuOpened
        {
            add { AddHandler(ContextMenuOpenedEvent, value); }
            remove { RemoveHandler(ContextMenuOpenedEvent, value); }
        }
        #endregion
        #region Solution Tree Committed
        public static readonly RoutedEvent SolutionTreeCommittedEvent = EventManager.RegisterRoutedEvent("SolutionTreeCommitted", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        public event RoutedEventHandler SolutionTreeCommitted
        {
            add { AddHandler(SolutionTreeCommittedEvent, value); SolutionTreeCommittedEventArgs.IncrementHandlerCount(); }
            remove { RemoveHandler(SolutionTreeCommittedEvent, value); SolutionTreeCommittedEventArgs.DecrementHandlerCount(); }
        }

        protected virtual void NotifySolutionTreeCommitted(CommitInfo info, SolutionOperation solutionOperation, IRootPath rootPath, IRootPath newRootPath, string destinationPath)
        {
            SolutionTreeCommittedEventContext EventContext = new SolutionTreeCommittedEventContext(info, solutionOperation, rootPath, newRootPath, destinationPath);

            if (SolutionTreeCommittedEventArgs.HasHandler)
            {
                SolutionTreeCommittedEventArgs Args = new SolutionTreeCommittedEventArgs(SolutionTreeCommittedEvent, EventContext);
                Args.EventCompleted += OnCommitComplete;
                RaiseEvent(Args);
            }
            else
                OnCommitComplete(null, new SolutionPresenterEventCompletedEventArgs(EventContext, new SolutionTreeCommittedCompletionArgs()));
        }
        #endregion
        #region Folder Enumerated
        public static readonly RoutedEvent FolderEnumeratedEvent = EventManager.RegisterRoutedEvent("FolderEnumerated", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        public event RoutedEventHandler FolderEnumerated
        {
            add { AddHandler(FolderEnumeratedEvent, value); FolderEnumeratedEventArgs.IncrementHandlerCount(); }
            remove { RemoveHandler(FolderEnumeratedEvent, value); FolderEnumeratedEventArgs.DecrementHandlerCount(); }
        }

        private void NotifyFolderEnumerated(IFolderPath ParentPath, ICollection<IFolderPath> ParentPathList, IRootProperties RootProperties, ICollection<IFolderPath> ExpandedFolderList, object Context)
        {
            FolderEnumeratedEventContext EventContext = new FolderEnumeratedEventContext(ParentPath, ParentPathList, RootProperties, ExpandedFolderList, Context);

            if (FolderEnumeratedEventArgs.HasHandler)
            {
                FolderEnumeratedEventArgs Args = new FolderEnumeratedEventArgs(FolderEnumeratedEvent, EventContext);
                Args.EventCompleted += OnFolderEnumeratedComplete;
                RaiseEvent(Args);
            }
            else
                OnFolderEnumeratedComplete(null, new SolutionPresenterEventCompletedEventArgs(EventContext, new FolderEnumeratedCompletionArgs()));
        }
        #endregion
        #region Solution Tree Loaded
        public static readonly RoutedEvent SolutionTreeLoadedEvent = EventManager.RegisterRoutedEvent("SolutionTreeLoaded", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        public event RoutedEventHandler SolutionTreeLoaded
        {
            add { AddHandler(SolutionTreeLoadedEvent, value); SolutionTreeLoadedEventArgs.IncrementHandlerCount(); }
            remove { RemoveHandler(SolutionTreeLoadedEvent, value); SolutionTreeLoadedEventArgs.DecrementHandlerCount(); }
        }

        protected virtual void NotifySolutionTreeLoaded(bool isCanceled)
        {
            SolutionTreeLoadedEventContext EventContext = new SolutionTreeLoadedEventContext(isCanceled);
            SolutionTreeLoadedEventArgs Args = new SolutionTreeLoadedEventArgs(SolutionTreeLoadedEvent, EventContext);
            RaiseEvent(Args);
        }
        #endregion
        #region Solution Selected
        public static readonly RoutedEvent SolutionSelectedEvent = EventManager.RegisterRoutedEvent("SolutionSelected", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        public event RoutedEventHandler SolutionSelected
        {
            add { AddHandler(SolutionSelectedEvent, value); SolutionSelectedEventArgs.IncrementHandlerCount(); }
            remove { RemoveHandler(SolutionSelectedEvent, value); SolutionSelectedEventArgs.DecrementHandlerCount(); }
        }

        protected virtual void NotifySolutionSelected()
        {
            SolutionSelectedEventContext EventContext = new SolutionSelectedEventContext();

            if (SolutionSelectedEventArgs.HasHandler)
            {
                SolutionSelectedEventArgs Args = new SolutionSelectedEventArgs(SolutionSelectedEvent, EventContext);
                Args.EventCompleted += OnSolutionSelectedComplete;
                RaiseEvent(Args);
            }
            else
                OnSolutionSelectedComplete(null, new SolutionPresenterEventCompletedEventArgs(EventContext, new SolutionSelectedCompletionArgs()));
        }
        #endregion
        #region Solution Created
        public static readonly RoutedEvent SolutionCreatedEvent = EventManager.RegisterRoutedEvent("SolutionCreated", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        public event RoutedEventHandler SolutionCreated
        {
            add { AddHandler(SolutionCreatedEvent, value); SolutionCreatedEventArgs.IncrementHandlerCount(); }
            remove { RemoveHandler(SolutionCreatedEvent, value); SolutionCreatedEventArgs.DecrementHandlerCount(); }
        }

        protected virtual void NotifySolutionCreated()
        {
            SolutionCreatedEventContext EventContext = new SolutionCreatedEventContext();

            if (SolutionCreatedEventArgs.HasHandler)
            {
                SolutionCreatedEventArgs Args = new SolutionCreatedEventArgs(SolutionCreatedEvent, EventContext);
                Args.EventCompleted += OnSolutionCreatedComplete;
                RaiseEvent(Args);
            }
            else
                OnSolutionCreatedComplete(null, new SolutionPresenterEventCompletedEventArgs(EventContext, new SolutionCreatedCompletionArgs()));
        }
        #endregion
        #region Solution Opened
        public static readonly RoutedEvent SolutionOpenedEvent = EventManager.RegisterRoutedEvent("SolutionOpened", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        public event RoutedEventHandler SolutionOpened
        {
            add { AddHandler(SolutionOpenedEvent, value); SolutionOpenedEventArgs.IncrementHandlerCount(); }
            remove { RemoveHandler(SolutionOpenedEvent, value); SolutionOpenedEventArgs.DecrementHandlerCount(); }
        }

        protected virtual void NotifySolutionOpened(IRootPath openedRootPath)
        {
            SolutionOpenedEventContext EventContext = new SolutionOpenedEventContext(openedRootPath);

            if (SolutionOpenedEventArgs.HasHandler)
            {
                SolutionOpenedEventArgs Args = new SolutionOpenedEventArgs(SolutionOpenedEvent, EventContext);
                Args.EventCompleted += OnSolutionOpenedComplete;
                RaiseEvent(Args);
            }
            else
                OnSolutionOpenedComplete(null, new SolutionPresenterEventCompletedEventArgs(EventContext, new SolutionOpenedCompletionArgs()));
        }
        #endregion
        #region Solution Closed
        public static readonly RoutedEvent SolutionClosedEvent = EventManager.RegisterRoutedEvent("SolutionClosed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        public event RoutedEventHandler SolutionClosed
        {
            add { AddHandler(SolutionClosedEvent, value); SolutionClosedEventArgs.IncrementHandlerCount(); }
            remove { RemoveHandler(SolutionClosedEvent, value); SolutionClosedEventArgs.DecrementHandlerCount(); }
        }

        protected virtual void NotifySolutionClosed(SolutionOperation solutionOperation, IRootPath closedRootPath, IRootPath newRootPath)
        {
            SolutionClosedEventContext EventContext = new SolutionClosedEventContext(solutionOperation, closedRootPath, newRootPath);

            if (SolutionClosedEventArgs.HasHandler)
            {
                SolutionClosedEventArgs Args = new SolutionClosedEventArgs(SolutionClosedEvent, EventContext);
                Args.EventCompleted += OnSolutionClosedComplete;
                RaiseEvent(Args);
            }
            else
                OnSolutionClosedComplete(null, new SolutionPresenterEventCompletedEventArgs(EventContext, new SolutionClosedCompletionArgs()));
        }
        #endregion
        #region Solution Deleted
        public static readonly RoutedEvent SolutionDeletedEvent = EventManager.RegisterRoutedEvent("SolutionDeleted", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        public event RoutedEventHandler SolutionDeleted
        {
            add { AddHandler(SolutionDeletedEvent, value); SolutionDeletedEventArgs.IncrementHandlerCount(); }
            remove { RemoveHandler(SolutionDeletedEvent, value); SolutionDeletedEventArgs.DecrementHandlerCount(); }
        }

        protected virtual void NotifySolutionDeleted(IRootPath deletedRootPath, IReadOnlyCollection<ITreeNodePath> deletedTree)
        {
            SolutionDeletedEventContext EventContext = new SolutionDeletedEventContext(deletedRootPath, deletedTree);

            if (SolutionDeletedEventArgs.HasHandler)
            {
                SolutionDeletedEventArgs Args = new SolutionDeletedEventArgs(SolutionDeletedEvent, EventContext);
                Args.EventCompleted += OnSolutionDeletedComplete;
                RaiseEvent(Args);
            }
            else
                OnSolutionDeletedComplete(null, new SolutionPresenterEventCompletedEventArgs(EventContext, new SolutionDeletedCompletionArgs()));
        }
        #endregion
        #region Solution Exported
        public static readonly RoutedEvent SolutionExportedEvent = EventManager.RegisterRoutedEvent("SolutionExported", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        public event RoutedEventHandler SolutionExported
        {
            add { AddHandler(SolutionExportedEvent, value); SolutionExportedEventArgs.IncrementHandlerCount(); }
            remove { RemoveHandler(SolutionExportedEvent, value); SolutionExportedEventArgs.DecrementHandlerCount(); }
        }

        protected virtual void NotifySolutionExported(IRootPath exportedRootPath, string destinationPath)
        {
            SolutionExportedEventContext EventContext = new SolutionExportedEventContext(exportedRootPath, destinationPath);

            if (SolutionExportedEventArgs.HasHandler)
            {
                SolutionExportedEventArgs Args = new SolutionExportedEventArgs(SolutionExportedEvent, EventContext);
                Args.EventCompleted += OnSolutionExportedComplete;
                RaiseEvent(Args);
            }
            else
                OnSolutionExportedComplete(null, new SolutionPresenterEventCompletedEventArgs(EventContext, new SolutionExportedCompletionArgs()));
        }
        #endregion
        #region Imported
        public static readonly RoutedEvent SolutionImportedEvent = SolutionExplorer.ImportedEvent;

        public event RoutedEventHandler SolutionImported
        {
            add { AddHandler(SolutionImportedEvent, value); }
            remove { RemoveHandler(SolutionImportedEvent, value); }
        }
        #endregion
        #region Folder Created
        public static readonly RoutedEvent FolderCreatedEvent = EventManager.RegisterRoutedEvent("FolderCreated", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        public event RoutedEventHandler FolderCreated
        {
            add { AddHandler(FolderCreatedEvent, value); FolderCreatedEventArgs.IncrementHandlerCount(); }
            remove { RemoveHandler(FolderCreatedEvent, value); FolderCreatedEventArgs.DecrementHandlerCount(); }
        }

        protected virtual void NotifyFolderCreated(IFolderPath parentPath, string folderName, IRootProperties rootProperties)
        {
            FolderCreatedEventContext EventContext = new FolderCreatedEventContext(parentPath, folderName, rootProperties);

            FolderCreatedEventArgs Args = new FolderCreatedEventArgs(FolderCreatedEvent, EventContext);
            Args.EventCompleted += OnFolderCreatedComplete;
            RaiseEvent(Args);
        }
        #endregion
        #region Node Pasted
        public static readonly RoutedEvent NodePastedEvent = EventManager.RegisterRoutedEvent("NodePasted", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        public event RoutedEventHandler NodePasted
        {
            add { AddHandler(NodePastedEvent, value); NodePastedEventArgs.IncrementHandlerCount(); }
            remove { RemoveHandler(NodePastedEvent, value); NodePastedEventArgs.DecrementHandlerCount(); }
        }

        protected virtual void NotifyNodePasted(ITreeNodePath path, IFolderPath parentPath, IReadOnlyDictionary<ITreeNodePath, IPathConnection> pathTable, Dictionary<ITreeNodePath, IFolderPath> updatedParentTable, IRootProperties rootProperties, bool isUndoRedo)
        {
            NodePastedEventContext EventContext = new NodePastedEventContext(path, parentPath, pathTable, updatedParentTable, rootProperties, isUndoRedo);

            NodePastedEventArgs Args = new NodePastedEventArgs(NodePastedEvent, EventContext);
            Args.EventCompleted += OnNodePastedComplete;
            RaiseEvent(Args);
        }
        #endregion
        #region Node Renamed
        public static readonly RoutedEvent NodeRenamedEvent = EventManager.RegisterRoutedEvent("NodeRenamed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        public event RoutedEventHandler NodeRenamed
        {
            add { AddHandler(NodeRenamedEvent, value); NodeRenamedEventArgs.IncrementHandlerCount(); }
            remove { RemoveHandler(NodeRenamedEvent, value); NodeRenamedEventArgs.DecrementHandlerCount(); }
        }

        protected virtual void NotifyNodeRenamed(ITreeNodePath path, string newName, bool isUndoRedo, IRootProperties rootProperties)
        {
            NodeRenamedEventContext EventContext = new NodeRenamedEventContext(path, newName, isUndoRedo, rootProperties);

            NodeRenamedEventArgs Args = new NodeRenamedEventArgs(NodeRenamedEvent, EventContext);
            Args.EventCompleted += OnNodeRenamedComplete;
            RaiseEvent(Args);
        }
        #endregion
        #region Node Moved
        public static readonly RoutedEvent NodeMovedEvent = EventManager.RegisterRoutedEvent("NodeMoved", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        public event RoutedEventHandler NodeMoved
        {
            add { AddHandler(NodeMovedEvent, value); NodeMovedEventArgs.IncrementHandlerCount(); }
            remove { RemoveHandler(NodeMovedEvent, value); NodeMovedEventArgs.DecrementHandlerCount(); }
        }

        protected virtual void NotifyNodeMoved(ITreeNodePath path, IFolderPath newParentPath, bool isUndoRedo, IRootProperties rootProperties)
        {
            NodeMovedEventContext EventContext = new NodeMovedEventContext(path, newParentPath, isUndoRedo, rootProperties);

            NodeMovedEventArgs Args = new NodeMovedEventArgs(NodeMovedEvent, EventContext);
            Args.EventCompleted += OnNodeMovedComplete;
            RaiseEvent(Args);
        }
        #endregion
        #region Document Created
        public static readonly RoutedEvent DocumentCreatedEvent = EventManager.RegisterRoutedEvent("DocumentCreated", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        public event RoutedEventHandler DocumentCreated
        {
            add { AddHandler(DocumentCreatedEvent, value); DocumentCreatedEventArgs.IncrementHandlerCount(); }
            remove { RemoveHandler(DocumentCreatedEvent, value); DocumentCreatedEventArgs.DecrementHandlerCount(); }
        }

        protected virtual void NotifyDocumentCreated(IFolderPath destinationFolderPath, IDocumentType type, IRootProperties rootProperties)
        {
            DocumentCreatedEventContext EventContext = new DocumentCreatedEventContext(destinationFolderPath, type, rootProperties);
            DocumentCreatedEventArgs Args = new DocumentCreatedEventArgs(DocumentCreatedEvent, EventContext);
            Args.EventCompleted += OnDocumentCreatedComplete;
            RaiseEvent(Args);
        }
        #endregion
        #region Document Selected
        public static readonly RoutedEvent DocumentSelectedEvent = EventManager.RegisterRoutedEvent("DocumentSelected", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        public event RoutedEventHandler DocumentSelected
        {
            add { AddHandler(DocumentSelectedEvent, value); DocumentSelectedEventArgs.IncrementHandlerCount(); }
            remove { RemoveHandler(DocumentSelectedEvent, value); DocumentSelectedEventArgs.DecrementHandlerCount(); }
        }

        protected virtual void NotifyDocumentSelected()
        {
            DocumentSelectedEventContext EventContext = new DocumentSelectedEventContext();

            if (DocumentSelectedEventArgs.HasHandler)
            {
                DocumentSelectedEventArgs Args = new DocumentSelectedEventArgs(DocumentSelectedEvent, EventContext);
                Args.EventCompleted += OnDocumentSelectedComplete;
                RaiseEvent(Args);
            }
            else
                OnDocumentSelectedComplete(null, new SolutionPresenterEventCompletedEventArgs(EventContext, new DocumentSelectedCompletionArgs()));
        }
        #endregion
        #region Document Added
        public static readonly RoutedEvent DocumentAddedEvent = EventManager.RegisterRoutedEvent("DocumentAdded", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        public event RoutedEventHandler DocumentAdded
        {
            add { AddHandler(DocumentAddedEvent, value); DocumentAddedEventArgs.IncrementHandlerCount(); }
            remove { RemoveHandler(DocumentAddedEvent, value); DocumentAddedEventArgs.DecrementHandlerCount(); }
        }

        protected virtual void NotifyDocumentAdded(DocumentOperation documentOperation, IFolderPath destinationFolderPath, IList<IDocumentPath> documentPathList, IRootProperties rootProperties)
        {
            DocumentAddedEventContext EventContext = new DocumentAddedEventContext(documentOperation, destinationFolderPath, documentPathList, rootProperties);

            if (DocumentAddedEventArgs.HasHandler)
            {
                DocumentAddedEventArgs Args = new DocumentAddedEventArgs(DocumentAddedEvent, EventContext);
                Args.EventCompleted += OnDocumentAddedComplete;
                RaiseEvent(Args);
            }
            else
                OnDocumentAddedComplete(null, new SolutionPresenterEventCompletedEventArgs(EventContext, new DocumentAddedCompletionArgs()));
        }
        #endregion
        #region Document Opened
        public static readonly RoutedEvent DocumentOpenedEvent = EventManager.RegisterRoutedEvent("DocumentOpened", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        public event RoutedEventHandler DocumentOpened
        {
            add { AddHandler(DocumentOpenedEvent, value); DocumentOpenedEventArgs.IncrementHandlerCount(); }
            remove { RemoveHandler(DocumentOpenedEvent, value); DocumentOpenedEventArgs.DecrementHandlerCount(); }
        }

        protected virtual void NotifyDocumentOpened(DocumentOperation documentOperation, IFolderPath destinationFolderPath, IList<IDocumentPath> openedDocumentPathList, IList<IDocumentPath> documentPathList, object errorLocation)
        {
            DocumentOpenedEventContext EventContext = new DocumentOpenedEventContext(documentOperation, destinationFolderPath, openedDocumentPathList, documentPathList, errorLocation);

            if (DocumentOpenedEventArgs.HasHandler)
            {
                DocumentOpenedEventArgs Args = new DocumentOpenedEventArgs(DocumentOpenedEvent, EventContext);
                Args.EventCompleted += OnDocumentOpenedComplete;
                RaiseEvent(Args);
            }
            else
                OnDocumentOpenedComplete(null, new SolutionPresenterEventCompletedEventArgs(EventContext, new DocumentOpenedCompletionArgs()));
        }
        #endregion
        #region Document Closed
        public static readonly RoutedEvent DocumentClosedEvent = EventManager.RegisterRoutedEvent("DocumentClosed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        public event RoutedEventHandler DocumentClosed
        {
            add { AddHandler(DocumentClosedEvent, value); DocumentClosedEventArgs.IncrementHandlerCount(); }
            remove { RemoveHandler(DocumentClosedEvent, value); DocumentClosedEventArgs.DecrementHandlerCount(); }
        }

        protected virtual void NotifyDocumentClosed(DocumentOperation documentOperation, IDocument closedDocument)
        {
            List<IDocument> ClosedDocumentList = new List<IDocument>();
            ClosedDocumentList.Add(closedDocument);

            NotifyDocumentClosed(documentOperation, ClosedDocumentList, new Dictionary<ITreeNodePath, IPathConnection>(), false,null);
        }

        protected virtual void NotifyDocumentClosed(DocumentOperation documentOperation, IList<IDocument> closedDocumentList, IReadOnlyDictionary<ITreeNodePath, IPathConnection> closedTree, bool isUndoRedo, object clientInfo)
        {
            DocumentClosedEventContext EventContext = new DocumentClosedEventContext(documentOperation, closedDocumentList, closedTree, isUndoRedo, clientInfo);

            if (DocumentClosedEventArgs.HasHandler)
            {
                DocumentClosedEventArgs Args = new DocumentClosedEventArgs(DocumentClosedEvent, EventContext);
                Args.EventCompleted += OnDocumentClosedComplete;
                RaiseEvent(Args);
            }
            else
                OnDocumentClosedComplete(null, new SolutionPresenterEventCompletedEventArgs(EventContext, new DocumentClosedCompletionArgs()));
        }
        #endregion
        #region Document Saved
        public static readonly RoutedEvent DocumentSavedEvent = EventManager.RegisterRoutedEvent("DocumentSaved", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        public event RoutedEventHandler DocumentSaved
        {
            add { AddHandler(DocumentSavedEvent, value); DocumentSavedEventArgs.IncrementHandlerCount(); }
            remove { RemoveHandler(DocumentSavedEvent, value); DocumentSavedEventArgs.DecrementHandlerCount(); }
        }

        protected virtual void NotifyDocumentSaved(DocumentOperation documentOperation, IDocument savedDocument, string fileName)
        {
            DocumentSavedEventContext EventContext = new DocumentSavedEventContext(documentOperation, savedDocument, fileName);

            if (DocumentSavedEventArgs.HasHandler)
            {
                DocumentSavedEventArgs Args = new DocumentSavedEventArgs(DocumentSavedEvent, EventContext);
                Args.EventCompleted += OnDocumentSavedComplete;
                RaiseEvent(Args);
            }
            else
                OnDocumentSavedComplete(null, new SolutionPresenterEventCompletedEventArgs(EventContext, new DocumentSavedCompletionArgs()));
        }
        #endregion
        #region Document Removed
        public static readonly RoutedEvent DocumentRemovedEvent = EventManager.RegisterRoutedEvent("DocumentRemoved", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        public event RoutedEventHandler DocumentRemoved
        {
            add { AddHandler(DocumentRemovedEvent, value); DocumentRemovedEventArgs.IncrementHandlerCount(); }
            remove { RemoveHandler(DocumentRemovedEvent, value); DocumentRemovedEventArgs.DecrementHandlerCount(); }
        }

        protected virtual void NotifyDocumentRemoved(IRootPath rootPath, IReadOnlyDictionary<ITreeNodePath, IPathConnection> deletedTree, bool isUndoRedo, object clientInfo)
        {
            DocumentRemovedEventContext EventContext = new DocumentRemovedEventContext(rootPath, deletedTree, isUndoRedo, clientInfo);

            if (DocumentRemovedEventArgs.HasHandler)
            {
                DocumentRemovedEventArgs Args = new DocumentRemovedEventArgs(DocumentRemovedEvent, EventContext);
                Args.EventCompleted += OnDocumentRemovedComplete;
                RaiseEvent(Args);
            }
            else
                OnDocumentRemovedComplete(null, new SolutionPresenterEventCompletedEventArgs(EventContext, new DocumentRemovedCompletionArgs()));
        }
        #endregion
        #region Document Exported
        public static readonly RoutedEvent DocumentExportedEvent = EventManager.RegisterRoutedEvent("DocumentExported", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        public event RoutedEventHandler DocumentExported
        {
            add { AddHandler(DocumentExportedEvent, value); DocumentExportedEventArgs.IncrementHandlerCount(); }
            remove { RemoveHandler(DocumentExportedEvent, value); DocumentExportedEventArgs.DecrementHandlerCount(); }
        }

        protected virtual void NotifyDocumentExported(DocumentOperation documentOperation, IDocument exportedDocument, string fileName)
        {
            List<IDocument> ExportedDocumentList = new List<IDocument>();
            ExportedDocumentList.Add(exportedDocument);
            NotifyDocumentExported(documentOperation, ExportedDocumentList, false, fileName);
        }

        protected virtual void NotifyDocumentExported(DocumentOperation documentOperation, ICollection<IDocument> exportedDocumentList, bool isDestinationFolder, string destinationPath)
        {
            DocumentExportedEventContext EventContext = new DocumentExportedEventContext(documentOperation, exportedDocumentList, isDestinationFolder, destinationPath);

            if (DocumentExportedEventArgs.HasHandler)
            {
                DocumentExportedEventArgs Args = new DocumentExportedEventArgs(DocumentExportedEvent, EventContext);
                Args.EventCompleted += OnDocumentExportedComplete;
                RaiseEvent(Args);
            }
            else
                OnDocumentExportedComplete(null, new SolutionPresenterEventCompletedEventArgs(EventContext, new DocumentExportedCompletionArgs()));
        }
        #endregion
        #region Error Focused
        public static readonly RoutedEvent ErrorFocusedEvent = EventManager.RegisterRoutedEvent("ErrorFocused", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        public event RoutedEventHandler ErrorFocused
        {
            add { AddHandler(ErrorFocusedEvent, value); ErrorFocusedEventArgs.IncrementHandlerCount(); }
            remove { RemoveHandler(ErrorFocusedEvent, value); ErrorFocusedEventArgs.DecrementHandlerCount(); }
        }

        protected virtual void NotifyErrorFocused(IDocument document, object errorLocation)
        {
            ErrorFocusedEventContext EventContext = new ErrorFocusedEventContext(document, errorLocation);

            if (ErrorFocusedEventArgs.HasHandler)
            {
                ErrorFocusedEventArgs Args = new ErrorFocusedEventArgs(ErrorFocusedEvent, EventContext);
                Args.EventCompleted += OnErrorFocusedComplete;
                RaiseEvent(Args);
            }
            else
                OnErrorFocusedComplete(null, new SolutionPresenterEventCompletedEventArgs(EventContext, new ErrorFocusedCompletionArgs()));
        }
        #endregion
        #region Add New Items Requested
        public static readonly RoutedEvent AddNewItemsRequestedEvent = EventManager.RegisterRoutedEvent("AddNewItemsRequested", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        public event RoutedEventHandler AddNewItemsRequested
        {
            add { AddHandler(AddNewItemsRequestedEvent, value); AddNewItemsRequestedEventArgs.IncrementHandlerCount(); }
            remove { RemoveHandler(AddNewItemsRequestedEvent, value); AddNewItemsRequestedEventArgs.DecrementHandlerCount(); }
        }

        protected virtual void NotifyAddNewItemsRequested(IFolderPath destinationFolderPath)
        {
            IAddNewItemsRequestedEventContext EventContext = new AddNewItemsRequestedEventContext(destinationFolderPath);

            if (AddNewItemsRequestedEventArgs.HasHandler)
            {
                AddNewItemsRequestedEventArgs Args = new AddNewItemsRequestedEventArgs(AddNewItemsRequestedEvent, EventContext);
                Args.EventCompleted += OnAddNewItemsRequestedComplete;
                RaiseEvent(Args);
            }
            else
                OnAddNewItemsRequestedComplete(null, new SolutionPresenterEventCompletedEventArgs(EventContext, new AddNewItemsRequestedCompletionArgs()));
        }
        #endregion
        #region Exit Requested
        public static readonly RoutedEvent ExitRequestedEvent = EventManager.RegisterRoutedEvent("ExitRequested", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        public event RoutedEventHandler ExitRequested
        {
            add { AddHandler(ExitRequestedEvent, value); }
            remove { RemoveHandler(ExitRequestedEvent, value); }
        }

        protected virtual void NotifyExitRequested()
        {
            RaiseEvent(new RoutedEventArgs(ExitRequestedEvent));
        }
        #endregion
        #region Document Import Descriptors
        private static readonly DependencyPropertyKey DocumentImportDescriptorsPropertyKey = DependencyProperty.RegisterReadOnly("DocumentImportDescriptors", typeof(ICollection<IDocumentImportDescriptor>), typeof(SolutionPresenter), new PropertyMetadata(null));
        public static readonly DependencyProperty DocumentImportDescriptorsProperty = DocumentImportDescriptorsPropertyKey.DependencyProperty;

        public ICollection<IDocumentImportDescriptor> DocumentImportDescriptors
        {
            get { return (ICollection<IDocumentImportDescriptor>)GetValue(DocumentImportDescriptorsProperty); }
        }

        public virtual void SetDocumentImportDescriptors(ICollection<IDocumentImportDescriptor> documentImportDescriptors)
        {
            SetValue(DocumentImportDescriptorsPropertyKey, documentImportDescriptors);
        }
        #endregion
        #region Import Folder
        public static readonly DependencyProperty ImportFolderProperty = DependencyProperty.Register("ImportFolder", typeof(string), typeof(SolutionPresenter), new PropertyMetadata(null));

        public string ImportFolder
        {
            get { return (string)GetValue(ImportFolderProperty); }
            set { SetValue(ImportFolderProperty, value); }
        }
        #endregion
        #region Export Folder
        public static readonly DependencyProperty ExportFolderProperty = DependencyProperty.Register("ExportFolder", typeof(string), typeof(SolutionPresenter), new PropertyMetadata(null));

        public string ExportFolder
        {
            get { return (string)GetValue(ExportFolderProperty); }
            set { SetValue(ExportFolderProperty, value); }
        }
        #endregion
        #region Import New Items Requested
        public static readonly RoutedEvent ImportNewItemsRequestedEvent = EventManager.RegisterRoutedEvent("ImportNewItemsRequested", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        public event RoutedEventHandler ImportNewItemsRequested
        {
            add { AddHandler(ImportNewItemsRequestedEvent, value); ImportNewItemsRequestedEventArgs.IncrementHandlerCount(); }
            remove { RemoveHandler(ImportNewItemsRequestedEvent, value); ImportNewItemsRequestedEventArgs.DecrementHandlerCount(); }
        }

        protected virtual void NotifyImportNewItemsRequested(Dictionary<object, IDocumentType> importedDocumentTable, IList<IDocumentPath> documentPathList)
        {
            ImportNewItemsRequestedEventContext EventContext = new ImportNewItemsRequestedEventContext(importedDocumentTable, documentPathList);

            if (ImportNewItemsRequestedEventArgs.HasHandler)
            {
                ImportNewItemsRequestedEventArgs Args = new ImportNewItemsRequestedEventArgs(ImportNewItemsRequestedEvent, EventContext);
                Args.EventCompleted += OnImportNewItemsRequestedComplete;
                RaiseEvent(Args);
            }
            else
                OnImportNewItemsRequestedComplete(null, new SolutionPresenterEventCompletedEventArgs(EventContext, new ImportNewItemsRequestedCompletionArgs()));
        }
        #endregion
        #region Build Solution Requested
        public static readonly RoutedEvent BuildSolutionRequestedEvent = EventManager.RegisterRoutedEvent("BuildSolutionRequested", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        public event RoutedEventHandler BuildSolutionRequested
        {
            add { AddHandler(BuildSolutionRequestedEvent, value); BuildSolutionRequestedEventArgs.IncrementHandlerCount(); }
            remove { RemoveHandler(BuildSolutionRequestedEvent, value); BuildSolutionRequestedEventArgs.DecrementHandlerCount(); }
        }

        protected virtual void NotifyBuildSolutionRequested()
        {
            BuildSolutionRequestedEventContext EventContext = new BuildSolutionRequestedEventContext();

            if (BuildSolutionRequestedEventArgs.HasHandler)
            {
                BuildSolutionRequestedEventArgs Args = new BuildSolutionRequestedEventArgs(BuildSolutionRequestedEvent, EventContext);
                Args.EventCompleted += OnBuildSolutionRequestedComplete;
                RaiseEvent(Args);
            }
            else
                OnBuildSolutionRequestedComplete(null, new SolutionPresenterEventCompletedEventArgs(EventContext, new BuildSolutionRequestedCompletionArgs()));
        }
        #endregion
        #region Options Changed
        public static readonly RoutedEvent OptionsChangedEvent = EventManager.RegisterRoutedEvent("OptionsChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        public event RoutedEventHandler OptionsChanged
        {
            add { AddHandler(OptionsChangedEvent, value); }
            remove { RemoveHandler(OptionsChangedEvent, value); }
        }

        protected virtual void NotifyOptionsChanged()
        {
            RaiseEvent(new RoutedEventArgs(OptionsChangedEvent));
        }
        #endregion
        #region Root Properties Requested
        public static readonly RoutedEvent RootPropertiesRequestedEvent = EventManager.RegisterRoutedEvent("RootPropertiesRequested", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        public event RoutedEventHandler RootPropertiesRequested
        {
            add { AddHandler(RootPropertiesRequestedEvent, value); RootPropertiesRequestedEventArgs.IncrementHandlerCount(); }
            remove { RemoveHandler(RootPropertiesRequestedEvent, value); RootPropertiesRequestedEventArgs.DecrementHandlerCount(); }
        }

        protected virtual void NotifyRootPropertiesRequested(IRootProperties properties)
        {
            RootPropertiesRequestedEventContext EventContext = new RootPropertiesRequestedEventContext(properties);

            if (RootPropertiesRequestedEventArgs.HasHandler)
            {
                RootPropertiesRequestedEventArgs Args = new RootPropertiesRequestedEventArgs(RootPropertiesRequestedEvent, EventContext);
                Args.EventCompleted += OnRootPropertiesRequestedComplete;
                RaiseEvent(Args);
            }
            else
                OnRootPropertiesRequestedComplete(null, new SolutionPresenterEventCompletedEventArgs(EventContext, new RootPropertiesRequestedCompletionArgs()));
        }
        #endregion
        #region Show About Requested
        public static readonly RoutedEvent ShowAboutRequestedEvent = EventManager.RegisterRoutedEvent("ShowAboutRequested", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        public event RoutedEventHandler ShowAboutRequested
        {
            add { AddHandler(ShowAboutRequestedEvent, value); }
            remove { RemoveHandler(ShowAboutRequestedEvent, value); }
        }

        protected virtual void NotifyShowAboutRequested()
        {
            RaiseEvent(new RoutedEventArgs(ShowAboutRequestedEvent));
        }
        #endregion
        #endregion

        #region Init
        public SolutionPresenter()
        {
            InitializeComponent();
            InitializeDocuments();
            InitializeMergedProperties();
            InitializeSolutionTree();
            InitializeContextMenu();
            InitializeDockManager();
            InitializeCompilerTool();
            InitUndoRedo();
        }
        #endregion

        #region Properties
        protected virtual Window Owner
        {
            get { return Window.GetWindow(this); }
        }

        public IReadOnlyDictionary<ITreeNodePath, IPathConnection> SelectedNodes
        {
            get { return spcSolutionExplorer.SelectedNodes; }
        }

        public IList<IItemPath> SelectedItems
        {
            get
            {
                IReadOnlyDictionary<ITreeNodePath, IPathConnection> SelectedNodes = spcSolutionExplorer.SelectedNodes;

                List<IItemPath> Result = new List<IItemPath>();
                foreach (KeyValuePair<ITreeNodePath, IPathConnection> Entry in SelectedNodes)
                {
                    IItemPath AsItemPath;
                    if ((AsItemPath = Entry.Key as IItemPath) != null)
                        Result.Add(AsItemPath);
                }

                return Result;
            }
        }

        public IReadOnlyDictionary<ITreeNodePath, IPathConnection> SelectedTree
        {
            get { return spcSolutionExplorer.SelectedTree; }
        }

        public ObservableCollection<ICompilationError> CompilationErrorList { get; private set; }

        public StatusTheme StatusTheme { get; private set; }

        public FrameworkElement ActiveDocumentContent
        {
            get
            {
                SplitView ctrl = GetActiveControl();
                if (ctrl != null)
                    return ctrl.GetRowContent(1);
                else
                    return null;
            }
        }

        #endregion

        #region Client Interface
        public new virtual void Focus()
        {
            if (dockManager.ActiveContent == spcSolutionExplorer || IsToolVisible("toolSolutionExplorer"))
                spcSolutionExplorer.Focus();

            else if (dockManager.ActiveContent == listviewCompilerOutput || IsToolVisible("toolCompilerOutput"))
                listviewCompilerOutput.Focus();

            else
            {
                IDocument Document = dockManager.ActiveContent as IDocument;
                if (Document != null)
                    Document.SetViewGotFocus();
            }
        }

        public virtual ICollection<IItemPath> Items
        {
            get { return spcSolutionExplorer.SolutionItems; }
        }

        public virtual IItemProperties GetItemProperties(IItemPath path)
        {
            return spcSolutionExplorer.GetItemProperties(path);
        }

        public virtual IList<IFolderPath> ExpandedFolderList
        {
            get { return spcSolutionExplorer.ExpandedFolderList; }
        }

        public virtual CommitOption CommitDirty(bool isExit)
        {
            CommitInfo Info = CheckToSaveCurrentSolution();

            if (Info.Option == CommitOption.CommitAndContinue)
                NotifySolutionTreeCommitted(Info, isExit ? SolutionOperation.Exit : SolutionOperation.Save, null, null, null);

            return Info.Option;
        }

        public virtual void RemoveDocuments(IReadOnlyCollection<IDocumentPath> documentPathList, object clientInfo)
        {
            IList<IDocument> ClosedDocumentList = FindOpenDocuments(documentPathList);
            IReadOnlyDictionary<ITreeNodePath, IPathConnection> ClosedTree = spcSolutionExplorer.FindItemsByDocumentPath(documentPathList);

            NotifyDocumentClosed(DocumentOperation.Remove, ClosedDocumentList, ClosedTree, false, clientInfo);
        }

        private IList<IDocument> FindOpenDocuments(IReadOnlyCollection<IDocumentPath> DocumentPathList)
        {
            IList<IDocument> Result = new List<IDocument>();

            foreach (IDocument Document in OpenDocuments)
                foreach (IDocumentPath DocumentPath in DocumentPathList)
                    if (Document.Path.IsEqual(DocumentPath))
                    {
                        Result.Add(Document);
                        break;
                    }

            return Result;
        }

        public virtual ICollection<IItemPath> SolutionItems
        {
            get { return spcSolutionExplorer.SolutionItems; }
        }
        #endregion

        #region Serialization
        public virtual string SerializeState()
        {
            string[] StateList = new string[] { SerializeDockManagerState(), SerializeToolBarState(), SerializePresenterState() };

            string MergedState = "";
            foreach (string State in StateList)
            {
                if (MergedState.Length > 0)
                    MergedState += '|';
                MergedState += State;
            }

            return MergedState;
        }

        public virtual void DeserializeState(string mergedState)
        {
            Assert.ValidateReference(mergedState);

            string[] StateList = mergedState.Split('|');

            int Index = 0;
            if (Index < StateList.Length)
                DeserializeDockManagerState(StateList[Index++]);
            if (Index < StateList.Length)
                DeserializeToolBarState(StateList[Index++]);
            if (Index < StateList.Length)
                DeserializePresenterState(StateList[Index++]);
        }

        public virtual void ResetState()
        {
            ResetDockManagerState();
            ResetToolBarState();
            ResetPresenterState();
        }

        protected virtual string SerializeDockManagerState()
        {
            using (StringWriter writer = new StringWriter(CultureInfo.InvariantCulture))
            {
                XmlLayoutSerializer DockManagerSerializer = new XmlLayoutSerializer(dockManager);
                DockManagerSerializer.Serialize(writer);
                writer.WriteLine();
                return writer.ToString();
            }
        }

        protected virtual string SerializeToolBarState()
        {
            return toolbarMain.SerializeActiveButtons();
        }

        protected virtual string SerializePresenterState()
        {
            string[] StateList = new string[] { SerializeThemeState(), SerializeCompilerState() };

            string MergedState = "";
            foreach (string State in StateList)
            {
                if (MergedState.Length > 0)
                    MergedState += ',';
                MergedState += State;
            }

            return MergedState;
        }

        protected virtual string SerializeThemeState()
        {
            return ThemeOption.ToString();
        }

        protected virtual string SerializeCompilerState()
        {
            return SaveBeforeCompiling.ToString();
        }

        public virtual void DeserializeDockManagerState(string state)
        {
            using (StringReader sr = new StringReader(state))
            {
                XmlLayoutSerializer DockManagerSerializer = new XmlLayoutSerializer(dockManager);
                DockManagerSerializer.Deserialize(sr);
            }
        }

        protected virtual void DeserializeToolBarState(string state)
        {
            toolbarMain.DeserializeActiveButtons(state);
        }

        protected virtual void DeserializePresenterState(string mergedState)
        {
            Assert.ValidateReference(mergedState);

            string[] StateList = mergedState.Split(',');

            int Index = 0;
            if (Index < StateList.Length)
                DeserializeThemeState(StateList[Index++]);
            if (Index < StateList.Length)
                DeserializeCompilerState(StateList[Index++]);
        }

        protected virtual void DeserializeThemeState(string state)
        {
            string[] ThemeNames = typeof(ThemeOption).GetEnumNames();
            Array ThemeValues = typeof(ThemeOption).GetEnumValues();

            for (int i = 0; i < ThemeNames.Length && i < ThemeValues.Length; i++)
                if (state == ThemeNames[i])
                {
                    ThemeOption = (ThemeOption)ThemeValues.GetValue(i);
                    break;
                }
        }

        protected virtual void DeserializeCompilerState(string state)
        {
            bool BoolValue;
            if (bool.TryParse(state, out BoolValue))
                SaveBeforeCompiling = BoolValue;
        }

        protected virtual void ResetDockManagerState()
        {
            string[] ResourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            foreach (string ResourceName in ResourceNames)
                if (ResourceName.EndsWith(".AvalonDock.config", StringComparison.Ordinal))
                {
                    ResetDockManagerStateFromResource(ResourceName);
                    break;
                }
        }

        protected virtual void ResetDockManagerStateFromResource(string resourceName)
        {
            using (Stream ResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                XmlLayoutSerializer DockManagerSerializer = new XmlLayoutSerializer(dockManager);
                DockManagerSerializer.Deserialize(ResourceStream);
            }
        }

        protected virtual void ResetToolBarState()
        {
            toolbarMain.Reset();
        }

        protected virtual void ResetPresenterState()
        {
            ResetThemeState();
            ResetCompilerState();
        }

        protected virtual void ResetThemeState()
        {
            ThemeOption = default(ThemeOption);
        }

        protected virtual void ResetCompilerState()
        {
            SaveBeforeCompiling = true;
        }
        #endregion

        #region Command
        protected virtual void UpdateDocumentTypeCommands()
        {
            Separator FirstMenuSeparator;
            Separator FirstToolBarSeparator;
            Separator FirstContextMenuSeparator;

            FirstMenuSeparator = toolbarMain.DocumentMenuSeparator;
            FirstToolBarSeparator = toolbarMain.DocumentToolBarSeparator;
            FirstContextMenuSeparator = spcSolutionExplorer.DocumentMenuSeparator;

            RemoveDocumentControls(FirstMenuSeparator);
            RemoveDocumentControls(FirstToolBarSeparator);
            RemoveDocumentControls(FirstContextMenuSeparator);

            RemoveDocumentCommandBindings();
            RemoveDocumentKeyBindings();

            if (DocumentTypes != null)
            {
                DocumentRoutedCommand PreferredDocumentCommand = null;

                for (int i = 0; i < DocumentTypes.Count; i++)
                {
                    IDocumentType DocumentType = DocumentTypes[i];
                    DocumentRoutedCommand DocumentCommand = CreateDocumentCommand(DocumentType);

                    AddDocumentControls(FirstMenuSeparator, i, CreateDocumentMenuItem(DocumentCommand));
                    AddDocumentControls(FirstToolBarSeparator, i, CreateDocumentToolBarButton(DocumentCommand));
                    AddDocumentControls(FirstContextMenuSeparator, i, CreateDocumentMenuItem(DocumentCommand));

                    AddDocumentCommandBinding(DocumentCommand);
                    if (DocumentType.IsPreferred)
                        PreferredDocumentCommand = DocumentCommand;
                }

                if (PreferredDocumentCommand != null)
                    AddDocumentKeyBinding(PreferredDocumentCommand);
            }
        }

        protected virtual void RemoveDocumentControls(Separator firstSeparator)
        {
            Assert.ValidateReference(firstSeparator);

            ItemsControl Container = firstSeparator.Parent as ItemsControl;
            ItemCollection Items = Container.Items;
            int FirstIndex = Items.IndexOf(firstSeparator) + 1;

            while (FirstIndex < Items.Count && !(Items[FirstIndex] is Separator))
                Items.RemoveAt(FirstIndex);
        }

        protected virtual DocumentRoutedCommand CreateDocumentCommand(IDocumentType documentType)
        {
            return new DocumentRoutedCommand(documentType);
        }

        protected virtual ExtendedToolBarMenuItem CreateDocumentMenuItem(ICommand documentCommand)
        {
            ExtendedToolBarMenuItem DocumentMenuItem = new ExtendedToolBarMenuItem();
            DocumentMenuItem.Command = documentCommand;
            return DocumentMenuItem;
        }

        protected virtual ExtendedToolBarButton CreateDocumentToolBarButton(ICommand documentCommand)
        {
            ExtendedToolBarButton DocumentToolBarButton = new ExtendedToolBarButton();
            DocumentToolBarButton.Command = documentCommand;
            return DocumentToolBarButton;
        }

        protected virtual void AddDocumentControls(Separator firstSeparator, int index, FrameworkElement documentControl)
        {
            Assert.ValidateReference(firstSeparator);

            ItemsControl Container = firstSeparator.Parent as ItemsControl;
            ItemCollection Items = Container.Items;
            int FirstIndex = Items.IndexOf(firstSeparator) + 1;

            Items.Insert(FirstIndex + index, documentControl);
        }

        protected virtual void RemoveDocumentCommandBindings()
        {
            List<CommandBinding> ToRemove = new List<CommandBinding>();
            foreach (CommandBinding Binding in CommandBindings)
            {
                if (Binding.Command is DocumentRoutedCommand)
                    ToRemove.Add(Binding);
            }
            foreach (CommandBinding Binding in ToRemove)
                CommandBindings.Remove(Binding);
        }

        protected virtual void AddDocumentCommandBinding(DocumentRoutedCommand newDocumentCommand)
        {
            CommandBinding NewDocumentBinding = new CommandBinding(newDocumentCommand, OnAddNewDocument, CanAddNewDocument);
            CommandBindings.Add(NewDocumentBinding);
        }

        protected virtual void RemoveDocumentKeyBindings()
        {
            foreach (InputBinding Binding in InputBindings)
            {
                KeyBinding AsKeyBinding;
                if ((AsKeyBinding = Binding as KeyBinding) != null)
                {
                    DocumentRoutedCommand AsDocumentCommand;
                    if ((AsDocumentCommand = AsKeyBinding.Command as DocumentRoutedCommand) != null)
                        if (AsDocumentCommand.DocumentType.IsPreferred)
                        {
                            InputBindings.Remove(AsKeyBinding);
                            break;
                        }
                }
            }
        }

        protected virtual void AddDocumentKeyBinding(DocumentRoutedCommand newDocumentCommand)
        {
            KeyBinding NewDocumentBinding = new KeyBinding(newDocumentCommand, new KeyGesture(Key.N, ModifierKeys.Control));
            InputBindings.Add(NewDocumentBinding);
        }

        protected virtual void CanAddNewDocument(object sender, CanExecuteRoutedEventArgs e)
        {
            Assert.ValidateReference(e);

            if (DocumentCreatedEventArgs.HasHandler)
                e.CanExecute = true;
        }

        protected virtual void OnAddNewDocument(object sender, ExecutedRoutedEventArgs e)
        {
            Assert.ValidateReference(e);

            if (DocumentCreatedEventArgs.HasHandler)
            {
                IFolderPath DestinationPath;

                IFolderPath AsFolderPath;
                if ((AsFolderPath = spcSolutionExplorer.GetEventSource(sender, e) as IFolderPath) != null)
                    DestinationPath = AsFolderPath;
                else
                    DestinationPath = RootPath;

                DocumentRoutedCommand Command = (DocumentRoutedCommand)e.Command;
                NotifyDocumentCreated(DestinationPath, Command.DocumentType, RootProperties);
            }
        }

        protected virtual void OnDocumentCreatedComplete(object sender, SolutionPresenterEventCompletedEventArgs e)
        {
            Assert.ValidateReference(e);

            DocumentCreatedEventContext EventContext = (DocumentCreatedEventContext)e.EventContext;
            IDocumentCreatedCompletionArgs CompletionArgs = (IDocumentCreatedCompletionArgs)e.CompletionArgs;
            IFolderPath DestinationFolderPath = EventContext.DestinationFolderPath;
            IItemPath NewItemPath = CompletionArgs.NewItemPath;
            IItemProperties NewItemProperties = CompletionArgs.NewItemProperties;

            spcSolutionExplorer.AddItem(DestinationFolderPath, NewItemPath, NewItemProperties);
        }
        #endregion

        #region Command: File / Create New Solution
        protected virtual void CanCreateNewSolution(object sender, CanExecuteRoutedEventArgs e)
        {
            Assert.ValidateReference(e);

            if (SolutionCreatedEventArgs.HasHandler)
                e.CanExecute = true;
        }

        protected virtual void OnCreateNewSolution(object sender, ExecutedRoutedEventArgs e)
        {
            IRootPath ClosedRootPath = RootPath;

            CommitInfo Info = CheckToSaveCurrentSolution();

            if (Info.Option == CommitOption.CommitAndContinue)
                NotifySolutionTreeCommitted(Info, SolutionOperation.Create, ClosedRootPath, null, null);

            else if (Info.Option == CommitOption.Continue)
                NotifySolutionClosed(SolutionOperation.Create, ClosedRootPath, null);
        }

        protected virtual void OnSolutionCreatedComplete(object sender, SolutionPresenterEventCompletedEventArgs e)
        {
            Assert.ValidateReference(e);

            ISolutionCreatedCompletionArgs CompletionArgs = (ISolutionCreatedCompletionArgs)e.CompletionArgs;
            IRootPath CreatedRootPath = CompletionArgs.CreatedRootPath;

            if (CreatedRootPath != null)
                NotifySolutionOpened(CreatedRootPath);
        }
        #endregion

        #region Command: File / Open Solution
        protected virtual void CanOpenSolution(object sender, CanExecuteRoutedEventArgs e)
        {
            Assert.ValidateReference(e);

            if (SolutionSelectedEventArgs.HasHandler && SolutionOpenedEventArgs.HasHandler)
                e.CanExecute = true;
        }

        protected virtual void OnOpenSolution(object sender, ExecutedRoutedEventArgs e)
        {
            NotifySolutionSelected();
        }

        protected virtual void OnSolutionSelectedComplete(object sender, SolutionPresenterEventCompletedEventArgs e)
        {
            Assert.ValidateReference(e);

            ISolutionSelectedCompletionArgs CompletionArgs = (ISolutionSelectedCompletionArgs)e.CompletionArgs;
            IRootPath SelectedRootPath = CompletionArgs.SelectedRootPath;
            IRootPath ClosedRootPath = RootPath;

            CommitInfo Info = CheckToSaveCurrentSolution();

            if (Info.Option == CommitOption.CommitAndContinue)
                NotifySolutionTreeCommitted(Info, SolutionOperation.Open, ClosedRootPath, SelectedRootPath, null);

            else if (Info.Option == CommitOption.Continue)
                if (ClosedRootPath != null)
                    NotifySolutionClosed(SolutionOperation.Open, ClosedRootPath, SelectedRootPath);
                else
                    NotifySolutionOpened(SelectedRootPath);
        }

        protected virtual void OnSolutionOpenedComplete(object sender, SolutionPresenterEventCompletedEventArgs e)
        {
            Assert.ValidateReference(e);

            SolutionOpenedEventContext EventContext = (SolutionOpenedEventContext)e.EventContext;
            ISolutionOpenedCompletionArgs CompletionArgs = (ISolutionOpenedCompletionArgs)e.CompletionArgs; 
            IRootPath OpenedRootPath = EventContext.OpenedRootPath;
            IRootProperties OpenedRootProperties = CompletionArgs.OpenedRootProperties;
            IComparer<ITreeNodePath> OpenedRootComparer = CompletionArgs.OpenedRootComparer;
            IList<IFolderPath> ExpandedFolderList = CompletionArgs.ExpandedFolderList;
            object Context = CompletionArgs.Context;

            SetValue(TreeNodeComparerPropertyKey, OpenedRootComparer);
            LoadTree(OpenedRootPath, OpenedRootProperties, OpenedRootComparer, ExpandedFolderList, Context);
        }
        #endregion

        #region Command: File / Open Existing Document
        protected virtual void CanOpenExistingDocument(object sender, CanExecuteRoutedEventArgs e)
        {
            Assert.ValidateReference(e);

            if (DocumentSelectedEventArgs.HasHandler)
                e.CanExecute = true;
        }

        protected virtual void OnOpenExistingDocument(object sender, ExecutedRoutedEventArgs e)
        {
            NotifyDocumentSelected();
        }

        protected virtual void OnDocumentSelectedComplete(object sender, SolutionPresenterEventCompletedEventArgs e)
        {
            Assert.ValidateReference(e);

            IDocumentSelectedCompletionArgs CompletionArgs = (IDocumentSelectedCompletionArgs)e.CompletionArgs;
            IList<IDocumentPath> DocumentPathList = CompletionArgs.DocumentPathList;
            OpenNextDocument(DocumentPathList);
        }
        #endregion

        #region Command: File / Close Document
        protected virtual void CanCloseDocument(object sender, CanExecuteRoutedEventArgs e)
        {
            Assert.ValidateReference(e);

            if (ActiveDocument != null)
                e.CanExecute = true;
        }

        protected virtual void OnCloseDocument(object sender, ExecutedRoutedEventArgs e)
        {
            if (ActiveDocument != null)
                if (ActiveDocument.IsDirty)
                {
                    CommitOption Option = IsSingleDocumentSaveConfirmed(ActiveDocument);

                    if (Option == CommitOption.CommitAndContinue)
                        NotifyDocumentSaved(DocumentOperation.Close, ActiveDocument, null);

                    else if (Option == CommitOption.Continue)
                        NotifyDocumentClosed(DocumentOperation.Close, ActiveDocument);
                }
                else
                    NotifyDocumentClosed(DocumentOperation.Close, ActiveDocument);
        }

        protected virtual void OnDocumentClosedComplete(object sender, SolutionPresenterEventCompletedEventArgs e)
        {
            Assert.ValidateReference(e);

            DocumentClosedEventContext EventContext = (DocumentClosedEventContext)e.EventContext;
            DocumentOperation DocumentOperation = EventContext.DocumentOperation;
            IList<IDocument> ClosedDocumentList = EventContext.ClosedDocumentList;
            IReadOnlyDictionary<ITreeNodePath, IPathConnection> ClosedTree = EventContext.ClosedTree;
            bool IsUndoRedo = EventContext.IsUndoRedo;
            object ClientInfo = EventContext.ClientInfo;

            foreach (IDocument ClosedDocument in ClosedDocumentList)
                OpenDocuments.Remove(ClosedDocument);

            switch (DocumentOperation)
            {
                case DocumentOperation.Close:
                    break;

                case DocumentOperation.Remove:
                    NotifyDocumentRemoved(RootPath, ClosedTree, IsUndoRedo, ClientInfo);
                    break;
            }
        }
        #endregion

        #region Command: File / Close Solution
        protected virtual void CanCloseSolution(object sender, CanExecuteRoutedEventArgs e)
        {
            Assert.ValidateReference(e);

            if (SolutionClosedEventArgs.HasHandler)
                e.CanExecute = true;
        }

        protected virtual void OnCloseSolution(object sender, ExecutedRoutedEventArgs e)
        {
            IRootPath ClosedRootPath = RootPath;

            CommitInfo Info = CheckToSaveCurrentSolution();

            if (Info.Option == CommitOption.CommitAndContinue)
                NotifySolutionTreeCommitted(Info, SolutionOperation.Close, ClosedRootPath, null, null);

            else if (Info.Option == CommitOption.Continue)
                NotifySolutionClosed(SolutionOperation.Close, ClosedRootPath, null);
        }

        protected virtual void OnSolutionClosedComplete(object sender, SolutionPresenterEventCompletedEventArgs e)
        {
            Assert.ValidateReference(e);

            SolutionClosedEventContext EventContext = (SolutionClosedEventContext)e.EventContext;
            SolutionOperation SolutionOperation = EventContext.SolutionOperation;
            IRootPath ClosedRootPath = EventContext.ClosedRootPath;
            IRootPath NewRootPath = EventContext.NewRootPath;

            IReadOnlyCollection<ITreeNodePath> DeletedTree;
            if (SolutionOperation == SolutionOperation.Delete)
                DeletedTree = spcSolutionExplorer.GetTree(ClosedRootPath);
            else
                DeletedTree = null;

            SetValue(RootPathPropertyKey, null);
            SetValue(RootPropertiesPropertyKey, null);
            spcSolutionExplorer.ResetRoot();

            switch (SolutionOperation)
            {
                case SolutionOperation.Create:
                    NotifySolutionCreated();
                    break;

                case SolutionOperation.Open:
                    NotifySolutionOpened(NewRootPath);
                    break;

                case SolutionOperation.Close:
                    break;

                case SolutionOperation.Delete:
                    NotifySolutionDeleted(ClosedRootPath, DeletedTree);
                    break;
            }
        }
        #endregion

        #region Command: File / Save Document
        protected virtual void CanSaveDocument(object sender, CanExecuteRoutedEventArgs e)
        {
            Assert.ValidateReference(e);

            if (ActiveDocument != null)
                e.CanExecute = true;
        }

        protected virtual void OnSaveDocument(object sender, ExecutedRoutedEventArgs e)
        {
            if (ActiveDocument != null)
                NotifyDocumentSaved(DocumentOperation.Save, ActiveDocument, null);
        }

        protected virtual void OnDocumentSavedComplete(object sender, SolutionPresenterEventCompletedEventArgs e)
        {
            Assert.ValidateReference(e);

            DocumentSavedEventContext EventContext = (DocumentSavedEventContext)e.EventContext;
            DocumentOperation DocumentOperation = EventContext.DocumentOperation;
            IDocument SavedDocument = EventContext.SavedDocument;
            string FileName = EventContext.FileName;

            SavedDocument.ClearIsDirty();

            switch (DocumentOperation)
            {
                case DocumentOperation.Save:
                    break;

                case DocumentOperation.Close:
                    NotifyDocumentClosed(DocumentOperation, SavedDocument);
                    break;

                case DocumentOperation.Export:
                    NotifyDocumentExported(DocumentOperation, SavedDocument, FileName);
                    break;
            }
        }
        #endregion

        #region Command: File / Save All
        protected virtual void CanSaveAll(object sender, CanExecuteRoutedEventArgs e)
        {
            Assert.ValidateReference(e);

            CommitInfo Info = GetDirtyObjects();

            if (Info.DirtyItemList.Count > 0 || Info.DirtyPropertiesList.Count > 0 || Info.DirtyDocumentList.Count > 0)
                e.CanExecute = true;
        }

        protected virtual void OnSaveAll(object sender, ExecutedRoutedEventArgs e)
        {
            CommitInfo Info = GetDirtyObjects();

            if (Info.DirtyItemList.Count > 0 || Info.DirtyPropertiesList.Count > 0 || Info.DirtyDocumentList.Count > 0)
                NotifySolutionTreeCommitted(Info, SolutionOperation.Save, null, null, null);
        }
        #endregion

        #region Command: File / Import
        protected virtual void CanImport(object sender, CanExecuteRoutedEventArgs e)
        {
            Assert.ValidateReference(e);

            if (DocumentImportDescriptors != null && DocumentImportDescriptors.Count > 0)
                e.CanExecute = true;
        }

        protected virtual void OnImport(object sender, ExecutedRoutedEventArgs e)
        {
            if (DocumentImportDescriptors == null || DocumentImportDescriptors.Count == 0)
                return;

            DocumentTypeFilter Filter = GetFilters();

            string CompleteFilter = Filter.FilterValue;
            if (CompleteFilter.Length > 0)
                CompleteFilter += "|";
            CompleteFilter += CompleteSolutionExtensionFilter;
            if (CompleteFilter.Length > 0)
                CompleteFilter += "|";
            CompleteFilter += SolutionPresenterInternal.Properties.Resources.AllFiles + " (*.*)|*.*";

            OpenFileDialog Dlg = new OpenFileDialog();
            Dlg.DefaultExt = "*." + Filter.DefaultExtension;
            Dlg.Filter = CompleteFilter;
            Dlg.Multiselect = true;

            if (ImportFolder != null && Directory.Exists(ImportFolder))
                Dlg.InitialDirectory = ImportFolder;
            else
                Dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            bool? Result = Dlg.ShowDialog();
            if (Result.HasValue && Result.Value && Dlg.FileNames.Length > 0)
            {
                string LastFileName = Dlg.FileName;
                if (LastFileName != null)
                    ImportFolder = Path.GetDirectoryName(LastFileName);

                Dictionary<object, IDocumentType> ImportedDocumentTable = new Dictionary<object, IDocumentType>();
                foreach (string FileName in Dlg.FileNames)
                {
                    string FileExtension = Path.GetExtension(FileName);
                    foreach (IDocumentImportDescriptor Descriptor in DocumentImportDescriptors)
                        if ("." + Descriptor.FileExtension == FileExtension)
                        {
                            ImportedContentDescriptor ContentDescriptor = Descriptor.Import(FileName);
                            if (ContentDescriptor != null)
                                ImportedDocumentTable.Add(ContentDescriptor.ImportedContent, ContentDescriptor .DocumentType);

                            break;
                        }
                }

                if (ImportedDocumentTable.Count > 0)
                    NotifyImportNewItemsRequested(ImportedDocumentTable, new List<IDocumentPath>());
            }
        }

        private DocumentTypeFilter GetFilters()
        {
            string DefaultExtension = null;
            Dictionary<string, string> FileExtensionTable = new Dictionary<string, string>();
            foreach (IDocumentImportDescriptor Descriptor in DocumentImportDescriptors)
            {
                if (!FileExtensionTable.ContainsKey(Descriptor.FileExtension))
                    FileExtensionTable.Add(Descriptor.FileExtension, Descriptor.FriendlyImportName);

                if (DefaultExtension == null || Descriptor.IsDefault)
                    DefaultExtension = Descriptor.FileExtension;
            }

            string FilterString = "";
            foreach (KeyValuePair<string, string> Entry in FileExtensionTable)
            {
                if (FilterString.Length > 0)
                    FilterString += "|";

                FilterString += Entry.Value + " (*." + Entry.Key + ")|*." + Entry.Key;
            }

            return new DocumentTypeFilter(FilterString, DefaultExtension);
        }

        protected virtual void OnImportNewItemsRequestedComplete(object sender, SolutionPresenterEventCompletedEventArgs e)
        {
            Assert.ValidateReference(e);

            ImportNewItemsRequestedEventContext EventContext = (ImportNewItemsRequestedEventContext)e.EventContext;
            IImportNewItemsRequestedCompletionArgs CompletionArgs = (IImportNewItemsRequestedCompletionArgs)e.CompletionArgs;
            Dictionary<object, IDocumentType> ImportedDocumentTable = EventContext.ImportedDocumentTable;
            IList<IDocumentPath> DocumentPathList = EventContext.DocumentPathList;
            IReadOnlyDictionary<object, IDocumentPath> OpenedDocumentTable = CompletionArgs.OpenedDocumentTable;

            List<IDocumentPath> OpenedDocumentPathList = new List<IDocumentPath>();
            foreach (KeyValuePair<object, IDocumentPath> OpenedDocumentEntry in OpenedDocumentTable)
            {
                foreach (KeyValuePair<object, IDocumentType> ImportedDocumentEntry in ImportedDocumentTable)
                    if (ImportedDocumentEntry.Key == OpenedDocumentEntry.Key)
                    {
                        ImportedDocumentTable.Remove(ImportedDocumentEntry.Key);

                        IDocumentPath OpenedDocumentPath = OpenedDocumentEntry.Value;
                        if (OpenedDocumentPath != null)
                            OpenedDocumentPathList.Add(OpenedDocumentPath);
                        break;
                    }
            }

            if (OpenedDocumentPathList.Count > 0)
            {
                foreach (IDocumentPath Path in OpenedDocumentPathList)
                    DocumentPathList.Add(Path);

                if (ImportedDocumentTable.Count > 0)
                    NotifyImportNewItemsRequested(ImportedDocumentTable, DocumentPathList);

                else
                    NotifyDocumentOpened(DocumentOperation.Open, null, DocumentPathList, null, null);
            }
        }
        #endregion

        #region Command: File / Import Solution
        protected virtual void CanImportSolution(object sender, CanExecuteRoutedEventArgs e)
        {
            Assert.ValidateReference(e);

            if (DocumentImportDescriptors != null && DocumentImportDescriptors.Count > 0)
                e.CanExecute = true;
        }

        protected virtual void OnImportSolution(object sender, ExecutedRoutedEventArgs e)
        {
            if (DocumentImportDescriptors == null || DocumentImportDescriptors.Count == 0)
                return;

            string CompleteFilter = "";
            if (CompleteFilter.Length > 0)
                CompleteFilter += "|";
            CompleteFilter += CompleteSolutionExtensionFilter;
            if (CompleteFilter.Length > 0)
                CompleteFilter += "|";
            CompleteFilter += SolutionPresenterInternal.Properties.Resources.AllFiles + " (*.*)|*.*";

            OpenFileDialog Dlg = new OpenFileDialog();
            Dlg.DefaultExt = "*." + SolutionExtension;
            Dlg.Filter = CompleteFilter;
            Dlg.Multiselect = false;

            if (ImportFolder != null && Directory.Exists(ImportFolder))
                Dlg.InitialDirectory = ImportFolder;
            else
                Dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            bool? Result = Dlg.ShowDialog();
            if (Result.HasValue && Result.Value && Dlg.FileName != null && Dlg.FileName.Length > 0)
            {
                ImportFolder = Path.GetDirectoryName(Dlg.FileName);

                SolutionPackage Package = new SolutionPackage();
                spcSolutionExplorer.ReadImportedSolutionPackage(Package, Dlg.FileName);

                if (!Package.CreateSolution)
                {
                    string SolutionName = Path.GetFileNameWithoutExtension(Dlg.FileName);
                    string QuestionFormat = SolutionPresenterInternal.Properties.Resources.ReplaceSolution;
                    string Question = String.Format(CultureInfo.CurrentCulture, QuestionFormat, SolutionName);

                    if (MessageBox.Show(Question, Owner.Title, MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                    {
                        Package.InitCreationTime();
                        spcSolutionExplorer.ReadImportedSolutionPackage(Package, Dlg.FileName);
                    }
                }
            }
        }

        protected virtual void OnImportSolutionRequestedComplete(object sender, SolutionPresenterEventCompletedEventArgs e)
        {
        }
        #endregion

        #region Command: File / Export Document
        protected virtual void CanExportDocument(object sender, CanExecuteRoutedEventArgs e)
        {
            Assert.ValidateReference(e);

            if (DocumentImportDescriptors != null && DocumentImportDescriptors.Count > 0 && ActiveDocument != null)
                e.CanExecute = true;
        }

        protected virtual void OnExportDocument(object sender, ExecutedRoutedEventArgs e)
        {
            if (DocumentImportDescriptors != null && DocumentImportDescriptors.Count > 0 && ActiveDocument != null)
            {
                DocumentTypeFilter Filter = GetFilters();

                string CompleteFilter = Filter.FilterValue;
                if (CompleteFilter.Length > 0)
                    CompleteFilter += "|";
                CompleteFilter += SolutionPresenterInternal.Properties.Resources.AllFiles + " (*.*)|*.*";

                SaveFileDialog Dlg = new SaveFileDialog();
                Dlg.DefaultExt = "*." + Filter.DefaultExtension;
                Dlg.Filter = CompleteFilter;
                Dlg.FileName = ActiveDocument.Path.HeaderName;

                if (ExportFolder != null && Directory.Exists(ExportFolder))
                    Dlg.InitialDirectory = ExportFolder;
                else
                    Dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                bool? Result = Dlg.ShowDialog();
                if (Result.HasValue && Result.Value)
                {
                    CommitOption Option;

                    if (ActiveDocument.IsDirty)
                        Option = IsSingleDocumentSaveConfirmed(ActiveDocument);
                    else
                        Option = CommitOption.Continue;

                    if (Option == CommitOption.CommitAndContinue)
                    {
                        ExportFolder = Path.GetDirectoryName(Dlg.FileName);
                        NotifyDocumentSaved(DocumentOperation.Export, ActiveDocument, Dlg.FileName);
                    }

                    else if (Option == CommitOption.Continue)
                    {
                        ExportFolder = Path.GetDirectoryName(Dlg.FileName);
                        NotifyDocumentExported(DocumentOperation.Export, ActiveDocument, Dlg.FileName);
                    }
                }
            }
        }

        protected virtual void OnDocumentExportedComplete(object sender, SolutionPresenterEventCompletedEventArgs e)
        {
        }
        #endregion

        #region Command: File / Export All
        protected virtual void CanExportAll(object sender, CanExecuteRoutedEventArgs e)
        {
            Assert.ValidateReference(e);

            if (DocumentImportDescriptors != null && DocumentImportDescriptors.Count > 0 && OpenDocuments.Count > 0)
                e.CanExecute = true;
        }

        protected virtual void OnExportAll(object sender, ExecutedRoutedEventArgs e)
        {
            if (DocumentImportDescriptors != null && DocumentImportDescriptors.Count > 0 && OpenDocuments.Count > 0)
            {
                using (System.Windows.Forms.FolderBrowserDialog Dlg = new System.Windows.Forms.FolderBrowserDialog())
                {
                    if (Directory.Exists(ExportFolder))
                        Dlg.SelectedPath = ExportFolder;
                    else
                        Dlg.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                    System.Windows.Forms.DialogResult Result = Dlg.ShowDialog();
                    if (Result == System.Windows.Forms.DialogResult.OK)
                    {
                        CommitInfo Info = CheckToSaveOpenDocuments();

                        if (Info.Option == CommitOption.CommitAndContinue)
                        {
                            ExportFolder = Dlg.SelectedPath;
                            NotifySolutionTreeCommitted(Info, SolutionOperation.ExportDocument, null, null, ExportFolder);
                        }

                        else if (Info.Option == CommitOption.Continue)
                        {
                            ExportFolder = Dlg.SelectedPath;
                            NotifyDocumentExported(DocumentOperation.Export, OpenDocuments, true, Dlg.SelectedPath);
                        }
                    }
                }
            }
        }
        #endregion

        #region Command: File / Export Solution
        protected virtual void CanExportSolution(object sender, CanExecuteRoutedEventArgs e)
        {
            Assert.ValidateReference(e);

            if (SolutionExportedEventArgs.HasHandler && SolutionExtension != null && SolutionExtensionFilter != null)
                e.CanExecute = true;
        }

        protected virtual void OnExportSolution(object sender, ExecutedRoutedEventArgs e)
        {
            IRootPath ExportedRootPath = RootPath;

            CommitInfo Info = CheckToSaveCurrentSolution();

            if (Info.Option == CommitOption.CommitAndContinue)
                NotifySolutionTreeCommitted(Info, SolutionOperation.ExportSolution, ExportedRootPath, null, null);

            else if (Info.Option == CommitOption.Continue)
                ExportSolution(ExportedRootPath);
        }

        private void ExportSolution(IRootPath ExportedRootPath)
        {
            string DestinationPath = GetSolutionExportFileName(ExportedRootPath.FriendlyName);
            if (DestinationPath == null)
                return;

            NotifySolutionExported(ExportedRootPath, DestinationPath);
        }

        private string CompleteSolutionExtensionFilter
        {
            get { return SolutionExtensionFilter + " (*." + SolutionExtension + ")|*." + SolutionExtension; }
        }

        private string GetSolutionExportFileName(string solutionName)
        {
            string CompleteFilter = CompleteSolutionExtensionFilter;
            if (CompleteFilter.Length > 0)
                CompleteFilter += "|";
            CompleteFilter += SolutionPresenterInternal.Properties.Resources.AllFiles + " (*.*)|*.*";

            SaveFileDialog Dlg = new SaveFileDialog();
            Dlg.DefaultExt = "*." + SolutionExtension;
            Dlg.Filter = CompleteFilter;
            Dlg.FileName = solutionName + "." + SolutionExtension;

            if (ExportFolder != null && Directory.Exists(ExportFolder))
                Dlg.InitialDirectory = ExportFolder;
            else
                Dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            bool? Result = Dlg.ShowDialog();
            if (Result.HasValue && Result.Value && Dlg.FileName.Length > 0)
                return Dlg.FileName;
            else
                return null;
        }

        protected virtual void OnSolutionExportedComplete(object sender, SolutionPresenterEventCompletedEventArgs e)
        {
            Assert.ValidateReference(e);

            SolutionExportedEventContext EventContext = (SolutionExportedEventContext)e.EventContext;
            string DestinationPath = EventContext.DestinationPath;
            SolutionExportedCompletionArgs Args = (SolutionExportedCompletionArgs)e.CompletionArgs;
            Dictionary<IDocumentPath, byte[]> ContentTable = Args.ContentTable;

            spcSolutionExplorer.CreateExportedSolutionPackage(DestinationPath, ContentTable);
        }
        #endregion

        #region Command: File / Exit
        protected virtual void CanExit(object sender, CanExecuteRoutedEventArgs e)
        {
            Assert.ValidateReference(e);

            e.CanExecute = true;
        }

        protected virtual void OnExit(object sender, ExecutedRoutedEventArgs e)
        {
            CommitInfo Info = CheckToSaveCurrentSolution();

            if (Info.Option == CommitOption.CommitAndContinue)
                NotifySolutionTreeCommitted(Info, SolutionOperation.Exit, null, null, null);

            else if (Info.Option == CommitOption.Continue)
            {
                spcSolutionExplorer.ClearDirtyItemsAndProperties();
                ClearDirtyDocuments();

                NotifyExitRequested();
            }
        }
        #endregion

        #region Command: Undo
        protected virtual void CanUndo(object sender, CanExecuteRoutedEventArgs e)
        {
            Assert.ValidateReference(e);

            IDocument Document = dockManager.ActiveContent as IDocument;
            if (Document != null)
            {
                if (Document.CanUndo())
                    e.CanExecute = true;
            }

            else if (dockManager.ActiveContent == spcSolutionExplorer)
            {
                if (spcSolutionExplorer.CanUndo)
                    e.CanExecute = true;
            }
        }

        protected virtual void OnUndo(object sender, ExecutedRoutedEventArgs e)
        {
            IDocument Document = dockManager.ActiveContent as IDocument;
            if (Document != null)
                Document.OnUndo();

            else if (dockManager.ActiveContent == spcSolutionExplorer)
                spcSolutionExplorer.Undo();
        }
        #endregion

        #region Command: Redo
        protected virtual void CanRedo(object sender, CanExecuteRoutedEventArgs e)
        {
            Assert.ValidateReference(e);

            IDocument Document = dockManager.ActiveContent as IDocument;
            if (Document != null)
            {
                if (Document.CanRedo())
                    e.CanExecute = true;
            }

            else if (dockManager.ActiveContent == spcSolutionExplorer)
            {
                if (spcSolutionExplorer.CanRedo)
                    e.CanExecute = true;
            }
        }

        protected virtual void OnRedo(object sender, ExecutedRoutedEventArgs e)
        {
            IDocument Document = dockManager.ActiveContent as IDocument;
            if (Document != null)
                Document.OnRedo();

            else if (dockManager.ActiveContent == spcSolutionExplorer)
                spcSolutionExplorer.Redo();
        }
        #endregion

        #region Command: Select All
        protected virtual void CanSelectAll(object sender, CanExecuteRoutedEventArgs e)
        {
            Assert.ValidateReference(e);

            IDocument Document = dockManager.ActiveContent as IDocument;
            if (Document != null)
            {
                if (Document.CanSelectAll())
                    e.CanExecute = true;
            }

            else if (dockManager.ActiveContent == spcSolutionExplorer)
                e.CanExecute = true;
        }

        protected virtual void OnSelectAll(object sender, ExecutedRoutedEventArgs e)
        {
            IDocument Document = dockManager.ActiveContent as IDocument;
            if (Document != null)
                Document.OnSelectAll();

            else if (dockManager.ActiveContent == spcSolutionExplorer)
                spcSolutionExplorer.SelectAll();
        }
        #endregion

        #region Command: Edit / Change Options
        protected virtual void CanChangeOptions(object sender, CanExecuteRoutedEventArgs e)
        {
            Assert.ValidateReference(e);

            e.CanExecute = true;
        }

        protected virtual void OnChangeOptions(object sender, ExecutedRoutedEventArgs e)
        {
            OptionsWindow Dlg = new OptionsWindow(OptionPageIndex, ThemeOption, SaveBeforeCompiling, OptionPages);
            Dlg.Owner = Owner;
            Dlg.ShowDialog();

            if (Dlg.DialogResult.HasValue && Dlg.DialogResult.Value)
            {
                UpdatePresenterOptions(Dlg);
                NotifyOptionsChanged();
            }

            OptionPageIndex = Dlg.OptionPageIndex;
        }

        protected virtual void UpdatePresenterOptions(OptionsWindow optionDialog)
        {
            Assert.ValidateReference(optionDialog);

            if (ThemeOption != optionDialog.Theme)
            {
                ThemeOption = optionDialog.Theme;
                //UpdateTheme();
            }

            SaveBeforeCompiling = optionDialog.SaveBeforeCompiling;
        }

        protected int OptionPageIndex { get; private set; }
        #endregion

        #region Command: Project / Build Solution
        protected virtual void CanBuildSolution(object sender, CanExecuteRoutedEventArgs e)
        {
            Assert.ValidateReference(e);

            if (RootPath != null && BuildSolutionRequestedEventArgs.HasHandler)
                e.CanExecute = true;
        }

        protected virtual void OnBuildSolution(object sender, ExecutedRoutedEventArgs e)
        {
            if (RootPath != null && BuildSolutionRequestedEventArgs.HasHandler)
            {
                CommitInfo Info = CheckToSaveCurrentSolution();

                if (Info.Option == CommitOption.CommitAndContinue)
                    NotifySolutionTreeCommitted(Info, SolutionOperation.Build, null, null, null);

                else if (Info.Option == CommitOption.Continue)
                    NotifyBuildSolutionRequested();
            }
        }

        protected virtual void OnBuildSolutionRequestedComplete(object sender, SolutionPresenterEventCompletedEventArgs e)
        {
            Assert.ValidateReference(e);

            IBuildSolutionRequestedCompletionArgs CompletionArgs = (IBuildSolutionRequestedCompletionArgs)e.CompletionArgs;
            IReadOnlyList<ICompilationError> ErrorList = CompletionArgs.ErrorList;

            CompilationErrorList.Clear();
            foreach (ICompilationError Error in ErrorList)
                CompilationErrorList.Add(Error);
        }
        #endregion

        #region Command: Project / Change Properties
        protected virtual void CanChangeProperties(object sender, CanExecuteRoutedEventArgs e)
        {
            Assert.ValidateReference(e);

            if (RootPath != null && RootPropertiesRequestedEventArgs.HasHandler)
                e.CanExecute = true;
        }

        protected virtual void OnChangeProperties(object sender, ExecutedRoutedEventArgs e)
        {
            if (RootPath != null && RootPropertiesRequestedEventArgs.HasHandler)
                NotifyRootPropertiesRequested(RootProperties);
        }

        protected virtual void OnRootPropertiesRequestedComplete(object sender, SolutionPresenterEventCompletedEventArgs e)
        {
        }
        #endregion

        #region Command: Window / Show Solution Explorer Tool
        protected virtual void CanShowSolutionExplorerTool(object sender, CanExecuteRoutedEventArgs e)
        {
            Assert.ValidateReference(e);

            e.CanExecute = true;
        }

        protected virtual void OnShowSolutionExplorerTool(object sender, ExecutedRoutedEventArgs e)
        {
            ShowTool("toolSolutionExplorer", ToolOperation.Show);
        }
        #endregion

        #region Command: Window / Show Compiler Output Tool
        protected virtual void CanShowCompilerOutputTool(object sender, CanExecuteRoutedEventArgs e)
        {
            Assert.ValidateReference(e);

            e.CanExecute = true;
        }

        protected virtual void OnShowCompilerOutputTool(object sender, ExecutedRoutedEventArgs e)
        {
            ShowTool("toolCompilerOutput", ToolOperation.Show);
        }
        #endregion

        #region Command: Window / Show Properties Tool
        protected virtual void CanShowPropertiesTool(object sender, CanExecuteRoutedEventArgs e)
        {
            Assert.ValidateReference(e);

            e.CanExecute = true;
        }

        protected virtual void OnShowPropertiesTool(object sender, ExecutedRoutedEventArgs e)
        {
            ShowTool("toolProperties", ToolOperation.Toggle);
        }
        #endregion

        #region Command: Window / Reset Layout
        protected virtual void CanResetLayout(object sender, CanExecuteRoutedEventArgs e)
        {
            Assert.ValidateReference(e);

            e.CanExecute = true;
        }

        protected virtual void OnResetLayout(object sender, ExecutedRoutedEventArgs e)
        {
            ResetDockManagerState();
        }
        #endregion

        #region Command: Window / Split Window
        protected virtual void CanSplitWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            Assert.ValidateReference(e);

            if (ActiveDocument != null)
            {
                SplitView Ctrl = GetActiveControl();
                if (Ctrl != null && !Ctrl.IsSplitRemovable)
                    e.CanExecute = true;
            }
        }

        protected virtual void OnSplitWindow(object sender, ExecutedRoutedEventArgs e)
        {
            if (ActiveDocument != null)
            {
                SplitView Ctrl = GetActiveControl();
                if (Ctrl != null)
                    Ctrl.Split();
            }
        }
        #endregion

        #region Command: Window / Remove Window Split
        protected virtual void CanRemoveWindowSplit(object sender, CanExecuteRoutedEventArgs e)
        {
            Assert.ValidateReference(e);

            if (ActiveDocument != null)
            {
                SplitView Ctrl = GetActiveControl();
                if (Ctrl != null && Ctrl.IsSplitRemovable)
                    e.CanExecute = true;
            }
        }

        protected virtual void OnRemoveWindowSplit(object sender, ExecutedRoutedEventArgs e)
        {
            if (ActiveDocument != null)
            {
                SplitView Ctrl = GetActiveControl();
                if (Ctrl != null)
                    Ctrl.RemoveSplit();
            }
        }
        #endregion

        #region Command: Window / List Windows
        protected virtual void CanListWindows(object sender, CanExecuteRoutedEventArgs e)
        {
            Assert.ValidateReference(e);

            e.CanExecute = true;
        }

        protected virtual void OnListWindows(object sender, ExecutedRoutedEventArgs e)
        {
            DocumentsWindow Dlg = new DocumentsWindow(Documents);
            Dlg.Owner = Owner;
            Dlg.DocumentActivated += OnDocumentActivated;
            Dlg.DocumentSaved += OnDocumentSaved;
            Dlg.DocumentClosed += OnDocumentClosed;
            Dlg.ShowDialog();
        }

        protected virtual void OnDocumentActivated(object sender, DocumentWindowEventArgs e)
        {
            Assert.ValidateReference(e);

            UserActivateDocument(e.Document);
        }

        protected virtual void OnDocumentSaved(object sender, DocumentWindowEventArgs e)
        {
            Assert.ValidateReference(e);

            NotifyDocumentSaved(DocumentOperation.Save, e.Document, null);
        }

        protected virtual void OnDocumentClosed(object sender, DocumentWindowEventArgs e)
        {
            Assert.ValidateReference(e);

            if (e.Document.IsDirty)
            {
                List<IDocument> DirtyDocumentList = new List<IDocument>();
                DirtyDocumentList.Add(e.Document);

                CommitInfo Info = new CommitInfo(CommitOption.Stop, new List<ITreeNodePath>(), new List<ITreeNodePath>(), DirtyDocumentList);
                CommitOption Option = IsMultipleSaveConfirmed(Info);

                if (Option == CommitOption.CommitAndContinue)
                    NotifyDocumentSaved(DocumentOperation.Close, e.Document, null);

                else if (Option == CommitOption.Continue)
                    NotifyDocumentClosed(DocumentOperation.Close, e.Document);
            }
            else
                NotifyDocumentClosed(DocumentOperation.Close, e.Document);
        }
        #endregion

        #region Command: Window / Activate Next Window
        protected virtual void CanActivateNextWindow(object sender, CanExecuteRoutedEventArgs e)
        {
        }

        protected virtual void OnActivateNextWindow(object sender, ExecutedRoutedEventArgs e)
        {
            ChangeActiveDocument(+1);
        }
        #endregion

        #region Command: Window / Activate Previous Window
        protected virtual void CanActivatePreviousWindow(object sender, CanExecuteRoutedEventArgs e)
        {
        }

        protected virtual void OnActivatePreviousWindow(object sender, ExecutedRoutedEventArgs e)
        {
            ChangeActiveDocument(-1);
        }
        #endregion

        #region Command: Help / Show About
        protected virtual void CanShowAbout(object sender, CanExecuteRoutedEventArgs e)
        {
            Assert.ValidateReference(e);

            e.CanExecute = true;
        }

        protected virtual void OnShowAbout(object sender, ExecutedRoutedEventArgs e)
        {
            NotifyShowAboutRequested();
        }
        #endregion

        #region Command: Context / Add Existing Item
        protected virtual void CanAddExistingItem(object sender, CanExecuteRoutedEventArgs e)
        {
            Assert.ValidateReference(e);

            if (AddNewItemsRequestedEventArgs.HasHandler)
            {
                IFolderPath DestinationFolderPath = spcSolutionExplorer.SelectedFolder;
                if (DestinationFolderPath != null)
                    e.CanExecute = true;
            }
        }

        protected virtual void OnAddExistingItem(object sender, ExecutedRoutedEventArgs e)
        {
            IFolderPath DestinationFolderPath = spcSolutionExplorer.SelectedFolder;
            if (DestinationFolderPath != null)
                NotifyAddNewItemsRequested(DestinationFolderPath);
        }

        protected virtual void OnAddNewItemsRequestedComplete(object sender, SolutionPresenterEventCompletedEventArgs e)
        {
            Assert.ValidateReference(e);

            IAddNewItemsRequestedEventContext EventContext = (IAddNewItemsRequestedEventContext)e.EventContext;
            IAddNewItemsRequestedCompletionArgs CompletionArgs = (IAddNewItemsRequestedCompletionArgs)e.CompletionArgs;
            IFolderPath DestinationFolderPath = EventContext.DestinationFolderPath;
            IList<IDocumentPath> DocumentPathList = CompletionArgs.DocumentPathList;

            AddNextDocument(DocumentOperation.Add, DestinationFolderPath, DocumentPathList, RootProperties);
        }

        protected virtual void AddNextDocument(DocumentOperation documentOperation, IFolderPath destinationFolderPath, IList<IDocumentPath> documentPathList, IRootProperties rootProperties)
        {
            Assert.ValidateReference(documentPathList);

            if (documentPathList.Count > 0)
                NotifyDocumentAdded(documentOperation, destinationFolderPath, documentPathList, rootProperties);
        }

        protected virtual void OnDocumentAddedComplete(object sender, SolutionPresenterEventCompletedEventArgs e)
        {
            Assert.ValidateReference(e);

            DocumentAddedEventContext EventContext = (DocumentAddedEventContext)e.EventContext;
            IDocumentAddedCompletionArgs CompletionArgs = (IDocumentAddedCompletionArgs)e.CompletionArgs;
            DocumentOperation DocumentOperation = EventContext.DocumentOperation;
            IFolderPath DestinationFolderPath = EventContext.DestinationFolderPath;
            IList<IDocumentPath> DocumentPathList = EventContext.DocumentPathList;
            IReadOnlyDictionary<IDocumentPath, IItemPath> AddedItemTable = CompletionArgs.AddedItemTable;
            IReadOnlyDictionary<IDocumentPath, IItemProperties> AddedPropertiesTable = CompletionArgs.AddedPropertiesTable;

            List<IDocumentPath> OpenedDocumentPathList = new List<IDocumentPath>();
            foreach (IDocumentPath Path in DocumentPathList)
                if (AddedItemTable.ContainsKey(Path) && AddedPropertiesTable.ContainsKey(Path))
                {
                    IItemPath NewItemPath = AddedItemTable[Path];
                    IItemProperties NewItemProperties = AddedPropertiesTable[Path];

                    spcSolutionExplorer.AddItem(DestinationFolderPath, NewItemPath, NewItemProperties);
                    OpenedDocumentPathList.Add(Path);
                }

            foreach (IDocumentPath Path in OpenedDocumentPathList)
                DocumentPathList.Remove(Path);

            switch (DocumentOperation)
            {
                case DocumentOperation.Add:
                    if (OpenedDocumentPathList.Count > 0)
                        NotifyDocumentOpened(DocumentOperation, DestinationFolderPath, OpenedDocumentPathList, DocumentPathList, null);
                    break;

                case DocumentOperation.Paste:
                    AddNextDocument(DocumentOperation, DestinationFolderPath, DocumentPathList, RootProperties);
                    break;
            }
        }
        #endregion

        #region Command: Context / Add New Folder
        protected virtual void CanAddNewFolder(object sender, CanExecuteRoutedEventArgs e)
        {
            Assert.ValidateReference(e);

            if (dockManager.ActiveContent == spcSolutionExplorer)
                if (spcSolutionExplorer.ValidEditOperations.AddFolder)
                    if (FolderCreatedEventArgs.HasHandler)
                        e.CanExecute = true;
        }

        protected virtual void OnAddNewFolder(object sender, ExecutedRoutedEventArgs e)
        {
            if (dockManager.ActiveContent != spcSolutionExplorer)
                return;

            if (!spcSolutionExplorer.ValidEditOperations.AddFolder)
                return;

            if (!FolderCreatedEventArgs.HasHandler)
                return;

            IFolderPath DestinationPath;

            IFolderPath AsFolderPath;
            if ((AsFolderPath = spcSolutionExplorer.GetEventSource(sender, e) as IFolderPath) != null)
                DestinationPath = AsFolderPath;
            else
                DestinationPath = RootPath;

            string NewFolderName = GetUniqueName(DestinationPath, SolutionPresenterInternal.Properties.Resources.NewFolder);
            NotifyFolderCreated(DestinationPath, NewFolderName, RootProperties);
        }

        protected virtual string GetUniqueName(IFolderPath destinationPath, string originalName)
        {
            if (destinationPath == null)
                return originalName;

            string TentativeName = originalName;

            IReadOnlyCollection<ITreeNodePath> Children = spcSolutionExplorer.GetChildren(destinationPath);
            int Index = 1;

            while (IsNameTaken(Children, TentativeName))
                TentativeName = String.Format(CultureInfo.CurrentCulture, SolutionPresenterInternal.Properties.Resources.NameCopy, originalName, Index++);

            return TentativeName;
        }

        protected virtual bool IsNameTaken(IReadOnlyCollection<ITreeNodePath> folderChildren, string folderName)
        {
            Assert.ValidateReference(folderChildren);

            bool FolderNameAlreadyExist = false;
            foreach (ITreeNodePath Path in folderChildren)
            {
                IFolderPath AsFolderChild;
                if ((AsFolderChild = Path as IFolderPath) != null)
                    if (AsFolderChild.FriendlyName == folderName)
                    {
                        FolderNameAlreadyExist = true;
                        break;
                    }
            }

            return FolderNameAlreadyExist;
        }

        protected virtual void OnFolderCreatedComplete(object sender, SolutionPresenterEventCompletedEventArgs e)
        {
            Assert.ValidateReference(e);

            FolderCreatedEventContext EventContext = (FolderCreatedEventContext)e.EventContext;
            IFolderCreatedCompletionArgs CompletionArgs = (IFolderCreatedCompletionArgs)e.CompletionArgs;
            IFolderPath ParentPath = EventContext.ParentPath;
            IFolderPath NewFolderPath = CompletionArgs.NewFolderPath;
            IFolderProperties NewFolderProperties = CompletionArgs.NewFolderProperties;

            spcSolutionExplorer.AddFolder(ParentPath, NewFolderPath, NewFolderProperties);
        }
        #endregion

        #region Command: Open
        protected virtual void CanOpen(object sender, CanExecuteRoutedEventArgs e)
        {
            Assert.ValidateReference(e);

            if (DocumentOpenedEventArgs.HasHandler)
                foreach (IItemPath ItemPath in SelectedItems)
                {
                    bool IsAlreadyOpen = false;
                    foreach (IDocument Document in OpenDocuments)
                        if (Document.Path.IsEqual(ItemPath.DocumentPath))
                        {
                            IsAlreadyOpen = true;
                            break;
                        }

                    if (!IsAlreadyOpen)
                    {
                        e.CanExecute = true;
                        break;
                    }
                }
        }

        protected virtual void OnOpen(object sender, ExecutedRoutedEventArgs e)
        {
            List<IDocumentPath> DocumentPathList = new List<IDocumentPath>();
            foreach (IItemPath ItemPath in SelectedItems)
            {
                bool IsAlreadyOpen = false;
                foreach (IDocument Document in OpenDocuments)
                    if (Document.Path.IsEqual(ItemPath.DocumentPath))
                    {
                        IsAlreadyOpen = true;
                        break;
                    }

                if (!IsAlreadyOpen)
                    DocumentPathList.Add(ItemPath.DocumentPath);
            }

            OpenNextDocument(DocumentPathList);
        }

        protected virtual void OpenNextDocument(IList<IDocumentPath> documentPathList)
        {
            Assert.ValidateReference(documentPathList);

            if (documentPathList.Count > 0)
                NotifyDocumentOpened(DocumentOperation.Open, null, documentPathList, new List<IDocumentPath>(), null);
        }

        protected virtual void OnDocumentOpenedComplete(object sender, SolutionPresenterEventCompletedEventArgs e)
        {
            Assert.ValidateReference(e);

            DocumentOpenedEventContext EventContext = (DocumentOpenedEventContext)e.EventContext;
            IDocumentOpenedCompletionArgs CompletionArgs = (IDocumentOpenedCompletionArgs)e.CompletionArgs;
            DocumentOperation DocumentOperation = EventContext.DocumentOperation;
            IFolderPath DestinationFolderPath = EventContext.DestinationFolderPath;
            IList<IDocumentPath> DocumentPathList = EventContext.DocumentPathList;
            object ErrorLocation = EventContext.ErrorLocation;
            IReadOnlyList < IDocument > OpenedDocumentList = CompletionArgs.OpenedDocumentList;

            foreach (IDocument OpenedDocument in OpenedDocumentList)
            {
                OpenDocuments.Add(OpenedDocument);
                UserActivateDocument(OpenedDocument);
            }

            switch (DocumentOperation)
            {
                case DocumentOperation.Open:
                    break;

                case DocumentOperation.ShowError:
                    if (OpenedDocumentList.Count > 0)
                        NotifyErrorFocused(OpenedDocumentList[0], ErrorLocation);
                    break;

                case DocumentOperation.Add:
                    AddNextDocument(DocumentOperation, DestinationFolderPath, DocumentPathList, RootProperties);
                    break;

                case DocumentOperation.Paste:
                    AddNextDocument(DocumentOperation, DestinationFolderPath, DocumentPathList, RootProperties);
                    break;
            }
        }

        protected virtual void OnErrorFocusedComplete(object sender, SolutionPresenterEventCompletedEventArgs e)
        {
        }
        #endregion

        #region Command: Cut
        protected virtual void CanCut(object sender, CanExecuteRoutedEventArgs e)
        {
            Assert.ValidateReference(e);

            IDocument Document = dockManager.ActiveContent as IDocument;
            if (Document != null)
            {
                if (Document.CanCut())
                    e.CanExecute = true;
            }

            else if (dockManager.ActiveContent == spcSolutionExplorer)
            {
                if (spcSolutionExplorer.ValidEditOperations.Cut)
                    e.CanExecute = true;
            }
        }

        protected virtual void OnCut(object sender, ExecutedRoutedEventArgs e)
        {
            IDocument Document = dockManager.ActiveContent as IDocument;
            if (Document != null)
                Document.OnCut();

            else if (dockManager.ActiveContent == spcSolutionExplorer)
            {
                IReadOnlyDictionary<ITreeNodePath, IPathConnection> DeletedTree = spcSolutionExplorer.SelectedTree;
                spcSolutionExplorer.Copy();

                DeletePathList(DeletedTree, false);
            }
        }
        #endregion

        #region Command: Copy
        protected virtual void CanCopy(object sender, CanExecuteRoutedEventArgs e)
        {
            Assert.ValidateReference(e);

            IDocument Document = dockManager.ActiveContent as IDocument;
            if (Document != null)
            {
                if (Document.CanCopy())
                    e.CanExecute = true;
            }

            else if (dockManager.ActiveContent == spcSolutionExplorer)
            {
                if (spcSolutionExplorer.ValidEditOperations.Copy)
                    e.CanExecute = true;
            }
        }

        protected virtual void OnCopy(object sender, ExecutedRoutedEventArgs e)
        {
            IDocument Document = dockManager.ActiveContent as IDocument;
            if (Document != null)
                Document.OnCopy();

            else if (dockManager.ActiveContent == spcSolutionExplorer)
                spcSolutionExplorer.Copy();
        }
        #endregion

        #region Command: Paste
        protected virtual void CanPaste(object sender, CanExecuteRoutedEventArgs e)
        {
            Assert.ValidateReference(e);

            IDocument Document = dockManager.ActiveContent as IDocument;
            if (Document != null)
            {
                if (Document.CanPaste())
                    e.CanExecute = true;
            }

            else if (dockManager.ActiveContent == spcSolutionExplorer)
            {
                if (spcSolutionExplorer.ValidEditOperations.Paste)
                    if (NodePastedEventArgs.HasHandler)
                        e.CanExecute = true;
            }
        }

        protected virtual void OnPaste(object sender, ExecutedRoutedEventArgs e)
        {
            IDocument Document = dockManager.ActiveContent as IDocument;
            if (Document != null)
                Document.OnPaste();

            else if (dockManager.ActiveContent == spcSolutionExplorer)
            {
                if (!NodePastedEventArgs.HasHandler)
                    return;

                IFolderPath DestinationPath;

                IFolderPath AsFolderPath;
                if ((AsFolderPath = spcSolutionExplorer.GetEventSource(sender, e) as IFolderPath) != null)
                    DestinationPath = AsFolderPath;
                else
                    DestinationPath = RootPath;

                ClipboardPathData Data = SolutionExplorer.ReadClipboard();
                if (Data != null)
                {
                    IPathGroup PathGroup = new PathGroup(Data.PathTable, DestinationPath);
                    IReadOnlyDictionary<ITreeNodePath, IPathConnection> PathTable = PathGroup.PathTable;

                    Dictionary<ITreeNodePath, IFolderPath> ParentTable = new Dictionary<ITreeNodePath,IFolderPath>();
                    foreach (KeyValuePair<ITreeNodePath, IPathConnection> Entry in PathTable)
                        ParentTable.Add(Entry.Key, Entry.Value.ParentPath);

                    AddNextNode(DestinationPath, PathTable, ParentTable, false);
                }
            }
        }

        protected virtual void AddNextNode(IFolderPath destinationPath, IReadOnlyDictionary<ITreeNodePath, IPathConnection> pathTable, Dictionary<ITreeNodePath, IFolderPath> parentTable, bool isUndoRedo)
        {
            Assert.ValidateReference(pathTable);
            Assert.ValidateReference(parentTable);

            if (parentTable.Count > 0)
            {
                ITreeNodePath AddedPath = null;
                IFolderPath ParentPath = null;
                foreach (KeyValuePair<ITreeNodePath, IFolderPath> Entry in parentTable)
                {
                    AddedPath = Entry.Key;
                    ParentPath = Entry.Value;
                    break;
                }

                bool HasAncestor;
                do
                {
                    HasAncestor = false;
                    foreach (KeyValuePair<ITreeNodePath, IFolderPath> Entry in parentTable)
                        if (ParentPath == Entry.Key)
                        {
                            HasAncestor = true;
                            AddedPath = Entry.Key;
                            ParentPath = Entry.Value;
                            break;
                        }
                }
                while (HasAncestor);

                string NewName;
                if (destinationPath == null || isUndoRedo)
                    NewName = AddedPath.FriendlyName;
                else
                    NewName = GetUniqueName(destinationPath, AddedPath.FriendlyName);

                if (AddedPath.FriendlyName != NewName)
                    AddedPath.ChangeFriendlyName(NewName);

                //IPathConnection AddedNodeData = pathTable[AddedPath];
                parentTable.Remove(AddedPath);

                NotifyNodePasted(AddedPath, ParentPath, pathTable, parentTable, RootProperties, isUndoRedo);
            }
        }

        protected virtual void OnNodePastedComplete(object sender, SolutionPresenterEventCompletedEventArgs e)
        {
            Assert.ValidateReference(e);

            NodePastedEventContext EventContext = (NodePastedEventContext)e.EventContext;
            INodePastedCompletionArgs CompletionArgs = (INodePastedCompletionArgs)e.CompletionArgs;
            IFolderPath ParentPath = EventContext.ParentPath;
            IReadOnlyDictionary<ITreeNodePath, IPathConnection> PathTable = EventContext.PathTable;
            Dictionary<ITreeNodePath, IFolderPath> UpdatedParentTable = EventContext.UpdatedParentTable;
            bool IsUndoRedo = EventContext.IsUndoRedo;
            ITreeNodePath NewPath = CompletionArgs.NewPath;
            ITreeNodeProperties NewProperties = CompletionArgs.NewProperties;

            IFolderPath DestinationPath;

            IFolderPath AsFolderPath;
            IFolderProperties AsFolderProperties;
            IItemPath AsItemPath;
            IItemProperties AsItemProperties;

            if ((AsFolderPath = NewPath as IFolderPath) != null && (AsFolderProperties = NewProperties as IFolderProperties) != null)
            {
                DestinationPath = AsFolderPath;
                if (!IsUndoRedo)
                    spcSolutionExplorer.AddFolder(ParentPath, AsFolderPath, AsFolderProperties);
            }

            else if ((AsItemPath = NewPath as IItemPath) != null && (AsItemProperties = NewProperties as IItemProperties) != null)
            {
                DestinationPath = null;
                if (!IsUndoRedo)
                    spcSolutionExplorer.AddItem(ParentPath, AsItemPath, AsItemProperties);
            }

            else
                throw new InvalidCastException("Invalid Path");

            AddNextNode(DestinationPath, PathTable, UpdatedParentTable, IsUndoRedo);
        }
        #endregion

        #region Command: Delete
        protected virtual void CanDelete(object sender, CanExecuteRoutedEventArgs e)
        {
            Assert.ValidateReference(e);

            IDocument Document = dockManager.ActiveContent as IDocument;
            if (Document != null)
            {
                if (Document.CanDelete())
                    e.CanExecute = true;
            }

            else if (dockManager.ActiveContent == spcSolutionExplorer)
            {
                if (spcSolutionExplorer.ValidEditOperations.Delete)
                    e.CanExecute = true;
            }
        }

        protected virtual void OnDelete(object sender, ExecutedRoutedEventArgs e)
        {
            IDocument Document = dockManager.ActiveContent as IDocument;
            if (Document != null)
                Document.CanDelete();

            else if (dockManager.ActiveContent == spcSolutionExplorer)
            {
                IReadOnlyDictionary<ITreeNodePath, IPathConnection> SelectedTree = spcSolutionExplorer.SelectedTree;
                DeletePathList(SelectedTree, false);
            }
        }

        protected virtual void DeletePathList(IReadOnlyDictionary<ITreeNodePath, IPathConnection> deletedTree, bool isUndoRedo)
        {
            Assert.ValidateReference(deletedTree);

            List<IDocument> ClosedDocumentList = new List<IDocument>();
            foreach (IDocument Document in OpenDocuments)
            {
                bool IsClosed = false;
                foreach (KeyValuePair<ITreeNodePath, IPathConnection> Entry in deletedTree)
                {
                    IItemPath AsItemPath;
                    if ((AsItemPath = Entry.Key as IItemPath) != null)
                        if (AsItemPath.DocumentPath.IsEqual(Document.Path))
                        {
                            IsClosed = true;
                            break;
                        }
                }

                if (IsClosed)
                    ClosedDocumentList.Add(Document);
            }

            if (ClosedDocumentList.Count > 0)
                NotifyDocumentClosed(DocumentOperation.Remove, ClosedDocumentList, deletedTree, isUndoRedo, null);
            else
                NotifyDocumentRemoved(RootPath, deletedTree, isUndoRedo, null);
        }

        protected virtual void OnDocumentRemovedComplete(object sender, SolutionPresenterEventCompletedEventArgs e)
        {
            Assert.ValidateReference(e);

            DocumentRemovedEventContext EventContext = (DocumentRemovedEventContext)e.EventContext;
            IReadOnlyDictionary<ITreeNodePath, IPathConnection> DeletedTree = EventContext.DeletedTree;
            bool IsUndoRedo = EventContext.IsUndoRedo;

            ITreeNodePath ItemAfterLastSelected = spcSolutionExplorer.ItemAfterLastSelected;

            if (!IsUndoRedo)
            {
                spcSolutionExplorer.DeleteTree(DeletedTree);
                spcSolutionExplorer.SetSelected(ItemAfterLastSelected);
            }
        }
        #endregion

        #region Command: Context / Delete Solution
        protected virtual void CanDeleteSolution(object sender, CanExecuteRoutedEventArgs e)
        {
            Assert.ValidateReference(e);

            if (SolutionDeletedEventArgs.HasHandler)
                e.CanExecute = true;
        }

        protected virtual void OnDeleteSolution(object sender, ExecutedRoutedEventArgs e)
        {
            IRootPath DeletedRootPath = RootPath;
            if (!IsDeleteSolutionConfirmed(DeletedRootPath.FriendlyName))
                return;

            CommitInfo Info = CheckToSaveCurrentSolution();

            if (Info.Option == CommitOption.CommitAndContinue)
                NotifySolutionTreeCommitted(Info, SolutionOperation.Delete, DeletedRootPath, null, null);

            else if (Info.Option == CommitOption.Continue)
                NotifySolutionClosed(SolutionOperation.Delete, DeletedRootPath, null);
        }

        protected virtual bool IsDeleteSolutionConfirmed(string solutionName)
        {
            string QuestionFormat = SolutionPresenterInternal.Properties.Resources.SolutionWillBeDeleted;
            string Question = String.Format(CultureInfo.CurrentCulture, QuestionFormat, solutionName);

            MessageBoxResult Result = MessageBox.Show(Question, Owner.Title, MessageBoxButton.OKCancel, MessageBoxImage.Question);
            return (Result == MessageBoxResult.OK);
        }

        protected virtual void OnSolutionDeletedComplete(object sender, SolutionPresenterEventCompletedEventArgs e)
        {
        }
        #endregion

        #region Command: Context / Rename
        protected virtual void CanRename(object sender, CanExecuteRoutedEventArgs e)
        {
            Assert.ValidateReference(e);

            if (dockManager.ActiveContent == spcSolutionExplorer)
                if (spcSolutionExplorer.ValidEditOperations.Rename)
                    if (NodeRenamedEventArgs.HasHandler)
                        e.CanExecute = true;
        }

        protected virtual void OnRename(object sender, ExecutedRoutedEventArgs e)
        {
            if (dockManager.ActiveContent != spcSolutionExplorer)
                return;

            if (!NodeRenamedEventArgs.HasHandler)
                return;

            spcSolutionExplorer.TriggerRename();
        }

        private void OnNodeNameChanged(object sender, RoutedEventArgs e)
        {
            NameChangedEventArgs Args = e as NameChangedEventArgs;

            if (Args.Path is IFolderPath)
            {
                if (!Args.IsUndoRedo)
                    spcSolutionExplorer.ChangeName(Args.Path, Args.NewName);
            }
            else
                NotifyNodeRenamed(Args.Path, Args.NewName, Args.IsUndoRedo, RootProperties);
        }

        protected virtual void OnNodeRenamedComplete(object sender, SolutionPresenterEventCompletedEventArgs e)
        {
            Assert.ValidateReference(e);

            NodeRenamedEventContext EventContext = (NodeRenamedEventContext)e.EventContext;
            ITreeNodePath Path = EventContext.Path;
            string NewName = EventContext.NewName;
            bool IsUndoRedo = EventContext.IsUndoRedo;

            if (!IsUndoRedo)
                spcSolutionExplorer.ChangeName(Path, NewName);
        }

        private void OnNodeMoved(object sender, RoutedEventArgs e)
        {
            MovedEventArgs Args = e as MovedEventArgs;
            NotifyNodeMoved(Args.Path, Args.NewParentPath, Args.IsUndoRedo, RootProperties);
        }

        protected virtual void OnNodeMovedComplete(object sender, SolutionPresenterEventCompletedEventArgs e)
        {
            Assert.ValidateReference(e);

            NodeMovedEventContext EventContext = (NodeMovedEventContext)e.EventContext;
            ITreeNodePath Path = EventContext.Path;
            IFolderPath NewParentPath = EventContext.NewParentPath;
            bool IsUndoRedo = EventContext.IsUndoRedo;

            if (!IsUndoRedo)
                spcSolutionExplorer.Move(Path, NewParentPath);
        }

        private void OnNodeTreeChanged(object sender, RoutedEventArgs e)
        {
            TreeChangedEventArgs Args = e as TreeChangedEventArgs;
            if (Args.IsAdd)
            {
                IFolderPath DestinationPath = null;

                Dictionary<ITreeNodePath, IFolderPath> ParentTable = new Dictionary<ITreeNodePath,IFolderPath>();
                foreach (KeyValuePair<ITreeNodePath, IPathConnection> Entry in Args.PathTable)
                    ParentTable.Add(Entry.Key, Entry.Value.ParentPath);

                AddNextNode(DestinationPath, Args.PathTable, ParentTable, Args.IsUndoRedo);
            }
            else
            {
                IReadOnlyDictionary<ITreeNodePath, IPathConnection> PathTable = Args.PathTable;
                DeletePathList(PathTable, Args.IsUndoRedo);
            }
        }
        #endregion

        #region Command: Context / Properties
        protected virtual void CanEditProperties(object sender, CanExecuteRoutedEventArgs e)
        {
            Assert.ValidateReference(e);

            if (SolutionMergedProperties.Count > 0)
                e.CanExecute = true;
        }

        protected virtual void OnEditProperties(object sender, ExecutedRoutedEventArgs e)
        {
            if (SolutionMergedProperties.Count > 0)
            {
                ShowTool("toolProperties", ToolOperation.Show);
            }
        }
        #endregion

        #region Documents
        private void InitializeDocuments()
        {
            IsActiveDocumentChanging = false;
        }

        protected virtual void OnDockedDocumentClosing(object sender, Xceed.Wpf.AvalonDock.DocumentClosingEventArgs e)
        {
            if (e == null || e.Document == null)
                return;

            IDocument Document = e.Document.Content as IDocument;
            if (Document != null)
                if (Document.IsDirty)
                {
                    CommitOption Option = IsSingleDocumentSaveConfirmed(Document);

                    if (Option == CommitOption.CommitAndContinue)
                    {
                        NotifyDocumentSaved(DocumentOperation.Close, Document, null);
                        e.Cancel = true;
                    }

                    else if (Option == CommitOption.Continue)
                    {
                    }

                    else
                        e.Cancel = true;
                }
        }

        protected virtual void OnDockedDocumentClosed(object sender, Xceed.Wpf.AvalonDock.DocumentClosedEventArgs e)
        {
            if (e != null && e.Document != null)
            {
                IDocument Document = e.Document.Content as IDocument;
                if (Document != null)
                {
                    OpenDocuments.Remove(Document);
                    NotifyDocumentClosed(DocumentOperation.Close, Document);
                }
            }

            if (OpenDocuments.Count == 0)
                InternalChangeActiveDocument(null);
        }

        protected virtual CommitInfo CheckToSaveCurrentSolution()
        {
            CommitInfo Info = GetDirtyObjects();

            if (Info.DirtyItemList.Count == 0 && Info.DirtyPropertiesList.Count == 0 && Info.DirtyDocumentList.Count == 0)
                return new CommitInfo(CommitOption.Continue, null, null, null);

            CommitOption Option;
            if (SaveBeforeCompiling)
                Option = CommitOption.CommitAndContinue;
            else
                Option = IsMultipleSaveConfirmed(Info);

            if (Option != CommitOption.CommitAndContinue)
                return new CommitInfo(Option, null, null, null);
            else
                return new CommitInfo(Option, Info.DirtyItemList, Info.DirtyPropertiesList, Info.DirtyDocumentList);
        }

        protected virtual CommitInfo CheckToSaveOpenDocuments()
        {
            ICollection<IDocument> DirtyDocumentList = GetDirtyDocuments();

            if (DirtyDocumentList.Count == 0)
            {
                DirtyDocumentList = null;
                return new CommitInfo(CommitOption.Continue, new List<ITreeNodePath>(), new List<ITreeNodePath>(), null);
            }

            CommitInfo Info = new CommitInfo(CommitOption.Stop, new List<ITreeNodePath>(), new List<ITreeNodePath>(), DirtyDocumentList);
            CommitOption Option = IsMultipleSaveConfirmed(Info);
            if (Option != CommitOption.CommitAndContinue)
                return new CommitInfo(Option, new List<ITreeNodePath>(), new List<ITreeNodePath>(), null);
            else
                return new CommitInfo(Option, new List<ITreeNodePath>(), new List<ITreeNodePath>(), DirtyDocumentList);
        }

        private CommitInfo GetDirtyObjects()
        {
            ICollection<ITreeNodePath> DirtyItemList = spcSolutionExplorer.DirtyItems;
            ICollection<ITreeNodePath> DirtyPropertiesList = spcSolutionExplorer.DirtyProperties;
            ICollection<IDocument> DirtyDocumentList = GetDirtyDocuments();

            return new CommitInfo(CommitOption.Stop, DirtyItemList, DirtyPropertiesList, DirtyDocumentList);
        }

        private ICollection<IDocument> GetDirtyDocuments()
        {
            List<IDocument> DirtyDocumentList = new List<IDocument>();

            foreach (IDocument Document in OpenDocuments)
                if (Document.IsDirty)
                    DirtyDocumentList.Add(Document);

            return DirtyDocumentList;
        }

        protected virtual void ClearDirtyDocuments()
        {
            foreach (IDocument Document in OpenDocuments)
                if (Document.IsDirty)
                    Document.ClearIsDirty();
        }

        protected virtual CommitOption IsMultipleSaveConfirmed(CommitInfo info)
        {
            Assert.ValidateReference(info);

            bool NoDocumentDirty = false;
            bool IsSolutionOnlyDirtyItem = false;
            bool IsSolutionOnlyDirtyProperties = false;
            bool IsSolutionOnlyDirty = false;
            bool DirtySolutionItem = false;
            bool DirtySolutionProperties = false;

            if (info.DirtyDocumentList.Count == 0)
                NoDocumentDirty = true;

            if (info.DirtyItemList.Count == 1)
                foreach (ITreeNodePath Path in info.DirtyItemList)
                    if (Path == RootPath)
                    {
                        DirtySolutionItem = true;
                        if (info.DirtyPropertiesList.Count == 0)
                            IsSolutionOnlyDirtyItem = true;
                    }

            if (info.DirtyPropertiesList.Count == 1)
                foreach (ITreeNodePath Path in info.DirtyPropertiesList)
                    if (Path == RootPath)
                    {
                        DirtySolutionProperties = true;
                        if (info.DirtyItemList.Count == 0)
                            IsSolutionOnlyDirtyProperties = true;
                    }

            IsSolutionOnlyDirty = ((info.DirtyItemList.Count == 1) && DirtySolutionItem && (info.DirtyPropertiesList.Count == 1) && DirtySolutionProperties);

            bool IsSolutionOnly = NoDocumentDirty && (IsSolutionOnlyDirtyItem || IsSolutionOnlyDirtyProperties || IsSolutionOnlyDirty);

            if (IsSolutionOnly)
                return IsSolutionOnlySaveConfirmed();
            else
                return IsAllElementsSaveConfirmed(info);
        }

        protected virtual CommitOption IsSolutionOnlySaveConfirmed()
        {
            string QuestionFormat = SolutionPresenterInternal.Properties.Resources.ConfirmSaveSolutionChanges;
            string Question = String.Format(CultureInfo.CurrentCulture, QuestionFormat, RootPath.FriendlyName);
            MessageBoxResult Result = MessageBox.Show(Question, Owner.Title, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

            return IsConfirmed(Result);
        }

        protected virtual CommitOption IsAllElementsSaveConfirmed(CommitInfo info)
        {
            Assert.ValidateReference(info);

            SaveAllWindow Dlg = new SaveAllWindow();
            Dlg.Owner = Owner;

            if (info.DirtyItemList.Contains(RootPath) || info.DirtyPropertiesList.Contains(RootPath))
                Dlg.DirtySolutionName = RootPath.FriendlyName;

            foreach (ITreeNodePath Path in info.DirtyItemList)
                Dlg.TitleList.Add(Path.FriendlyName);

            foreach (ITreeNodePath Path in info.DirtyPropertiesList)
                if (!info.DirtyItemList.Contains(Path))
                    Dlg.TitleList.Add(Path.FriendlyName);

            foreach (IDocument Document in info.DirtyDocumentList)
            {
                bool AlreadyListed = false;

                foreach (ITreeNodePath Path in info.DirtyItemList)
                {
                    IItemPath AsItemPath;
                    if ((AsItemPath = Path as IItemPath) != null)
                        if (AsItemPath.DocumentPath.IsEqual(Document.Path))
                        {
                            AlreadyListed = true;
                            break;
                        }
                }

                if (!AlreadyListed)
                    foreach (ITreeNodePath Path in info.DirtyPropertiesList)
                    {
                        IItemPath AsItemPath;
                        if ((AsItemPath = Path as IItemPath) != null)
                            if (AsItemPath.DocumentPath.IsEqual(Document.Path))
                            {
                                AlreadyListed = true;
                                break;
                            }
                    }

                if (!AlreadyListed)
                    Dlg.TitleList.Add(Document.Path.HeaderName);
            }

            Dlg.ShowDialog();
            MessageBoxResult Result = Dlg.Result;

            return IsConfirmed(Result);
        }

        protected virtual CommitOption IsSingleDocumentSaveConfirmed(IDocument savedDocument)
        {
            Assert.ValidateReference(savedDocument);

            string QuestionFormat = SolutionPresenterInternal.Properties.Resources.ConfirmSaveDocumentChanges;
            string Question = String.Format(CultureInfo.CurrentCulture, QuestionFormat, savedDocument.Path.HeaderName);
            MessageBoxResult Result = MessageBox.Show(Question, Owner.Title, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

            return IsConfirmed(Result);
        }

        protected virtual CommitOption IsConfirmed(MessageBoxResult result)
        {
            if (result == MessageBoxResult.Cancel)
                return CommitOption.Stop;

            else if (result != MessageBoxResult.Yes)
                return CommitOption.Continue;

            else
                return CommitOption.CommitAndContinue;
        }

        protected void InternalChangeActiveDocument(IDocument newDocument)
        {
            IsActiveDocumentChanging = true;

            if (ActiveDocument != newDocument)
                ActiveDocument = newDocument;

            dockManager.ActiveContent = newDocument;

            IsActiveDocumentChanging = false;
        }

        private bool IsActiveDocumentChanging;
        #endregion

        #region Properties Tool
        private void InitializeMergedProperties()
        {
            SolutionMergedProperties = new ObservableCollection<IPropertyEntry>();
        }

        protected virtual void MergeProperties()
        {
            SolutionMergedProperties.Clear();

            List<ITreeNodeProperties> PropertiesList = new List<ITreeNodeProperties>();

            foreach (KeyValuePair<ITreeNodePath, IPathConnection> Entry in spcSolutionExplorer.SelectedNodes)
            {
                IPathConnection Connection = Entry.Value;
                ITreeNodeProperties Properties = Connection.Properties;
                if (Properties == null)
                    continue;

                if (PropertiesList.Count == 0)
                    PropertiesList.Add(Properties);

                else if (PropertiesList[0].GetType() == Properties.GetType())
                    PropertiesList.Add(Properties);

                else
                {
                    PropertiesList.Clear();
                    break;
                }
            }

            if (PropertiesList.Count > 0)
            {
                ITreeNodeProperties First = PropertiesList[0];
                PropertyInfo[] PropertyInfos = First.GetType().GetProperties();

                foreach (PropertyInfo Info in PropertyInfos)
                {
                    if (Info.Name == "IsDirty")
                        continue;

                    string FriendlyName = First.FriendlyPropertyName(Info.Name);
                    if (FriendlyName != null)
                    {
                        if (Info.PropertyType == typeof(string))
                            SolutionMergedProperties.Add(GetMergedStringProperty(PropertiesList, Info, FriendlyName));

                        else if (Info.PropertyType == typeof(bool))
                            SolutionMergedProperties.Add(GetMergedBoolProperty(PropertiesList, Info, FriendlyName));

                        else if (Info.PropertyType.IsEnum)
                            SolutionMergedProperties.Add(GetMergedEnumProperty(PropertiesList, Info, FriendlyName));
                    }
                }
            }
        }

        private static IPropertyEntry GetMergedStringProperty(List<ITreeNodeProperties> PropertiesList, PropertyInfo Info, string FriendlyName)
        {
            string MergedText = null;

            foreach (ITreeNodeProperties Properties in PropertiesList)
            {
                string NextText = (string)Info.GetValue(Properties);

                if (MergedText == null)
                    MergedText = NextText;

                else if (MergedText != NextText)
                {
                    MergedText = "";
                    break;
                }
            }

            return new StringPropertyEntry(PropertiesList, Info.Name, FriendlyName, MergedText);
        }

        private static IPropertyEntry GetMergedBoolProperty(List<ITreeNodeProperties> PropertiesList, PropertyInfo Info, string FriendlyName)
        {
            int MergedSelectedIndex = -1;

            foreach (ITreeNodeProperties Properties in PropertiesList)
            {
                int NextSelectedIndex = ((bool)Info.GetValue(Properties) ? 1 : 0);

                if (MergedSelectedIndex == -1)
                    MergedSelectedIndex = NextSelectedIndex;

                else if (MergedSelectedIndex != NextSelectedIndex)
                {
                    MergedSelectedIndex = -1;
                    break;
                }
            }

            string[] EnumNames = new string[] { SolutionPresenterInternal.Properties.Resources.False, SolutionPresenterInternal.Properties.Resources.True };
            return new EnumPropertyEntry(PropertiesList, Info.Name, FriendlyName, EnumNames, MergedSelectedIndex);
        }

        private static IPropertyEntry GetMergedEnumProperty(List<ITreeNodeProperties> PropertiesList, PropertyInfo Info, string FriendlyName)
        {
            int MergedSelectedIndex = -1;

            foreach (ITreeNodeProperties Properties in PropertiesList)
            {
                int NextSelectedIndex = (int)Info.GetValue(Properties);

                if (MergedSelectedIndex == -1)
                    MergedSelectedIndex = NextSelectedIndex;

                else if (MergedSelectedIndex != NextSelectedIndex)
                {
                    MergedSelectedIndex = -1;
                    break;
                }
            }

            string[] EnumNames = Info.PropertyType.GetEnumNames();
            return new EnumPropertyEntry(PropertiesList, Info.Name, FriendlyName, EnumNames, MergedSelectedIndex);
        }

        public ObservableCollection<IPropertyEntry> SolutionMergedProperties { get; private set; }
        #endregion

        #region Solution Tree
        private void InitializeSolutionTree()
        {
            spcSolutionExplorer.SelectionChanged += OnSolutionTreeSelectionChanged;
        }

        protected virtual void OnSolutionTreeSelectionChanged(object sender, RoutedEventArgs e)
        {
            MergeProperties();
        }

        protected virtual void OnCommitComplete(object sender, SolutionPresenterEventCompletedEventArgs e)
        {
            Assert.ValidateReference(e);

            SolutionTreeCommittedEventContext EventContext = (SolutionTreeCommittedEventContext)e.EventContext;
            SolutionOperation SolutionOperation = EventContext.SolutionOperation;
            IRootPath RootPath = EventContext.RootPath;
            IRootPath NewRootPath = EventContext.NewRootPath;
            string DestinationPath = EventContext.DestinationPath;

            spcSolutionExplorer.ClearDirtyItemsAndProperties();
            ClearDirtyDocuments();

            switch (SolutionOperation)
            {
                case SolutionOperation.Save:
                    break;

                case SolutionOperation.Create:
                    NotifySolutionClosed(SolutionOperation, RootPath, null);
                    break;

                case SolutionOperation.Delete:
                    NotifySolutionClosed(SolutionOperation, RootPath, null);
                    break;

                case SolutionOperation.Open:
                    if (RootPath != null)
                        NotifySolutionClosed(SolutionOperation, RootPath, NewRootPath);
                    else
                        NotifySolutionOpened(NewRootPath);
                    break;

                case SolutionOperation.Close:
                    if (RootPath != null)
                        NotifySolutionClosed(SolutionOperation, RootPath, null);
                    break;

                case SolutionOperation.Exit:
                    NotifyExitRequested();
                    break;

                case SolutionOperation.ExportDocument:
                    NotifyDocumentExported(DocumentOperation.Export, OpenDocuments, true, DestinationPath);
                    break;

                case SolutionOperation.ExportSolution:
                    ExportSolution(RootPath);
                    break;

                case SolutionOperation.Build:
                    NotifyBuildSolutionRequested();
                    break;

                default:
                    throw new ArgumentException("Invalid SolutionOperation");
            }
        }

        protected virtual void LoadTree(IRootPath newRootPath, IRootProperties newRootProperties, IComparer<ITreeNodePath> newComparer, IList<IFolderPath> expandedFolderList, object context)
        {
            if (newRootPath != null)
            {
                SetValue(RootPathPropertyKey, newRootPath);
                SetValue(RootPropertiesPropertyKey, newRootProperties);

                spcSolutionExplorer.SetRoot(newRootPath, newRootProperties, newComparer);

                StartLoadTree(RootProperties, ExpandedFolderList, context);
            }
            else
                NotifySolutionTreeLoaded(false);
        }

        protected virtual void BeginLoadSolutionTree(object context)
        {
            SetValue(IsLoadingTreePropertyKey, true);
        }

        protected virtual void EndLoadSolutionTree(object context)
        {
            SetValue(IsLoadingTreePropertyKey, false);
            spcSolutionExplorer.ResetUndoRedo();
            NotifySolutionTreeLoaded(false);
        }

        protected virtual void StartLoadTree(IRootProperties rootProperties, IList<IFolderPath> expandedFolderList, object context)
        {
            BeginLoadSolutionTree(context);

            List<IFolderPath> ParentPathList = new List<IFolderPath>();
            ParentPathList.Add(RootPath);
            LoadNestedTree(ParentPathList, rootProperties, expandedFolderList, context, new List<IFolderPath>());
        }

        private void LoadNestedTree(ICollection<IFolderPath> ParentPathList, IRootProperties RootProperties, ICollection<IFolderPath> ExpandedFolderList, object Context, List<IFolderPath> ExpandedFolders)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new LoadTreeHandler(OnLoadTree), ParentPathList, RootProperties, ExpandedFolderList, Context, ExpandedFolders);
        }

        private delegate void LoadTreeHandler(ICollection<IFolderPath> ParentPathList, IRootProperties RootProperties, ICollection<IFolderPath> ExpandedFolderList, object Context, List<IFolderPath> ExpandedFolders);
        private void OnLoadTree(ICollection<IFolderPath> ParentPathList, IRootProperties RootProperties, ICollection<IFolderPath> ExpandedFolderList, object Context, List<IFolderPath> ExpandedFolders)
        {
            if (ParentPathList.Count == 0)
            {
                EndLoadSolutionTree(Context);
                return;
            }

            IFolderPath ParentPath = null;
            foreach (IFolderPath Path in ParentPathList)
            {
                ParentPath = Path;
                ParentPathList.Remove(Path);
                break;
            }

            NotifyFolderEnumerated(ParentPath, ParentPathList, RootProperties, ExpandedFolderList, Context);
        }

        protected virtual void OnFolderEnumeratedComplete(object sender, SolutionPresenterEventCompletedEventArgs e)
        {
            Assert.ValidateReference(e);

            FolderEnumeratedEventContext EventContext = (FolderEnumeratedEventContext)e.EventContext;
            IFolderEnumeratedCompletionArgs CompletionArgs = (IFolderEnumeratedCompletionArgs)e.CompletionArgs;
            IFolderPath ParentPath = EventContext.ParentPath;
            ICollection<IFolderPath> ParentPathList = EventContext.ParentPathList;
            IRootProperties RootProperties = EventContext.RootProperties;
            ICollection<IFolderPath> ExpandedFolderList = EventContext.ExpandedFolderList;
            object Context = EventContext.Context;
            IReadOnlyList<ITreeNodePath> Children = CompletionArgs.Children;
            IReadOnlyDictionary<ITreeNodePath, ITreeNodeProperties> ChildrenProperties = CompletionArgs.ChildrenProperties;

            Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new LoadChildrenHandler(OnLoadChildren), ParentPath, Children, ChildrenProperties, ParentPathList, RootProperties, ExpandedFolderList, Context);
        }

        private delegate void LoadChildrenHandler(IFolderPath ParentPath, IReadOnlyList<ITreeNodePath> ChildrenPathList, IReadOnlyDictionary<ITreeNodePath, ITreeNodeProperties> ChildrenProperties, ICollection<IFolderPath> ParentPathList, IRootProperties RootProperties, ICollection<IFolderPath> ExpandedFolderList, object Context);
        private void OnLoadChildren(IFolderPath ParentPath, IReadOnlyList<ITreeNodePath> ChildrenPathList, IReadOnlyDictionary<ITreeNodePath, ITreeNodeProperties> ChildrenProperties, ICollection<IFolderPath> ParentPathList, IRootProperties RootProperties, ICollection<IFolderPath> ExpandedFolderList, object Context)
        {
            Dictionary<ITreeNodePath, IPathConnection> AddedPathTable = new Dictionary<ITreeNodePath, IPathConnection>();
            foreach (ITreeNodePath ChildPath in ChildrenPathList)
            {
                bool IsExpanded = false;

                IFolderPath AsFolderPath;
                if ((AsFolderPath = ChildPath as IFolderPath) != null)
                    foreach (IFolderPath Path in ExpandedFolderList)
                        if (Path.IsEqual(AsFolderPath))
                        {
                            IsExpanded = true;
                            break;
                        }

                AddedPathTable.Add(ChildPath, new PathConnection(ParentPath, ChildrenProperties[ChildPath], IsExpanded));
            }

            spcSolutionExplorer.AddTree(AddedPathTable);

            List<IFolderPath> FolderPathList = new List<IFolderPath>();
            List<IFolderPath> ExpandedFolders = new List<IFolderPath>();
            foreach (ITreeNodePath ChildPath in ChildrenPathList)
            {
                IFolderPath AsFolderPath;
                if ((AsFolderPath = ChildPath as IFolderPath) != null)
                {
                    FolderPathList.Add(AsFolderPath);

                    if (IsChildExpanded(ExpandedFolderList, AsFolderPath))
                        ExpandedFolders.Add(AsFolderPath);
                }
            }

            foreach (IFolderPath Path in FolderPathList)
                ParentPathList.Add(Path);
            LoadNestedTree(ParentPathList, RootProperties, ExpandedFolderList, Context, ExpandedFolders);
        }

        protected virtual bool IsChildExpanded(ICollection<IFolderPath> expandedFolderList, IFolderPath folderPath)
        {
            Assert.ValidateReference(expandedFolderList);
            Assert.ValidateReference(folderPath);

            foreach (IFolderPath Path in ExpandedFolderList)
                if (folderPath.IsEqual(Path))
                    return true;

            return false;
        }
        #endregion

        #region Compiler Tool
        private void InitializeCompilerTool()
        {
            CompilationErrorList = new ObservableCollection<ICompilationError>();
        }

        protected virtual void OnErrorLineDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Assert.ValidateReference(e);

            ICompilationError Error;
            if ((Error = listviewCompilerOutput.SelectedItem as ICompilationError) != null)
                if (Error.Source != null)
                {
                    bool IsOpened = false;
                    foreach (IDocument Document in OpenDocuments)
                        if (Document.Path.IsEqual(Error.Source))
                        {
                            IsOpened = true;
                            UserActivateDocument(Document);
                            NotifyErrorFocused(Document, Error.Location);
                            break;
                        }

                    if (!IsOpened)
                    {
                        List<IDocumentPath> DocumentPathList = new List<IDocumentPath>();
                        DocumentPathList.Add(Error.Source);
                        NotifyDocumentOpened(DocumentOperation.ShowError, null, DocumentPathList, new List<IDocumentPath>(), Error.Location);
                    }
                }

            e.Handled = true;
        }
        #endregion

        #region Custom Menus
        protected virtual void OnMainMenuLoaded(object sender, RoutedEventArgs e)
        {
            NotifyMainMenuLoaded(e);
        }

        protected virtual void OnMainToolBarLoaded(object sender, RoutedEventArgs e)
        {
            NotifyMainToolBarLoaded(e);
        }

        public virtual void InsertCustomControl(SolutionMenus solutionMenu, FrameworkElement childItem)
        {
            Separator InsertionSeparator = GetSeparator(solutionMenu);
            InsertItem(InsertionSeparator, childItem);
        }

        public virtual void ReplaceMenuItem(SolutionMenus solutionMenu, ICommand byCommand, MenuItem newItem)
        {
            ItemsControl InsertionControl = GetMenuControl(solutionMenu);
            ReplaceMenuItem(InsertionControl.Items, byCommand, newItem);
        }

        protected virtual Separator GetSeparator(SolutionMenus solutionMenu)
        {
            switch (solutionMenu)
            {
                case SolutionMenus.FileMenu:
                    return toolbarMain.FileCustomMenuSeparator;

                case SolutionMenus.FileToolBar:
                    return toolbarMain.FileToolBarSeparator;

                case SolutionMenus.EditMenu:
                    return toolbarMain.EditCustomMenuSeparator;

                case SolutionMenus.EditToolBar:
                    return toolbarMain.EditToolBarSeparator;

                case SolutionMenus.ContextMenu:
                    return spcSolutionExplorer.ContextMenuSeparator;

                default:
                    throw new ArgumentException("Invalid SolutionMenu");
            }
        }

        protected virtual ItemsControl GetMenuControl(SolutionMenus solutionMenu)
        {
            switch (solutionMenu)
            {
                case SolutionMenus.FileMenu:
                    return toolbarMain.MainMenu;

                case SolutionMenus.EditMenu:
                    return toolbarMain.MainMenu;

                default:
                    throw new ArgumentException("Invalid SolutionMenu");
            }
        }

        protected virtual void InsertItem(Separator insertionSeparator, FrameworkElement childItem)
        {
            Assert.ValidateReference(insertionSeparator);

            ItemsControl ParentCollection = (ItemsControl)insertionSeparator.Parent;
            int Index = ParentCollection.Items.IndexOf(insertionSeparator);
            ParentCollection.Items.Insert(Index, childItem);
        }

        protected virtual bool ReplaceMenuItem(ItemCollection items, ICommand byCommand, MenuItem newItem)
        {
            Assert.ValidateReference(items);
            Assert.ValidateReference(newItem);

            foreach (object Item in items)
            {
                MenuItem AsMenuItem;
                if ((AsMenuItem = Item as MenuItem) != null)
                {
                    if (AsMenuItem.Command == byCommand)
                    {
                        int Index = items.IndexOf(AsMenuItem);
                        items.RemoveAt(Index);
                        items.Insert(Index, newItem);

                        ReinsertRemovedMenuItem(newItem.Items, AsMenuItem);
                        return true;
                    }
                    else if (ReplaceMenuItem(AsMenuItem.Items, byCommand, newItem))
                        break;
                }
            }

            return false;
        }

        protected virtual bool ReinsertRemovedMenuItem(ItemCollection items, MenuItem removedMenuItem)
        {
            Assert.ValidateReference(items);
            Assert.ValidateReference(removedMenuItem);

            foreach (object Item in items)
            {
                MenuItem AsMenuItem;
                if ((AsMenuItem = Item as MenuItem) != null)
                {
                    if (AsMenuItem.Command == removedMenuItem.Command)
                    {
                        int Index = items.IndexOf(AsMenuItem);
                        items.RemoveAt(Index);
                        items.Insert(Index, removedMenuItem);
                        return true;
                    }
                    else if (ReinsertRemovedMenuItem(AsMenuItem.Items, removedMenuItem))
                        break;
                }
            }

            return false;
        }
        #endregion

        #region Context Menu
        private void InitializeContextMenu()
        {
            ContextMenuInitialized = false;
        }

        protected virtual void OnContextMenuLoaded(object sender, RoutedEventArgs e)
        {
            if (!ContextMenuInitialized)
            {
                ContextMenuInitialized = true;
                NotifyContextMenuLoaded(e);
            }
        }

        protected virtual bool ContextMenuInitialized { get; private set; }
        #endregion

        #region Clipboard
        public virtual void CopySelectionToClipboard()
        {
        }

        public virtual void PasteSelectionFromClipboard()
        {
        }
        #endregion

        #region Dock Manager
        private void InitializeDockManager()
        {
            Documents = new ObservableCollection<IDocument>();
            Documents.CollectionChanged += OnDocumentsCollectionChanged;
            FocusSortedDocuments = new List<IDocument>();
            SetValue(OpenDocumentsPropertyKey, Documents);
            dockManager.ActiveContentChanged += OnActiveContentChanged;
            toolbarMain.DocumentActivated += OnDocumentActivated;
            UpdateThemeOption();
        }

        private void OnDocumentsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            List<IDocument> ToRemove = new List<IDocument>();
            foreach (IDocument Document in FocusSortedDocuments)
                if (!OpenDocuments.Contains(Document))
                    ToRemove.Add(Document);

            foreach (IDocument Model in ToRemove)
                FocusSortedDocuments.Remove(Model);

            foreach (IDocument Model in OpenDocuments)
                if (!FocusSortedDocuments.Contains(Model))
                    FocusSortedDocuments.Insert(0, Model);
        }

        protected virtual void OnActiveContentChanged(object sender, EventArgs e)
        {
            IDocument Document = dockManager.ActiveContent as IDocument;
            if (Document != null)
            {
                InternalChangeActiveDocument(Document);

                FocusSortedDocuments.Remove(Document);
                FocusSortedDocuments.Insert(0, Document);
                Document.SetViewGotFocus();
            }
        }

        private void ChangeActiveDocument(int Direction)
        {
            if (ActiveDocument != null)
            {
                int Index = FocusSortedDocuments.IndexOf(ActiveDocument);
                if (Index >= 0)
                {
                    Index += Direction;
                    if (Index >= FocusSortedDocuments.Count)
                        Index = 0;
                    else if (Index < 0)
                        Index = FocusSortedDocuments.Count - 1;

                    UserActivateDocument(FocusSortedDocuments[Index]);
                }
            }
        }

        protected virtual void OnDocumentActivated(object sender, RoutedEventArgs e)
        {
            DocumentActivatedEventArgs Args = (DocumentActivatedEventArgs)e;
            UserActivateDocument(Args.ActiveDocument);
        }

        protected virtual void UserActivateDocument(IDocument newDocument)
        {
            dockManager.ActiveContentChanged -= OnActiveContentChanged;
            toolbarMain.DocumentActivated -= OnDocumentActivated;

            InternalChangeActiveDocument(newDocument);

            toolbarMain.DocumentActivated += OnDocumentActivated;
            dockManager.ActiveContentChanged += OnActiveContentChanged;
        }

        private bool IsToolVisible(string ContentId)
        {
            foreach (ILayoutElement Item in dockManager.Layout.Descendents())
            {
                LayoutAnchorable AsAnchorable;
                if ((AsAnchorable = Item as LayoutAnchorable) != null)
                    if (AsAnchorable.ContentId == ContentId)
                        return AsAnchorable.IsVisible;
            }

            return false;
        }

        private void ShowTool(string ContentId, ToolOperation ToolOperation)
        {
            foreach (ILayoutElement Item in dockManager.Layout.Descendents())
            {
                LayoutAnchorable AsAnchorable;
                if ((AsAnchorable = Item as LayoutAnchorable) != null)
                    if (AsAnchorable.ContentId == ContentId)
                    {
                        if (!AsAnchorable.IsVisible && ToolOperation != ToolOperation.Hide)
                            AsAnchorable.Show();
                        else if (AsAnchorable.IsVisible)
                            if (ToolOperation == ToolOperation.Show)
                                dockManager.ActiveContent = AsAnchorable.Content;
                            else
                                AsAnchorable.Hide();

                        break;
                    }
            }
        }

        private SplitView GetActiveControl()
        {
            return GetActiveDockedControl(dockManager.Layout);
        }

        private SplitView GetActiveDockedControl(ILayoutElement Layout)
        {
            LayoutDocument AsDocument;
            ILayoutContainer AsContainer;

            if ((AsDocument = Layout as LayoutDocument) != null)
            {
                if (AsDocument.Content == dockManager.ActiveContent)
                {
                    LayoutItem Item = dockManager.GetLayoutItemFromModel(AsDocument);
                    if (VisualTreeHelper.GetChildrenCount(Item.View) > 0)
                        return VisualTreeHelper.GetChild(Item.View, 0) as SplitView;
                    else
                        return null;
                }
            }

            else if ((AsContainer = Layout as ILayoutContainer) != null)
            {
                foreach (ILayoutElement Child in AsContainer.Children)
                {
                    SplitView Ctrl = GetActiveDockedControl(Child);
                    if (Ctrl != null)
                        return Ctrl;
                }
            }

            return null;
        }

        private ObservableCollection<IDocument> Documents;
        private List<IDocument> FocusSortedDocuments;
        #endregion

        #region Themes
        public void UpdateThemeOption()
        {
            LoadTheme(ThemeOption);

            Theme Theme;
            switch (ThemeOption)
            {
                case ThemeOption.Aero:
                    Theme = new AeroTheme();
                    break;

                case ThemeOption.Metro:
                    Theme = new MetroTheme();
                    break;

                case ThemeOption.VS2010:
                    Theme = new VS2010Theme();
                    break;

                default:
                    throw new ArgumentException("Invalid ThemeOption");
            }

            dockManager.Theme = Theme;

            CompositeCollection BackgroundResourceKeys = FindResource("ThemeBackgroundBrushKeys") as CompositeCollection;
            CompositeCollection ForegroundResourceKeys = FindResource("ThemeForegroundBrushKeys") as CompositeCollection;
            StatusTheme = new AvalonStatusTheme(Theme, BackgroundResourceKeys, ForegroundResourceKeys);
        }

        private static readonly Dictionary<ThemeOption, string> ThemeAssemblyTable = new Dictionary<ThemeOption, string>()
        {
            { ThemeOption.Aero, "Aero" },
            { ThemeOption.Metro, "Metro" },
            { ThemeOption.VS2010, "VS2010" },
        };

        private static readonly Dictionary<ThemeOption, string> ThemeResourceTable = new Dictionary<ThemeOption, string>()
        {
            { ThemeOption.Aero, "Theme.xaml" },
            { ThemeOption.Metro, "Theme.xaml" },
            { ThemeOption.VS2010, "Theme.xaml" },
        };

        public void LoadTheme(ThemeOption themeOption)
        {
            Collection<ResourceDictionary> MergedDictionaries = Resources.MergedDictionaries;

            string ThemeAssembly = ThemeAssemblyTable[themeOption];
            string ThemeResource = ThemeResourceTable[themeOption];
            string SourcePath = String.Format(CultureInfo.InvariantCulture, @"pack://application:,,,/Xceed.Wpf.AvalonDock.Themes.{0};component/{1}", ThemeAssembly, ThemeResource);

            ResourceDictionary NewDictionary = new ResourceDictionary();
            NewDictionary.Source = new Uri(SourcePath);

            if (MergedDictionaries.Count > 3)
                MergedDictionaries.RemoveAt(3);

            foreach (ResourceDictionary ExistingDictionary in MergedDictionaries)
                if (ExistingDictionary.Source == NewDictionary.Source)
                    return;

            MergedDictionaries.Add(NewDictionary);
        }
        #endregion

        #region Undo/Redo
        private void InitUndoRedo()
        {
            UndoRedoManager = new UndoRedoManager();
            spcSolutionExplorer.UndoRedoManager = UndoRedoManager;
            toolbarMain.UndoRedoManager = UndoRedoManager;
        }

        public UndoRedoManager UndoRedoManager { get; private set; }
        #endregion

        #region Status Bar
        public virtual void EndInitializingStatus(IRootPath startupRootPath)
        {
            ResetStatus(spcStatus.DefaultInitializingStatus);

            if (startupRootPath != null)
                NotifySolutionOpened(startupRootPath);
        }

        public virtual void SetStatus(IApplicationStatus status)
        {
            spcStatus.SetStatus(status);
        }

        public virtual void ResetStatus(IApplicationStatus status)
        {
            spcStatus.ResetStatus(status);
        }

        public virtual void SetProgress(double current, double max)
        {
            spcStatus.ProgressMax = max;
            spcStatus.ProgressValue = current;
        }

        public virtual void SetFailureStatus()
        {
            spcStatus.SetFailureStatus();
        }
        #endregion
    }
}
