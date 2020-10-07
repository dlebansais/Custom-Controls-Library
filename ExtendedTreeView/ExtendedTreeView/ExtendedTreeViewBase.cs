namespace CustomControls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Media;

    /// <summary>
    /// Represents a control with a tree of nodes that can be moved around with Drag and Drop.
    /// </summary>
    public abstract class ExtendedTreeViewBase : MultiSelector
    {
        #region Constants
        /// <summary>
        /// Enables traces.
        /// </summary>
        public const bool EnableTraces = false;
        #endregion

        #region Default Styles
        /// <summary>
        /// Gets the default control style.
        /// </summary>
        public static Style DefaultStyle { get; private set; } = new Style();

        /// <summary>
        /// Gets the default control container style.
        /// </summary>
        public static Style DefaultItemContainerStyle { get; private set; } = new Style();
        #endregion

        #region Custom properties and events
        #region Content
        /// <summary>
        /// Identifies the <see cref="Content"/> attached property.
        /// </summary>
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(object), typeof(ExtendedTreeViewBase), new FrameworkPropertyMetadata(null, OnContentChanged));

        /// <summary>
        /// Gets or sets the control content.
        /// </summary>
        public object Content
        {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        /// <summary>
        /// Handles changes of the <see cref="Content"/> property.
        /// </summary>
        /// <param name="modifiedObject">The modified object.</param>
        /// <param name="e">An object that contains event data.</param>
        protected static void OnContentChanged(DependencyObject modifiedObject, DependencyPropertyChangedEventArgs e)
        {
            if (modifiedObject == null)
                throw new ArgumentNullException(nameof(modifiedObject));

            ExtendedTreeViewBase ctrl = (ExtendedTreeViewBase)modifiedObject;
            ctrl.OnContentChanged(e);
        }

        /// <summary>
        /// Handles changes of the <see cref="Content"/> property.
        /// </summary>
        /// <param name="e">An object that contains event data.</param>
        protected virtual void OnContentChanged(DependencyPropertyChangedEventArgs e)
        {
            BuildFlatChildrenTables();
        }
        #endregion
        #region Selection Mode
        /// <summary>
        /// Identifies the <see cref="SelectionMode"/> attached property.
        /// </summary>
        public static readonly DependencyProperty SelectionModeProperty = DependencyProperty.Register("SelectionMode", typeof(SelectionMode), typeof(ExtendedTreeViewBase), new FrameworkPropertyMetadata(SelectionMode.Single));

        /// <summary>
        /// Gets or sets the control selection mode.
        /// </summary>
        public SelectionMode SelectionMode
        {
            get { return (SelectionMode)GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }
        #endregion
        #region Preview Collection Modified
        /// <summary>
        /// Identifies the <see cref="PreviewCollectionModified"/> routed event.
        /// </summary>
        public static readonly RoutedEvent PreviewCollectionModifiedEvent = EventManager.RegisterRoutedEvent("PreviewCollectionModified", RoutingStrategy.Tunnel, typeof(RoutedEventHandler), typeof(ExtendedTreeViewBase));

        /// <summary>
        /// Occurs before the content collection is modified.
        /// </summary>
        public event RoutedEventHandler PreviewCollectionModified
        {
            add { AddHandler(PreviewCollectionModifiedEvent, value); }
            remove { RemoveHandler(PreviewCollectionModifiedEvent, value); }
        }

        /// <summary>
        /// Invokes handlers of the <see cref="PreviewCollectionModified"/> event.
        /// </summary>
        /// <param name="treeViewCollectionOperation">The modifying operation.</param>
        protected virtual void NotifyPreviewCollectionModified(TreeViewCollectionOperation treeViewCollectionOperation)
        {
            RaiseEvent(new TreeViewCollectionModifiedEventArgs(PreviewCollectionModifiedEvent, treeViewCollectionOperation, Items.Count));
        }
        #endregion
        #region Collection Modified
        /// <summary>
        /// Identifies the <see cref="CollectionModified"/> routed event.
        /// </summary>
        public static readonly RoutedEvent CollectionModifiedEvent = EventManager.RegisterRoutedEvent("CollectionModified", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ExtendedTreeViewBase));

        /// <summary>
        /// Occurs after the content collection is modified.
        /// </summary>
        public event RoutedEventHandler CollectionModified
        {
            add { AddHandler(CollectionModifiedEvent, value); }
            remove { RemoveHandler(CollectionModifiedEvent, value); }
        }

        /// <summary>
        /// Invokes handlers of the <see cref="CollectionModified"/> event.
        /// </summary>
        /// <param name="treeViewCollectionOperation">The modifying operation.</param>
        protected virtual void NotifyCollectionModified(TreeViewCollectionOperation treeViewCollectionOperation)
        {
            RaiseEvent(new TreeViewCollectionModifiedEventArgs(CollectionModifiedEvent, treeViewCollectionOperation, Items.Count));
        }
        #endregion
        #region Is Root Always Expanded
        /// <summary>
        /// Identifies the <see cref="IsRootAlwaysExpanded"/> attached property.
        /// </summary>
        public static readonly DependencyProperty IsRootAlwaysExpandedProperty = DependencyProperty.Register("IsRootAlwaysExpanded", typeof(bool), typeof(ExtendedTreeViewBase), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value indicating whether the control root is always expanded.
        /// </summary>
        public bool IsRootAlwaysExpanded
        {
            get { return (bool)GetValue(IsRootAlwaysExpandedProperty); }
            set { SetValue(IsRootAlwaysExpandedProperty, value); }
        }
        #endregion
        #region Is Item Expanded At Start
        /// <summary>
        /// Identifies the <see cref="IsItemExpandedAtStart"/> attached property.
        /// </summary>
        public static readonly DependencyProperty IsItemExpandedAtStartProperty = DependencyProperty.Register("IsItemExpandedAtStart", typeof(bool), typeof(ExtendedTreeViewBase), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value indicating whether items should start expanded when the content changes.
        /// </summary>
        public bool IsItemExpandedAtStart
        {
            get { return (bool)GetValue(IsItemExpandedAtStartProperty); }
            set { SetValue(IsItemExpandedAtStartProperty, value); }
        }
        #endregion
        #region Allow Drag Drop
        /// <summary>
        /// Identifies the <see cref="AllowDragDrop"/> attached property.
        /// </summary>
        public static readonly DependencyProperty AllowDragDropProperty = DependencyProperty.Register("AllowDragDrop", typeof(bool), typeof(ExtendedTreeViewBase), new FrameworkPropertyMetadata(false, OnAllowDragDropChanged));

        /// <summary>
        /// Gets or sets a value indicating whether drag and drop is allowed.
        /// </summary>
        public bool AllowDragDrop
        {
            get { return (bool)GetValue(AllowDragDropProperty); }
            set { SetValue(AllowDragDropProperty, value); }
        }

        /// <summary>
        /// Handles changes of the <see cref="AllowDragDrop"/> property.
        /// </summary>
        /// <param name="modifiedObject">The modified object.</param>
        /// <param name="e">An object that contains event data.</param>
        protected static void OnAllowDragDropChanged(DependencyObject modifiedObject, DependencyPropertyChangedEventArgs e)
        {
            if (modifiedObject == null)
                throw new ArgumentNullException(nameof(modifiedObject));

            ExtendedTreeViewBase ctrl = (ExtendedTreeViewBase)modifiedObject;
            ctrl.OnAllowDragDropChanged(e);
        }

        /// <summary>
        /// Handles changes of the <see cref="AllowDragDrop"/> property.
        /// </summary>
        /// <param name="e">An object that contains event data.</param>
        protected virtual void OnAllowDragDropChanged(DependencyPropertyChangedEventArgs e)
        {
            bool NewValue = (bool)e.NewValue;
            if (NewValue)
                AllowDrop = true;
        }
        #endregion
        #region Use Default Cursors
        /// <summary>
        /// Identifies the <see cref="UseDefaultCursors"/> attached property.
        /// </summary>
        public static readonly DependencyProperty UseDefaultCursorsProperty = DependencyProperty.Register("UseDefaultCursors", typeof(bool), typeof(ExtendedTreeViewBase), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value indicating whether to use default cursors.
        /// </summary>
        public bool UseDefaultCursors
        {
            get { return (bool)GetValue(UseDefaultCursorsProperty); }
            set { SetValue(UseDefaultCursorsProperty, value); }
        }
        #endregion
        #region Cursor Forbidden
        /// <summary>
        /// Identifies the <see cref="CursorForbidden"/> attached property.
        /// </summary>
        public static readonly DependencyProperty CursorForbiddenProperty = DependencyProperty.Register("CursorForbidden", typeof(Cursor), typeof(ExtendedTreeViewBase), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the forbidden cursor.
        /// </summary>
        public Cursor CursorForbidden
        {
            get { return (Cursor)GetValue(CursorForbiddenProperty); }
            set { SetValue(CursorForbiddenProperty, value); }
        }
        #endregion
        #region Cursor Move
        /// <summary>
        /// Identifies the <see cref="CursorMove"/> attached property.
        /// </summary>
        public static readonly DependencyProperty CursorMoveProperty = DependencyProperty.Register("CursorMove", typeof(Cursor), typeof(ExtendedTreeViewBase), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the move cursor.
        /// </summary>
        public Cursor CursorMove
        {
            get { return (Cursor)GetValue(CursorMoveProperty); }
            set { SetValue(CursorMoveProperty, value); }
        }
        #endregion
        #region Cursor Copy
        /// <summary>
        /// Identifies the <see cref="CursorCopy"/> attached property.
        /// </summary>
        public static readonly DependencyProperty CursorCopyProperty = DependencyProperty.Register("CursorCopy", typeof(Cursor), typeof(ExtendedTreeViewBase), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the copy cursor.
        /// </summary>
        public Cursor CursorCopy
        {
            get { return (Cursor)GetValue(CursorCopyProperty); }
            set { SetValue(CursorCopyProperty, value); }
        }
        #endregion
        #region Drag Starting
        /// <summary>
        /// Identifies the <see cref="DragStarting"/> routed event.
        /// </summary>
        public static readonly RoutedEvent DragStartingEvent = EventManager.RegisterRoutedEvent("DragStarting", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ExtendedTreeViewBase));

        /// <summary>
        /// Occurs when drag is starting.
        /// </summary>
        public event RoutedEventHandler DragStarting
        {
            add { AddHandler(DragStartingEvent, value); }
            remove { RemoveHandler(DragStartingEvent, value); }
        }

        /// <summary>
        /// Invokes handlers of the <see cref="DragStarting"/> event.
        /// </summary>
        /// <param name="cancellation">The cancellation token.</param>
        protected virtual void NotifyDragStarting(CancellationToken cancellation)
        {
            DragStartingEventArgs Args = new DragStartingEventArgs(DragStartingEvent, DragSource, cancellation);
            RaiseEvent(Args);
        }
        #endregion
        #region Drop Check
        /// <summary>
        /// Identifies the <see cref="DropCheck"/> routed event.
        /// </summary>
        public static readonly RoutedEvent DropCheckEvent = EventManager.RegisterRoutedEvent("DropCheck", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ExtendedTreeViewBase));

        /// <summary>
        /// Occurs when checking if drop is permitted.
        /// </summary>
        public event RoutedEventHandler DropCheck
        {
            add { AddHandler(DropCheckEvent, value); }
            remove { RemoveHandler(DropCheckEvent, value); }
        }

        /// <summary>
        /// Invokes handlers of the <see cref="DropCheck"/> event.
        /// </summary>
        /// <param name="dropDestinationItem">The drop destination item.</param>
        /// <param name="permission">The drop permission token.</param>
        protected virtual void NotifyDropCheck(object dropDestinationItem, PermissionToken permission)
        {
            DropCheckEventArgs Args = new DropCheckEventArgs(DropCheckEvent, DragSource, dropDestinationItem, permission);
            RaiseEvent(Args);
        }
        #endregion
        #region Preview Drop Completed
        /// <summary>
        /// Identifies the <see cref="PreviewDropCompleted"/> routed event.
        /// </summary>
        public static readonly RoutedEvent PreviewDropCompletedEvent = EventManager.RegisterRoutedEvent("PreviewDropCompleted", RoutingStrategy.Tunnel, typeof(RoutedEventHandler), typeof(ExtendedTreeViewBase));

        /// <summary>
        /// Occurs before a drop operation is completed.
        /// </summary>
        public event RoutedEventHandler PreviewDropCompleted
        {
            add { AddHandler(PreviewDropCompletedEvent, value); }
            remove { RemoveHandler(PreviewDropCompletedEvent, value); }
        }

        /// <summary>
        /// Invokes handlers of the <see cref="PreviewDropCompleted"/> event.
        /// </summary>
        /// <param name="dropDestinationItem">The drop destination item.</param>
        /// <param name="cloneList">The list of dropped items.</param>
        protected virtual void NotifyPreviewDropCompleted(object dropDestinationItem, IList? cloneList)
        {
            DropCompletedEventArgs Args = new DropCompletedEventArgs(PreviewDropCompletedEvent, DragSource, dropDestinationItem, cloneList);
            RaiseEvent(Args);
        }
        #endregion
        #region Drop Completed
        /// <summary>
        /// Identifies the <see cref="DropCompleted"/> routed event.
        /// </summary>
        public static readonly RoutedEvent DropCompletedEvent = EventManager.RegisterRoutedEvent("DropCompleted", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ExtendedTreeViewBase));

        /// <summary>
        /// Occurs after a drop operation is completed.
        /// </summary>
        public event RoutedEventHandler DropCompleted
        {
            add { AddHandler(DropCompletedEvent, value); }
            remove { RemoveHandler(DropCompletedEvent, value); }
        }

        /// <summary>
        /// Invokes handlers of the <see cref="DropCompleted"/> event.
        /// </summary>
        /// <param name="dropDestinationItem">The drop destination item.</param>
        /// <param name="cloneList">The list of dropped items.</param>
        protected virtual void NotifyDropCompleted(object dropDestinationItem, IList? cloneList)
        {
            DropCompletedEventArgs Args = new DropCompletedEventArgs(DropCompletedEvent, DragSource, dropDestinationItem, cloneList);
            RaiseEvent(Args);
        }
        #endregion
        #region Expand Button Width
        /// <summary>
        /// Identifies the <see cref="ExpandButtonWidth"/> attached property.
        /// </summary>
        public static readonly DependencyProperty ExpandButtonWidthProperty = DependencyProperty.Register("ExpandButtonWidth", typeof(double), typeof(ExtendedTreeViewBase), new FrameworkPropertyMetadata(0.0), new ValidateValueCallback(IsValidExpandButtonWidth));

        /// <summary>
        /// Gets or sets the expand button width.
        /// </summary>
        public double ExpandButtonWidth
        {
            get { return (double)GetValue(ExpandButtonWidthProperty); }
            set { SetValue(ExpandButtonWidthProperty, value); }
        }

        /// <summary>
        /// Checks if an expand button width is valid.
        /// </summary>
        /// <param name="value">The width to check.</param>
        /// <returns>True if valid; Otherwise, false.</returns>
        public static bool IsValidExpandButtonWidth(object value)
        {
            double Width = (double)value;
            return Width >= 0;
        }
        #endregion
        #region Expand Button Style
        /// <summary>
        /// Identifies the <see cref="ExpandButtonStyle"/> attached property.
        /// </summary>
        public static readonly DependencyProperty ExpandButtonStyleProperty = DependencyProperty.Register("ExpandButtonStyle", typeof(Style), typeof(ExtendedTreeViewBase));

        /// <summary>
        /// Gets or sets the expand button style.
        /// </summary>
        public Style ExpandButtonStyle
        {
            get { return (Style)GetValue(ExpandButtonStyleProperty); }
            set { SetValue(ExpandButtonStyleProperty, value); }
        }
        #endregion
        #region Indentation Width
        /// <summary>
        /// Identifies the <see cref="IndentationWidth"/> attached property.
        /// </summary>
        public static readonly DependencyProperty IndentationWidthProperty = DependencyProperty.Register("IndentationWidth", typeof(double), typeof(ExtendedTreeViewBase), new FrameworkPropertyMetadata(0.0), new ValidateValueCallback(IsValidIndentationWidth));

        /// <summary>
        /// Gets or sets the indentation width.
        /// </summary>
        public double IndentationWidth
        {
            get { return (double)GetValue(IndentationWidthProperty); }
            set { SetValue(IndentationWidthProperty, value); }
        }

        /// <summary>
        /// Checks if an indentation width is valid.
        /// </summary>
        /// <param name="value">The width to check.</param>
        /// <returns>True if valid; Otherwise, false.</returns>
        public static bool IsValidIndentationWidth(object value)
        {
            double Width = (double)value;
            return Width >= 0;
        }
        #endregion
        #region Has Context Menu Open
        /// <summary>
        /// Identifies the HasContextMenuOpen attached property.
        /// </summary>
        public static readonly DependencyProperty HasContextMenuOpenProperty = HasContextMenuOpenPropertyKey.DependencyProperty;
        private static DependencyPropertyKey HasContextMenuOpenPropertyKey = DependencyProperty.RegisterAttachedReadOnly("HasContextMenuOpen", typeof(bool), typeof(ExtendedTreeViewBase), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets the value of the HasContextMenuOpen property.
        /// </summary>
        /// <param name="element">The element with the property.</param>
        /// <returns>The property value.</returns>
        public static bool GetHasContextMenuOpen(DependencyObject element)
        {
            if (element != null)
                return (bool)element.GetValue(HasContextMenuOpenProperty);
            else
                return false;
        }
        #endregion
        #endregion

        #region Init
        static ExtendedTreeViewBase()
        {
            OverrideAncestorMetadata();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedTreeViewBase"/> class.
        /// </summary>
        protected ExtendedTreeViewBase()
        {
            if (TryFindResource(typeof(ExtendedTreeViewBase)) is Style DirectDefaultStyle && TryFindResource(typeof(ExtendedTreeViewItemBase)) is Style DirectDefaultItemContainerStyle)
            {
                DefaultStyle = DirectDefaultStyle;
                DefaultItemContainerStyle = DirectDefaultItemContainerStyle;
            }
            else
            {
                Resources.MergedDictionaries.Add(SharedResourceDictionaryManager.SharedDictionary);

                DefaultStyle = (Style)FindResource(typeof(ExtendedTreeViewBase));
                DefaultItemContainerStyle = (Style)FindResource(typeof(ExtendedTreeViewItemBase));
            }

            InitAncestor();
            InitializeImplementation();
            InitializeContextMenu();

            Initialized += OnInitialized; // Dirty trick to avoid warning CA2214.
        }

        /// <summary>
        /// Called when the control has been initialized and before properties are set.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnInitialized(object sender, EventArgs e)
        {
            InitializeDragAndDrop();
        }
        #endregion

        #region Client Interface
        /// <summary>
        /// Gets the list of visible items.
        /// </summary>
        public virtual IList VisibleItems
        {
            get
            {
                IList Result = CreateItemList();
                foreach (object item in VisibleChildren)
                    Result.Add(item);

                return Result;
            }
        }

        /// <summary>
        /// Scrolls the view to make selected items visible.
        /// </summary>
        /// <param name="item">Item to be made visible.</param>
        public virtual void ScrollIntoView(object item)
        {
            ExtendedTreeViewItemBase ItemContainer = ContainerFromItem(item);
            if (ItemContainer != null)
                ItemContainer.BringIntoView();
        }

        /// <summary>
        /// Checks if an item is expanded.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>True if expanded; Otherwise, false.</returns>
        public virtual bool IsExpanded(object item)
        {
            return ExpandedChildren.ContainsKey(item);
        }

        /// <summary>
        /// Expands an item.
        /// </summary>
        /// <param name="item">The item.</param>
        public virtual void Expand(object item)
        {
            ExtendedTreeViewItemBase ItemContainer = ContainerFromItem(item);
            if (ItemContainer != null)
                ItemContainer.IsExpanded = true;
        }

        /// <summary>
        /// Collapses an item.
        /// </summary>
        /// <param name="item">The item.</param>
        public virtual void Collapse(object item)
        {
            ExtendedTreeViewItemBase ItemContainer = ContainerFromItem(item);
            if (ItemContainer != null)
                ItemContainer.IsExpanded = false;
        }

        /// <summary>
        /// Toggles whether an item is expanded.
        /// </summary>
        /// <param name="item">The item.</param>
        public virtual void ToggleIsExpanded(object item)
        {
            ExtendedTreeViewItemBase ItemContainer = ContainerFromItem(item);
            if (ItemContainer != null)
                ItemContainer.IsExpanded = !ItemContainer.IsExpanded;
        }

        /// <summary>
        /// Checks if an item is selected.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>True if selected; Otherwise, false.</returns>
        public virtual bool IsSelected(object item)
        {
            return SelectedItems.Contains(item);
        }

        /// <summary>
        /// Adds an item to the selection.
        /// </summary>
        /// <param name="item">The item.</param>
        public virtual void SetSelected(object item)
        {
            SelectedItem = item;
        }

        /// <summary>
        /// Checks if an item is visible in the scrolled view.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>True if visible; Otherwise, false.</returns>
        public virtual bool IsItemVisible(object item)
        {
            return VisibleChildren.Contains(item);
        }

        /// <summary>
        /// Gets a value indicating whether the control allows to copy items.
        /// </summary>
        /// <returns>True if allowed; Otherwise, false.</returns>
        public virtual bool IsCopyPossible
        {
            get { return DragSource.IsDragPossible(); }
        }
        #endregion

        #region Ancestor Interface
        /// <summary>
        /// Overrides inherited metadata.
        /// </summary>
        protected static void OverrideAncestorMetadata()
        {
            OverrideMetadataItemsSource();
            OverrideMetadataDefaultStyleKey();
        }

        /// <summary>
        /// Overrides inherited metadata for the <see cref="ItemsControl.ItemsSource"/> property.
        /// </summary>
        protected static void OverrideMetadataItemsSource()
        {
            ItemsSourceProperty.OverrideMetadata(typeof(ExtendedTreeViewBase), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.NotDataBindable, null, new CoerceValueCallback(CoerceItemsSource), true));
        }

        /// <summary>
        /// Overrides inherited metadata for the <see cref="FrameworkElement.DefaultStyleKey"/> property.
        /// </summary>
        protected static void OverrideMetadataDefaultStyleKey()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ExtendedTreeViewBase), new FrameworkPropertyMetadata(typeof(ExtendedTreeViewBase)));
        }

        /// <summary>
        /// Ensures the <see cref="ItemsControl.ItemsSource"/> property contains a valid value.
        /// </summary>
        /// <param name="modifiedObject">The object with the modified property.</param>
        /// <param name="value">The value to check.</param>
        /// <returns>True if valid; Otherwise, false.</returns>
        protected static object? CoerceItemsSource(DependencyObject modifiedObject, object value)
        {
            if (modifiedObject is ExtendedTreeViewBase ctrl)
                return ctrl.CoerceItemsSource(value);
            else
                throw new ArgumentOutOfRangeException(nameof(modifiedObject));
        }

        /// <summary>
        /// Ensures the <see cref="ItemsControl.ItemsSource"/> property contains a valid value.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>True if valid; Otherwise, false.</returns>
        protected virtual object? CoerceItemsSource(object value)
        {
            if (value == VisibleChildren)
                return value;
            else
                return null;
        }

        private void InitAncestor()
        {
            CanSelectMultipleItems = true;
        }

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>The element that is used to display the given item.</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return CreateContainerItem();
        }

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own container.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>True if the item is (or is eligible to be) its own container; otherwise, false.</returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is ExtendedTreeViewItemBase;
        }

        /// <summary>
        /// Creates a container for an item.
        /// </summary>
        /// <returns>The created container.</returns>
        protected virtual ExtendedTreeViewItemBase CreateContainerItem()
        {
            return new ExtendedTreeViewItemBase(this);
        }

        /// <summary>
        /// Called when the selection changes.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);

            UpdateIsDragDropPossible();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the list of child items visible in the scroll view.
        /// </summary>
        protected ObservableCollection<object> VisibleChildren { get; } = new ObservableCollection<object>();

        /// <summary>
        /// Gets a list of expanded items.
        /// </summary>
        protected Dictionary<object, IList> ExpandedChildren { get; } = new Dictionary<object, IList>();
        #endregion

        #region Implementation
        private void InitializeImplementation()
        {
            ItemsSource = VisibleChildren;
        }

        /// <summary>
        /// Builds the flat children table.
        /// </summary>
        protected virtual void BuildFlatChildrenTables()
        {
            ResetFlatChildren();
            InsertChildrenFromRoot();
        }

        /// <summary>
        /// Resets the flat children table.
        /// </summary>
        protected virtual void ResetFlatChildren()
        {
            UninstallAllHandlers();

            VisibleChildren.Clear();
            ExpandedChildren.Clear();
        }

        /// <summary>
        /// Uninstall all handlers on items.
        /// </summary>
        protected virtual void UninstallAllHandlers()
        {
            foreach (object item in VisibleChildren)
                UninstallHandlers(item);
        }

        /// <summary>
        /// Inserts child items starting from the content root.
        /// </summary>
        protected virtual void InsertChildrenFromRoot()
        {
            NotifyPreviewCollectionModified(TreeViewCollectionOperation.Insert);

            if (Content != null)
            {
                IInsertItemContext Context = CreateInsertItemContext(Content, 0);
                Context.Start();

                InsertChildren(Context, Content, null);

                Context.Complete();
                Context.Close();
            }

            NotifyCollectionModified(TreeViewCollectionOperation.Insert);
        }

        /// <summary>
        /// Inserts children of an item.
        /// </summary>
        /// <param name="context">The insertion context.</param>
        /// <param name="item">The item with children.</param>
        /// <param name="parentItem">The parent item.</param>
        protected virtual void InsertChildren(IInsertItemContext context, object item, object? parentItem)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            IList Children = GetItemChildren(item);
            bool IsExpanded = IsItemExpandedAtStart || (parentItem == null && IsRootAlwaysExpanded);

            if (IsExpanded)
                ExpandedChildren.Add(item, Children);

            InternalInsert(context.ShownIndex, item);
            context.NextIndex();

            if (IsExpanded)
                foreach (object ChildItem in Children)
                    InsertChildren(context, ChildItem, item);
        }

        /// <summary>
        /// Removes items from the tree.
        /// </summary>
        /// <param name="context">The remove context.</param>
        /// <param name="item">The item from which to remove children.</param>
        /// <param name="parentItem">The parent item.</param>
        protected virtual void RemoveChildren(IRemoveItemContext context, object item, object? parentItem)
        {
            ExtendedTreeViewItemBase ItemContainer = ContainerFromItem(item);

            if ((ItemContainer != null && ItemContainer.IsExpanded) || (ItemContainer == null && IsItemExpandedAtStart))
            {
                IList Children = GetItemChildren(item);
                foreach (object ChildItem in Children)
                    RemoveChildren(context, ChildItem, item);
            }

            if (context != null)
            {
                InternalRemove(context.ShownIndex, item);
                context.NextIndex();
            }

            if (ExpandedChildren.ContainsKey(item))
                ExpandedChildren.Remove(item);
        }

        /// <summary>
        /// Performs the insertion operation.
        /// </summary>
        /// <param name="index">The item index in the list of visible children.</param>
        /// <param name="item">The item to insert.</param>
        protected virtual void InternalInsert(int index, object item)
        {
            VisibleChildren.Insert(index, item);
            InstallHandlers(item);
        }

        /// <summary>
        /// Performs the remove operation.
        /// </summary>
        /// <param name="index">The item index in the list of visible children.</param>
        /// <param name="item">The item to remove.</param>
        protected virtual void InternalRemove(int index, object item)
        {
            UninstallHandlers(item);
            VisibleChildren.RemoveAt(index);
        }
        #endregion

        #region Observable Collection
        /// <summary>
        /// Called when children of an item have changed.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="e">The event data.</param>
        protected virtual void HandleChildrenChanged(object item, NotifyCollectionChangedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    OnItemAddChildren(item, e.NewStartingIndex, e.NewItems);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    OnItemRemoveChildren(item, e.OldStartingIndex, e.OldItems);
                    break;

                case NotifyCollectionChangedAction.Replace:
                    OnItemRemoveChildren(item, e.OldStartingIndex, e.OldItems);
                    OnItemAddChildren(item, e.NewStartingIndex, e.NewItems);
                    break;

                case NotifyCollectionChangedAction.Move:
                    OnItemMoveChildren(item, e.OldStartingIndex, e.NewStartingIndex, e.NewItems);
                    break;

                case NotifyCollectionChangedAction.Reset:
                    OnItemResetChildren(item);
                    break;
            }
        }

        /// <summary>
        /// Called when children have been added to an item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="startIndex">Index where the first child is added.</param>
        /// <param name="itemList">The list of children.</param>
        protected virtual void OnItemAddChildren(object item, int startIndex, IList itemList)
        {
            NotifyPreviewCollectionModified(TreeViewCollectionOperation.Insert);

            if (IsExpanded(item))
            {
                int ShownPreviousChildrenCount = VisibleChildren.IndexOf(item);
                ShownPreviousChildrenCount += CountPreviousChildrenExpanded(item, startIndex, -1);

                int ShownIndex = ShownPreviousChildrenCount;

                IInsertItemContext Context = CreateInsertItemContext(item, ShownIndex);
                Context.Start();

                if (itemList != null)
                    foreach (object ChildItem in itemList)
                        InsertChildren(Context, ChildItem, item);

                Context.Complete();
                Context.Close();
            }

            NotifyCollectionModified(TreeViewCollectionOperation.Insert);
        }

        /// <summary>
        /// Called when children of an item have been removed.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="startIndex">Index of the first removed child.</param>
        /// <param name="itemList">The list of removed children.</param>
        protected virtual void OnItemRemoveChildren(object item, int startIndex, IList itemList)
        {
            NotifyPreviewCollectionModified(TreeViewCollectionOperation.Remove);

            if (IsExpanded(item))
            {
                int ShownPreviousChildrenCount = VisibleChildren.IndexOf(item);
                ShownPreviousChildrenCount += CountPreviousChildrenExpanded(item, startIndex, -1);

                int ShownIndex = ShownPreviousChildrenCount;

                IRemoveItemContext Context = CreateRemoveItemContext(item, ShownIndex);
                Context.Start();

                if (itemList != null)
                    foreach (object ChildItem in itemList)
                        RemoveChildren(Context, ChildItem, item);

                Context.Complete();
                Context.Close();
            }

            NotifyCollectionModified(TreeViewCollectionOperation.Remove);
        }

        /// <summary>
        /// Called when children of an item have been moved.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="oldIndex">Index of the previous position of the first child.</param>
        /// <param name="newIndex">Index of the new position of the first child.</param>
        /// <param name="itemList">The list of moved children.</param>
        protected virtual void OnItemMoveChildren(object item, int oldIndex, int newIndex, IList itemList)
        {
            NotifyPreviewCollectionModified(TreeViewCollectionOperation.Move);

            if (IsExpanded(item))
            {
                if (oldIndex > newIndex)
                {
                    int ShownPreviousChildrenCount = VisibleChildren.IndexOf(item);

                    int RemoveIndex = ShownPreviousChildrenCount;
                    RemoveIndex += CountPreviousChildrenExpanded(item, oldIndex + 1, newIndex);

                    IRemoveItemContext RemoveContext = CreateRemoveItemContext(item, RemoveIndex);
                    RemoveContext.Start();

                    if (itemList != null)
                        foreach (object ChildItem in itemList)
                            RemoveChildren(RemoveContext, ChildItem, item);

                    RemoveContext.Complete();
                    RemoveContext.Close();

                    int InsertIndex = ShownPreviousChildrenCount;
                    InsertIndex += CountPreviousChildrenExpanded(item, newIndex, -1);

                    IInsertItemContext InsertContext = CreateInsertItemContext(item, InsertIndex);
                    InsertContext.Start();

                    if (itemList != null)
                        foreach (object ChildItem in itemList)
                            InsertChildren(InsertContext, ChildItem, item);

                    InsertContext.Complete();
                    InsertContext.Close();
                }
                else if (oldIndex < newIndex)
                {
                    int ShownPreviousChildrenCount = VisibleChildren.IndexOf(item);

                    int RemoveIndex = ShownPreviousChildrenCount;
                    RemoveIndex += CountPreviousChildrenExpanded(item, oldIndex, -1);

                    IRemoveItemContext RemoveContext = CreateRemoveItemContext(item, RemoveIndex);
                    RemoveContext.Start();

                    if (itemList != null)
                        foreach (object ChildItem in itemList)
                            RemoveChildren(RemoveContext, ChildItem, item);

                    RemoveContext.Complete();
                    RemoveContext.Close();

                    int InsertIndex = ShownPreviousChildrenCount;
                    InsertIndex += CountPreviousChildrenExpanded(item, newIndex, -1);

                    IInsertItemContext InsertContext = CreateInsertItemContext(item, InsertIndex);
                    InsertContext.Start();

                    if (itemList != null)
                        foreach (object ChildItem in itemList)
                            InsertChildren(InsertContext, ChildItem, item);

                    InsertContext.Complete();
                    InsertContext.Close();
                }
            }

            NotifyCollectionModified(TreeViewCollectionOperation.Move);
        }

        /// <summary>
        /// Called when children of an item are reset.
        /// </summary>
        /// <param name="item">The item.</param>
        protected virtual void OnItemResetChildren(object item)
        {
            NotifyPreviewCollectionModified(TreeViewCollectionOperation.Remove);

            if (IsExpanded(item))
            {
                object? ParentItem = GetItemParent(item);

                int StartIndex;
                int RemoveCount;

                if (ParentItem != null)
                {
                    StartIndex = VisibleChildren.IndexOf(item) + 1;

                    IList Siblings = GetItemChildren(ParentItem);
                    int ItemIndex = Siblings.IndexOf(item);
                    if (ItemIndex + 1 < Siblings.Count)
                    {
                        object NextItem = Siblings[ItemIndex + 1];
                        int EndIndex = VisibleChildren.IndexOf(NextItem);
                        RemoveCount = EndIndex - StartIndex;
                    }
                    else
                        RemoveCount = CountVisibleChildren(item);
                }
                else
                {
                    StartIndex = 1;
                    RemoveCount = VisibleChildren.Count - 1;
                }

                IRemoveItemContext Context = CreateRemoveItemContext(item, StartIndex);
                Context.Start();

                for (int i = 0; i < RemoveCount; i++)
                {
                    object RemovedItem = VisibleChildren[Context.ShownIndex];

                    InternalRemove(Context.ShownIndex, RemovedItem);
                    Context.NextIndex();

                    if (ExpandedChildren.ContainsKey(RemovedItem))
                        ExpandedChildren.Remove(RemovedItem);
                }

                Context.Complete();
                Context.Close();
            }

            NotifyCollectionModified(TreeViewCollectionOperation.Remove);
        }
        #endregion

        #region Misc
        /// <summary>
        /// Gets the container associated to an item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The container.</returns>
        protected virtual ExtendedTreeViewItemBase ContainerFromItem(object item)
        {
            return (ExtendedTreeViewItemBase)ItemContainerGenerator.ContainerFromItem(item);
        }

        /// <summary>
        /// Gets the container associated to the item at the provided position.
        /// </summary>
        /// <param name="index">the item position.</param>
        /// <returns>The container.</returns>
        protected virtual ExtendedTreeViewItemBase ContainerFromIndex(int index)
        {
            return (ExtendedTreeViewItemBase)ItemContainerGenerator.ContainerFromIndex(index);
        }

        /// <summary>
        /// Counts how many children are expanded.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="index">The position from where to count.</param>
        /// <param name="excludedIndex">Index of the child item excluded from the count.</param>
        /// <returns>The number of expanded children.</returns>
        protected virtual int CountPreviousChildrenExpanded(object item, int index, int excludedIndex)
        {
            int Result = 1;

            ExtendedTreeViewItemBase ItemContainer = ContainerFromItem(item);

            if ((ItemContainer != null && ItemContainer.IsExpanded) || (ItemContainer == null && IsItemExpandedAtStart) || (ItemContainer == null && item == Content && IsRootAlwaysExpanded))
                for (int i = 0; i < index; i++)
                    if (i != excludedIndex)
                    {
                        object Child = GetItemChild(item, i);
                        Result += CountPreviousChildrenExpanded(Child, GetItemChildrenCount(Child), -1);
                    }

            return Result;
        }

        /// <summary>
        /// Counts how many children are visible.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The number of visible children.</returns>
        protected virtual int CountVisibleChildren(object item)
        {
            int Result = 0;

            ExtendedTreeViewItemBase ItemContainer = ContainerFromItem(item);

            if ((ItemContainer != null && ItemContainer.IsExpanded) || (ItemContainer == null && IsItemExpandedAtStart))
            {
                IList Children = GetItemChildren(item);
                Result += Children.Count;

                foreach (object Child in Children)
                    Result += CountVisibleChildren(Child);
            }

            return Result;
        }

        /// <summary>
        /// Sets an item to be expanded.
        /// </summary>
        /// <param name="item">The item.</param>
        public void SetItemExpanded(object item)
        {
            IList Children = GetItemChildren(item);
            ExpandedChildren.Add(item, Children);

            int Index = VisibleChildren.IndexOf(item) + 1;
            Expand(item, Index);
        }

        /// <summary>
        /// Checks if an item can be expanded.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>True if expandable; othewise, false.</returns>
        public bool IsItemExpandable(object item)
        {
            return !IsCtrlDown() && GetItemChildrenCount(item) > 0;
        }

        /// <summary>
        /// Checks if an item can be collapsed.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>True if collapsible; othewise, false.</returns>
        public bool IsItemCollapsible(object item)
        {
            return !IsCtrlDown() && GetItemChildrenCount(item) > 0 && (item != Content || !IsRootAlwaysExpanded);
        }

        /// <summary>
        /// Expands an item at the provided position.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="index">The item position.</param>
        /// <returns>The new position.</returns>
        protected virtual int Expand(object item, int index)
        {
            int NewIndex = index;

            IList Children = GetItemChildren(item);
            foreach (object ChildItem in Children)
            {
                InternalInsert(NewIndex++, ChildItem);

                if (IsExpanded(ChildItem))
                    NewIndex = Expand(ChildItem, NewIndex);
            }

            return NewIndex;
        }

        /// <summary>
        /// Collapses an item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void SetItemCollapsed(object item)
        {
            ExpandedChildren.Remove(item);

            int Index = VisibleChildren.IndexOf(item) + 1;
            Collapse(item, Index);
        }

        /// <summary>
        /// Collapses an item at the provided position.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="index">The item position.</param>
        protected virtual void Collapse(object item, int index)
        {
            IList Children = GetItemChildren(item);
            foreach (object ChildItem in Children)
            {
                InternalRemove(index, ChildItem);

                if (IsExpanded(ChildItem))
                    Collapse(ChildItem, index);
            }
        }

        /// <summary>
        /// Checks whether an item can be cloned.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>True if cloneable; otherwise, false.</returns>
        protected virtual bool IsItemCloneable(object item)
        {
            return item is ICloneable;
        }

        /// <summary>
        /// Gets the indentation level of an item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The indentation level.</returns>
        public int ItemLevel(object item)
        {
            int Level = -1;

            object? CurrentItem = item;
            while (CurrentItem != null)
            {
                Level++;
                CurrentItem = GetItemParent(CurrentItem);
            }

            return Level;
        }

        /// <summary>
        /// Checks whether a container is the destination of a drop.
        /// </summary>
        /// <param name="itemContainer">The container.</param>
        /// <returns>True if destination of a drop; otherwise, false.</returns>
        public static bool IsDropOver(ExtendedTreeViewItemBase itemContainer)
        {
            return itemContainer == DropTargetContainer;
        }

        /// <summary>
        /// Clicks the item before <paramref name="item"/>.
        /// </summary>
        /// <param name="item">The item.</param>
        public void ClickPreviousItem(object item)
        {
            if (!IsCtrlDown())
            {
                int ItemIndex = VisibleChildren.IndexOf(item);
                if (ItemIndex > 0)
                    LeftClickSelect(VisibleChildren[ItemIndex - 1]);
            }
        }

        /// <summary>
        /// Clicks the item after <paramref name="item"/>.
        /// </summary>
        /// <param name="item">The item.</param>
        public void ClickNextItem(object item)
        {
            if (!IsCtrlDown())
            {
                int ItemIndex = VisibleChildren.IndexOf(item);
                if (ItemIndex >= 0 && ItemIndex + 1 < VisibleChildren.Count)
                    LeftClickSelect(VisibleChildren[ItemIndex + 1]);
            }
        }

        /// <summary>
        /// Clicks an item to select it.
        /// </summary>
        /// <param name="item">The item.</param>
        public void LeftClickSelect(object item)
        {
            switch (SelectionMode)
            {
                case SelectionMode.Single:
                    if (!SelectedItems.Contains(item))
                    {
                        UnselectAll();
                        AddSelectedItem(item);
                        SetLastSelectedItem(item);
                    }
                    break;

                case SelectionMode.Multiple:
                    if (!SelectedItems.Contains(item))
                    {
                        AddSelectedItem(item);
                        SetLastSelectedItem(item);
                        SetLastClickedItem(item);
                    }
                    break;

                case SelectionMode.Extended:
                    if (IsShiftDown())
                    {
                        int FirstIndex = VisibleChildren.IndexOf(item);
                        int LastIndex = LastSelectedItem != null && IsLastSelectedItemSet ? VisibleChildren.IndexOf(LastSelectedItem) : FirstIndex;

                        if (FirstIndex >= 0 && LastIndex >= 0)
                        {
                            if (FirstIndex > LastIndex)
                            {
                                int Index = FirstIndex;
                                FirstIndex = LastIndex;
                                LastIndex = Index;
                            }

                            BeginUpdateSelectedItems();

                            if (!IsCtrlDown())
                                SelectedItems.Clear();
                            for (int i = FirstIndex; i <= LastIndex; i++)
                                AddSelectedItem(VisibleChildren[i]);

                            EndUpdateSelectedItems();

                            if (!IsLastSelectedItemSet)
                                SetLastSelectedItem(item);
                        }
                    }
                    else if (IsCtrlDown())
                    {
                        if (!SelectedItems.Contains(item))
                        {
                            AddSelectedItem(item);
                            SetLastClickedItem(item);
                        }
                    }
                    else if (!SelectedItems.Contains(item))
                    {
                        UnselectAll();
                        AddSelectedItem(item);
                        SetLastSelectedItem(item);
                        SetLastClickedItem(item);
                    }
                    break;
            }
        }

        /// <summary>
        /// Clicks an item and unselects it.
        /// </summary>
        /// <param name="item">The item.</param>
        public void LeftClickUnselect(object item)
        {
            switch (SelectionMode)
            {
                case SelectionMode.Single:
                    break;

                case SelectionMode.Multiple:
                    if (SelectedItems.Contains(item))
                        if (!IsLastClickedItem(item))
                            RemoveSelectedItem(item);
                        else
                            ClearLastClickedItem();
                    break;

                case SelectionMode.Extended:
                    if (IsShiftDown())
                    {
                    }
                    else if (IsCtrlDown())
                    {
                        if (SelectedItems.Contains(item))
                            if (!IsLastClickedItem(item))
                                RemoveSelectedItem(item);
                            else
                                ClearLastClickedItem();
                    }
                    else
                    {
                        UnselectAll();
                        AddSelectedItem(item);
                        SetLastSelectedItem(item);
                        ClearLastClickedItem();
                    }
                    break;
            }
        }

        /// <summary>
        /// Right-click an item to select it.
        /// </summary>
        /// <param name="item">The item.</param>
        public void RightClickSelect(object item)
        {
            switch (SelectionMode)
            {
                case SelectionMode.Single:
                    if (!SelectedItems.Contains(item))
                    {
                        UnselectAll();
                        AddSelectedItem(item);
                        SetLastSelectedItem(item);
                    }
                    break;

                case SelectionMode.Multiple:
                    if (!SelectedItems.Contains(item))
                    {
                        AddSelectedItem(item);
                        SetLastSelectedItem(item);
                        SetLastClickedItem(item);
                    }
                    break;

                case SelectionMode.Extended:
                    if (!IsShiftDown() && !IsCtrlDown() && !SelectedItems.Contains(item))
                    {
                        UnselectAll();
                        AddSelectedItem(item);
                        SetLastSelectedItem(item);
                    }
                    break;
            }
        }

        /// <summary>
        /// Right-click an item and unselects it.
        /// </summary>
        /// <param name="item">The item.</param>
        public void RightClickUnselect(object item)
        {
            switch (SelectionMode)
            {
                case SelectionMode.Single:
                    break;

                case SelectionMode.Multiple:
                    if (SelectedItems.Contains(item))
                        if (!IsLastClickedItem(item))
                            RemoveSelectedItem(item);
                        else
                            ClearLastClickedItem();
                    break;

                case SelectionMode.Extended:
                    break;
            }
        }

        /// <summary>
        /// Gets the source of an event.
        /// </summary>
        /// <param name="e">The event data.</param>
        /// <returns>The event source.</returns>
        protected virtual ExtendedTreeViewItemBase? GetEventSourceItem(RoutedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            DependencyObject? Current = e.OriginalSource as DependencyObject;
            while (Current != null)
            {
                if (Current is ExtendedTreeViewItemBase AsContainerItem)
                    return AsContainerItem;

                if (Current is ToggleButton)
                    return null;

                Current = VisualTreeHelper.GetParent(Current);
            }

            return null;
        }

        /// <summary>
        /// Gets the target of a drop event.
        /// </summary>
        /// <param name="e">The event data.</param>
        /// <returns>The event target.</returns>
        protected virtual ExtendedTreeViewItemBase? GetTarget(DragEventArgs e)
        {
            return GetEventSourceItem(e);
        }

        /// <summary>
        /// Checks whether the CTRL key is down.
        /// </summary>
        /// <returns>True if down; otherwise, false.</returns>
        protected virtual bool IsCtrlDown()
        {
            return (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && !(Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt));
        }

        /// <summary>
        /// Checks whether the SHIFT key is down.
        /// </summary>
        /// <returns>True if down; otherwise, false.</returns>
        protected virtual bool IsShiftDown()
        {
            return Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
        }

        /// <summary>
        /// Sorts two items by index.
        /// </summary>
        /// <param name="item1">The first item.</param>
        /// <param name="item2">The second item.</param>
        /// <returns>The sort result.</returns>
        protected virtual int SortByIndex(object item1, object item2)
        {
            return VisibleChildren.IndexOf(item1) - VisibleChildren.IndexOf(item2);
        }

        /// <summary>
        /// Adds an item to the list of selected items.
        /// </summary>
        /// <param name="item">The item.</param>
        protected virtual void AddSelectedItem(object item)
        {
            SelectedItems.Add(item);
        }

        /// <summary>
        /// Removes an item from the list of selected items.
        /// </summary>
        /// <param name="item">The item.</param>
        protected virtual void RemoveSelectedItem(object item)
        {
            if (IsLastSelectedItem(item))
                ClearLastSelectedItem();

            SelectedItems.Remove(item);
        }

        /// <summary>
        /// Gets a value indicating whether there is a last selected item.
        /// </summary>
        protected virtual bool IsLastSelectedItemSet
        {
            get { return LastSelectedItem != null; }
        }

        /// <summary>
        /// Sets the last selected item.
        /// </summary>
        /// <param name="item">The item.</param>
        protected virtual void SetLastSelectedItem(object item)
        {
            LastSelectedItem = item;
        }

        /// <summary>
        /// Checks whether an item is the last selected.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>True if the last selected.</returns>
        protected virtual bool IsLastSelectedItem(object item)
        {
            return LastSelectedItem == item;
        }

        /// <summary>
        /// Clears the last selected item.
        /// </summary>
        protected virtual void ClearLastSelectedItem()
        {
            LastSelectedItem = null;
        }

        /// <summary>
        /// Gets a value indicating whether there is a last clicked item.
        /// </summary>
        protected virtual bool IsLastClickedItemSet
        {
            get { return LastClickedItem != null; }
        }

        /// <summary>
        /// Sets the last clicked item.
        /// </summary>
        /// <param name="item">The item.</param>
        protected virtual void SetLastClickedItem(object item)
        {
            LastClickedItem = item;
        }

        /// <summary>
        /// Checks whether an item is the last clicked.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>True if the last clicked.</returns>
        protected virtual bool IsLastClickedItem(object item)
        {
            return LastClickedItem == item;
        }

        /// <summary>
        /// Clears the last clicked item.
        /// </summary>
        protected virtual void ClearLastClickedItem()
        {
            LastClickedItem = null;
        }

        /// <summary>
        /// Gets the last selected item.
        /// </summary>
        protected object? LastSelectedItem { get; private set; }

        /// <summary>
        /// Gets the last clicked item.
        /// </summary>
        protected object? LastClickedItem { get; private set; }
        #endregion

        #region Cursors
        /// <summary>
        /// Initializes cursors by index.
        /// </summary>
        /// <param name="cursorIndex">Index of the cursor.</param>
        /// <returns>The initialized cursor.</returns>
        protected static Cursor InitializeCursor(int cursorIndex)
        {
            string SystemPath = Environment.GetFolderPath(Environment.SpecialFolder.System);
            string Ole32Path = Path.Combine(SystemPath, "ole32.dll");

            return LoadCursorFromResourceFile(Ole32Path, cursorIndex);
        }

        /// <summary>
        /// Loads a cursor from a file.
        /// </summary>
        /// <param name="filePath">The path to the file.</param>
        /// <param name="resourceId">The cursor resource Id.</param>
        /// <returns>The loaded cursor.</returns>
        protected static Cursor LoadCursorFromResourceFile(string filePath, int resourceId)
        {
            CursorResource CursorFromResource = new CursorResource(filePath, (uint)resourceId);
            return CursorFromResource.AsCursor;
        }

        /// <summary>
        /// Gets the default forbidden cursor.
        /// </summary>
        protected static Cursor DefaultCursorForbidden { get; } = InitializeCursor(1);

        /// <summary>
        /// Gets the default move cursor.
        /// </summary>
        protected static Cursor DefaultCursorMove { get; } = InitializeCursor(2);

        /// <summary>
        /// Gets the default copy cursor.
        /// </summary>
        protected static Cursor DefaultCursorCopy { get; } = InitializeCursor(3);
        #endregion

        #region Mouse Events
        /// <summary>
        /// Invoked when an unhandled <see cref="System.Windows.UIElement.PreviewMouseLeftButtonDown"/> routed event reaches an element in its route that is derived from this class.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            DebugCall();

            base.OnPreviewMouseLeftButtonDown(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="System.Windows.UIElement.MouseLeftButtonDown"/> routed event is raised on this element.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            DebugCall();
            base.OnMouseLeftButtonDown(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="System.Windows.UIElement.PreviewMouseLeftButtonUp"/> routed event reaches an element in its route that is derived from this class.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            DebugCall();
            base.OnPreviewMouseLeftButtonUp(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="System.Windows.UIElement.MouseLeftButtonUp"/> routed event is raised on this element.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            DebugCall();
            base.OnMouseLeftButtonUp(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="System.Windows.UIElement.PreviewMouseMove"/> routed event reaches an element in its route that is derived from this class.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            DebugCall();
            base.OnPreviewMouseMove(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="System.Windows.UIElement.MouseMove"/> routed event is raised on this element.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            DebugCall();
            DragAfterMouseMove(e);
            base.OnMouseMove(e);
        }
        #endregion

        #region Drag & Drop
        private void InitializeDragAndDrop()
        {
            DragSource = CreateSourceControl();
            DragSource.DragActivityChanged += OnDragActivityChanged;
            DropTargetContainer = null;
            DropTargetContainerIndex = -1;
        }

        /// <summary>
        /// Performs a drag after the mouse has moved.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected virtual void DragAfterMouseMove(MouseEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (AllowDragDrop && e.LeftButton == MouseButtonState.Pressed && (Keyboard.FocusedElement is ExtendedTreeViewItemBase))
                if (IsCopyPossible)
                {
                    ExtendedTreeViewItemBase? ItemContainer = GetEventSourceItem(e);
                    if (ItemContainer != null)
                    {
                        DebugMessage("Drag Started");

                        DragSource.DragAfterMouseMove(e);
                    }
                }
        }

        /// <summary>
        /// Updates the drag drop allowed properties.
        /// </summary>
        protected virtual void UpdateIsDragDropPossible()
        {
            CanonicSelection canonicSelectedItemList = new CanonicSelection(CreateItemList());
            if (GetCanonicSelectedItemList(canonicSelectedItemList))
                DragSource.SetIsDragPossible(canonicSelectedItemList);
            else
                DragSource.ClearIsDragPossible();
        }

        /// <summary>
        /// Gets the list of selected ims.
        /// </summary>
        /// <param name="canonicSelectedItemList">The list to fill.</param>
        /// <returns>True if successful; otherwise, false.</returns>
        protected virtual bool GetCanonicSelectedItemList(CanonicSelection canonicSelectedItemList)
        {
            if (canonicSelectedItemList == null)
                return false;

            if (SelectedItems.Count == 0)
                return false;

            List<object> SortedSelectedItems = new List<object>();
            foreach (object item in SelectedItems)
                SortedSelectedItems.Add(item);

            SortedSelectedItems.Sort(SortByIndex);

            object FirstItem = SortedSelectedItems[0];
            if (FirstItem == Content)
                return false;

            object? FirstItemParent = GetItemParent(FirstItem);

            if (FirstItemParent != null)
            {
                if (GetItemsWithSameParent(SortedSelectedItems, FirstItemParent, canonicSelectedItemList))
                {
                    canonicSelectedItemList.DraggedItemParent = FirstItemParent;
                    return true;
                }

                if (GetItemsInSameBranch(SortedSelectedItems, FirstItemParent, canonicSelectedItemList))
                {
                    canonicSelectedItemList.DraggedItemParent = FirstItemParent;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the list of selected items with the same parent.
        /// </summary>
        /// <param name="sortedSelectedItems">The list of selected items, sorted.</param>
        /// <param name="firstItemParent">The parent of the first item.</param>
        /// <param name="canonicSelectedItemList">The list of selected items, unsorted.</param>
        /// <returns>True if successful; otherwise, false.</returns>
        protected virtual bool GetItemsWithSameParent(IList sortedSelectedItems, object firstItemParent, CanonicSelection canonicSelectedItemList)
        {
            if (sortedSelectedItems == null || canonicSelectedItemList == null)
                return false;

            canonicSelectedItemList.AllItemsCloneable = true;

            foreach (object item in sortedSelectedItems)
            {
                if (GetItemParent(item) != firstItemParent)
                    return false;

                canonicSelectedItemList.ItemList.Add(item);

                if (!IsItemCloneable(item))
                    canonicSelectedItemList.AllItemsCloneable = false;
            }

            return true;
        }

        /// <summary>
        /// Gets the list of selected items in the same branch.
        /// </summary>
        /// <param name="sortedSelectedItems">The list of selected items, sorted.</param>
        /// <param name="firstItemParent">The parent of the first item.</param>
        /// <param name="canonicSelectedItemList">The list of selected items, unsorted.</param>
        /// <returns>True if successful; otherwise, false.</returns>
        protected virtual bool GetItemsInSameBranch(IList sortedSelectedItems, object firstItemParent, CanonicSelection canonicSelectedItemList)
        {
            if (sortedSelectedItems == null || canonicSelectedItemList == null)
                return false;

            IList Children = GetItemChildren(firstItemParent);
            foreach (object ChildItem in Children)
            {
                if (sortedSelectedItems.Contains(ChildItem))
                {
                    if (!canonicSelectedItemList.ItemList.Contains(ChildItem))
                        canonicSelectedItemList.ItemList.Add(ChildItem);
                    if (!IsEntireBranchSelected(sortedSelectedItems, ChildItem, canonicSelectedItemList))
                        return false;
                }
            }

            if (canonicSelectedItemList.RecordCount < sortedSelectedItems.Count)
                return false;

            return true;
        }

        /// <summary>
        /// Checks if all items of a branch are selected.
        /// </summary>
        /// <param name="sortedSelectedItems">The list of selected items, sorted.</param>
        /// <param name="item">The parent item.</param>
        /// <param name="canonicSelectedItemList">The list of selected items, unsorted.</param>
        /// <returns>True if in the same branch; otherwise, false.</returns>
        protected virtual bool IsEntireBranchSelected(IList sortedSelectedItems, object item, CanonicSelection canonicSelectedItemList)
        {
            if (sortedSelectedItems == null || canonicSelectedItemList == null)
                return false;

            if (!IsItemCloneable(item))
                canonicSelectedItemList.AllItemsCloneable = false;

            canonicSelectedItemList.RecordCount++;

            if (IsExpanded(item))
            {
                IList Children = GetItemChildren(item);
                foreach (object ChildItem in Children)
                {
                    if (!sortedSelectedItems.Contains(ChildItem))
                        return false;

                    if (!IsEntireBranchSelected(sortedSelectedItems, ChildItem, canonicSelectedItemList))
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Occurs when activity of the current drag drop operation has changed.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnDragActivityChanged(object sender, EventArgs e)
        {
            if (sender == null)
                throw new ArgumentNullException(nameof(sender));

            IDragSourceControl Ctrl = (IDragSourceControl)sender;
            bool IsHandled = false;

            switch (Ctrl.DragActivity)
            {
                case DragActivity.Idle:
                    IsHandled = true;
                    break;

                case DragActivity.Starting:
                    DragSource.SetDragItemList(Content, FlatItemList(DragSource.ItemList));

                    CancellationToken cancellation = new CancellationToken();
                    NotifyDragStarting(cancellation);
                    if (cancellation.IsCanceled)
                        Ctrl.CancelDrag();

                    IsHandled = true;
                    break;

                case DragActivity.Started:
                    DataObject Data = new DataObject(DragSource.GetType(), DragSource);
                    DragDropEffects AllowedEffects = DragDropEffects.Move;
                    if (DragSource.AllowDropCopy)
                        AllowedEffects |= DragDropEffects.Copy;
                    DragDrop.DoDragDrop(this, Data, AllowedEffects);
                    ClearCurrentDropTarget();

                    IsHandled = true;
                    break;

                default:
                    break;
            }

            Debug.Assert(IsHandled);
        }

        /// <summary>
        /// Gets the flattened list of items.
        /// </summary>
        /// <param name="other">the source list.</param>
        /// <returns>The flattened list.</returns>
        protected virtual IList FlatItemList(IList? other)
        {
            IList Result = CreateItemList();

            if (other != null)
                foreach (object item in other)
                {
                    Result.Add(item);

                    IList FlatChildren = FlatItemList(GetItemChildren(item));
                    foreach (object Child in FlatChildren)
                        Result.Add(Child);
                }

            return Result;
        }

        /// <summary>
        /// Called when feedback for the user is needed.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnGiveFeedback(GiveFeedbackEventArgs e)
        {
            if (e != null)
            {
                Cursor? FeedbackCursor;

                if (UseDefaultCursors)
                    FeedbackCursor = null;
                else if (e.Effects.HasFlag(DragDropEffects.Copy))
                    FeedbackCursor = (CursorCopy != null) ? CursorCopy : DefaultCursorCopy;
                else if (e.Effects.HasFlag(DragDropEffects.Move))
                    FeedbackCursor = (CursorMove != null) ? CursorMove : DefaultCursorMove;
                else
                    FeedbackCursor = (CursorForbidden != null) ? CursorForbidden : DefaultCursorForbidden;

                if (FeedbackCursor != null)
                {
                    e.UseDefaultCursors = false;
                    Mouse.SetCursor(FeedbackCursor);
                    e.Handled = true;
                }
                else
                    base.OnGiveFeedback(e);
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="System.Windows.UIElement.DragEnter"/> attached event reaches an element in its route that is derived from this class.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnDragEnter(DragEventArgs e)
        {
            if (e != null)
            {
                UpdateCurrentDropTarget(e, false);

                e.Effects = GetAllowedDropEffects(e);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="System.Windows.UIElement.DragLeave"/> attached event reaches an element in its route that is derived from this class.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnDragLeave(DragEventArgs e)
        {
            if (e != null)
            {
                UpdateCurrentDropTarget(e, true);

                e.Effects = GetAllowedDropEffects(e);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="System.Windows.UIElement.DragOver"/> attached event reaches an element in its route that is derived from this class.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnDragOver(DragEventArgs e)
        {
            if (e != null)
            {
                UpdateCurrentDropTarget(e, false);

                e.Effects = GetAllowedDropEffects(e);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="System.Windows.UIElement.Drop"/> attached event reaches an element in its route that is derived from this class.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnDrop(DragEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            e.Effects = GetAllowedDropEffects(e);
            e.Handled = true;

            if (e.Effects != DragDropEffects.None)
            {
                ExtendedTreeViewItemBase? ItemContainer = GetEventSourceItem(e);
                IDragSourceControl AsDragSource = (IDragSourceControl)e.Data.GetData(DragSource.GetType());
                object? SourceItem = AsDragSource.DragParentItem;
                IList? ItemList = AsDragSource.ItemList;
                object? DestinationItem = ItemContainer != null ? ItemContainer.Content : null;

                UnselectAll();

                if (SourceItem != null && DestinationItem != null)
                {
                    if (e.Effects == DragDropEffects.Copy)
                    {
                        IList CloneList = CreateItemList();
                        NotifyPreviewDropCompleted(DestinationItem, CloneList);
                        DragDropCopy(SourceItem, DestinationItem, ItemList, CloneList);
                        NotifyDropCompleted(DestinationItem, CloneList);
                    }
                    else if (e.Effects == DragDropEffects.Move)
                    {
                        NotifyPreviewDropCompleted(DestinationItem, null);
                        DragDropMove(SourceItem, DestinationItem, ItemList);
                        NotifyDropCompleted(DestinationItem, null);
                    }

                    Expand(DestinationItem);
                }
            }
        }

        /// <summary>
        /// Gets the list of allowed drag drop effects for an operation.
        /// </summary>
        /// <param name="e">The event data.</param>
        /// <returns>The list of allowed effects.</returns>
        protected virtual DragDropEffects GetAllowedDropEffects(DragEventArgs e)
        {
            IDragSourceControl? AsDragSource = ValidDragSourceFromArgs(e);
            if (AsDragSource != null)
            {
                object? DropDestinationItem = ValidDropDestinationFromArgs(e, AsDragSource);
                if (DropDestinationItem != null)
                {
                    PermissionToken permission = new PermissionToken();
                    NotifyDropCheck(DropDestinationItem, permission);

                    if (permission.IsAllowed)
                        return MergedAllowedEffects(e);
                }
            }

            return DragDropEffects.None;
        }

        /// <summary>
        /// Updates the target of a drag drop operation.
        /// </summary>
        /// <param name="e">The event data.</param>
        /// <param name="isLeaving">True if the operation is to leave the target.</param>
        protected virtual void UpdateCurrentDropTarget(DragEventArgs e, bool isLeaving)
        {
            ExtendedTreeViewItemBase? DropTarget;
            if (isLeaving)
                DropTarget = null;
            else
                DropTarget = GetTarget(e);

            if (DropTargetContainer != DropTarget)
            {
                ExtendedTreeViewItemBase? OldDropTarget = DropTargetContainer;
                DropTargetContainer = DropTarget;
                ExtendedTreeViewItemBase? NewDropTarget = DropTargetContainer;

                if (OldDropTarget != null)
                    OldDropTarget.UpdateIsDropOver();
                if (NewDropTarget != null)
                {
                    NewDropTarget.UpdateIsDropOver();

                    object NewItem = NewDropTarget.Content;
                    if (NewItem != null)
                    {
                        int NewIndex = VisibleChildren.IndexOf(NewItem);
                        if (NewIndex != -1 && DropTargetContainerIndex != -1)
                        {
                            if (NewIndex > DropTargetContainerIndex && NewIndex + 1 < VisibleChildren.Count)
                                ScrollIntoView(VisibleChildren[NewIndex + 1]);
                            if (NewIndex < DropTargetContainerIndex && NewIndex > 0)
                                ScrollIntoView(VisibleChildren[NewIndex - 1]);
                        }

                        DropTargetContainerIndex = NewIndex;
                    }
                }
            }
        }

        /// <summary>
        /// Clears the target of a drag drop operation.
        /// </summary>
        protected virtual void ClearCurrentDropTarget()
        {
            Dispatcher.BeginInvoke(new ClearCurrentDropTargetHandler(OnClearCurrentDropTarget));
        }

        /// <summary>
        /// Represents the method that will handle a ClearCurrentDropTarget event.
        /// </summary>
        protected delegate void ClearCurrentDropTargetHandler();

        /// <summary>
        /// Handles a ClearCurrentDropTarget event.
        /// </summary>
        protected virtual void OnClearCurrentDropTarget()
        {
            ExtendedTreeViewItemBase? OldDropTarget = DropTargetContainer;
            DropTargetContainer = null;
            DropTargetContainerIndex = -1;

            if (OldDropTarget != null)
                OldDropTarget.UpdateIsDropOver();
        }

        /// <summary>
        /// Gets the drag source from arguments of a drag drop event.
        /// </summary>
        /// <param name="e">The event data.</param>
        /// <returns>The drag source.</returns>
        protected virtual IDragSourceControl? ValidDragSourceFromArgs(DragEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (!e.Data.GetDataPresent(DragSource.GetType()))
                return null;

            IDragSourceControl? AsDragSource = e.Data.GetData(DragSource.GetType()) as IDragSourceControl;
            if (AsDragSource == null)
                return null;

            if (AsDragSource.RootItem == null || Content == null)
                return null;

            if (AsDragSource.RootItem.GetType() != Content.GetType())
                return null;

            IList? FlatItemList = AsDragSource.FlatItemList;
            if (FlatItemList == null || FlatItemList.Count == 0)
                return null;

            return AsDragSource;
        }

        /// <summary>
        /// Gets the drag target from arguments of a drag drop event.
        /// </summary>
        /// <param name="e">The event data.</param>
        /// <param name="asDragSource">The drag source.</param>
        /// <returns>The drag target.</returns>
        protected virtual object? ValidDropDestinationFromArgs(DragEventArgs e, IDragSourceControl asDragSource)
        {
            if (DropTargetContainer == null || e == null || asDragSource == null)
                return null;

            object DropDestinationItem = DropTargetContainer.Content;

            if (asDragSource.SourceGuid != DragSource.SourceGuid)
                return null;

            if (asDragSource.DragParentItem == DropDestinationItem)
                if (!e.AllowedEffects.HasFlag(DragDropEffects.Copy) || !e.KeyStates.HasFlag(DragDropKeyStates.ControlKey))
                    return null;

            IList? FlatItemList = asDragSource.FlatItemList;
            if (FlatItemList != null && FlatItemList.Contains(DropDestinationItem))
                return null;

            return DropDestinationItem;
        }

        /// <summary>
        /// Merges allowed effects with those allowed by a drag drop event.
        /// </summary>
        /// <param name="e">The event data.</param>
        /// <returns>Merged allowed effects.</returns>
        protected virtual DragDropEffects MergedAllowedEffects(DragEventArgs e)
        {
            if (e != null)
            {
                if (e.AllowedEffects.HasFlag(DragDropEffects.Move))
                    if (e.AllowedEffects.HasFlag(DragDropEffects.Copy) && e.KeyStates.HasFlag(DragDropKeyStates.ControlKey))
                        return DragDropEffects.Copy;
                    else
                        return DragDropEffects.Move;
                else if (e.AllowedEffects.HasFlag(DragDropEffects.Copy))
                    return DragDropEffects.Copy;
                else
                    return DragDropEffects.None;
            }
            else
                return DragDropEffects.None;
        }

        private IDragSourceControl DragSource = new EmptyDragSourceControl();
        private static ExtendedTreeViewItemBase? DropTargetContainer;
        private static int DropTargetContainerIndex;
        #endregion

        #region Context Menu Open
        private void InitializeContextMenu()
        {
            CurrentlyFocusedContainer = null;
        }

        /// <summary>
        /// Calleds when a container looses the focus.
        /// </summary>
        public void ContainerLostFocus()
        {
            RemoveKeyboardFocusWithinHandler();
        }

        /// <summary>
        /// Calleds when a container gets the focus.
        /// </summary>
        /// <param name="container">The container.</param>
        public void ContainerGotFocus(ExtendedTreeViewItemBase container)
        {
            AddKeyboardFocusWithinHandler(container);
        }

        /// <summary>
        /// Adds a keyboard focus handler for a container.
        /// </summary>
        /// <param name="container">The container.</param>
        protected virtual void AddKeyboardFocusWithinHandler(ExtendedTreeViewItemBase container)
        {
            RemoveKeyboardFocusWithinHandler();

            CurrentlyFocusedContainer = container ?? throw new ArgumentNullException(nameof(container));
            CurrentlyFocusedContainer.IsKeyboardFocusWithinChanged += OnIsKeyboardFocusWithinChanged;
        }

        /// <summary>
        /// Removes a keyboard focus handler from a container.
        /// </summary>
        protected virtual void RemoveKeyboardFocusWithinHandler()
        {
            if (CurrentlyFocusedContainer != null)
            {
                CurrentlyFocusedContainer.IsKeyboardFocusWithinChanged -= OnIsKeyboardFocusWithinChanged;
                CurrentlyFocusedContainer = null;
            }
        }

        /// <summary>
        /// Calleds when the focus changed in a container.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnIsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Keyboard.FocusedElement is ContextMenu AsContextMenu)
            {
                ClearAllContainerTags();

                foreach (object item in SelectedItems)
                {
                    ExtendedTreeViewItemBase Container = ContainerFromItem(item);
                    if (Container != null)
                        TagContainer(Container);
                }

                AsContextMenu.Closed += OnContextMenuClosed;
            }
        }

        /// <summary>
        /// Called when a context menu is closed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnContextMenuClosed(object sender, RoutedEventArgs e)
        {
            if (sender is ContextMenu AsContextMenu)
            {
                AsContextMenu.Closed -= OnContextMenuClosed;
                ClearAllContainerTags();
            }
        }

        /// <summary>
        /// Adds a tag to a container.
        /// </summary>
        /// <param name="container">The container.</param>
        protected virtual void TagContainer(ExtendedTreeViewItemBase container)
        {
            if (container != null)
            {
                container.SetValue(HasContextMenuOpenPropertyKey, true);
                MarkedContainerList.Add(container);
            }
        }

        /// <summary>
        /// Removes all tags in containers.
        /// </summary>
        protected virtual void ClearAllContainerTags()
        {
            foreach (ExtendedTreeViewItemBase Container in MarkedContainerList)
                Container.SetValue(HasContextMenuOpenPropertyKey, false);

            MarkedContainerList.Clear();
        }

        /// <summary>
        /// Gets the currently focused container.
        /// </summary>
        protected ExtendedTreeViewItemBase? CurrentlyFocusedContainer { get; private set; }

        /// <summary>
        /// Gets the list of tagged containers.
        /// </summary>
        protected Collection<ExtendedTreeViewItemBase> MarkedContainerList { get; } = new Collection<ExtendedTreeViewItemBase>();
        #endregion

        #region Descendant Interface
        /// <summary>
        /// Gets the parent of an item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The parent item.</returns>
        protected abstract object? GetItemParent(object item);

        /// <summary>
        /// Gets the number of children of an item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The number of children.</returns>
        protected abstract int GetItemChildrenCount(object item);

        /// <summary>
        /// Gets children of an item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The children.</returns>
        protected abstract IList GetItemChildren(object item);

        /// <summary>
        /// Gets the child of an item at the provided index.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="index">The child index.</param>
        /// <returns>The child at the provided index.</returns>
        protected abstract object GetItemChild(object item, int index);

        /// <summary>
        /// Installs event handlers on an item.
        /// </summary>
        /// <param name="item">The item.</param>
        protected abstract void InstallHandlers(object item);

        /// <summary>
        /// Uninstalls event handlers from an item.
        /// </summary>
        /// <param name="item">The item.</param>
        protected abstract void UninstallHandlers(object item);

        /// <summary>
        /// Moves items from source to destination.
        /// </summary>
        /// <param name="sourceItem">The source item.</param>
        /// <param name="destinationItem">The destination item.</param>
        /// <param name="itemList">Moved children.</param>
        protected abstract void DragDropMove(object sourceItem, object destinationItem, IList? itemList);

        /// <summary>
        /// Copy items from source to destination.
        /// </summary>
        /// <param name="sourceItem">The source item.</param>
        /// <param name="destinationItem">The destination item.</param>
        /// <param name="itemList">Children at the source.</param>
        /// <param name="cloneList">Cloned children at the destination.</param>
        protected abstract void DragDropCopy(object sourceItem, object destinationItem, IList? itemList, IList cloneList);

        /// <summary>
        /// Creates a list of items.
        /// </summary>
        /// <returns>The created list of items.</returns>
        protected virtual IList CreateItemList()
        {
            return new List<object>();
        }

        /// <summary>
        /// Creates a context for inserting.
        /// </summary>
        /// <param name="item">The item where insertion takes place.</param>
        /// <param name="shownIndex">Index where insertion takes place.</param>
        /// <returns>The context.</returns>
        protected virtual IInsertItemContext CreateInsertItemContext(object item, int shownIndex)
        {
            return new InsertItemContext(shownIndex);
        }

        /// <summary>
        /// Creates a context for removing.
        /// </summary>
        /// <param name="item">The item where removal takes place.</param>
        /// <param name="shownIndex">Index where removal takes place.</param>
        /// <returns>The context.</returns>
        protected virtual IRemoveItemContext CreateRemoveItemContext(object item, int shownIndex)
        {
            return new RemoveItemContext(shownIndex);
        }

        /// <summary>
        /// Creates the control used for drag and drop.
        /// </summary>
        /// <returns>The control.</returns>
        protected virtual IDragSourceControl CreateSourceControl()
        {
            return new DragSourceControl(this);
        }
        #endregion

        #region Debugging
        /// <summary>
        /// Logs a call entry.
        /// </summary>
        /// <param name="callerName">Name of the caller.</param>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameter is mandatory with [CallerMemberName]")]
        protected virtual void DebugCall([CallerMemberName] string callerName = "")
        {
            bool EnableTraces = ExtendedTreeViewBase.EnableTraces;
            if (EnableTraces)
                System.Diagnostics.Debug.Print(GetType().Name + ": " + callerName);
        }

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        [Localizable(false)]
        protected virtual void DebugMessage(string message)
        {
            bool EnableTraces = ExtendedTreeViewBase.EnableTraces;
            if (EnableTraces)
                System.Diagnostics.Debug.Print(GetType().Name + ": " + message);
        }
        #endregion
    }
}
