namespace CustomControls
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Threading;
    using Microsoft.Win32;
    using UndoRedo;
    using Xceed.Wpf.AvalonDock.Controls;
    using Xceed.Wpf.AvalonDock.Layout;
    using Xceed.Wpf.AvalonDock.Layout.Serialization;
    using Xceed.Wpf.AvalonDock.Themes;

    /// <summary>
    /// Representer a solution presenter control.
    /// </summary>
    public partial class SolutionPresenter : UserControl, IGestureSource, IActiveDocumentSource
    {
        #region Custom properties and events
        #region Application Name
        /// <summary>
        /// Identifies the <see cref="ApplicationName"/> attached property.
        /// </summary>
        public static readonly DependencyProperty ApplicationNameProperty = DependencyProperty.Register("ApplicationName", typeof(string), typeof(SolutionPresenter), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the application name.
        /// </summary>
        public string ApplicationName
        {
            get { return (string)GetValue(ApplicationNameProperty); }
            set { SetValue(ApplicationNameProperty, value); }
        }
        #endregion
        #region Document Types
        /// <summary>
        /// Identifies the <see cref="DocumentTypes"/> attached property.
        /// </summary>
        public static readonly DependencyProperty DocumentTypesProperty = DocumentTypesPropertyKey.DependencyProperty;
        private static readonly DependencyPropertyKey DocumentTypesPropertyKey = DependencyProperty.RegisterReadOnly("DocumentTypes", typeof(DocumentTypeCollection), typeof(SolutionPresenter), new PropertyMetadata(null));

        /// <summary>
        /// Gets the list of document types.
        /// </summary>
        public DocumentTypeCollection DocumentTypes
        {
            get { return (DocumentTypeCollection)GetValue(DocumentTypesProperty); }
        }

        /// <summary>
        /// Sets the list of document types.
        /// </summary>
        /// <param name="documentTypes">The list of document types.</param>
        public virtual void SetDocumentTypes(DocumentTypeCollection documentTypes)
        {
            SetValue(DocumentTypesPropertyKey, documentTypes);
            UpdateDocumentTypeCommands();
        }
        #endregion
        #region Open Documents
        /// <summary>
        /// Identifies the <see cref="OpenDocuments"/> attached property.
        /// </summary>
        public static readonly DependencyProperty OpenDocumentsProperty = OpenDocumentsPropertyKey.DependencyProperty;
        private static readonly DependencyPropertyKey OpenDocumentsPropertyKey = DependencyProperty.RegisterReadOnly("OpenDocuments", typeof(ICollection<IDocument>), typeof(SolutionPresenter), new PropertyMetadata(null));

        /// <summary>
        /// Gets the list of open documents.
        /// </summary>
        public ICollection<IDocument> OpenDocuments
        {
            get { return (ICollection<IDocument>)GetValue(OpenDocumentsProperty); }
        }
        #endregion
        #region Active Document
        /// <summary>
        /// Identifies the <see cref="ActiveDocument"/> attached property.
        /// </summary>
        public static readonly DependencyProperty ActiveDocumentProperty = DependencyProperty.Register("ActiveDocument", typeof(IDocument), typeof(SolutionPresenter), new PropertyMetadata(null, OnActiveDocumentChanged));

        /// <summary>
        /// Gets or sets the active document.
        /// </summary>
        public IDocument? ActiveDocument
        {
            get { return (IDocument?)GetValue(ActiveDocumentProperty); }
            set { SetValue(ActiveDocumentProperty, value); }
        }

        /// <summary>
        /// Handles changes of the <see cref="ActiveDocument"/> property.
        /// </summary>
        /// <param name="modifiedObject">The modified object.</param>
        /// <param name="e">An object that contains event data.</param>
        protected static void OnActiveDocumentChanged(DependencyObject modifiedObject, DependencyPropertyChangedEventArgs e)
        {
            if (modifiedObject == null)
                throw new ArgumentNullException(nameof(modifiedObject));

            SolutionPresenter ctrl = (SolutionPresenter)modifiedObject;
            ctrl.OnActiveDocumentChanged(e);
        }

        /// <summary>
        /// Handles changes of the <see cref="ActiveDocument"/> property.
        /// </summary>
        /// <param name="e">An object that contains event data.</param>
        protected virtual void OnActiveDocumentChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ActiveDocument != null && OpenDocuments.Contains(ActiveDocument) && !IsActiveDocumentChanging)
                UserActivateDocument(ActiveDocument);
        }
        #endregion
        #region Solution Icon
        /// <summary>
        /// Identifies the <see cref="SolutionIcon"/> attached property.
        /// </summary>
        public static readonly DependencyProperty SolutionIconProperty = DependencyProperty.Register("SolutionIcon", typeof(ImageSource), typeof(SolutionPresenter), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the solution icon.
        /// </summary>
        public ImageSource SolutionIcon
        {
            get { return (ImageSource)GetValue(SolutionIconProperty); }
            set { SetValue(SolutionIconProperty, value); }
        }
        #endregion
        #region Solution Extension
        /// <summary>
        /// Identifies the <see cref="SolutionExtension"/> attached property.
        /// </summary>
        public static readonly DependencyProperty SolutionExtensionProperty = DependencyProperty.Register("SolutionExtension", typeof(string), typeof(SolutionPresenter), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the solution file extension.
        /// </summary>
        public string SolutionExtension
        {
            get { return (string)GetValue(SolutionExtensionProperty); }
            set { SetValue(SolutionExtensionProperty, value); }
        }
        #endregion
        #region Solution Extension Filter
        /// <summary>
        /// Identifies the <see cref="SolutionExtensionFilter"/> attached property.
        /// </summary>
        public static readonly DependencyProperty SolutionExtensionFilterProperty = DependencyProperty.Register("SolutionExtensionFilter", typeof(string), typeof(SolutionPresenter), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the solution extension filter.
        /// </summary>
        public string SolutionExtensionFilter
        {
            get { return (string)GetValue(SolutionExtensionFilterProperty); }
            set { SetValue(SolutionExtensionFilterProperty, value); }
        }
        #endregion
        #region Theme Option
        /// <summary>
        /// Identifies the <see cref="ThemeOption"/> attached property.
        /// </summary>
        public static readonly DependencyProperty ThemeOptionProperty = DependencyProperty.Register("ThemeOption", typeof(ThemeOption), typeof(SolutionPresenter), new PropertyMetadata(ThemeOption.Expression, OnThemeOptionChanged));

        /// <summary>
        /// Gets or sets the solution theme.
        /// </summary>
        public ThemeOption ThemeOption
        {
            get { return (ThemeOption)GetValue(ThemeOptionProperty); }
            set { SetValue(ThemeOptionProperty, value); }
        }

        /// <summary>
        /// Handles changes of the <see cref="ThemeOption"/> property.
        /// </summary>
        /// <param name="modifiedObject">The modified object.</param>
        /// <param name="e">An object that contains event data.</param>
        protected static void OnThemeOptionChanged(DependencyObject modifiedObject, DependencyPropertyChangedEventArgs e)
        {
            if (modifiedObject == null)
                throw new ArgumentNullException(nameof(modifiedObject));

            SolutionPresenter ctrl = (SolutionPresenter)modifiedObject;
            ctrl.OnThemeOptionChanged(e);
        }

        /// <summary>
        /// Handles changes of the <see cref="ThemeOption"/> property.
        /// </summary>
        /// <param name="e">An object that contains event data.</param>
        protected virtual void OnThemeOptionChanged(DependencyPropertyChangedEventArgs e)
        {
            UpdateThemeOption();
        }
        #endregion
        #region SaveBeforeCompiling
        /// <summary>
        /// Identifies the <see cref="SaveBeforeCompiling"/> attached property.
        /// </summary>
        public static readonly DependencyProperty SaveBeforeCompilingProperty = DependencyProperty.Register("SaveBeforeCompiling", typeof(bool), typeof(SolutionPresenter), new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets a value indicating whether documents should be saved before building the solution.
        /// </summary>
        public bool SaveBeforeCompiling
        {
            get { return (bool)GetValue(SaveBeforeCompilingProperty); }
            set { SetValue(SaveBeforeCompilingProperty, value); }
        }
        #endregion
        #region Gesture Translator
        /// <summary>
        /// Identifies the <see cref="GestureTranslator"/> attached property.
        /// </summary>
        public static readonly DependencyProperty GestureTranslatorProperty = DependencyProperty.Register("GestureTranslator", typeof(IGestureTranslator), typeof(SolutionPresenter), new PropertyMetadata(new GestureTranslator()));

        /// <summary>
        /// Gets or sets the gesture translator.
        /// </summary>
        public IGestureTranslator GestureTranslator
        {
            get { return (IGestureTranslator)GetValue(GestureTranslatorProperty); }
            set { SetValue(GestureTranslatorProperty, value); }
        }
        #endregion
        #region Root Path
        /// <summary>
        /// Identifies the <see cref="RootPath"/> attached property.
        /// </summary>
        public static readonly DependencyProperty RootPathProperty = RootPathPropertyKey.DependencyProperty;
        private static readonly DependencyPropertyKey RootPathPropertyKey = DependencyProperty.RegisterReadOnly("RootPath", typeof(IRootPath), typeof(SolutionPresenter), new PropertyMetadata(new EmptyPath()));

        /// <summary>
        /// Gets the solution root path.
        /// </summary>
        public IRootPath RootPath
        {
            get { return (IRootPath)GetValue(RootPathProperty); }
        }
        #endregion
        #region Root Properties
        /// <summary>
        /// Identifies the <see cref="RootProperties"/> attached property.
        /// </summary>
        public static readonly DependencyProperty RootPropertiesProperty = RootPropertiesPropertyKey.DependencyProperty;
        private static readonly DependencyPropertyKey RootPropertiesPropertyKey = DependencyProperty.RegisterReadOnly("RootProperties", typeof(IRootProperties), typeof(SolutionPresenter), new PropertyMetadata(null));

        /// <summary>
        /// Gets the root properties.
        /// </summary>
        public IRootProperties RootProperties
        {
            get { return (IRootProperties)GetValue(RootPropertiesProperty); }
        }
        #endregion
        #region Option Pages
        /// <summary>
        /// Identifies the <see cref="OptionPages"/> attached property.
        /// </summary>
        public static readonly DependencyProperty OptionPagesProperty = OptionPagesPropertyKey.DependencyProperty;
        private static readonly DependencyPropertyKey OptionPagesPropertyKey = DependencyProperty.RegisterReadOnly("OptionPages", typeof(ICollection<TabItem>), typeof(SolutionPresenter), new PropertyMetadata(null));

        /// <summary>
        /// Gets the option pages.
        /// </summary>
        public ICollection<TabItem> OptionPages
        {
            get { return (ICollection<TabItem>)GetValue(OptionPagesProperty); }
        }

        /// <summary>
        /// Sets the option pages.
        /// </summary>
        /// <param name="optionPages">The option pages.</param>
        public virtual void SetOptionPages(ICollection<TabItem> optionPages)
        {
            SetValue(OptionPagesPropertyKey, optionPages);
        }
        #endregion
        #region Is Loading Tree
        /// <summary>
        /// Identifies the <see cref="IsLoadingTree"/> attached property.
        /// </summary>
        public static readonly DependencyProperty IsLoadingTreeProperty = IsLoadingTreePropertyKey.DependencyProperty;
        private static readonly DependencyPropertyKey IsLoadingTreePropertyKey = DependencyProperty.RegisterReadOnly("IsLoadingTree", typeof(bool), typeof(SolutionPresenter), new PropertyMetadata(false));

        /// <summary>
        /// Gets a value indicating whether the solution tree is currently being loaded.
        /// </summary>
        public bool IsLoadingTree
        {
            get { return (bool)GetValue(IsLoadingTreeProperty); }
        }
        #endregion
        #region Tree Node Comparer
        /// <summary>
        /// Identifies the <see cref="TreeNodeComparer"/> attached property.
        /// </summary>
        public static readonly DependencyProperty TreeNodeComparerProperty = TreeNodeComparerPropertyKey.DependencyProperty;
        private static readonly DependencyPropertyKey TreeNodeComparerPropertyKey = DependencyProperty.RegisterReadOnly("TreeNodeComparer", typeof(IComparer<ITreeNodePath>), typeof(SolutionPresenter), new PropertyMetadata(null));

        /// <summary>
        /// Gets the tree node comparer.
        /// </summary>
        public IComparer<ITreeNodePath> TreeNodeComparer
        {
            get { return (IComparer<ITreeNodePath>)GetValue(TreeNodeComparerProperty); }
        }
        #endregion
        #region Main Menu Loaded
        /// <summary>
        /// Identifies the <see cref="MainMenuLoaded"/> routed event.
        /// </summary>
        public static readonly RoutedEvent MainMenuLoadedEvent = EventManager.RegisterRoutedEvent("MainMenuLoaded", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        /// <summary>
        /// Occurs when the main menu is loaded.
        /// </summary>
        public event RoutedEventHandler MainMenuLoaded
        {
            add { AddHandler(MainMenuLoadedEvent, value); }
            remove { RemoveHandler(MainMenuLoadedEvent, value); }
        }

        /// <summary>
        /// Invokes handlers of the <see cref="MainMenuLoaded"/> event.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected virtual void NotifyMainMenuLoaded(RoutedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            RaiseEvent(new RoutedEventArgs(MainMenuLoadedEvent, e.OriginalSource));
        }
        #endregion
        #region Main ToolBar Loaded
        /// <summary>
        /// Identifies the <see cref="MainToolBarLoaded"/> routed event.
        /// </summary>
        public static readonly RoutedEvent MainToolBarLoadedEvent = EventManager.RegisterRoutedEvent("MainToolBarLoaded", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        /// <summary>
        /// Occurs when the main toolbar is loaded.
        /// </summary>
        public event RoutedEventHandler MainToolBarLoaded
        {
            add { AddHandler(MainToolBarLoadedEvent, value); }
            remove { RemoveHandler(MainToolBarLoadedEvent, value); }
        }

        /// <summary>
        /// Invokes handlers of the <see cref="MainToolBarLoaded"/> event.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected virtual void NotifyMainToolBarLoaded(RoutedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            RaiseEvent(new RoutedEventArgs(MainToolBarLoadedEvent, e.OriginalSource));
        }
        #endregion
        #region Context Menu Loaded
        /// <summary>
        /// Identifies the <see cref="ContextMenuLoaded"/> routed event.
        /// </summary>
        public static readonly RoutedEvent ContextMenuLoadedEvent = EventManager.RegisterRoutedEvent("ContextMenuLoaded", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

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
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            RaiseEvent(new RoutedEventArgs(ContextMenuLoadedEvent, e.OriginalSource));
        }
        #endregion
        #region Context Menu Opened
        /// <summary>
        /// Identifies the <see cref="ContextMenuOpened"/> routed event.
        /// </summary>
        public static readonly RoutedEvent ContextMenuOpenedEvent = SolutionExplorer.ContextMenuOpenedEvent;

        /// <summary>
        /// Occurs when the context menu is opened.
        /// </summary>
        public event RoutedEventHandler ContextMenuOpened
        {
            add { AddHandler(ContextMenuOpenedEvent, value); }
            remove { RemoveHandler(ContextMenuOpenedEvent, value); }
        }
        #endregion
        #region Solution Tree Committed
        /// <summary>
        /// Identifies the <see cref="SolutionTreeCommitted"/> routed event.
        /// </summary>
        public static readonly RoutedEvent SolutionTreeCommittedEvent = EventManager.RegisterRoutedEvent("SolutionTreeCommitted", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        /// <summary>
        /// Occurs when the solution tree is committed on files.
        /// </summary>
        public event RoutedEventHandler SolutionTreeCommitted
        {
            add { AddHandler(SolutionTreeCommittedEvent, value); SolutionTreeCommittedEventArgs.IncrementHandlerCount(); }
            remove { RemoveHandler(SolutionTreeCommittedEvent, value); SolutionTreeCommittedEventArgs.DecrementHandlerCount(); }
        }

        /// <summary>
        /// Invokes handlers of the <see cref="SolutionTreeCommitted"/> event.
        /// </summary>
        /// <param name="info">Information about the commit.</param>
        /// <param name="solutionOperation">The solution operation.</param>
        /// <param name="rootPath">The root path.</param>
        /// <param name="newRootPath">The new root path.</param>
        /// <param name="destinationPath">The destination path.</param>
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
                OnCommitComplete(this, new SolutionPresenterEventCompletedEventArgs(EventContext, new SolutionTreeCommittedCompletionArgs()));
        }
        #endregion
        #region Folder Enumerated
        /// <summary>
        /// Identifies the <see cref="FolderEnumerated"/> routed event.
        /// </summary>
        public static readonly RoutedEvent FolderEnumeratedEvent = EventManager.RegisterRoutedEvent("FolderEnumerated", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        /// <summary>
        /// Occurs when folders are enumerated.
        /// </summary>
        public event RoutedEventHandler FolderEnumerated
        {
            add { AddHandler(FolderEnumeratedEvent, value); FolderEnumeratedEventArgs.IncrementHandlerCount(); }
            remove { RemoveHandler(FolderEnumeratedEvent, value); FolderEnumeratedEventArgs.DecrementHandlerCount(); }
        }

        /// <summary>
        /// Invokes handlers of the <see cref="FolderEnumerated"/> event.
        /// </summary>
        /// <param name="parentPath">The parent path.</param>
        /// <param name="parentPathList">The list of parent of children.</param>
        /// <param name="rootProperties">The properties of the root object.</param>
        /// <param name="expandedFolderList">The list of expanded folders.</param>
        /// <param name="context">The enumeration context.</param>
        private void NotifyFolderEnumerated(IFolderPath parentPath, ICollection<IFolderPath> parentPathList, IRootProperties rootProperties, ICollection<IFolderPath> expandedFolderList, object context)
        {
            FolderEnumeratedEventContext EventContext = new FolderEnumeratedEventContext(parentPath, parentPathList, rootProperties, expandedFolderList, context);

            if (FolderEnumeratedEventArgs.HasHandler)
            {
                FolderEnumeratedEventArgs Args = new FolderEnumeratedEventArgs(FolderEnumeratedEvent, EventContext);
                Args.EventCompleted += OnFolderEnumeratedComplete;
                RaiseEvent(Args);
            }
            else
                OnFolderEnumeratedComplete(this, new SolutionPresenterEventCompletedEventArgs(EventContext, new FolderEnumeratedCompletionArgs()));
        }
        #endregion
        #region Solution Tree Loaded
        /// <summary>
        /// Identifies the <see cref="SolutionTreeLoaded"/> routed event.
        /// </summary>
        public static readonly RoutedEvent SolutionTreeLoadedEvent = EventManager.RegisterRoutedEvent("SolutionTreeLoaded", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        /// <summary>
        /// Occurs when the solution tree is loaded.
        /// </summary>
        public event RoutedEventHandler SolutionTreeLoaded
        {
            add { AddHandler(SolutionTreeLoadedEvent, value); SolutionTreeLoadedEventArgs.IncrementHandlerCount(); }
            remove { RemoveHandler(SolutionTreeLoadedEvent, value); SolutionTreeLoadedEventArgs.DecrementHandlerCount(); }
        }

        /// <summary>
        /// Invokes handlers of the <see cref="SolutionTreeLoaded"/> event.
        /// </summary>
        /// <param name="isCanceled">True if loading is canceled.</param>
        protected virtual void NotifySolutionTreeLoaded(bool isCanceled)
        {
            SolutionTreeLoadedEventContext EventContext = new SolutionTreeLoadedEventContext(isCanceled);
            SolutionTreeLoadedEventArgs Args = new SolutionTreeLoadedEventArgs(SolutionTreeLoadedEvent, EventContext);
            RaiseEvent(Args);
        }
        #endregion
        #region Solution Selected
        /// <summary>
        /// Identifies the <see cref="SolutionSelected"/> routed event.
        /// </summary>
        public static readonly RoutedEvent SolutionSelectedEvent = EventManager.RegisterRoutedEvent("SolutionSelected", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        /// <summary>
        /// Occurs when the solution is selected.
        /// </summary>
        public event RoutedEventHandler SolutionSelected
        {
            add { AddHandler(SolutionSelectedEvent, value); SolutionSelectedEventArgs.IncrementHandlerCount(); }
            remove { RemoveHandler(SolutionSelectedEvent, value); SolutionSelectedEventArgs.DecrementHandlerCount(); }
        }

        /// <summary>
        /// Invokes handlers of the <see cref="SolutionSelected"/> event.
        /// </summary>
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
                OnSolutionSelectedComplete(this, new SolutionPresenterEventCompletedEventArgs(EventContext, new SolutionSelectedCompletionArgs()));
        }
        #endregion
        #region Solution Created
        /// <summary>
        /// Identifies the <see cref="SolutionCreated"/> routed event.
        /// </summary>
        public static readonly RoutedEvent SolutionCreatedEvent = EventManager.RegisterRoutedEvent("SolutionCreated", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        /// <summary>
        /// Occurs when a solution is created.
        /// </summary>
        public event RoutedEventHandler SolutionCreated
        {
            add { AddHandler(SolutionCreatedEvent, value); SolutionCreatedEventArgs.IncrementHandlerCount(); }
            remove { RemoveHandler(SolutionCreatedEvent, value); SolutionCreatedEventArgs.DecrementHandlerCount(); }
        }

        /// <summary>
        /// Invokes handlers of the <see cref="SolutionCreated"/> event.
        /// </summary>
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
                OnSolutionCreatedComplete(this, new SolutionPresenterEventCompletedEventArgs(EventContext, new SolutionCreatedCompletionArgs()));
        }
        #endregion
        #region Solution Opened
        /// <summary>
        /// Identifies the <see cref="SolutionOpened"/> routed event.
        /// </summary>
        public static readonly RoutedEvent SolutionOpenedEvent = EventManager.RegisterRoutedEvent("SolutionOpened", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        /// <summary>
        /// Occurs when the solution is opened.
        /// </summary>
        public event RoutedEventHandler SolutionOpened
        {
            add { AddHandler(SolutionOpenedEvent, value); SolutionOpenedEventArgs.IncrementHandlerCount(); }
            remove { RemoveHandler(SolutionOpenedEvent, value); SolutionOpenedEventArgs.DecrementHandlerCount(); }
        }

        /// <summary>
        /// Invokes handlers of the <see cref="SolutionOpened"/> event.
        /// </summary>
        /// <param name="openedRootPath">The path of the opened solution.</param>
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
                OnSolutionOpenedComplete(this, new SolutionPresenterEventCompletedEventArgs(EventContext, new SolutionOpenedCompletionArgs()));
        }
        #endregion
        #region Solution Closed
        /// <summary>
        /// Identifies the <see cref="SolutionClosed"/> routed event.
        /// </summary>
        public static readonly RoutedEvent SolutionClosedEvent = EventManager.RegisterRoutedEvent("SolutionClosed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        /// <summary>
        /// Occurs when the solution is closed.
        /// </summary>
        public event RoutedEventHandler SolutionClosed
        {
            add { AddHandler(SolutionClosedEvent, value); SolutionClosedEventArgs.IncrementHandlerCount(); }
            remove { RemoveHandler(SolutionClosedEvent, value); SolutionClosedEventArgs.DecrementHandlerCount(); }
        }

        /// <summary>
        /// Invokes handlers of the <see cref="SolutionClosed"/> event.
        /// </summary>
        /// <param name="solutionOperation">The solution operation.</param>
        /// <param name="closedRootPath">The path of the closed solution.</param>
        /// <param name="newRootPath">The new root path.</param>
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
                OnSolutionClosedComplete(this, new SolutionPresenterEventCompletedEventArgs(EventContext, new SolutionClosedCompletionArgs()));
        }
        #endregion
        #region Solution Deleted
        /// <summary>
        /// Identifies the <see cref="SolutionDeleted"/> routed event.
        /// </summary>
        public static readonly RoutedEvent SolutionDeletedEvent = EventManager.RegisterRoutedEvent("SolutionDeleted", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        /// <summary>
        /// Occurs when a solution is deleted.
        /// </summary>
        public event RoutedEventHandler SolutionDeleted
        {
            add { AddHandler(SolutionDeletedEvent, value); SolutionDeletedEventArgs.IncrementHandlerCount(); }
            remove { RemoveHandler(SolutionDeletedEvent, value); SolutionDeletedEventArgs.DecrementHandlerCount(); }
        }

        /// <summary>
        /// Invokes handlers of the <see cref="SolutionDeleted"/> event.
        /// </summary>
        /// <param name="deletedRootPath">The path of the deleted solution.</param>
        /// <param name="deletedTree">The deleted tree.</param>
        protected virtual void NotifySolutionDeleted(IRootPath deletedRootPath, IReadOnlyCollection<ITreeNodePath>? deletedTree)
        {
            SolutionDeletedEventContext EventContext = new SolutionDeletedEventContext(deletedRootPath, deletedTree);

            if (SolutionDeletedEventArgs.HasHandler)
            {
                SolutionDeletedEventArgs Args = new SolutionDeletedEventArgs(SolutionDeletedEvent, EventContext);
                Args.EventCompleted += OnSolutionDeletedComplete;
                RaiseEvent(Args);
            }
            else
                OnSolutionDeletedComplete(this, new SolutionPresenterEventCompletedEventArgs(EventContext, new SolutionDeletedCompletionArgs()));
        }
        #endregion
        #region Solution Exported
        /// <summary>
        /// Identifies the <see cref="SolutionExported"/> routed event.
        /// </summary>
        public static readonly RoutedEvent SolutionExportedEvent = EventManager.RegisterRoutedEvent("SolutionExported", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        /// <summary>
        /// Occurs when a solution is exported.
        /// </summary>
        public event RoutedEventHandler SolutionExported
        {
            add { AddHandler(SolutionExportedEvent, value); SolutionExportedEventArgs.IncrementHandlerCount(); }
            remove { RemoveHandler(SolutionExportedEvent, value); SolutionExportedEventArgs.DecrementHandlerCount(); }
        }

        /// <summary>
        /// Invokes handlers of the <see cref="SolutionExported"/> event.
        /// </summary>
        /// <param name="exportedRootPath">The path of the exported solution.</param>
        /// <param name="destinationPath">The destination path.</param>
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
                OnSolutionExportedComplete(this, new SolutionPresenterEventCompletedEventArgs(EventContext, new SolutionExportedCompletionArgs()));
        }
        #endregion
        #region Imported
        /// <summary>
        /// Identifies the <see cref="SolutionImported"/> routed event.
        /// </summary>
        public static readonly RoutedEvent SolutionImportedEvent = SolutionExplorer.ImportedEvent;

        /// <summary>
        /// Occurs when a solution is imported.
        /// </summary>
        public event RoutedEventHandler SolutionImported
        {
            add { AddHandler(SolutionImportedEvent, value); }
            remove { RemoveHandler(SolutionImportedEvent, value); }
        }
        #endregion
        #region Folder Created
        /// <summary>
        /// Identifies the <see cref="FolderCreated"/> routed event.
        /// </summary>
        public static readonly RoutedEvent FolderCreatedEvent = EventManager.RegisterRoutedEvent("FolderCreated", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        /// <summary>
        /// Occurs when a folder is created.
        /// </summary>
        public event RoutedEventHandler FolderCreated
        {
            add { AddHandler(FolderCreatedEvent, value); FolderCreatedEventArgs.IncrementHandlerCount(); }
            remove { RemoveHandler(FolderCreatedEvent, value); FolderCreatedEventArgs.DecrementHandlerCount(); }
        }

        /// <summary>
        /// Invokes handlers of the <see cref="FolderCreated"/> event.
        /// </summary>
        /// <param name="parentPath">The parent path.</param>
        /// <param name="folderName">The folder name.</param>
        /// <param name="rootProperties">The root properties.</param>
        protected virtual void NotifyFolderCreated(IFolderPath parentPath, string folderName, IRootProperties rootProperties)
        {
            FolderCreatedEventContext EventContext = new FolderCreatedEventContext(parentPath, folderName, rootProperties);

            FolderCreatedEventArgs Args = new FolderCreatedEventArgs(FolderCreatedEvent, EventContext);
            Args.EventCompleted += OnFolderCreatedComplete;
            RaiseEvent(Args);
        }
        #endregion
        #region Node Pasted
        /// <summary>
        /// Identifies the <see cref="NodePasted"/> routed event.
        /// </summary>
        public static readonly RoutedEvent NodePastedEvent = EventManager.RegisterRoutedEvent("NodePasted", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        /// <summary>
        /// Occurs when a node is pasted in the solution.
        /// </summary>
        public event RoutedEventHandler NodePasted
        {
            add { AddHandler(NodePastedEvent, value); NodePastedEventArgs.IncrementHandlerCount(); }
            remove { RemoveHandler(NodePastedEvent, value); NodePastedEventArgs.DecrementHandlerCount(); }
        }

        /// <summary>
        /// Invokes handlers of the <see cref="NodePasted"/> event.
        /// </summary>
        /// <param name="path">The path where the node is pasted.</param>
        /// <param name="parentPath">Thye parent path.</param>
        /// <param name="pathTable">The table of paths.</param>
        /// <param name="updatedParentTable">The updated table.</param>
        /// <param name="rootProperties">The root properties.</param>
        /// <param name="isUndoRedo">True if the operation can be undone.</param>
        protected virtual void NotifyNodePasted(ITreeNodePath path, IFolderPath parentPath, IReadOnlyDictionary<ITreeNodePath, IPathConnection> pathTable, Dictionary<ITreeNodePath, IFolderPath> updatedParentTable, IRootProperties rootProperties, bool isUndoRedo)
        {
            NodePastedEventContext EventContext = new NodePastedEventContext(path, parentPath, pathTable, updatedParentTable, rootProperties, isUndoRedo);

            NodePastedEventArgs Args = new NodePastedEventArgs(NodePastedEvent, EventContext);
            Args.EventCompleted += OnNodePastedComplete;
            RaiseEvent(Args);
        }
        #endregion
        #region Node Renamed
        /// <summary>
        /// Identifies the <see cref="NodeRenamed"/> routed event.
        /// </summary>
        public static readonly RoutedEvent NodeRenamedEvent = EventManager.RegisterRoutedEvent("NodeRenamed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        /// <summary>
        /// Occurs when a solution node is renamed.
        /// </summary>
        public event RoutedEventHandler NodeRenamed
        {
            add { AddHandler(NodeRenamedEvent, value); NodeRenamedEventArgs.IncrementHandlerCount(); }
            remove { RemoveHandler(NodeRenamedEvent, value); NodeRenamedEventArgs.DecrementHandlerCount(); }
        }

        /// <summary>
        /// Invokes handlers of the <see cref="NodeRenamed"/> event.
        /// </summary>
        /// <param name="path">The node path.</param>
        /// <param name="newName">The new name.</param>
        /// <param name="isUndoRedo">True if the operation can be undone.</param>
        /// <param name="rootProperties">The root properties.</param>
        protected virtual void NotifyNodeRenamed(ITreeNodePath path, string newName, bool isUndoRedo, IRootProperties rootProperties)
        {
            NodeRenamedEventContext EventContext = new NodeRenamedEventContext(path, newName, isUndoRedo, rootProperties);

            NodeRenamedEventArgs Args = new NodeRenamedEventArgs(NodeRenamedEvent, EventContext);
            Args.EventCompleted += OnNodeRenamedComplete;
            RaiseEvent(Args);
        }
        #endregion
        #region Node Moved
        /// <summary>
        /// Identifies the <see cref="NodeMoved"/> routed event.
        /// </summary>
        public static readonly RoutedEvent NodeMovedEvent = EventManager.RegisterRoutedEvent("NodeMoved", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        /// <summary>
        /// Occurs when a solution node is moved.
        /// </summary>
        public event RoutedEventHandler NodeMoved
        {
            add { AddHandler(NodeMovedEvent, value); NodeMovedEventArgs.IncrementHandlerCount(); }
            remove { RemoveHandler(NodeMovedEvent, value); NodeMovedEventArgs.DecrementHandlerCount(); }
        }

        /// <summary>
        /// Invokes handlers of the <see cref="NodeMoved"/> event.
        /// </summary>
        /// <param name="path">The node path.</param>
        /// <param name="newParentPath">The new parent path.</param>
        /// <param name="isUndoRedo">True if the operation can be undone.</param>
        /// <param name="rootProperties">The root properties.</param>
        protected virtual void NotifyNodeMoved(ITreeNodePath path, IFolderPath newParentPath, bool isUndoRedo, IRootProperties rootProperties)
        {
            NodeMovedEventContext EventContext = new NodeMovedEventContext(path, newParentPath, isUndoRedo, rootProperties);

            NodeMovedEventArgs Args = new NodeMovedEventArgs(NodeMovedEvent, EventContext);
            Args.EventCompleted += OnNodeMovedComplete;
            RaiseEvent(Args);
        }
        #endregion
        #region Document Created
        /// <summary>
        /// Identifies the <see cref="DocumentCreated"/> routed event.
        /// </summary>
        public static readonly RoutedEvent DocumentCreatedEvent = EventManager.RegisterRoutedEvent("DocumentCreated", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        /// <summary>
        /// Occurs when a document is created.
        /// </summary>
        public event RoutedEventHandler DocumentCreated
        {
            add { AddHandler(DocumentCreatedEvent, value); DocumentCreatedEventArgs.IncrementHandlerCount(); }
            remove { RemoveHandler(DocumentCreatedEvent, value); DocumentCreatedEventArgs.DecrementHandlerCount(); }
        }

        /// <summary>
        /// Invokes handlers of the <see cref="DocumentCreated"/> event.
        /// </summary>
        /// <param name="destinationFolderPath">The destination folder path.</param>
        /// <param name="type">The document type.</param>
        /// <param name="rootProperties">The root properties.</param>
        protected virtual void NotifyDocumentCreated(IFolderPath destinationFolderPath, IDocumentType type, IRootProperties rootProperties)
        {
            DocumentCreatedEventContext EventContext = new DocumentCreatedEventContext(destinationFolderPath, type, rootProperties);
            DocumentCreatedEventArgs Args = new DocumentCreatedEventArgs(DocumentCreatedEvent, EventContext);
            Args.EventCompleted += OnDocumentCreatedComplete;
            RaiseEvent(Args);
        }
        #endregion
        #region Document Selected
        /// <summary>
        /// Identifies the <see cref="DocumentSelected"/> routed event.
        /// </summary>
        public static readonly RoutedEvent DocumentSelectedEvent = EventManager.RegisterRoutedEvent("DocumentSelected", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        /// <summary>
        /// Occurs when a document is selected.
        /// </summary>
        public event RoutedEventHandler DocumentSelected
        {
            add { AddHandler(DocumentSelectedEvent, value); DocumentSelectedEventArgs.IncrementHandlerCount(); }
            remove { RemoveHandler(DocumentSelectedEvent, value); DocumentSelectedEventArgs.DecrementHandlerCount(); }
        }

        /// <summary>
        /// Invokes handlers of the <see cref="DocumentSelected"/> event.
        /// </summary>
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
                OnDocumentSelectedComplete(this, new SolutionPresenterEventCompletedEventArgs(EventContext, new DocumentSelectedCompletionArgs()));
        }
        #endregion
        #region Document Added
        /// <summary>
        /// Identifies the <see cref="DocumentAdded"/> routed event.
        /// </summary>
        public static readonly RoutedEvent DocumentAddedEvent = EventManager.RegisterRoutedEvent("DocumentAdded", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        /// <summary>
        /// Occurs when a document is added to the solution.
        /// </summary>
        public event RoutedEventHandler DocumentAdded
        {
            add { AddHandler(DocumentAddedEvent, value); DocumentAddedEventArgs.IncrementHandlerCount(); }
            remove { RemoveHandler(DocumentAddedEvent, value); DocumentAddedEventArgs.DecrementHandlerCount(); }
        }

        /// <summary>
        /// Invokes handlers of the <see cref="DocumentAdded"/> event.
        /// </summary>
        /// <param name="documentOperation">The document operation.</param>
        /// <param name="destinationFolderPath">The destination folder path.</param>
        /// <param name="documentPathList">The list of documents added.</param>
        /// <param name="rootProperties">The root properties.</param>
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
                OnDocumentAddedComplete(this, new SolutionPresenterEventCompletedEventArgs(EventContext, new DocumentAddedCompletionArgs()));
        }
        #endregion
        #region Document Opened
        /// <summary>
        /// Identifies the <see cref="DocumentOpened"/> routed event.
        /// </summary>
        public static readonly RoutedEvent DocumentOpenedEvent = EventManager.RegisterRoutedEvent("DocumentOpened", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        /// <summary>
        /// Occurs when a document is opened.
        /// </summary>
        public event RoutedEventHandler DocumentOpened
        {
            add { AddHandler(DocumentOpenedEvent, value); DocumentOpenedEventArgs.IncrementHandlerCount(); }
            remove { RemoveHandler(DocumentOpenedEvent, value); DocumentOpenedEventArgs.DecrementHandlerCount(); }
        }

        /// <summary>
        /// Invokes handlers of the <see cref="DocumentOpened"/> event.
        /// </summary>
        /// <param name="documentOperation">The document operation.</param>
        /// <param name="destinationFolderPath">The destination folder path.</param>
        /// <param name="openedDocumentPathList">The list of documents opened.</param>
        /// <param name="documentPathList">The list of documents.</param>
        /// <param name="errorLocation">The error location.</param>
        protected virtual void NotifyDocumentOpened(DocumentOperation documentOperation, IFolderPath destinationFolderPath, IList<IDocumentPath> openedDocumentPathList, IList<IDocumentPath> documentPathList, object? errorLocation)
        {
            DocumentOpenedEventContext EventContext = new DocumentOpenedEventContext(documentOperation, destinationFolderPath, openedDocumentPathList, documentPathList, errorLocation);

            if (DocumentOpenedEventArgs.HasHandler)
            {
                DocumentOpenedEventArgs Args = new DocumentOpenedEventArgs(DocumentOpenedEvent, EventContext);
                Args.EventCompleted += OnDocumentOpenedComplete;
                RaiseEvent(Args);
            }
            else
                OnDocumentOpenedComplete(this, new SolutionPresenterEventCompletedEventArgs(EventContext, new DocumentOpenedCompletionArgs()));
        }
        #endregion
        #region Document Closed
        /// <summary>
        /// Identifies the <see cref="DocumentClosed"/> routed event.
        /// </summary>
        public static readonly RoutedEvent DocumentClosedEvent = EventManager.RegisterRoutedEvent("DocumentClosed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        /// <summary>
        /// Occurs when a document is closed.
        /// </summary>
        public event RoutedEventHandler DocumentClosed
        {
            add { AddHandler(DocumentClosedEvent, value); DocumentClosedEventArgs.IncrementHandlerCount(); }
            remove { RemoveHandler(DocumentClosedEvent, value); DocumentClosedEventArgs.DecrementHandlerCount(); }
        }

        /// <summary>
        /// Invokes handlers of the <see cref="DocumentClosed"/> event.
        /// </summary>
        /// <param name="documentOperation">The document operation.</param>
        /// <param name="closedDocument">The closed document.</param>
        protected virtual void NotifyDocumentClosed(DocumentOperation documentOperation, IDocument closedDocument)
        {
            List<IDocument> ClosedDocumentList = new List<IDocument>();
            ClosedDocumentList.Add(closedDocument);

            NotifyDocumentClosed(documentOperation, ClosedDocumentList, new Dictionary<ITreeNodePath, IPathConnection>(), false, null);
        }

        /// <summary>
        /// Invokes handlers of the <see cref="DocumentClosed"/> event.
        /// </summary>
        /// <param name="documentOperation">The document operation.</param>
        /// <param name="closedDocumentList">The list of closed documents.</param>
        /// <param name="closedTree">The closed tree.</param>
        /// <param name="isUndoRedo">True if the operation can be undone.</param>
        /// <param name="clientInfo">The operation data.</param>
        protected virtual void NotifyDocumentClosed(DocumentOperation documentOperation, IList<IDocument> closedDocumentList, IReadOnlyDictionary<ITreeNodePath, IPathConnection> closedTree, bool isUndoRedo, object? clientInfo)
        {
            DocumentClosedEventContext EventContext = new DocumentClosedEventContext(documentOperation, closedDocumentList, closedTree, isUndoRedo, clientInfo);

            if (DocumentClosedEventArgs.HasHandler)
            {
                DocumentClosedEventArgs Args = new DocumentClosedEventArgs(DocumentClosedEvent, EventContext);
                Args.EventCompleted += OnDocumentClosedComplete;
                RaiseEvent(Args);
            }
            else
                OnDocumentClosedComplete(this, new SolutionPresenterEventCompletedEventArgs(EventContext, new DocumentClosedCompletionArgs()));
        }
        #endregion
        #region Document Saved
        /// <summary>
        /// Identifies the <see cref="DocumentSaved"/> routed event.
        /// </summary>
        public static readonly RoutedEvent DocumentSavedEvent = EventManager.RegisterRoutedEvent("DocumentSaved", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        /// <summary>
        /// Occurs when a document is saved.
        /// </summary>
        public event RoutedEventHandler DocumentSaved
        {
            add { AddHandler(DocumentSavedEvent, value); DocumentSavedEventArgs.IncrementHandlerCount(); }
            remove { RemoveHandler(DocumentSavedEvent, value); DocumentSavedEventArgs.DecrementHandlerCount(); }
        }

        /// <summary>
        /// Invokes handlers of the <see cref="DocumentSaved"/> event.
        /// </summary>
        /// <param name="documentOperation">The document operation.</param>
        /// <param name="savedDocument">The saved document.</param>
        /// <param name="fileName">The file name.</param>
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
                OnDocumentSavedComplete(this, new SolutionPresenterEventCompletedEventArgs(EventContext, new DocumentSavedCompletionArgs()));
        }
        #endregion
        #region Document Removed
        /// <summary>
        /// Identifies the <see cref="DocumentRemoved"/> routed event.
        /// </summary>
        public static readonly RoutedEvent DocumentRemovedEvent = EventManager.RegisterRoutedEvent("DocumentRemoved", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        /// <summary>
        /// Occurs when a document is removed from the solution.
        /// </summary>
        public event RoutedEventHandler DocumentRemoved
        {
            add { AddHandler(DocumentRemovedEvent, value); DocumentRemovedEventArgs.IncrementHandlerCount(); }
            remove { RemoveHandler(DocumentRemovedEvent, value); DocumentRemovedEventArgs.DecrementHandlerCount(); }
        }

        /// <summary>
        /// Invokes handlers of the <see cref="DocumentRemoved"/> event.
        /// </summary>
        /// <param name="rootPath">The root path.</param>
        /// <param name="deletedTree">The deleted tree.</param>
        /// <param name="isUndoRedo">True if the operation can be undone.</param>
        /// <param name="clientInfo">The operation data.</param>
        protected virtual void NotifyDocumentRemoved(IRootPath rootPath, IReadOnlyDictionary<ITreeNodePath, IPathConnection> deletedTree, bool isUndoRedo, object? clientInfo)
        {
            DocumentRemovedEventContext EventContext = new DocumentRemovedEventContext(rootPath, deletedTree, isUndoRedo, clientInfo);

            if (DocumentRemovedEventArgs.HasHandler)
            {
                DocumentRemovedEventArgs Args = new DocumentRemovedEventArgs(DocumentRemovedEvent, EventContext);
                Args.EventCompleted += OnDocumentRemovedComplete;
                RaiseEvent(Args);
            }
            else
                OnDocumentRemovedComplete(this, new SolutionPresenterEventCompletedEventArgs(EventContext, new DocumentRemovedCompletionArgs()));
        }
        #endregion
        #region Document Exported
        /// <summary>
        /// Identifies the <see cref="DocumentExported"/> routed event.
        /// </summary>
        public static readonly RoutedEvent DocumentExportedEvent = EventManager.RegisterRoutedEvent("DocumentExported", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        /// <summary>
        /// Occurs when a document is exported.
        /// </summary>
        public event RoutedEventHandler DocumentExported
        {
            add { AddHandler(DocumentExportedEvent, value); DocumentExportedEventArgs.IncrementHandlerCount(); }
            remove { RemoveHandler(DocumentExportedEvent, value); DocumentExportedEventArgs.DecrementHandlerCount(); }
        }

        /// <summary>
        /// Invokes handlers of the <see cref="DocumentExported"/> event.
        /// </summary>
        /// <param name="documentOperation">The document operation.</param>
        /// <param name="exportedDocument">The exported document.</param>
        /// <param name="fileName">The file name.</param>
        protected virtual void NotifyDocumentExported(DocumentOperation documentOperation, IDocument exportedDocument, string fileName)
        {
            List<IDocument> ExportedDocumentList = new List<IDocument>();
            ExportedDocumentList.Add(exportedDocument);
            NotifyDocumentExported(documentOperation, ExportedDocumentList, false, fileName);
        }

        /// <summary>
        /// Invokes handlers of the <see cref="DocumentExported"/> event.
        /// </summary>
        /// <param name="documentOperation">The document operation.</param>
        /// <param name="exportedDocumentList">The list of exported documents.</param>
        /// <param name="isDestinationFolder">True if the destination is a folder.</param>
        /// <param name="destinationPath">The destination path.</param>
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
                OnDocumentExportedComplete(this, new SolutionPresenterEventCompletedEventArgs(EventContext, new DocumentExportedCompletionArgs()));
        }
        #endregion
        #region Error Focused
        /// <summary>
        /// Identifies the <see cref="ErrorFocused"/> routed event.
        /// </summary>
        public static readonly RoutedEvent ErrorFocusedEvent = EventManager.RegisterRoutedEvent("ErrorFocused", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        /// <summary>
        /// Occurs when an error gets the focus.
        /// </summary>
        public event RoutedEventHandler ErrorFocused
        {
            add { AddHandler(ErrorFocusedEvent, value); ErrorFocusedEventArgs.IncrementHandlerCount(); }
            remove { RemoveHandler(ErrorFocusedEvent, value); ErrorFocusedEventArgs.DecrementHandlerCount(); }
        }

        /// <summary>
        /// Invokes handlers of the <see cref="ErrorFocused"/> event.
        /// </summary>
        /// <param name="document">The document where the error is.</param>
        /// <param name="errorLocation">The error location.</param>
        protected virtual void NotifyErrorFocused(IDocument document, object? errorLocation)
        {
            ErrorFocusedEventContext EventContext = new ErrorFocusedEventContext(document, errorLocation);

            if (ErrorFocusedEventArgs.HasHandler)
            {
                ErrorFocusedEventArgs Args = new ErrorFocusedEventArgs(ErrorFocusedEvent, EventContext);
                Args.EventCompleted += OnErrorFocusedComplete;
                RaiseEvent(Args);
            }
            else
                OnErrorFocusedComplete(this, new SolutionPresenterEventCompletedEventArgs(EventContext, new ErrorFocusedCompletionArgs()));
        }
        #endregion
        #region Add New Items Requested
        /// <summary>
        /// Identifies the <see cref="AddNewItemsRequested"/> routed event.
        /// </summary>
        public static readonly RoutedEvent AddNewItemsRequestedEvent = EventManager.RegisterRoutedEvent("AddNewItemsRequested", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        /// <summary>
        /// Occurs when the user requests to add new items.
        /// </summary>
        public event RoutedEventHandler AddNewItemsRequested
        {
            add { AddHandler(AddNewItemsRequestedEvent, value); AddNewItemsRequestedEventArgs.IncrementHandlerCount(); }
            remove { RemoveHandler(AddNewItemsRequestedEvent, value); AddNewItemsRequestedEventArgs.DecrementHandlerCount(); }
        }

        /// <summary>
        /// Invokes handlers of the <see cref="AddNewItemsRequested"/> event.
        /// </summary>
        /// <param name="destinationFolderPath">The destination folder path.</param>
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
                OnAddNewItemsRequestedComplete(this, new SolutionPresenterEventCompletedEventArgs(EventContext, new AddNewItemsRequestedCompletionArgs()));
        }
        #endregion
        #region Exit Requested
        /// <summary>
        /// Identifies the <see cref="ExitRequested"/> routed event.
        /// </summary>
        public static readonly RoutedEvent ExitRequestedEvent = EventManager.RegisterRoutedEvent("ExitRequested", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        /// <summary>
        /// Occurs when the user requests to exit the application.
        /// </summary>
        public event RoutedEventHandler ExitRequested
        {
            add { AddHandler(ExitRequestedEvent, value); }
            remove { RemoveHandler(ExitRequestedEvent, value); }
        }

        /// <summary>
        /// Invokes handlers of the <see cref="ExitRequested"/> event.
        /// </summary>
        protected virtual void NotifyExitRequested()
        {
            RaiseEvent(new RoutedEventArgs(ExitRequestedEvent));
        }
        #endregion
        #region Document Import Descriptors
        /// <summary>
        /// Identifies the <see cref="DocumentImportDescriptors"/> attached property.
        /// </summary>
        public static readonly DependencyProperty DocumentImportDescriptorsProperty = DocumentImportDescriptorsPropertyKey.DependencyProperty;
        private static readonly DependencyPropertyKey DocumentImportDescriptorsPropertyKey = DependencyProperty.RegisterReadOnly("DocumentImportDescriptors", typeof(ICollection<IDocumentImportDescriptor>), typeof(SolutionPresenter), new PropertyMetadata(null));

        /// <summary>
        /// Gets the list of descriptor of imported documents.
        /// </summary>
        public ICollection<IDocumentImportDescriptor> DocumentImportDescriptors
        {
            get { return (ICollection<IDocumentImportDescriptor>)GetValue(DocumentImportDescriptorsProperty); }
        }

        /// <summary>
        /// Sets the <see cref="DocumentImportDescriptors"/> property.
        /// </summary>
        /// <param name="documentImportDescriptors">The list of descriptor of imported documents.</param>
        public virtual void SetDocumentImportDescriptors(ICollection<IDocumentImportDescriptor> documentImportDescriptors)
        {
            SetValue(DocumentImportDescriptorsPropertyKey, documentImportDescriptors);
        }
        #endregion
        #region Import Folder
        /// <summary>
        /// Identifies the <see cref="ImportFolder"/> attached property.
        /// </summary>
        public static readonly DependencyProperty ImportFolderProperty = DependencyProperty.Register("ImportFolder", typeof(string), typeof(SolutionPresenter), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the import folder.
        /// </summary>
        public string ImportFolder
        {
            get { return (string)GetValue(ImportFolderProperty); }
            set { SetValue(ImportFolderProperty, value); }
        }
        #endregion
        #region Export Folder
        /// <summary>
        /// Identifies the <see cref="ExportFolder"/> attached property.
        /// </summary>
        public static readonly DependencyProperty ExportFolderProperty = DependencyProperty.Register("ExportFolder", typeof(string), typeof(SolutionPresenter), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the export folder.
        /// </summary>
        public string ExportFolder
        {
            get { return (string)GetValue(ExportFolderProperty); }
            set { SetValue(ExportFolderProperty, value); }
        }
        #endregion
        #region Import New Items Requested
        /// <summary>
        /// Identifies the <see cref="ImportNewItemsRequested"/> routed event.
        /// </summary>
        public static readonly RoutedEvent ImportNewItemsRequestedEvent = EventManager.RegisterRoutedEvent("ImportNewItemsRequested", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        /// <summary>
        /// Occurs when the user requests to import new documents.
        /// </summary>
        public event RoutedEventHandler ImportNewItemsRequested
        {
            add { AddHandler(ImportNewItemsRequestedEvent, value); ImportNewItemsRequestedEventArgs.IncrementHandlerCount(); }
            remove { RemoveHandler(ImportNewItemsRequestedEvent, value); ImportNewItemsRequestedEventArgs.DecrementHandlerCount(); }
        }

        /// <summary>
        /// Invokes handlers of the <see cref="ImportNewItemsRequested"/> event.
        /// </summary>
        /// <param name="importedDocumentTable">The table of imported documents.</param>
        /// <param name="documentPathList">The list of imported document paths.</param>
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
                OnImportNewItemsRequestedComplete(this, new SolutionPresenterEventCompletedEventArgs(EventContext, new ImportNewItemsRequestedCompletionArgs()));
        }
        #endregion
        #region Build Solution Requested
        /// <summary>
        /// Identifies the <see cref="BuildSolutionRequested"/> routed event.
        /// </summary>
        public static readonly RoutedEvent BuildSolutionRequestedEvent = EventManager.RegisterRoutedEvent("BuildSolutionRequested", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        /// <summary>
        /// Occurs when the user requests to build the solution.
        /// </summary>
        public event RoutedEventHandler BuildSolutionRequested
        {
            add { AddHandler(BuildSolutionRequestedEvent, value); BuildSolutionRequestedEventArgs.IncrementHandlerCount(); }
            remove { RemoveHandler(BuildSolutionRequestedEvent, value); BuildSolutionRequestedEventArgs.DecrementHandlerCount(); }
        }

        /// <summary>
        /// Invokes handlers of the <see cref="BuildSolutionRequested"/> event.
        /// </summary>
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
                OnBuildSolutionRequestedComplete(this, new SolutionPresenterEventCompletedEventArgs(EventContext, new BuildSolutionRequestedCompletionArgs()));
        }
        #endregion
        #region Options Changed
        /// <summary>
        /// Identifies the <see cref="OptionsChanged"/> routed event.
        /// </summary>
        public static readonly RoutedEvent OptionsChangedEvent = EventManager.RegisterRoutedEvent("OptionsChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        /// <summary>
        /// Occurs when the solution options have changed.
        /// </summary>
        public event RoutedEventHandler OptionsChanged
        {
            add { AddHandler(OptionsChangedEvent, value); }
            remove { RemoveHandler(OptionsChangedEvent, value); }
        }

        /// <summary>
        /// Invokes handlers of the <see cref="OptionsChanged"/> event.
        /// </summary>
        protected virtual void NotifyOptionsChanged()
        {
            RaiseEvent(new RoutedEventArgs(OptionsChangedEvent));
        }
        #endregion
        #region Root Properties Requested
        /// <summary>
        /// Identifies the <see cref="RootPropertiesRequested"/> routed event.
        /// </summary>
        public static readonly RoutedEvent RootPropertiesRequestedEvent = EventManager.RegisterRoutedEvent("RootPropertiesRequested", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        /// <summary>
        /// Occurs when the root properties are requested.
        /// </summary>
        public event RoutedEventHandler RootPropertiesRequested
        {
            add { AddHandler(RootPropertiesRequestedEvent, value); RootPropertiesRequestedEventArgs.IncrementHandlerCount(); }
            remove { RemoveHandler(RootPropertiesRequestedEvent, value); RootPropertiesRequestedEventArgs.DecrementHandlerCount(); }
        }

        /// <summary>
        /// Invokes handlers of the <see cref="RootPropertiesRequested"/> event.
        /// </summary>
        /// <param name="properties">The root properties.</param>
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
                OnRootPropertiesRequestedComplete(this, new SolutionPresenterEventCompletedEventArgs(EventContext, new RootPropertiesRequestedCompletionArgs()));
        }
        #endregion
        #region Show About Requested
        /// <summary>
        /// Identifies the <see cref="ShowAboutRequested"/> routed event.
        /// </summary>
        public static readonly RoutedEvent ShowAboutRequestedEvent = EventManager.RegisterRoutedEvent("ShowAboutRequested", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionPresenter));

        /// <summary>
        /// Occurs when the About window should be displayed.
        /// </summary>
        public event RoutedEventHandler ShowAboutRequested
        {
            add { AddHandler(ShowAboutRequestedEvent, value); }
            remove { RemoveHandler(ShowAboutRequestedEvent, value); }
        }

        /// <summary>
        /// Invokes handlers of the <see cref="ShowAboutRequested"/> event.
        /// </summary>
        protected virtual void NotifyShowAboutRequested()
        {
            RaiseEvent(new RoutedEventArgs(ShowAboutRequestedEvent));
        }
        #endregion
        #endregion

        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionPresenter"/> class.
        /// </summary>
        public SolutionPresenter()
        {
            InitializeComponent();
            InitializeDocuments();
            InitializeSolutionTree();
            InitializeContextMenu();
            InitializeDockManager();
            InitUndoRedo();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the window owner.
        /// </summary>
        protected virtual Window Owner
        {
            get { return Window.GetWindow(this); }
        }

        /// <summary>
        /// Gets the table of selected nodes.
        /// </summary>
        public IReadOnlyDictionary<ITreeNodePath, IPathConnection> SelectedNodes
        {
            get { return spcSolutionExplorer.SelectedNodes; }
        }

        /// <summary>
        /// Gets the list of selected items.
        /// </summary>
        public IList<IItemPath> SelectedItems
        {
            get
            {
                IReadOnlyDictionary<ITreeNodePath, IPathConnection> SelectedNodes = spcSolutionExplorer.SelectedNodes;

                List<IItemPath> Result = new List<IItemPath>();
                foreach (KeyValuePair<ITreeNodePath, IPathConnection> Entry in SelectedNodes)
                    if (Entry.Key is IItemPath AsItemPath)
                        Result.Add(AsItemPath);

                return Result;
            }
        }

        /// <summary>
        /// Gets the selected tree.
        /// </summary>
        public IReadOnlyDictionary<ITreeNodePath, IPathConnection> SelectedTree
        {
            get { return spcSolutionExplorer.SelectedTree; }
        }

        /// <summary>
        /// Gets the list of compilation errors.
        /// </summary>
        public ObservableCollection<ICompilationError> CompilationErrorList { get; } = new ObservableCollection<ICompilationError>();

        /// <summary>
        /// Gets the status theme.
        /// </summary>
        public StatusTheme StatusTheme { get; private set; } = new StatusTheme();

        /// <summary>
        /// Gets the content of the active document.
        /// </summary>
        public FrameworkElement? ActiveDocumentContent
        {
            get
            {
                if (GetActiveControl() is SplitView ctrl)
                    return ctrl.GetRowContent(1);
                else
                    return null;
            }
        }

        #endregion

        #region Client Interface
        /// <summary>
        /// Sets the focus to this control.
        /// </summary>
        public new virtual void Focus()
        {
            if (dockManager.ActiveContent == spcSolutionExplorer || IsToolVisible("toolSolutionExplorer"))
                spcSolutionExplorer.Focus();
            else if (dockManager.ActiveContent == listviewCompilerOutput || IsToolVisible("toolCompilerOutput"))
                listviewCompilerOutput.Focus();
            else if (dockManager.ActiveContent is IDocument Document)
                    Document.SetViewGotFocus();
        }

        /// <summary>
        /// Gets the list of items.
        /// </summary>
        public virtual ICollection<IItemPath> Items
        {
            get { return spcSolutionExplorer.SolutionItems; }
        }

        /// <summary>
        /// Gets the properties of an item.
        /// </summary>
        /// <param name="path">The path to the item.</param>
        /// <returns>The properties.</returns>
        public virtual IItemProperties? GetItemProperties(IItemPath path)
        {
            return spcSolutionExplorer.GetItemProperties(path);
        }

        /// <summary>
        /// Gets the list if expanded folders.
        /// </summary>
        public virtual IList<IFolderPath> ExpandedFolderList
        {
            get { return spcSolutionExplorer.ExpandedFolderList; }
        }

        /// <summary>
        /// Commits modified objects.
        /// </summary>
        /// <param name="isExit">True if done during an exit.</param>
        /// <returns>The commit option.</returns>
        public virtual CommitOption CommitDirty(bool isExit)
        {
            CommitInfo Info = CheckToSaveCurrentSolution();

            if (Info.Option == CommitOption.CommitAndContinue)
                NotifySolutionTreeCommitted(Info, isExit ? SolutionOperation.Exit : SolutionOperation.Save, new EmptyPath(), new EmptyPath(), string.Empty);

            return Info.Option;
        }

        /// <summary>
        /// Removes documents from the solution.
        /// </summary>
        /// <param name="documentPathList">The list of documents to remove.</param>
        /// <param name="clientInfo">The operation data.</param>
        public virtual void RemoveDocuments(IReadOnlyCollection<IDocumentPath> documentPathList, object clientInfo)
        {
            if (documentPathList == null)
                throw new ArgumentNullException(nameof(documentPathList));

            IList<IDocument> ClosedDocumentList = FindOpenDocuments(documentPathList);
            IReadOnlyDictionary<ITreeNodePath, IPathConnection> ClosedTree = spcSolutionExplorer.FindItemsByDocumentPath(documentPathList);

            NotifyDocumentClosed(DocumentOperation.Remove, ClosedDocumentList, ClosedTree, false, clientInfo);
        }

        private IList<IDocument> FindOpenDocuments(IReadOnlyCollection<IDocumentPath> documentPathList)
        {
            IList<IDocument> Result = new List<IDocument>();

            foreach (IDocument Document in OpenDocuments)
                foreach (IDocumentPath DocumentPath in documentPathList)
                    if (Document.Path.IsEqual(DocumentPath))
                    {
                        Result.Add(Document);
                        break;
                    }

            return Result;
        }

        /// <summary>
        /// Gets the list of items in the solution.
        /// </summary>
        public virtual ICollection<IItemPath> SolutionItems
        {
            get { return spcSolutionExplorer.SolutionItems; }
        }
        #endregion

        #region Serialization
        /// <summary>
        /// Serializes the solution state.
        /// </summary>
        /// <returns>The serialized state.</returns>
        public virtual string SerializeState()
        {
            string[] StateList = new string[] { SerializeDockManagerState(), SerializeToolBarState(), SerializePresenterState() };

            string MergedState = string.Empty;
            foreach (string State in StateList)
            {
                if (MergedState.Length > 0)
                    MergedState += '|';
                MergedState += State;
            }

            return MergedState;
        }

        /// <summary>
        /// Deserializes the solution state.
        /// </summary>
        /// <param name="mergedState">The state to deserialize.</param>
        public virtual void DeserializeState(string mergedState)
        {
            if (mergedState == null)
                throw new ArgumentNullException(nameof(mergedState));

            string[] StateList = mergedState.Split('|');

            int Index = 0;
            if (Index < StateList.Length)
                DeserializeDockManagerState(StateList[Index++]);
            if (Index < StateList.Length)
                DeserializeToolBarState(StateList[Index++]);
            if (Index < StateList.Length)
                DeserializePresenterState(StateList[Index++]);
        }

        /// <summary>
        /// Resets the solution state.
        /// </summary>
        public virtual void ResetState()
        {
            ResetDockManagerState();
            ResetToolBarState();
            ResetPresenterState();
        }

        /// <summary>
        /// Serializes the dock manager state.
        /// </summary>
        /// <returns>The serialized state.</returns>
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

        /// <summary>
        /// Serializes the toolbar state.
        /// </summary>
        /// <returns>The serialized state.</returns>
        protected virtual string SerializeToolBarState()
        {
            return toolbarMain.SerializeActiveButtons();
        }

        /// <summary>
        /// Serializes the presenter state.
        /// </summary>
        /// <returns>The serialized state.</returns>
        protected virtual string SerializePresenterState()
        {
            string[] StateList = new string[] { SerializeThemeState(), SerializeCompilerState() };

            string MergedState = string.Empty;
            foreach (string State in StateList)
            {
                if (MergedState.Length > 0)
                    MergedState += ',';
                MergedState += State;
            }

            return MergedState;
        }

        /// <summary>
        /// Serializes the theme state.
        /// </summary>
        /// <returns>The serialized state.</returns>
        protected virtual string SerializeThemeState()
        {
            return ThemeOption.ToString();
        }

        /// <summary>
        /// Serializes the compiler state.
        /// </summary>
        /// <returns>The serialized state.</returns>
        protected virtual string SerializeCompilerState()
        {
            return SaveBeforeCompiling.ToString(CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Deserializes the dock manager state.
        /// </summary>
        /// <param name="state">The serialized state.</param>
        public virtual void DeserializeDockManagerState(string state)
        {
            using (StringReader sr = new StringReader(state))
            {
                XmlLayoutSerializer DockManagerSerializer = new XmlLayoutSerializer(dockManager);
                DockManagerSerializer.Deserialize(sr);
            }
        }

        /// <summary>
        /// Deserializes the toolbar state.
        /// </summary>
        /// <param name="state">The serialized state.</param>
        protected virtual void DeserializeToolBarState(string state)
        {
            toolbarMain.DeserializeActiveButtons(state);
        }

        /// <summary>
        /// Deserializes the presenter state.
        /// </summary>
        /// <param name="mergedState">The serialized state.</param>
        protected virtual void DeserializePresenterState(string mergedState)
        {
            if (mergedState == null)
                throw new ArgumentNullException(nameof(mergedState));

            string[] StateList = mergedState.Split(',');

            int Index = 0;
            if (Index < StateList.Length)
                DeserializeThemeState(StateList[Index++]);
            if (Index < StateList.Length)
                DeserializeCompilerState(StateList[Index++]);
        }

        /// <summary>
        /// Deserializes the theme state.
        /// </summary>
        /// <param name="state">The serialized state.</param>
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

        /// <summary>
        /// Deserializes the compiler state.
        /// </summary>
        /// <param name="state">The serialized state.</param>
        protected virtual void DeserializeCompilerState(string state)
        {
            bool BoolValue;
            if (bool.TryParse(state, out BoolValue))
                SaveBeforeCompiling = BoolValue;
        }

        /// <summary>
        /// Resets the dock manager state.
        /// </summary>
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

        /// <summary>
        /// Resets the dock manager state.
        /// </summary>
        /// <param name="resourceName">The resource name.</param>
        protected virtual void ResetDockManagerStateFromResource(string resourceName)
        {
            using (Stream ResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                XmlLayoutSerializer DockManagerSerializer = new XmlLayoutSerializer(dockManager);
                DockManagerSerializer.Deserialize(ResourceStream);
            }
        }

        /// <summary>
        /// Resets the toolbar state.
        /// </summary>
        protected virtual void ResetToolBarState()
        {
            toolbarMain.Reset();
        }

        /// <summary>
        /// Resets the presenter state.
        /// </summary>
        protected virtual void ResetPresenterState()
        {
            ResetThemeState();
            ResetCompilerState();
        }

        /// <summary>
        /// Resets the theme state.
        /// </summary>
        protected virtual void ResetThemeState()
        {
            ThemeOption = default(ThemeOption);
        }

        /// <summary>
        /// Resets the compiler state.
        /// </summary>
        protected virtual void ResetCompilerState()
        {
            SaveBeforeCompiling = true;
        }
        #endregion

        #region Command
        /// <summary>
        /// Update document type commands.
        /// </summary>
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
                DocumentRoutedCommand? PreferredDocumentCommand = null;

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

        /// <summary>
        /// Removes the control associated to a document.
        /// </summary>
        /// <param name="firstSeparator">The first separator.</param>
        protected virtual void RemoveDocumentControls(Separator firstSeparator)
        {
            if (firstSeparator == null)
                throw new ArgumentNullException(nameof(firstSeparator));

            ItemsControl Container = (ItemsControl)firstSeparator.Parent;
            ItemCollection Items = Container.Items;
            int FirstIndex = Items.IndexOf(firstSeparator) + 1;

            while (FirstIndex < Items.Count && !(Items[FirstIndex] is Separator))
                Items.RemoveAt(FirstIndex);
        }

        /// <summary>
        /// Creates a create document command.
        /// </summary>
        /// <param name="documentType">The document type.</param>
        /// <returns>The create document command.</returns>
        protected virtual DocumentRoutedCommand CreateDocumentCommand(IDocumentType documentType)
        {
            return new DocumentRoutedCommand(documentType);
        }

        /// <summary>
        /// Creates the menu item associated to a document.
        /// </summary>
        /// <param name="documentCommand">The menu command.</param>
        /// <returns>The menu item.</returns>
        protected virtual ExtendedToolBarMenuItem CreateDocumentMenuItem(ICommand documentCommand)
        {
            ExtendedToolBarMenuItem DocumentMenuItem = new ExtendedToolBarMenuItem();
            DocumentMenuItem.Command = documentCommand;
            return DocumentMenuItem;
        }

        /// <summary>
        /// Creates the toolbar button associated to a document.
        /// </summary>
        /// <param name="documentCommand">The menu command.</param>
        /// <returns>The button.</returns>
        protected virtual ExtendedToolBarButton CreateDocumentToolBarButton(ICommand documentCommand)
        {
            ExtendedToolBarButton DocumentToolBarButton = new ExtendedToolBarButton();
            DocumentToolBarButton.Command = documentCommand;
            return DocumentToolBarButton;
        }

        /// <summary>
        /// Adds the control associated to a document.
        /// </summary>
        /// <param name="firstSeparator">The first separator.</param>
        /// <param name="index">Index of the document.</param>
        /// <param name="documentControl">The control.</param>
        protected virtual void AddDocumentControls(Separator firstSeparator, int index, FrameworkElement documentControl)
        {
            if (firstSeparator == null)
                throw new ArgumentNullException(nameof(firstSeparator));

            ItemsControl Container = (ItemsControl)firstSeparator.Parent;
            ItemCollection Items = Container.Items;
            int FirstIndex = Items.IndexOf(firstSeparator) + 1;

            Items.Insert(FirstIndex + index, documentControl);
        }

        /// <summary>
        /// Removes the command binding associated to a document.
        /// </summary>
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

        /// <summary>
        /// Adds the command binding associated to a document.
        /// </summary>
        /// <param name="newDocumentCommand">The command.</param>
        protected virtual void AddDocumentCommandBinding(DocumentRoutedCommand newDocumentCommand)
        {
            CommandBinding NewDocumentBinding = new CommandBinding(newDocumentCommand, OnAddNewDocument, CanAddNewDocument);
            CommandBindings.Add(NewDocumentBinding);
        }

        /// <summary>
        /// Removes the key binding associated to a document.
        /// </summary>
        protected virtual void RemoveDocumentKeyBindings()
        {
            foreach (InputBinding Binding in InputBindings)
                if (Binding is KeyBinding AsKeyBinding)
                    if (AsKeyBinding.Command is DocumentRoutedCommand AsDocumentCommand)
                        if (AsDocumentCommand.DocumentType.IsPreferred)
                        {
                            InputBindings.Remove(AsKeyBinding);
                            break;
                        }
        }

        /// <summary>
        /// Adds the key binding associated to a document.
        /// </summary>
        /// <param name="newDocumentCommand">The command.</param>
        protected virtual void AddDocumentKeyBinding(DocumentRoutedCommand newDocumentCommand)
        {
            KeyBinding NewDocumentBinding = new KeyBinding(newDocumentCommand, new KeyGesture(Key.N, ModifierKeys.Control));
            InputBindings.Add(NewDocumentBinding);
        }

        /// <summary>
        /// Called to check if a new document can be added.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void CanAddNewDocument(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (DocumentCreatedEventArgs.HasHandler)
                e.CanExecute = true;
        }

        /// <summary>
        /// Called when a new documents is added.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnAddNewDocument(object sender, ExecutedRoutedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (DocumentCreatedEventArgs.HasHandler)
            {
                IFolderPath DestinationPath;

                if (SolutionExplorer.GetEventSource(e) is IFolderPath AsFolderPath)
                    DestinationPath = AsFolderPath;
                else
                    DestinationPath = RootPath;

                DocumentRoutedCommand Command = (DocumentRoutedCommand)e.Command;
                NotifyDocumentCreated(DestinationPath, Command.DocumentType, RootProperties);
            }
        }

        /// <summary>
        /// Called when a new documents has been created.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnDocumentCreatedComplete(object sender, SolutionPresenterEventCompletedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            DocumentCreatedEventContext EventContext = (DocumentCreatedEventContext)e.EventContext;
            IDocumentCreatedCompletionArgs CompletionArgs = (IDocumentCreatedCompletionArgs)e.CompletionArgs;
            IFolderPath DestinationFolderPath = EventContext.DestinationFolderPath;
            IItemPath NewItemPath = CompletionArgs.NewItemPath;
            IItemProperties NewItemProperties = CompletionArgs.NewItemProperties;

            spcSolutionExplorer.AddItem(DestinationFolderPath, NewItemPath, NewItemProperties);
        }
        #endregion

        #region Command: File / Create New Solution
        /// <summary>
        /// Called to check if a new solution can be created.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void CanCreateNewSolution(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (SolutionCreatedEventArgs.HasHandler)
                e.CanExecute = true;
        }

        /// <summary>
        /// Called when a new solution should be created.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnCreateNewSolution(object sender, ExecutedRoutedEventArgs e)
        {
            IRootPath ClosedRootPath = RootPath;

            CommitInfo Info = CheckToSaveCurrentSolution();

            if (Info.Option == CommitOption.CommitAndContinue)
                NotifySolutionTreeCommitted(Info, SolutionOperation.Create, ClosedRootPath, new EmptyPath(), string.Empty);
            else if (Info.Option == CommitOption.Continue)
                NotifySolutionClosed(SolutionOperation.Create, ClosedRootPath, new EmptyPath());
        }

        /// <summary>
        /// Called when a new solution has been created.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnSolutionCreatedComplete(object sender, SolutionPresenterEventCompletedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            ISolutionCreatedCompletionArgs CompletionArgs = (ISolutionCreatedCompletionArgs)e.CompletionArgs;

            if (CompletionArgs.CreatedRootPath is IRootPath CreatedRootPath)
                NotifySolutionOpened(CreatedRootPath);
        }
        #endregion

        #region Command: File / Open Solution
        /// <summary>
        /// Called to check if a solution can be opened.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void CanOpenSolution(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (SolutionSelectedEventArgs.HasHandler && SolutionOpenedEventArgs.HasHandler)
                e.CanExecute = true;
        }

        /// <summary>
        /// Called when a solution should be opened.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnOpenSolution(object sender, ExecutedRoutedEventArgs e)
        {
            NotifySolutionSelected();
        }

        /// <summary>
        /// Called when a solution has been opened.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnSolutionSelectedComplete(object sender, SolutionPresenterEventCompletedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            ISolutionSelectedCompletionArgs CompletionArgs = (ISolutionSelectedCompletionArgs)e.CompletionArgs;
            if (CompletionArgs.SelectedRootPath is IRootPath SelectedRootPath)
            {
                IRootPath ClosedRootPath = RootPath;

                CommitInfo Info = CheckToSaveCurrentSolution();

                if (Info.Option == CommitOption.CommitAndContinue)
                    NotifySolutionTreeCommitted(Info, SolutionOperation.Open, ClosedRootPath, SelectedRootPath, string.Empty);
                else if (Info.Option == CommitOption.Continue)
                    NotifySolutionClosed(SolutionOperation.Open, ClosedRootPath, SelectedRootPath);
            }
        }

        /// <summary>
        /// Called when a solution has been opened.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnSolutionOpenedComplete(object sender, SolutionPresenterEventCompletedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            SolutionOpenedEventContext EventContext = (SolutionOpenedEventContext)e.EventContext;
            ISolutionOpenedCompletionArgs CompletionArgs = (ISolutionOpenedCompletionArgs)e.CompletionArgs;
            IRootPath OpenedRootPath = EventContext.OpenedRootPath;
            IRootProperties? OpenedRootProperties = CompletionArgs.OpenedRootProperties;
            IComparer<ITreeNodePath>? OpenedRootComparer = CompletionArgs.OpenedRootComparer;
            IList<IFolderPath>? ExpandedFolderList = CompletionArgs.ExpandedFolderList;
            object? Context = CompletionArgs.Context;

            SetValue(TreeNodeComparerPropertyKey, OpenedRootComparer);
            LoadTree(OpenedRootPath, OpenedRootProperties, OpenedRootComparer, ExpandedFolderList, Context);
        }
        #endregion

        #region Command: File / Open Existing Document
        /// <summary>
        /// Called to check if a document can be opened.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void CanOpenExistingDocument(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (DocumentSelectedEventArgs.HasHandler)
                e.CanExecute = true;
        }

        /// <summary>
        /// Called when a document should be opened.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnOpenExistingDocument(object sender, ExecutedRoutedEventArgs e)
        {
            NotifyDocumentSelected();
        }

        /// <summary>
        /// Called when a document has been opened.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnDocumentSelectedComplete(object sender, SolutionPresenterEventCompletedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            IDocumentSelectedCompletionArgs CompletionArgs = (IDocumentSelectedCompletionArgs)e.CompletionArgs;
            IList<IDocumentPath> DocumentPathList = CompletionArgs.DocumentPathList;
            OpenNextDocument(DocumentPathList);
        }
        #endregion

        #region Command: File / Close Document
        /// <summary>
        /// Called to check if a document can be closed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void CanCloseDocument(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (ActiveDocument != null)
                e.CanExecute = true;
        }

        /// <summary>
        /// Called when a document should be closed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnCloseDocument(object sender, ExecutedRoutedEventArgs e)
        {
            if (ActiveDocument != null)
                if (ActiveDocument.IsDirty)
                {
                    CommitOption Option = IsSingleDocumentSaveConfirmed(ActiveDocument);

                    if (Option == CommitOption.CommitAndContinue)
                        NotifyDocumentSaved(DocumentOperation.Close, ActiveDocument, string.Empty);
                    else if (Option == CommitOption.Continue)
                        NotifyDocumentClosed(DocumentOperation.Close, ActiveDocument);
                }
                else
                    NotifyDocumentClosed(DocumentOperation.Close, ActiveDocument);
        }

        /// <summary>
        /// Called when a document has been closed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnDocumentClosedComplete(object sender, SolutionPresenterEventCompletedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            DocumentClosedEventContext EventContext = (DocumentClosedEventContext)e.EventContext;
            DocumentOperation DocumentOperation = EventContext.DocumentOperation;
            IList<IDocument> ClosedDocumentList = EventContext.ClosedDocumentList;
            IReadOnlyDictionary<ITreeNodePath, IPathConnection> ClosedTree = EventContext.ClosedTree;
            bool IsUndoRedo = EventContext.IsUndoRedo;
            object? ClientInfo = EventContext.ClientInfo;

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
        /// <summary>
        /// Called to check if a solution can be closed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void CanCloseSolution(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (SolutionClosedEventArgs.HasHandler)
                e.CanExecute = true;
        }

        /// <summary>
        /// Called when a solution should be closed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnCloseSolution(object sender, ExecutedRoutedEventArgs e)
        {
            IRootPath ClosedRootPath = RootPath;

            CommitInfo Info = CheckToSaveCurrentSolution();

            if (Info.Option == CommitOption.CommitAndContinue)
                NotifySolutionTreeCommitted(Info, SolutionOperation.Close, ClosedRootPath, new EmptyPath(), string.Empty);
            else if (Info.Option == CommitOption.Continue)
                NotifySolutionClosed(SolutionOperation.Close, ClosedRootPath, new EmptyPath());
        }

        /// <summary>
        /// Called when a solution has been closed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnSolutionClosedComplete(object sender, SolutionPresenterEventCompletedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            SolutionClosedEventContext EventContext = (SolutionClosedEventContext)e.EventContext;
            SolutionOperation SolutionOperation = EventContext.SolutionOperation;
            IRootPath ClosedRootPath = EventContext.ClosedRootPath;
            IRootPath NewRootPath = EventContext.NewRootPath;

            IReadOnlyCollection<ITreeNodePath>? DeletedTree;
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
        /// <summary>
        /// Called to check if a solution can be saved.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void CanSaveDocument(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (ActiveDocument != null)
                e.CanExecute = true;
        }

        /// <summary>
        /// Called when a solution should be saved.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnSaveDocument(object sender, ExecutedRoutedEventArgs e)
        {
            if (ActiveDocument != null)
                NotifyDocumentSaved(DocumentOperation.Save, ActiveDocument, string.Empty);
        }

        /// <summary>
        /// Called when a solution has been saved.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnDocumentSavedComplete(object sender, SolutionPresenterEventCompletedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

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
        /// <summary>
        /// Called to check if all documents can be saved.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void CanSaveAll(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            CommitInfo Info = GetDirtyObjects();

            if ((Info.DirtyItemList != null && Info.DirtyItemList.Count > 0) || (Info.DirtyPropertiesList != null && Info.DirtyPropertiesList.Count > 0) || (Info.DirtyDocumentList != null && Info.DirtyDocumentList.Count > 0))
                e.CanExecute = true;
        }

        /// <summary>
        /// Called when all documents should be saved.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnSaveAll(object sender, ExecutedRoutedEventArgs e)
        {
            CommitInfo Info = GetDirtyObjects();

            if ((Info.DirtyItemList != null && Info.DirtyItemList.Count > 0) || (Info.DirtyPropertiesList != null && Info.DirtyPropertiesList.Count > 0) || (Info.DirtyDocumentList != null && Info.DirtyDocumentList.Count > 0))
                NotifySolutionTreeCommitted(Info, SolutionOperation.Save, new EmptyPath(), new EmptyPath(), string.Empty);
        }
        #endregion

        #region Command: File / Import
        /// <summary>
        /// Called to check if items can be imported.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void CanImport(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (DocumentImportDescriptors != null && DocumentImportDescriptors.Count > 0)
                e.CanExecute = true;
        }

        /// <summary>
        /// Called when importing items.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
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

            string FolderPattern = SolutionPresenterInternal.Properties.Resources.FolderAndSubfolders;
            OpenFileDialog Dlg = new OpenFileDialog();
            Dlg.DefaultExt = "*." + Filter.DefaultExtension;
            Dlg.Filter = CompleteFilter;
            Dlg.Multiselect = true;
            Dlg.FileName = FolderPattern;
            Dlg.CheckFileExists = false;

            if (ImportFolder != null && Directory.Exists(ImportFolder))
                Dlg.InitialDirectory = ImportFolder;
            else
                Dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            bool? Result = Dlg.ShowDialog();
            if (Result.HasValue && Result.Value && Dlg.FileNames.Length > 0)
            {
                List<string> FileNames = new List<string>(Dlg.FileNames);

                string LastFileName = Dlg.FileName;
                if (LastFileName != null)
                {
                    ImportFolder = Path.GetDirectoryName(LastFileName);

                    string LastFileRoot = Path.GetFileNameWithoutExtension(LastFileName);
                    if (FileNames.Count == 1 && LastFileRoot == FolderPattern)
                    {
                        FileNames.Clear();
                        AddAllFiles(FileNames, ImportFolder, Filter.DefaultExtension);
                    }
                }

                Dictionary<object, IDocumentType> ImportedDocumentTable = new Dictionary<object, IDocumentType>();
                foreach (string FileName in FileNames)
                {
                    string FileExtension = Path.GetExtension(FileName);
                    foreach (IDocumentImportDescriptor Descriptor in DocumentImportDescriptors)
                        if ("." + Descriptor.FileExtension == FileExtension)
                        {
                            ImportedContentDescriptor ContentDescriptor = Descriptor.Import(FileName);
                            if (ContentDescriptor != null)
                                ImportedDocumentTable.Add(ContentDescriptor.ImportedContent, ContentDescriptor.DocumentType);

                            break;
                        }
                }

                if (ImportedDocumentTable.Count > 0)
                    NotifyImportNewItemsRequested(ImportedDocumentTable, new List<IDocumentPath>());
            }
        }

        private void AddAllFiles(List<string> fileNames, string rootFolder, string extension)
        {
            foreach (string FileName in Directory.GetFiles(rootFolder, "*." + extension))
                fileNames.Add(FileName);

            foreach (string FolderName in Directory.GetDirectories(rootFolder))
                AddAllFiles(fileNames, FolderName, extension);
        }

        private DocumentTypeFilter GetFilters()
        {
            string DefaultExtension = string.Empty;
            Dictionary<string, string> FileExtensionTable = new Dictionary<string, string>();
            foreach (IDocumentImportDescriptor Descriptor in DocumentImportDescriptors)
            {
                if (!FileExtensionTable.ContainsKey(Descriptor.FileExtension))
                    FileExtensionTable.Add(Descriptor.FileExtension, Descriptor.FriendlyImportName);

                if (DefaultExtension.Length == 0 || Descriptor.IsDefault)
                    DefaultExtension = Descriptor.FileExtension;
            }

            string FilterString = string.Empty;
            foreach (KeyValuePair<string, string> Entry in FileExtensionTable)
            {
                if (FilterString.Length > 0)
                    FilterString += "|";

                FilterString += Entry.Value + " (*." + Entry.Key + ")|*." + Entry.Key;
            }

            return new DocumentTypeFilter(FilterString, DefaultExtension);
        }

        /// <summary>
        /// Called when importing items have been imported.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnImportNewItemsRequestedComplete(object sender, SolutionPresenterEventCompletedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

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
                    NotifyDocumentOpened(DocumentOperation.Open, new EmptyPath(), DocumentPathList, new List<IDocumentPath>(), null);
            }
        }
        #endregion

        #region Command: File / Import Solution
        /// <summary>
        /// Called to check if a solution can be imported.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void CanImportSolution(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (DocumentImportDescriptors != null && DocumentImportDescriptors.Count > 0)
                e.CanExecute = true;
        }

        /// <summary>
        /// Called when a solution should be imported.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnImportSolution(object sender, ExecutedRoutedEventArgs e)
        {
            if (DocumentImportDescriptors == null || DocumentImportDescriptors.Count == 0)
                return;

            string CompleteFilter = string.Empty;
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
                    string Question = string.Format(CultureInfo.CurrentCulture, QuestionFormat, SolutionName);

                    if (MessageBox.Show(Question, Owner.Title, MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                    {
                        Package.InitCreationTime();
                        spcSolutionExplorer.ReadImportedSolutionPackage(Package, Dlg.FileName);
                    }
                }
            }
        }

        /// <summary>
        /// Called when a solution has been imported.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnImportSolutionRequestedComplete(object sender, SolutionPresenterEventCompletedEventArgs e)
        {
        }
        #endregion

        #region Command: File / Export Document
        /// <summary>
        /// Called to check if a document can be exported.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void CanExportDocument(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (DocumentImportDescriptors != null && DocumentImportDescriptors.Count > 0 && ActiveDocument != null)
                e.CanExecute = true;
        }

        /// <summary>
        /// Called when a document should be exported.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
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

        /// <summary>
        /// Called when a document has been exported.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnDocumentExportedComplete(object sender, SolutionPresenterEventCompletedEventArgs e)
        {
        }
        #endregion

        #region Command: File / Export All
        /// <summary>
        /// Called to check if all items can be exported.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void CanExportAll(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (DocumentImportDescriptors != null && DocumentImportDescriptors.Count > 0 && OpenDocuments.Count > 0)
                e.CanExecute = true;
        }

        /// <summary>
        /// Called when all items are exported.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
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
                            NotifySolutionTreeCommitted(Info, SolutionOperation.ExportDocument, new EmptyPath(), new EmptyPath(), ExportFolder);
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
        /// <summary>
        /// Called to check if a solution can be exported.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void CanExportSolution(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (SolutionExportedEventArgs.HasHandler && SolutionExtension != null && SolutionExtensionFilter != null)
                e.CanExecute = true;
        }

        /// <summary>
        /// Called when a solution should be exported.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnExportSolution(object sender, ExecutedRoutedEventArgs e)
        {
            IRootPath ExportedRootPath = RootPath;

            CommitInfo Info = CheckToSaveCurrentSolution();

            if (Info.Option == CommitOption.CommitAndContinue)
                NotifySolutionTreeCommitted(Info, SolutionOperation.ExportSolution, ExportedRootPath, new EmptyPath(), string.Empty);
            else if (Info.Option == CommitOption.Continue)
                ExportSolution(ExportedRootPath);
        }

        private void ExportSolution(IRootPath exportedRootPath)
        {
            string DestinationPath = GetSolutionExportFileName(exportedRootPath.FriendlyName);
            if (DestinationPath == null)
                return;

            NotifySolutionExported(exportedRootPath, DestinationPath);
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
                return string.Empty;
        }

        /// <summary>
        /// Called when a solution has been exported.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnSolutionExportedComplete(object sender, SolutionPresenterEventCompletedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            SolutionExportedEventContext EventContext = (SolutionExportedEventContext)e.EventContext;
            string DestinationPath = EventContext.DestinationPath;
            SolutionExportedCompletionArgs Args = (SolutionExportedCompletionArgs)e.CompletionArgs;

            if (Args.ContentTable is Dictionary<IDocumentPath, byte[]> ContentTable)
                spcSolutionExplorer.CreateExportedSolutionPackage(DestinationPath, ContentTable);
        }
        #endregion

        #region Command: File / Exit
        /// <summary>
        /// Called to check if the application can exit.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void CanExit(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            e.CanExecute = true;
        }

        /// <summary>
        /// Called to exit the application.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnExit(object sender, ExecutedRoutedEventArgs e)
        {
            CommitInfo Info = CheckToSaveCurrentSolution();

            if (Info.Option == CommitOption.CommitAndContinue)
                NotifySolutionTreeCommitted(Info, SolutionOperation.Exit, new EmptyPath(), new EmptyPath(), string.Empty);
            else if (Info.Option == CommitOption.Continue)
            {
                spcSolutionExplorer.ClearDirtyItemsAndProperties();
                ClearDirtyDocuments();

                NotifyExitRequested();
            }
        }
        #endregion

        #region Command: Undo
        /// <summary>
        /// Called to check if the last operation can be undone.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void CanUndo(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (dockManager.ActiveContent is IDocument Document)
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

        /// <summary>
        /// Called when the last operation is undone.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnUndo(object sender, ExecutedRoutedEventArgs e)
        {
            if (dockManager.ActiveContent is IDocument Document)
                Document.OnUndo();
            else if (dockManager.ActiveContent == spcSolutionExplorer)
                spcSolutionExplorer.Undo();
        }
        #endregion

        #region Command: Redo
        /// <summary>
        /// Called to check if the last operation can be redone.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void CanRedo(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (dockManager.ActiveContent is IDocument Document)
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

        /// <summary>
        /// Called when the last operation is redone.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnRedo(object sender, ExecutedRoutedEventArgs e)
        {
            if (dockManager.ActiveContent is IDocument Document)
                Document.OnRedo();
            else if (dockManager.ActiveContent == spcSolutionExplorer)
                spcSolutionExplorer.Redo();
        }
        #endregion

        #region Command: Select All
        /// <summary>
        /// Called to check if all items can be selected.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void CanSelectAll(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (dockManager.ActiveContent is IDocument Document)
            {
                if (Document.CanSelectAll())
                    e.CanExecute = true;
            }
            else if (dockManager.ActiveContent == spcSolutionExplorer)
                e.CanExecute = true;
        }

        /// <summary>
        /// Called to when all items are selected.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnSelectAll(object sender, ExecutedRoutedEventArgs e)
        {
            if (dockManager.ActiveContent is IDocument Document)
                Document.OnSelectAll();
            else if (dockManager.ActiveContent == spcSolutionExplorer)
                spcSolutionExplorer.SelectAll();
        }
        #endregion

        #region Command: Edit / Change Options
        /// <summary>
        /// Called to check if options can be changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void CanChangeOptions(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            e.CanExecute = true;
        }

        /// <summary>
        /// Called when options are changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
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

        /// <summary>
        /// Called when options are changed.
        /// </summary>
        /// <param name="optionDialog">The dialog with current options.</param>
        protected virtual void UpdatePresenterOptions(OptionsWindow optionDialog)
        {
            if (optionDialog == null)
                throw new ArgumentNullException(nameof(optionDialog));

            if (ThemeOption != optionDialog.Theme)
            {
                ThemeOption = optionDialog.Theme;
#if CHECKTHIS
                //UpdateTheme();
#endif
            }

            SaveBeforeCompiling = optionDialog.SaveBeforeCompiling;
        }

        /// <summary>
        /// Gets the option page index.
        /// </summary>
        protected int OptionPageIndex { get; private set; }
        #endregion

        #region Command: Project / Build Solution
        /// <summary>
        /// Called to check if the solution can be built.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void CanBuildSolution(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (RootPath != null && BuildSolutionRequestedEventArgs.HasHandler)
                e.CanExecute = true;
        }

        /// <summary>
        /// Called when building the solution.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnBuildSolution(object sender, ExecutedRoutedEventArgs e)
        {
            if (RootPath != null && BuildSolutionRequestedEventArgs.HasHandler)
            {
                CommitInfo Info = CheckToSaveCurrentSolution();

                if (Info.Option == CommitOption.CommitAndContinue)
                    NotifySolutionTreeCommitted(Info, SolutionOperation.Build, new EmptyPath(), new EmptyPath(), string.Empty);
                else if (Info.Option == CommitOption.Continue)
                    NotifyBuildSolutionRequested();
            }
        }

        /// <summary>
        /// Called when building the solution is completed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnBuildSolutionRequestedComplete(object sender, SolutionPresenterEventCompletedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            IBuildSolutionRequestedCompletionArgs CompletionArgs = (IBuildSolutionRequestedCompletionArgs)e.CompletionArgs;
            IReadOnlyList<ICompilationError> ErrorList = CompletionArgs.ErrorList;

            CompilationErrorList.Clear();
            foreach (ICompilationError Error in ErrorList)
                CompilationErrorList.Add(Error);
        }
        #endregion

        #region Command: Project / Change Properties
        /// <summary>
        /// Called to check if properties can be changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void CanChangeProperties(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (RootPath != null && RootPropertiesRequestedEventArgs.HasHandler)
                e.CanExecute = true;
        }

        /// <summary>
        /// Called when properties should be changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnChangeProperties(object sender, ExecutedRoutedEventArgs e)
        {
            if (RootPath != null && RootPropertiesRequestedEventArgs.HasHandler)
                NotifyRootPropertiesRequested(RootProperties);
        }

        /// <summary>
        /// Called when properties have been changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnRootPropertiesRequestedComplete(object sender, SolutionPresenterEventCompletedEventArgs e)
        {
        }
        #endregion

        #region Command: Window / Show Solution Explorer Tool
        /// <summary>
        /// Called to check if the solution explorer can be shown.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void CanShowSolutionExplorerTool(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            e.CanExecute = true;
        }

        /// <summary>
        /// Called to show the solution explorer.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnShowSolutionExplorerTool(object sender, ExecutedRoutedEventArgs e)
        {
            ShowTool("toolSolutionExplorer", ToolOperation.Show);
        }
        #endregion

        #region Command: Window / Show Compiler Output Tool
        /// <summary>
        /// Called to check if the compiler output can be shown.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void CanShowCompilerOutputTool(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            e.CanExecute = true;
        }

        /// <summary>
        /// Called to show the compiler output.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnShowCompilerOutputTool(object sender, ExecutedRoutedEventArgs e)
        {
            ShowTool("toolCompilerOutput", ToolOperation.Show);
        }
        #endregion

        #region Command: Window / Show Properties Tool
        /// <summary>
        /// Called to check if the properties tool can be shown.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void CanShowPropertiesTool(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            e.CanExecute = true;
        }

        /// <summary>
        /// Called to show the properties tool.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnShowPropertiesTool(object sender, ExecutedRoutedEventArgs e)
        {
            ShowTool("toolProperties", ToolOperation.Toggle);
        }
        #endregion

        #region Command: Window / Reset Layout
        /// <summary>
        /// Called to check if the layout can be reset.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void CanResetLayout(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            e.CanExecute = true;
        }

        /// <summary>
        /// Called to reset the layout.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnResetLayout(object sender, ExecutedRoutedEventArgs e)
        {
            ResetDockManagerState();
        }
        #endregion

        #region Command: Window / Split Window
        /// <summary>
        /// Called to check if the current document window can be split.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void CanSplitWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (ActiveDocument != null)
                if (GetActiveControl() is SplitView Ctrl && !Ctrl.IsSplitRemovable)
                    e.CanExecute = true;
        }

        /// <summary>
        /// Called to split the current document window.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnSplitWindow(object sender, ExecutedRoutedEventArgs e)
        {
            if (ActiveDocument != null)
                if (GetActiveControl() is SplitView Ctrl)
                    Ctrl.Split();
        }
        #endregion

        #region Command: Window / Remove Window Split
        /// <summary>
        /// Called to check if the current document window split can be removed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void CanRemoveWindowSplit(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (ActiveDocument != null)
                if (GetActiveControl() is SplitView Ctrl && Ctrl.IsSplitRemovable)
                    e.CanExecute = true;
        }

        /// <summary>
        /// Called to remove the current document window split.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnRemoveWindowSplit(object sender, ExecutedRoutedEventArgs e)
        {
            if (ActiveDocument != null)
                if (GetActiveControl() is SplitView Ctrl)
                    Ctrl.RemoveSplit();
        }
        #endregion

        #region Command: Window / List Windows
        /// <summary>
        /// Called to check if windows can be listed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void CanListWindows(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            e.CanExecute = true;
        }

        /// <summary>
        /// Called when windows should be listed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnListWindows(object sender, ExecutedRoutedEventArgs e)
        {
            DocumentsWindow Dlg = new DocumentsWindow(Documents);
            Dlg.Owner = Owner;
            Dlg.DocumentActivated += OnDocumentActivated;
            Dlg.DocumentSaved += OnDocumentSaved;
            Dlg.DocumentClosed += OnDocumentClosed;
            Dlg.ShowDialog();
        }

        /// <summary>
        /// Called when a document is activated.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnDocumentActivated(object sender, DocumentWindowEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            UserActivateDocument(e.Document);
        }

        /// <summary>
        /// Called when a document is saved.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnDocumentSaved(object sender, DocumentWindowEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            NotifyDocumentSaved(DocumentOperation.Save, e.Document, string.Empty);
        }

        /// <summary>
        /// Called when a document is closed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnDocumentClosed(object sender, DocumentWindowEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (e.Document.IsDirty)
            {
                List<IDocument> DirtyDocumentList = new List<IDocument>();
                DirtyDocumentList.Add(e.Document);

                CommitInfo Info = new CommitInfo(CommitOption.Stop, new List<ITreeNodePath>(), new List<ITreeNodePath>(), DirtyDocumentList);
                CommitOption Option = IsMultipleSaveConfirmed(Info);

                if (Option == CommitOption.CommitAndContinue)
                    NotifyDocumentSaved(DocumentOperation.Close, e.Document, string.Empty);
                else if (Option == CommitOption.Continue)
                    NotifyDocumentClosed(DocumentOperation.Close, e.Document);
            }
            else
                NotifyDocumentClosed(DocumentOperation.Close, e.Document);
        }
        #endregion

        #region Command: Window / Activate Next Window
        /// <summary>
        /// Called to check if the next window can be activated.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void CanActivateNextWindow(object sender, CanExecuteRoutedEventArgs e)
        {
        }

        /// <summary>
        /// Called to activate the next window.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnActivateNextWindow(object sender, ExecutedRoutedEventArgs e)
        {
            ChangeActiveDocument(+1);
        }
        #endregion

        #region Command: Window / Activate Previous Window
        /// <summary>
        /// Called to check if the previous window can be activated.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void CanActivatePreviousWindow(object sender, CanExecuteRoutedEventArgs e)
        {
        }

        /// <summary>
        /// Called to activate the previous window.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnActivatePreviousWindow(object sender, ExecutedRoutedEventArgs e)
        {
            ChangeActiveDocument(-1);
        }
        #endregion

        #region Command: Help / Show About
        /// <summary>
        /// Called to check if the about window can be shown.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void CanShowAbout(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            e.CanExecute = true;
        }

        /// <summary>
        /// Called to show the about window.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnShowAbout(object sender, ExecutedRoutedEventArgs e)
        {
            NotifyShowAboutRequested();
        }
        #endregion

        #region Command: Context / Add Existing Item
        /// <summary>
        /// Called to check if an existing item can be added.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void CanAddExistingItem(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (AddNewItemsRequestedEventArgs.HasHandler)
                if (spcSolutionExplorer.SelectedFolder is IFolderPath)
                    e.CanExecute = true;
        }

        /// <summary>
        /// Called when an existing item should be added.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnAddExistingItem(object sender, ExecutedRoutedEventArgs e)
        {
            if (spcSolutionExplorer.SelectedFolder is IFolderPath DestinationFolderPath)
                NotifyAddNewItemsRequested(DestinationFolderPath);
        }

        /// <summary>
        /// Called when an existing item has been added.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnAddNewItemsRequestedComplete(object sender, SolutionPresenterEventCompletedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            IAddNewItemsRequestedEventContext EventContext = (IAddNewItemsRequestedEventContext)e.EventContext;
            IAddNewItemsRequestedCompletionArgs CompletionArgs = (IAddNewItemsRequestedCompletionArgs)e.CompletionArgs;
            IFolderPath DestinationFolderPath = EventContext.DestinationFolderPath;
            IList<IDocumentPath> DocumentPathList = CompletionArgs.DocumentPathList;

            AddNextDocument(DocumentOperation.Add, DestinationFolderPath, DocumentPathList, RootProperties);
        }

        /// <summary>
        /// Adds the next document in a add operation.
        /// </summary>
        /// <param name="documentOperation">The document operation.</param>
        /// <param name="destinationFolderPath">The destination folder path.</param>
        /// <param name="documentPathList">The list of documents to add.</param>
        /// <param name="rootProperties">The root properties.</param>
        protected virtual void AddNextDocument(DocumentOperation documentOperation, IFolderPath destinationFolderPath, IList<IDocumentPath> documentPathList, IRootProperties rootProperties)
        {
            if (documentPathList == null)
                throw new ArgumentNullException(nameof(documentPathList));

            if (documentPathList.Count > 0)
                NotifyDocumentAdded(documentOperation, destinationFolderPath, documentPathList, rootProperties);
        }

        /// <summary>
        /// Called when a document has been added.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnDocumentAddedComplete(object sender, SolutionPresenterEventCompletedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

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
        /// <summary>
        /// Called to check if a folder can be added.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void CanAddNewFolder(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (dockManager.ActiveContent == spcSolutionExplorer)
                if (spcSolutionExplorer.ValidEditOperations.AddFolder)
                    if (FolderCreatedEventArgs.HasHandler)
                        e.CanExecute = true;
        }

        /// <summary>
        /// Called when a folder should be added.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnAddNewFolder(object sender, ExecutedRoutedEventArgs e)
        {
            if (dockManager.ActiveContent != spcSolutionExplorer)
                return;

            if (!spcSolutionExplorer.ValidEditOperations.AddFolder)
                return;

            if (!FolderCreatedEventArgs.HasHandler)
                return;

            IFolderPath DestinationPath;

            if (SolutionExplorer.GetEventSource(e) is IFolderPath AsFolderPath)
                DestinationPath = AsFolderPath;
            else
                DestinationPath = RootPath;

            string NewFolderName = GetUniqueName(DestinationPath, SolutionPresenterInternal.Properties.Resources.NewFolder);
            NotifyFolderCreated(DestinationPath, NewFolderName, RootProperties);
        }

        /// <summary>
        /// Gets a unique name.
        /// </summary>
        /// <param name="destinationPath">The destination path.</param>
        /// <param name="originalName">The original name.</param>
        /// <returns>The unique name.</returns>
        protected virtual string GetUniqueName(IFolderPath destinationPath, string originalName)
        {
            if (destinationPath == null)
                return originalName;

            string TentativeName = originalName;

            if (spcSolutionExplorer.GetChildren(destinationPath) is IReadOnlyCollection<ITreeNodePath> Children)
            {
                int Index = 1;

                while (IsNameTaken(Children, TentativeName))
                    TentativeName = string.Format(CultureInfo.CurrentCulture, SolutionPresenterInternal.Properties.Resources.NameCopy, originalName, Index++);
            }

            return TentativeName;
        }

        /// <summary>
        /// Checks if a name is taken.
        /// </summary>
        /// <param name="folderChildren">The list of children with candidate names.</param>
        /// <param name="folderName">The name to check.</param>
        /// <returns>True if the name is taken; otherwise, false.</returns>
        protected virtual bool IsNameTaken(IReadOnlyCollection<ITreeNodePath> folderChildren, string folderName)
        {
            if (folderChildren == null)
                throw new ArgumentNullException(nameof(folderChildren));

            bool FolderNameAlreadyExist = false;
            foreach (ITreeNodePath Path in folderChildren)
                if (Path is IFolderPath AsFolderChild)
                    if (AsFolderChild.FriendlyName == folderName)
                    {
                        FolderNameAlreadyExist = true;
                        break;
                    }

            return FolderNameAlreadyExist;
        }

        /// <summary>
        /// Called when a folder has been added.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnFolderCreatedComplete(object sender, SolutionPresenterEventCompletedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            FolderCreatedEventContext EventContext = (FolderCreatedEventContext)e.EventContext;
            IFolderCreatedCompletionArgs CompletionArgs = (IFolderCreatedCompletionArgs)e.CompletionArgs;
            IFolderPath ParentPath = EventContext.ParentPath;
            IFolderPath NewFolderPath = CompletionArgs.NewFolderPath;
            IFolderProperties NewFolderProperties = CompletionArgs.NewFolderProperties;

            spcSolutionExplorer.AddFolder(ParentPath, NewFolderPath, NewFolderProperties);
        }
        #endregion

        #region Command: Open
        /// <summary>
        /// Called to check if an item can be opened.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void CanOpen(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

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

        /// <summary>
        /// Called to open an item.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
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

        /// <summary>
        /// Calleds to open the next document.
        /// </summary>
        /// <param name="documentPathList">The list of documents.</param>
        protected virtual void OpenNextDocument(IList<IDocumentPath> documentPathList)
        {
            if (documentPathList == null)
                throw new ArgumentNullException(nameof(documentPathList));

            if (documentPathList.Count > 0)
                NotifyDocumentOpened(DocumentOperation.Open, new EmptyPath(), documentPathList, new List<IDocumentPath>(), null);
        }

        /// <summary>
        /// Called when a document has been opened.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnDocumentOpenedComplete(object sender, SolutionPresenterEventCompletedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            DocumentOpenedEventContext EventContext = (DocumentOpenedEventContext)e.EventContext;
            IDocumentOpenedCompletionArgs CompletionArgs = (IDocumentOpenedCompletionArgs)e.CompletionArgs;
            DocumentOperation DocumentOperation = EventContext.DocumentOperation;
            IFolderPath DestinationFolderPath = EventContext.DestinationFolderPath;
            IList<IDocumentPath> DocumentPathList = EventContext.DocumentPathList;
            object? ErrorLocation = EventContext.ErrorLocation;
            IReadOnlyList<IDocument> OpenedDocumentList = CompletionArgs.OpenedDocumentList;

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

        /// <summary>
        /// Called when an error focus has been handled.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnErrorFocusedComplete(object sender, SolutionPresenterEventCompletedEventArgs e)
        {
        }
        #endregion

        #region Command: Cut
        /// <summary>
        /// Called to check if an item can be cut.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void CanCut(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (dockManager.ActiveContent is IDocument Document)
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

        /// <summary>
        /// Called when an item is cut.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnCut(object sender, ExecutedRoutedEventArgs e)
        {
            if (dockManager.ActiveContent is IDocument Document)
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
        /// <summary>
        /// Called to check if an item can be copied.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void CanCopy(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (dockManager.ActiveContent is IDocument Document)
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

        /// <summary>
        /// Called when an item is copied.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnCopy(object sender, ExecutedRoutedEventArgs e)
        {
            if (dockManager.ActiveContent is IDocument Document)
                Document.OnCopy();
            else if (dockManager.ActiveContent == spcSolutionExplorer)
                spcSolutionExplorer.Copy();
        }
        #endregion

        #region Command: Paste
        /// <summary>
        /// Called to check if an item can be pasted.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void CanPaste(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (dockManager.ActiveContent is IDocument Document)
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

        /// <summary>
        /// Called when an item is pasted.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnPaste(object sender, ExecutedRoutedEventArgs e)
        {
            if (dockManager.ActiveContent is IDocument Document)
                Document.OnPaste();
            else if (dockManager.ActiveContent == spcSolutionExplorer)
            {
                if (!NodePastedEventArgs.HasHandler)
                    return;

                IFolderPath DestinationPath;

                if (SolutionExplorer.GetEventSource(e) is IFolderPath AsFolderPath)
                    DestinationPath = AsFolderPath;
                else
                    DestinationPath = RootPath;

                if (SolutionExplorer.ReadClipboard() is ClipboardPathData Data)
                {
                    IPathGroup PathGroup = new PathGroup(Data.PathTable, DestinationPath);
                    IReadOnlyDictionary<ITreeNodePath, IPathConnection> PathTable = PathGroup.PathTable;

                    Dictionary<ITreeNodePath, IFolderPath> ParentTable = new Dictionary<ITreeNodePath, IFolderPath>();
                    foreach (KeyValuePair<ITreeNodePath, IPathConnection> Entry in PathTable)
                        if (Entry.Value.ParentPath is IFolderPath ParentPath)
                            ParentTable.Add(Entry.Key, ParentPath);

                    AddNextNode(DestinationPath, PathTable, ParentTable, false);
                }
            }
        }

        /// <summary>
        /// Adds the next node during a paste operation.
        /// </summary>
        /// <param name="destinationPath">The destination path.</param>
        /// <param name="pathTable">The table of paths.</param>
        /// <param name="parentTable">The table of parents.</param>
        /// <param name="isUndoRedo">True if the operation can be undone.</param>
        protected virtual void AddNextNode(IFolderPath? destinationPath, IReadOnlyDictionary<ITreeNodePath, IPathConnection> pathTable, Dictionary<ITreeNodePath, IFolderPath> parentTable, bool isUndoRedo)
        {
            if (pathTable == null)
                throw new ArgumentNullException(nameof(pathTable));
            if (parentTable == null)
                throw new ArgumentNullException(nameof(parentTable));

            if (parentTable.Count > 0)
            {
                ITreeNodePath? AddedPath = null;
                IFolderPath? ParentPath = null;
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

                if (AddedPath != null && ParentPath != null)
                {
                    string NewName;
                    if (destinationPath == null || isUndoRedo)
                        NewName = AddedPath.FriendlyName;
                    else
                        NewName = GetUniqueName(destinationPath, AddedPath.FriendlyName);

                    if (AddedPath.FriendlyName != NewName)
                        AddedPath.ChangeFriendlyName(NewName);

#if CHECKTHIS
                    IPathConnection AddedNodeData = pathTable[AddedPath];
#endif
                    parentTable.Remove(AddedPath);

                    NotifyNodePasted(AddedPath, ParentPath, pathTable, parentTable, RootProperties, isUndoRedo);
                }
            }
        }

        /// <summary>
        /// Called when an item has been pasted.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnNodePastedComplete(object sender, SolutionPresenterEventCompletedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            NodePastedEventContext EventContext = (NodePastedEventContext)e.EventContext;
            INodePastedCompletionArgs CompletionArgs = (INodePastedCompletionArgs)e.CompletionArgs;
            IFolderPath ParentPath = EventContext.ParentPath;
            IReadOnlyDictionary<ITreeNodePath, IPathConnection> PathTable = EventContext.PathTable;
            Dictionary<ITreeNodePath, IFolderPath> UpdatedParentTable = EventContext.UpdatedParentTable;
            bool IsUndoRedo = EventContext.IsUndoRedo;
            ITreeNodePath NewPath = CompletionArgs.NewPath;
            ITreeNodeProperties NewProperties = CompletionArgs.NewProperties;

            IFolderPath? DestinationPath = null;

            bool IsHandled = false;

            if (NewPath is IFolderPath AsFolderPath && NewProperties is IFolderProperties AsFolderProperties)
            {
                DestinationPath = AsFolderPath;
                if (!IsUndoRedo)
                    spcSolutionExplorer.AddFolder(ParentPath, AsFolderPath, AsFolderProperties);

                IsHandled = true;
            }
            else if (NewPath is IItemPath AsItemPath && NewProperties is IItemProperties AsItemProperties)
            {
                DestinationPath = null;
                if (!IsUndoRedo)
                    spcSolutionExplorer.AddItem(ParentPath, AsItemPath, AsItemProperties);

                IsHandled = true;
            }

            Debug.Assert(IsHandled);

            AddNextNode(DestinationPath, PathTable, UpdatedParentTable, IsUndoRedo);
        }
        #endregion

        #region Command: Delete
        /// <summary>
        /// Called to check if an item can be deleted.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void CanDelete(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (dockManager.ActiveContent is IDocument Document)
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

        /// <summary>
        /// Called when an item should be deleted.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnDelete(object sender, ExecutedRoutedEventArgs e)
        {
            if (dockManager.ActiveContent is IDocument Document)
                Document.CanDelete();
            else if (dockManager.ActiveContent == spcSolutionExplorer)
            {
                IReadOnlyDictionary<ITreeNodePath, IPathConnection> SelectedTree = spcSolutionExplorer.SelectedTree;
                DeletePathList(SelectedTree, false);
            }
        }

        /// <summary>
        /// Deletes a list of items by their path.
        /// </summary>
        /// <param name="deletedTree">The tree to delete.</param>
        /// <param name="isUndoRedo">True if the operation can be undone.</param>
        protected virtual void DeletePathList(IReadOnlyDictionary<ITreeNodePath, IPathConnection> deletedTree, bool isUndoRedo)
        {
            if (deletedTree == null)
                throw new ArgumentNullException(nameof(deletedTree));

            List<IDocument> ClosedDocumentList = new List<IDocument>();
            foreach (IDocument Document in OpenDocuments)
            {
                bool IsClosed = false;
                foreach (KeyValuePair<ITreeNodePath, IPathConnection> Entry in deletedTree)
                    if (Entry.Key is IItemPath AsItemPath)
                        if (AsItemPath.DocumentPath.IsEqual(Document.Path))
                        {
                            IsClosed = true;
                            break;
                        }

                if (IsClosed)
                    ClosedDocumentList.Add(Document);
            }

            if (ClosedDocumentList.Count > 0)
                NotifyDocumentClosed(DocumentOperation.Remove, ClosedDocumentList, deletedTree, isUndoRedo, null);
            else
                NotifyDocumentRemoved(RootPath, deletedTree, isUndoRedo, null);
        }

        /// <summary>
        /// Called when the document associated to a deleted item has been removed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnDocumentRemovedComplete(object sender, SolutionPresenterEventCompletedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            DocumentRemovedEventContext EventContext = (DocumentRemovedEventContext)e.EventContext;
            IReadOnlyDictionary<ITreeNodePath, IPathConnection> DeletedTree = EventContext.DeletedTree;
            bool IsUndoRedo = EventContext.IsUndoRedo;

            if (spcSolutionExplorer.ItemAfterLastSelected is ITreeNodePath ItemAfterLastSelected && !IsUndoRedo)
            {
                spcSolutionExplorer.DeleteTree(DeletedTree);
                spcSolutionExplorer.SetSelected(ItemAfterLastSelected);
            }
        }
        #endregion

        #region Command: Context / Delete Solution
        /// <summary>
        /// Called to check if a solution can be deleted.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void CanDeleteSolution(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (SolutionDeletedEventArgs.HasHandler)
                e.CanExecute = true;
        }

        /// <summary>
        /// Called when a solution is deleted.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnDeleteSolution(object sender, ExecutedRoutedEventArgs e)
        {
            IRootPath DeletedRootPath = RootPath;
            if (!IsDeleteSolutionConfirmed(DeletedRootPath.FriendlyName))
                return;

            CommitInfo Info = CheckToSaveCurrentSolution();

            if (Info.Option == CommitOption.CommitAndContinue)
                NotifySolutionTreeCommitted(Info, SolutionOperation.Delete, DeletedRootPath, new EmptyPath(), string.Empty);
            else if (Info.Option == CommitOption.Continue)
                NotifySolutionClosed(SolutionOperation.Delete, DeletedRootPath, new EmptyPath());
        }

        /// <summary>
        /// Asks the user to confirm the solution can be deleted.
        /// </summary>
        /// <param name="solutionName">The solution name.</param>
        /// <returns>True when confirmed; otherwise, false.</returns>
        protected virtual bool IsDeleteSolutionConfirmed(string solutionName)
        {
            string QuestionFormat = SolutionPresenterInternal.Properties.Resources.SolutionWillBeDeleted;
            string Question = string.Format(CultureInfo.CurrentCulture, QuestionFormat, solutionName);

            MessageBoxResult Result = MessageBox.Show(Question, Owner.Title, MessageBoxButton.OKCancel, MessageBoxImage.Question);
            return Result == MessageBoxResult.OK;
        }

        /// <summary>
        /// Called when a solution has been deleted.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnSolutionDeletedComplete(object sender, SolutionPresenterEventCompletedEventArgs e)
        {
        }
        #endregion

        #region Command: Context / Rename
        /// <summary>
        /// Called to check if a node can be renamed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void CanRename(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (dockManager.ActiveContent == spcSolutionExplorer)
                if (spcSolutionExplorer.ValidEditOperations.Rename)
                    if (NodeRenamedEventArgs.HasHandler)
                        e.CanExecute = true;
        }

        /// <summary>
        /// Called when a node should be renamed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnRename(object sender, ExecutedRoutedEventArgs e)
        {
            if (dockManager.ActiveContent != spcSolutionExplorer)
                return;

            if (!NodeRenamedEventArgs.HasHandler)
                return;

            spcSolutionExplorer.TriggerRename();
        }

        /// <summary>
        /// Called when the name name has changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void OnNodeNameChanged(object sender, RoutedEventArgs e)
        {
            NameChangedEventArgs Args = (NameChangedEventArgs)e;

            if (Args.Path is IFolderPath)
            {
                if (!Args.IsUndoRedo)
                    spcSolutionExplorer.ChangeName(Args.Path, Args.NewName);
            }
            else
                NotifyNodeRenamed(Args.Path, Args.NewName, Args.IsUndoRedo, RootProperties);
        }

        /// <summary>
        /// Called when a node has been renamed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnNodeRenamedComplete(object sender, SolutionPresenterEventCompletedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            NodeRenamedEventContext EventContext = (NodeRenamedEventContext)e.EventContext;
            ITreeNodePath Path = EventContext.Path;
            string NewName = EventContext.NewName;
            bool IsUndoRedo = EventContext.IsUndoRedo;

            if (!IsUndoRedo)
                spcSolutionExplorer.ChangeName(Path, NewName);
        }

        private void OnNodeMoved(object sender, RoutedEventArgs e)
        {
            MovedEventArgs Args = (MovedEventArgs)e;
            NotifyNodeMoved(Args.Path, Args.NewParentPath, Args.IsUndoRedo, RootProperties);
        }

        /// <summary>
        /// Called when a node has been moved.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnNodeMovedComplete(object sender, SolutionPresenterEventCompletedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            NodeMovedEventContext EventContext = (NodeMovedEventContext)e.EventContext;
            ITreeNodePath Path = EventContext.Path;
            IFolderPath NewParentPath = EventContext.NewParentPath;
            bool IsUndoRedo = EventContext.IsUndoRedo;

            if (!IsUndoRedo)
                spcSolutionExplorer.Move(Path, NewParentPath);
        }

        private void OnNodeTreeChanged(object sender, RoutedEventArgs e)
        {
            TreeChangedEventArgs Args = (TreeChangedEventArgs)e;
            if (Args.IsAdd)
            {
                IFolderPath? DestinationPath = null;

                Dictionary<ITreeNodePath, IFolderPath> ParentTable = new Dictionary<ITreeNodePath, IFolderPath>();
                foreach (KeyValuePair<ITreeNodePath, IPathConnection> Entry in Args.PathTable)
                    if (Entry.Value.ParentPath is IFolderPath ParentPath)
                        ParentTable.Add(Entry.Key, ParentPath);

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
        /// <summary>
        /// Called to check if properties of a node can be edited.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void CanEditProperties(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (SolutionMergedProperties.Count > 0)
                e.CanExecute = true;
        }

        /// <summary>
        /// Called when properties of a node are edited.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
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

        /// <summary>
        /// Called when a docked document is closing.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnDockedDocumentClosing(object sender, Xceed.Wpf.AvalonDock.DocumentClosingEventArgs e)
        {
            if (e == null || e.Document == null)
                return;

            if (e.Document.Content is IDocument Document)
                if (Document.IsDirty)
                {
                    CommitOption Option = IsSingleDocumentSaveConfirmed(Document);

                    if (Option == CommitOption.CommitAndContinue)
                    {
                        NotifyDocumentSaved(DocumentOperation.Close, Document, string.Empty);
                        e.Cancel = true;
                    }
                    else if (Option == CommitOption.Continue)
                    {
                    }
                    else
                        e.Cancel = true;
                }
        }

        /// <summary>
        /// Called when a docked document is closed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnDockedDocumentClosed(object sender, Xceed.Wpf.AvalonDock.DocumentClosedEventArgs e)
        {
            if (e != null && e.Document != null)
            {
                if (e.Document.Content is IDocument Document)
                {
                    OpenDocuments.Remove(Document);
                    NotifyDocumentClosed(DocumentOperation.Close, Document);
                }
            }

            if (OpenDocuments.Count == 0)
                InternalChangeActiveDocument(null);
        }

        /// <summary>
        /// Asks the user to save the solution.
        /// </summary>
        /// <returns>The commit information.</returns>
        protected virtual CommitInfo CheckToSaveCurrentSolution()
        {
            CommitInfo Info = GetDirtyObjects();

            if (Info.DirtyItemList != null && Info.DirtyItemList.Count == 0 && Info.DirtyPropertiesList != null && Info.DirtyPropertiesList.Count == 0 && Info.DirtyDocumentList != null && Info.DirtyDocumentList.Count == 0)
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

        /// <summary>
        /// Asks the user to save an open document.
        /// </summary>
        /// <returns>The commit information.</returns>
        protected virtual CommitInfo CheckToSaveOpenDocuments()
        {
            ICollection<IDocument>? DirtyDocumentList = GetDirtyDocuments();

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

        /// <summary>
        /// Clears all dirty flags.
        /// </summary>
        protected virtual void ClearDirtyDocuments()
        {
            foreach (IDocument Document in OpenDocuments)
                if (Document.IsDirty)
                    Document.ClearIsDirty();
        }

        /// <summary>
        /// Asks the user to save several objects.
        /// </summary>
        /// <param name="info">The commit information.</param>
        /// <returns>The commit option.</returns>
        protected virtual CommitOption IsMultipleSaveConfirmed(CommitInfo info)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            bool NoDocumentDirty = false;
            bool IsSolutionOnlyDirtyItem = false;
            bool IsSolutionOnlyDirtyProperties = false;
            bool IsSolutionOnlyDirty = false;
            bool DirtySolutionItem = false;
            bool DirtySolutionProperties = false;

            if (info.DirtyDocumentList != null && info.DirtyDocumentList.Count == 0)
                NoDocumentDirty = true;

            if (info.DirtyItemList != null && info.DirtyItemList.Count == 1)
                foreach (ITreeNodePath Path in info.DirtyItemList)
                    if (Path == RootPath)
                    {
                        DirtySolutionItem = true;
                        if (info.DirtyPropertiesList != null && info.DirtyPropertiesList.Count == 0)
                            IsSolutionOnlyDirtyItem = true;
                    }

            if (info.DirtyPropertiesList != null && info.DirtyPropertiesList.Count == 1)
                foreach (ITreeNodePath Path in info.DirtyPropertiesList)
                    if (Path == RootPath)
                    {
                        DirtySolutionProperties = true;
                        if (info.DirtyItemList != null && info.DirtyItemList.Count == 0)
                            IsSolutionOnlyDirtyProperties = true;
                    }

            IsSolutionOnlyDirty = info.DirtyItemList != null && info.DirtyItemList.Count == 1 && DirtySolutionItem && info.DirtyPropertiesList != null && info.DirtyPropertiesList.Count == 1 && DirtySolutionProperties;

            bool IsSolutionOnly = NoDocumentDirty && (IsSolutionOnlyDirtyItem || IsSolutionOnlyDirtyProperties || IsSolutionOnlyDirty);

            if (IsSolutionOnly)
                return IsSolutionOnlySaveConfirmed();
            else
                return IsAllElementsSaveConfirmed(info);
        }

        /// <summary>
        /// Asks the user to save the solution only.
        /// </summary>
        /// <returns>The commit option.</returns>
        protected virtual CommitOption IsSolutionOnlySaveConfirmed()
        {
            string QuestionFormat = SolutionPresenterInternal.Properties.Resources.ConfirmSaveSolutionChanges;
            string Question = string.Format(CultureInfo.CurrentCulture, QuestionFormat, RootPath.FriendlyName);
            MessageBoxResult Result = MessageBox.Show(Question, Owner.Title, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

            return IsConfirmed(Result);
        }

        /// <summary>
        /// Asks the user to save everything.
        /// </summary>
        /// <param name="info">The commit information.</param>
        /// <returns>The commit option.</returns>
        protected virtual CommitOption IsAllElementsSaveConfirmed(CommitInfo info)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            SaveAllWindow Dlg = new SaveAllWindow();
            Dlg.Owner = Owner;

            if ((info.DirtyItemList != null && info.DirtyItemList.Contains(RootPath)) || (info.DirtyPropertiesList != null && info.DirtyPropertiesList.Contains(RootPath)))
                Dlg.DirtySolutionName = RootPath.FriendlyName;

            if (info.DirtyItemList != null)
            {
                foreach (ITreeNodePath Path in info.DirtyItemList)
                    Dlg.TitleList.Add(Path.FriendlyName);
            }

            if (info.DirtyPropertiesList != null && info.DirtyItemList != null)
            {
                foreach (ITreeNodePath Path in info.DirtyPropertiesList)
                    if (!info.DirtyItemList.Contains(Path))
                        Dlg.TitleList.Add(Path.FriendlyName);
            }

            if (info.DirtyDocumentList != null && info.DirtyItemList != null)
            {
                foreach (IDocument Document in info.DirtyDocumentList)
                {
                    bool AlreadyListed = false;

                    foreach (ITreeNodePath Path in info.DirtyItemList)
                        if (Path is IItemPath AsItemPath)
                            if (AsItemPath.DocumentPath.IsEqual(Document.Path))
                            {
                                AlreadyListed = true;
                                break;
                            }

                    if (!AlreadyListed && info.DirtyPropertiesList != null)
                        foreach (ITreeNodePath Path in info.DirtyPropertiesList)
                            if (Path is IItemPath AsItemPath)
                                if (AsItemPath.DocumentPath.IsEqual(Document.Path))
                                {
                                    AlreadyListed = true;
                                    break;
                                }

                    if (!AlreadyListed)
                        Dlg.TitleList.Add(Document.Path.HeaderName);
                }
            }

            Dlg.ShowDialog();
            MessageBoxResult Result = Dlg.Result;

            return IsConfirmed(Result);
        }

        /// <summary>
        /// Asks the user to save a single document.
        /// </summary>
        /// <param name="savedDocument">The document to save.</param>
        /// <returns>The commit option.</returns>
        protected virtual CommitOption IsSingleDocumentSaveConfirmed(IDocument savedDocument)
        {
            if (savedDocument == null)
                throw new ArgumentNullException(nameof(savedDocument));

            string QuestionFormat = SolutionPresenterInternal.Properties.Resources.ConfirmSaveDocumentChanges;
            string Question = string.Format(CultureInfo.CurrentCulture, QuestionFormat, savedDocument.Path.HeaderName);
            MessageBoxResult Result = MessageBox.Show(Question, Owner.Title, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

            return IsConfirmed(Result);
        }

        /// <summary>
        /// Gets the commit options from a user response.
        /// </summary>
        /// <param name="result">The user response.</param>
        /// <returns>The commit option.</returns>
        protected virtual CommitOption IsConfirmed(MessageBoxResult result)
        {
            if (result == MessageBoxResult.Cancel)
                return CommitOption.Stop;
            else if (result != MessageBoxResult.Yes)
                return CommitOption.Continue;
            else
                return CommitOption.CommitAndContinue;
        }

        /// <summary>
        /// Changes the active documment.
        /// </summary>
        /// <param name="newDocument">The new active document.</param>
        protected void InternalChangeActiveDocument(IDocument? newDocument)
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
        /// <summary>
        /// Merge properties when displaying properties of several items.
        /// </summary>
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

        private static IPropertyEntry GetMergedStringProperty(List<ITreeNodeProperties> propertiesList, PropertyInfo info, string friendlyName)
        {
            string MergedText = string.Empty;

            foreach (ITreeNodeProperties Properties in propertiesList)
            {
                string NextText = (string)info.GetValue(Properties);

                if (MergedText.Length == 0)
                    MergedText = NextText;
                else if (MergedText != NextText)
                {
                    MergedText = string.Empty;
                    break;
                }
            }

            return new StringPropertyEntry(propertiesList, info.Name, friendlyName, MergedText);
        }

        private static IPropertyEntry GetMergedBoolProperty(List<ITreeNodeProperties> propertiesList, PropertyInfo info, string friendlyName)
        {
            int MergedSelectedIndex = -1;

            foreach (ITreeNodeProperties Properties in propertiesList)
            {
                int NextSelectedIndex = (bool)info.GetValue(Properties) ? 1 : 0;

                if (MergedSelectedIndex == -1)
                    MergedSelectedIndex = NextSelectedIndex;
                else if (MergedSelectedIndex != NextSelectedIndex)
                {
                    MergedSelectedIndex = -1;
                    break;
                }
            }

            string[] EnumNames = new string[] { SolutionPresenterInternal.Properties.Resources.False, SolutionPresenterInternal.Properties.Resources.True };
            return new EnumPropertyEntry(propertiesList, info.Name, friendlyName, EnumNames, MergedSelectedIndex);
        }

        private static IPropertyEntry GetMergedEnumProperty(List<ITreeNodeProperties> propertiesList, PropertyInfo info, string friendlyName)
        {
            int MergedSelectedIndex = -1;

            foreach (ITreeNodeProperties Properties in propertiesList)
            {
                int NextSelectedIndex = (int)info.GetValue(Properties);

                if (MergedSelectedIndex == -1)
                    MergedSelectedIndex = NextSelectedIndex;
                else if (MergedSelectedIndex != NextSelectedIndex)
                {
                    MergedSelectedIndex = -1;
                    break;
                }
            }

            string[] EnumNames = info.PropertyType.GetEnumNames();
            return new EnumPropertyEntry(propertiesList, info.Name, friendlyName, EnumNames, MergedSelectedIndex);
        }

        /// <summary>
        /// Gets the merged properties.
        /// </summary>
        public ObservableCollection<IPropertyEntry> SolutionMergedProperties { get; } = new ObservableCollection<IPropertyEntry>();
        #endregion

        #region Solution Tree
        private void InitializeSolutionTree()
        {
            spcSolutionExplorer.SelectionChanged += OnSolutionTreeSelectionChanged;
        }

        /// <summary>
        /// Called when the selection changed in the solution tree.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnSolutionTreeSelectionChanged(object sender, RoutedEventArgs e)
        {
            MergeProperties();
        }

        /// <summary>
        /// Called when a commit is complete.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnCommitComplete(object sender, SolutionPresenterEventCompletedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            SolutionTreeCommittedEventContext EventContext = (SolutionTreeCommittedEventContext)e.EventContext;
            SolutionOperation SolutionOperation = EventContext.SolutionOperation;
            IRootPath RootPath = EventContext.RootPath;
            IRootPath NewRootPath = EventContext.NewRootPath;
            string DestinationPath = EventContext.DestinationPath;

            spcSolutionExplorer.ClearDirtyItemsAndProperties();
            ClearDirtyDocuments();

            bool IsHandled = false;

            switch (SolutionOperation)
            {
                case SolutionOperation.Save:
                    IsHandled = true;
                    break;

                case SolutionOperation.Create:
                    NotifySolutionClosed(SolutionOperation, RootPath, new EmptyPath());
                    IsHandled = true;
                    break;

                case SolutionOperation.Delete:
                    NotifySolutionClosed(SolutionOperation, RootPath, new EmptyPath());
                    IsHandled = true;
                    break;

                case SolutionOperation.Open:
                    if (RootPath != null)
                        NotifySolutionClosed(SolutionOperation, RootPath, NewRootPath);
                    else
                        NotifySolutionOpened(NewRootPath);
                    IsHandled = true;
                    break;

                case SolutionOperation.Close:
                    if (RootPath != null)
                        NotifySolutionClosed(SolutionOperation, RootPath, new EmptyPath());
                    IsHandled = true;
                    break;

                case SolutionOperation.Exit:
                    NotifyExitRequested();
                    IsHandled = true;
                    break;

                case SolutionOperation.ExportDocument:
                    NotifyDocumentExported(DocumentOperation.Export, OpenDocuments, true, DestinationPath);
                    IsHandled = true;
                    break;

                case SolutionOperation.ExportSolution:
                    ExportSolution(RootPath);
                    IsHandled = true;
                    break;

                case SolutionOperation.Build:
                    NotifyBuildSolutionRequested();
                    IsHandled = true;
                    break;
            }

            Debug.Assert(IsHandled);
        }

        /// <summary>
        /// Loads a tree of nodes.
        /// </summary>
        /// <param name="newRootPath">The new root path.</param>
        /// <param name="newRootProperties">The new root properties.</param>
        /// <param name="newComparer">The new comparer.</param>
        /// <param name="expandedFolderList">The list of expanded fodlers.</param>
        /// <param name="context">The operation context.</param>
        protected virtual void LoadTree(IRootPath newRootPath, IRootProperties? newRootProperties, IComparer<ITreeNodePath>? newComparer, IList<IFolderPath>? expandedFolderList, object? context)
        {
            if (newRootPath == null)
                throw new ArgumentNullException(nameof(newRootPath));

            if (newRootPath.FriendlyName.Length > 0 && newRootProperties != null && newComparer != null && expandedFolderList != null && context != null)
            {
                SetValue(RootPathPropertyKey, newRootPath);
                SetValue(RootPropertiesPropertyKey, newRootProperties);

                spcSolutionExplorer.SetRoot(newRootPath, newRootProperties, newComparer);

                StartLoadTree(RootProperties, ExpandedFolderList, context);
            }
            else
                NotifySolutionTreeLoaded(false);
        }

        /// <summary>
        /// Begins loading a solution tree.
        /// </summary>
        /// <param name="context">The operation context.</param>
        protected virtual void BeginLoadSolutionTree(object context)
        {
            SetValue(IsLoadingTreePropertyKey, true);
        }

        /// <summary>
        /// Ends loading a solution tree.
        /// </summary>
        /// <param name="context">The operation context.</param>
        protected virtual void EndLoadSolutionTree(object context)
        {
            SetValue(IsLoadingTreePropertyKey, false);
            spcSolutionExplorer.ResetUndoRedo();
            NotifySolutionTreeLoaded(false);
        }

        /// <summary>
        /// Starts loading a tree of nodes.
        /// </summary>
        /// <param name="rootProperties">The root properties.</param>
        /// <param name="expandedFolderList">The list of expanded folders.</param>
        /// <param name="context">The operation context.</param>
        protected virtual void StartLoadTree(IRootProperties rootProperties, IList<IFolderPath> expandedFolderList, object context)
        {
            BeginLoadSolutionTree(context);

            List<IFolderPath> ParentPathList = new List<IFolderPath>();
            ParentPathList.Add(RootPath);
            LoadNestedTree(ParentPathList, rootProperties, expandedFolderList, context, new List<IFolderPath>());
        }

        private void LoadNestedTree(ICollection<IFolderPath> parentPathList, IRootProperties rootProperties, ICollection<IFolderPath> expandedFolderList, object context, List<IFolderPath> expandedFolders)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new LoadTreeHandler(OnLoadTree), parentPathList, rootProperties, expandedFolderList, context, expandedFolders);
        }

        private delegate void LoadTreeHandler(ICollection<IFolderPath> parentPathList, IRootProperties rootProperties, ICollection<IFolderPath> expandedFolderList, object context, List<IFolderPath> expandedFolders);
        private void OnLoadTree(ICollection<IFolderPath> parentPathList, IRootProperties rootProperties, ICollection<IFolderPath> expandedFolderList, object context, List<IFolderPath> expandedFolders)
        {
            if (parentPathList.Count == 0)
            {
                EndLoadSolutionTree(context);
                return;
            }

            IEnumerator<IFolderPath> Enumerator = parentPathList.GetEnumerator();
            Enumerator.MoveNext();
            IFolderPath ParentPath = Enumerator.Current;
            parentPathList.Remove(ParentPath);

            NotifyFolderEnumerated(ParentPath, parentPathList, rootProperties, expandedFolderList, context);
        }

        /// <summary>
        /// Called when a folder has been enumerated.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnFolderEnumeratedComplete(object sender, SolutionPresenterEventCompletedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

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

        private delegate void LoadChildrenHandler(IFolderPath parentPath, IReadOnlyList<ITreeNodePath> childrenPathList, IReadOnlyDictionary<ITreeNodePath, ITreeNodeProperties> childrenProperties, ICollection<IFolderPath> parentPathList, IRootProperties rootProperties, ICollection<IFolderPath> expandedFolderList, object context);
        private void OnLoadChildren(IFolderPath parentPath, IReadOnlyList<ITreeNodePath> childrenPathList, IReadOnlyDictionary<ITreeNodePath, ITreeNodeProperties> childrenProperties, ICollection<IFolderPath> parentPathList, IRootProperties rootProperties, ICollection<IFolderPath> expandedFolderList, object context)
        {
            Dictionary<ITreeNodePath, IPathConnection> AddedPathTable = new Dictionary<ITreeNodePath, IPathConnection>();
            foreach (ITreeNodePath ChildPath in childrenPathList)
            {
                bool IsExpanded = false;

                if (ChildPath is IFolderPath AsFolderPath)
                    foreach (IFolderPath Path in expandedFolderList)
                        if (Path.IsEqual(AsFolderPath))
                        {
                            IsExpanded = true;
                            break;
                        }

                AddedPathTable.Add(ChildPath, new PathConnection(parentPath, childrenProperties[ChildPath], IsExpanded));
            }

            spcSolutionExplorer.AddTree(AddedPathTable);

            List<IFolderPath> FolderPathList = new List<IFolderPath>();
            List<IFolderPath> ExpandedFolders = new List<IFolderPath>();
            foreach (ITreeNodePath ChildPath in childrenPathList)
                if (ChildPath is IFolderPath AsFolderPath)
                {
                    FolderPathList.Add(AsFolderPath);

                    if (IsChildExpanded(expandedFolderList, AsFolderPath))
                        ExpandedFolders.Add(AsFolderPath);
                }

            foreach (IFolderPath Path in FolderPathList)
                parentPathList.Add(Path);
            LoadNestedTree(parentPathList, rootProperties, expandedFolderList, context, ExpandedFolders);
        }

        /// <summary>
        /// Checks if a child folder node is expanded.
        /// </summary>
        /// <param name="expandedFolderList">The list of expanded folders.</param>
        /// <param name="folderPath">The folder path.</param>
        /// <returns>True if expanded; otherwise, false.</returns>
        protected virtual bool IsChildExpanded(ICollection<IFolderPath> expandedFolderList, IFolderPath folderPath)
        {
            if (expandedFolderList == null)
                throw new ArgumentNullException(nameof(expandedFolderList));
            if (folderPath == null)
                throw new ArgumentNullException(nameof(folderPath));

            foreach (IFolderPath Path in ExpandedFolderList)
                if (folderPath.IsEqual(Path))
                    return true;

            return false;
        }
        #endregion

        #region Compiler Tool
        /// <summary>
        /// Called when the user clicks on an error line.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnErrorLineDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (listviewCompilerOutput.SelectedItem is ICompilationError Error)
                if (Error.Source is IDocumentPath AsDocumentPath)
                {
                    bool IsOpened = false;
                    foreach (IDocument Document in OpenDocuments)
                        if (Document.Path.IsEqual(AsDocumentPath))
                        {
                            IsOpened = true;
                            UserActivateDocument(Document);
                            NotifyErrorFocused(Document, Error.Location);
                            break;
                        }

                    if (!IsOpened)
                    {
                        List<IDocumentPath> DocumentPathList = new List<IDocumentPath>();
                        DocumentPathList.Add(AsDocumentPath);
                        NotifyDocumentOpened(DocumentOperation.ShowError, new EmptyPath(), DocumentPathList, new List<IDocumentPath>(), Error.Location);
                    }
                }

            e.Handled = true;
        }
        #endregion

        #region Custom Menus
        /// <summary>
        /// Called when the main menu is loaded.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnMainMenuLoaded(object sender, RoutedEventArgs e)
        {
            NotifyMainMenuLoaded(e);
        }

        /// <summary>
        /// Called when the main toolbar is loaded.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnMainToolBarLoaded(object sender, RoutedEventArgs e)
        {
            NotifyMainToolBarLoaded(e);
        }

        /// <summary>
        /// Inserts a custom control in a menu.
        /// </summary>
        /// <param name="solutionMenu">The menu.</param>
        /// <param name="childItem">The control to insert.</param>
        public virtual void InsertCustomControl(SolutionMenu solutionMenu, FrameworkElement childItem)
        {
            Separator InsertionSeparator = GetSeparator(solutionMenu);
            InsertItem(InsertionSeparator, childItem);
        }

        /// <summary>
        /// Replaces a menu item.
        /// </summary>
        /// <param name="solutionMenu">The menu where to replace.</param>
        /// <param name="byCommand">The command of the menu item.</param>
        /// <param name="newItem">The menu item.</param>
        public virtual void ReplaceMenuItem(SolutionMenu solutionMenu, ICommand byCommand, MenuItem newItem)
        {
            ItemsControl InsertionControl = GetMenuControl(solutionMenu);
            ReplaceMenuItem(InsertionControl.Items, byCommand, newItem);
        }

        /// <summary>
        /// Gets the separator in a menu.
        /// </summary>
        /// <param name="solutionMenu">The solution menu.</param>
        /// <returns>The separator.</returns>
        protected virtual Separator GetSeparator(SolutionMenu solutionMenu)
        {
            switch (solutionMenu)
            {
                case SolutionMenu.FileMenu:
                    return toolbarMain.FileCustomMenuSeparator;

                case SolutionMenu.FileToolBar:
                    return toolbarMain.FileToolBarSeparator;

                case SolutionMenu.EditMenu:
                    return toolbarMain.EditCustomMenuSeparator;

                case SolutionMenu.EditToolBar:
                    return toolbarMain.EditToolBarSeparator;

                case SolutionMenu.ContextMenu:
                    return spcSolutionExplorer.ContextMenuSeparator;

                default:
                    throw new ArgumentOutOfRangeException(nameof(solutionMenu));
            }
        }

        /// <summary>
        /// Gets the control of a solution menu.
        /// </summary>
        /// <param name="solutionMenu">The solution menu.</param>
        /// <returns>The control.</returns>
        protected virtual ItemsControl GetMenuControl(SolutionMenu solutionMenu)
        {
            switch (solutionMenu)
            {
                case SolutionMenu.FileMenu:
                    return toolbarMain.MainMenu;

                case SolutionMenu.EditMenu:
                    return toolbarMain.MainMenu;

                default:
                    throw new ArgumentOutOfRangeException(nameof(solutionMenu));
            }
        }

        /// <summary>
        /// Inserts an item after a separator.
        /// </summary>
        /// <param name="insertionSeparator">The separator.</param>
        /// <param name="childItem">The item to insert.</param>
        protected virtual void InsertItem(Separator insertionSeparator, FrameworkElement childItem)
        {
            if (insertionSeparator == null)
                throw new ArgumentNullException(nameof(insertionSeparator));

            ItemsControl ParentCollection = (ItemsControl)insertionSeparator.Parent;
            int Index = ParentCollection.Items.IndexOf(insertionSeparator);
            ParentCollection.Items.Insert(Index, childItem);
        }

        /// <summary>
        /// Replaces a menu item.
        /// </summary>
        /// <param name="items">The collection of menu items.</param>
        /// <param name="byCommand">The command of the replaced item.</param>
        /// <param name="newItem">The new menu item.</param>
        /// <returns>True if the menu has been replaced; otherwise, false.</returns>
        protected virtual bool ReplaceMenuItem(ItemCollection items, ICommand byCommand, MenuItem newItem)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));
            if (newItem == null)
                throw new ArgumentNullException(nameof(newItem));

            foreach (object Item in items)
                if (Item is MenuItem AsMenuItem)
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

            return false;
        }

        /// <summary>
        /// Reinserts a menu item that was removed.
        /// </summary>
        /// <param name="items">The collection of menu items.</param>
        /// <param name="removedMenuItem">The menu item that was removed.</param>
        /// <returns>True if the menu has been reinserted; otherwise, false.</returns>
        protected virtual bool ReinsertRemovedMenuItem(ItemCollection items, MenuItem removedMenuItem)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));
            if (removedMenuItem == null)
                throw new ArgumentNullException(nameof(removedMenuItem));

            foreach (object Item in items)
                if (Item is MenuItem AsMenuItem)
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

            return false;
        }
        #endregion

        #region Context Menu
        private void InitializeContextMenu()
        {
            ContextMenuInitialized = false;
        }

        /// <summary>
        ///  Called when a context menu is loaded.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnContextMenuLoaded(object sender, RoutedEventArgs e)
        {
            if (!ContextMenuInitialized)
            {
                ContextMenuInitialized = true;
                NotifyContextMenuLoaded(e);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the context menu has been initialized.
        /// </summary>
        protected virtual bool ContextMenuInitialized { get; private set; }
        #endregion

        #region Clipboard
        /// <summary>
        /// Copies the selection to the clipboard.
        /// </summary>
        public virtual void CopySelectionToClipboard()
        {
        }

        /// <summary>
        /// Pastes the selection from the clipboard.
        /// </summary>
        public virtual void PasteSelectionFromClipboard()
        {
        }
        #endregion

        #region Dock Manager
        private void InitializeDockManager()
        {
            Documents.CollectionChanged += OnDocumentsCollectionChanged;
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

        /// <summary>
        /// Called when the content of the active document has changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnActiveContentChanged(object sender, EventArgs e)
        {
            if (dockManager.ActiveContent is IDocument Document)
            {
                InternalChangeActiveDocument(Document);

                FocusSortedDocuments.Remove(Document);
                FocusSortedDocuments.Insert(0, Document);
                Document.SetViewGotFocus();
            }
        }

        private void ChangeActiveDocument(int direction)
        {
            if (ActiveDocument != null)
            {
                int Index = FocusSortedDocuments.IndexOf(ActiveDocument);
                if (Index >= 0)
                {
                    Index += direction;
                    if (Index >= FocusSortedDocuments.Count)
                        Index = 0;
                    else if (Index < 0)
                        Index = FocusSortedDocuments.Count - 1;

                    UserActivateDocument(FocusSortedDocuments[Index]);
                }
            }
        }

        /// <summary>
        /// Called when a document has ben activated.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnDocumentActivated(object sender, RoutedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            DocumentActivatedEventArgs Args = (DocumentActivatedEventArgs)e;
            UserActivateDocument(Args.ActiveDocument);
        }

        /// <summary>
        /// Activates a document.
        /// </summary>
        /// <param name="newDocument">The document to activate.</param>
        protected virtual void UserActivateDocument(IDocument newDocument)
        {
            dockManager.ActiveContentChanged -= OnActiveContentChanged;
            toolbarMain.DocumentActivated -= OnDocumentActivated;

            InternalChangeActiveDocument(newDocument);

            toolbarMain.DocumentActivated += OnDocumentActivated;
            dockManager.ActiveContentChanged += OnActiveContentChanged;
        }

        private bool IsToolVisible(string contentId)
        {
            foreach (ILayoutElement Item in dockManager.Layout.Descendents())
                if (Item is LayoutAnchorable AsAnchorable)
                    if (AsAnchorable.ContentId == contentId)
                        return AsAnchorable.IsVisible;

            return false;
        }

        private void ShowTool(string contentId, ToolOperation toolOperation)
        {
            foreach (ILayoutElement Item in dockManager.Layout.Descendents())
                if (Item is LayoutAnchorable AsAnchorable)
                    if (AsAnchorable.ContentId == contentId)
                    {
                        if (!AsAnchorable.IsVisible && toolOperation != ToolOperation.Hide)
                            AsAnchorable.Show();
                        else if (AsAnchorable.IsVisible)
                            if (toolOperation == ToolOperation.Show)
                                dockManager.ActiveContent = AsAnchorable.Content;
                            else
                                AsAnchorable.Hide();

                        break;
                    }
        }

        private SplitView? GetActiveControl()
        {
            return GetActiveDockedControl(dockManager.Layout);
        }

        private SplitView? GetActiveDockedControl(ILayoutElement layout)
        {
            switch (layout)
            {
                case LayoutDocument AsDocument:
                    if (AsDocument.Content == dockManager.ActiveContent)
                    {
                        LayoutItem Item = dockManager.GetLayoutItemFromModel(AsDocument);
                        if (VisualTreeHelper.GetChildrenCount(Item.View) > 0)
                            return VisualTreeHelper.GetChild(Item.View, 0) as SplitView;
                        else
                            return null;
                    }
                    break;

                case ILayoutContainer AsContainer:
                    foreach (ILayoutElement Child in AsContainer.Children)
                        if (GetActiveDockedControl(Child) is SplitView Ctrl)
                            return Ctrl;
                    break;
            }

            return null;
        }

        private ObservableCollection<IDocument> Documents = new ObservableCollection<IDocument>();
        private List<IDocument> FocusSortedDocuments = new List<IDocument>();
        #endregion

        #region Themes
        /// <summary>
        /// Updates the theme options.
        /// </summary>
        public void UpdateThemeOption()
        {
            LoadTheme(ThemeOption);

            Theme? Theme = null;

            switch (ThemeOption)
            {
                case ThemeOption.Expression:
                    Theme = new ExpressionLightTheme();
                    break;

                case ThemeOption.Metro:
                    Theme = new MetroTheme();
                    break;

                case ThemeOption.VS2013:
                    Theme = new Vs2013BlueTheme();
                    break;
            }

            Debug.Assert(Theme != null);

            if (Theme != null)
            {
                dockManager.Theme = Theme;

                CompositeCollection BackgroundResourceKeys = (CompositeCollection)FindResource("ThemeBackgroundBrushKeys");
                CompositeCollection ForegroundResourceKeys = (CompositeCollection)FindResource("ThemeForegroundBrushKeys");
                StatusTheme = new AvalonStatusTheme(Theme, BackgroundResourceKeys, ForegroundResourceKeys);
            }
        }

        private static readonly Dictionary<ThemeOption, string> ThemeAssemblyTable = new Dictionary<ThemeOption, string>()
        {
            { ThemeOption.Expression, "Expression" },
            { ThemeOption.Metro, "Metro" },
            { ThemeOption.VS2013, "VS2013" },
        };

        private static readonly Dictionary<ThemeOption, string> ThemeResourceTable = new Dictionary<ThemeOption, string>()
        {
            { ThemeOption.Expression, "LightTheme.xaml" },
            { ThemeOption.Metro, "Theme.xaml" },
            { ThemeOption.VS2013, "BlueTheme.xaml" },
        };

        /// <summary>
        /// Loads a theme.
        /// </summary>
        /// <param name="themeOption">The theme to load.</param>
        public void LoadTheme(ThemeOption themeOption)
        {
            Collection<ResourceDictionary> MergedDictionaries = Resources.MergedDictionaries;

            string ThemeAssembly = ThemeAssemblyTable[themeOption];
            string ThemeResource = ThemeResourceTable[themeOption];
            string SourcePath = string.Format(CultureInfo.InvariantCulture, @"pack://application:,,,/Xceed.Wpf.AvalonDock.Themes.{0};component/{1}", ThemeAssembly, ThemeResource);

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
            spcSolutionExplorer.UndoRedoManager = UndoRedoManager;
            toolbarMain.UndoRedoManager = UndoRedoManager;
        }

        /// <summary>
        /// Gets the undo redo manager.
        /// </summary>
        public UndoRedoManager UndoRedoManager { get; } = new UndoRedoManager();
        #endregion

        #region Status Bar
        /// <summary>
        /// Ends initialization of the status bar.
        /// </summary>
        /// <param name="startupRootPath">The root path.</param>
        public virtual void EndInitializingStatus(IRootPath startupRootPath)
        {
            ResetStatus(spcStatus.DefaultInitializingStatus);

            if (startupRootPath != null)
                NotifySolutionOpened(startupRootPath);
        }

        /// <summary>
        /// Sets the current status.
        /// </summary>
        /// <param name="status">The current status.</param>
        public virtual void SetStatus(IApplicationStatus status)
        {
            spcStatus.SetStatus(status);
        }

        /// <summary>
        /// Resets the status.
        /// </summary>
        /// <param name="status">The status.</param>
        public virtual void ResetStatus(IApplicationStatus status)
        {
            spcStatus.ResetStatus(status);
        }

        /// <summary>
        /// Sets the progress of an operation.
        /// </summary>
        /// <param name="current">The current progress.</param>
        /// <param name="max">The max possible value of <paramref name="current"/>.</param>
        public virtual void SetProgress(double current, double max)
        {
            spcStatus.ProgressMax = max;
            spcStatus.ProgressValue = current;
        }

        /// <summary>
        /// Sets the failure status.
        /// </summary>
        public virtual void SetFailureStatus()
        {
            spcStatus.SetFailureStatus();
        }
        #endregion
    }
}
