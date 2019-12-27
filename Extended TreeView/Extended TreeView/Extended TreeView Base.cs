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

namespace CustomControls
{
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    public abstract class ExtendedTreeViewBase : MultiSelector
    {
        #region Constants
        public const bool EnableTraces = false;
        #endregion

        #region Default Styles
        public static Style DefaultStyle { get; private set; } = new Style();
        public static Style DefaultItemContainerStyle { get; private set; } = new Style();
        #endregion

        #region Custom properties and events
        #region Content
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(object), typeof(ExtendedTreeViewBase), new FrameworkPropertyMetadata(null, OnContentChanged));

        public object Content
        {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        protected static void OnContentChanged(DependencyObject modifiedObject, DependencyPropertyChangedEventArgs e)
        {
            if (modifiedObject == null)
                throw new ArgumentNullException(nameof(modifiedObject));

            ExtendedTreeViewBase ctrl = (ExtendedTreeViewBase)modifiedObject;
            ctrl.OnContentChanged(e);
        }

        protected virtual void OnContentChanged(DependencyPropertyChangedEventArgs e)
        {
            BuildFlatChildrenTables();
        }
        #endregion
        #region Selection Mode
        public static readonly DependencyProperty SelectionModeProperty = DependencyProperty.Register("SelectionMode", typeof(SelectionMode), typeof(ExtendedTreeViewBase), new FrameworkPropertyMetadata(SelectionMode.Single));

        public SelectionMode SelectionMode
        {
            get { return (SelectionMode)GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }
        #endregion
        #region Preview Collection Modified
        public static readonly RoutedEvent PreviewCollectionModifiedEvent = EventManager.RegisterRoutedEvent("PreviewCollectionModified", RoutingStrategy.Tunnel, typeof(RoutedEventHandler), typeof(ExtendedTreeViewBase));

        public event RoutedEventHandler PreviewCollectionModified
        {
            add { AddHandler(PreviewCollectionModifiedEvent, value); }
            remove { RemoveHandler(PreviewCollectionModifiedEvent, value); }
        }

        protected virtual void NotifyPreviewCollectionModified(TreeViewCollectionOperation treeViewCollectionOperation)
        {
            RaiseEvent(new TreeViewCollectionModifiedEventArgs(PreviewCollectionModifiedEvent, treeViewCollectionOperation, Items.Count));
        }
        #endregion
        #region Collection Modified
        public static readonly RoutedEvent CollectionModifiedEvent = EventManager.RegisterRoutedEvent("CollectionModified", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ExtendedTreeViewBase));

        public event RoutedEventHandler CollectionModified
        {
            add { AddHandler(CollectionModifiedEvent, value); }
            remove { RemoveHandler(CollectionModifiedEvent, value); }
        }

        protected virtual void NotifyCollectionModified(TreeViewCollectionOperation treeViewCollectionOperation)
        {
            RaiseEvent(new TreeViewCollectionModifiedEventArgs(CollectionModifiedEvent, treeViewCollectionOperation, Items.Count));
        }
        #endregion
        #region Is Root Always Expanded
        public static readonly DependencyProperty IsRootAlwaysExpandedProperty = DependencyProperty.Register("IsRootAlwaysExpanded", typeof(bool), typeof(ExtendedTreeViewBase), new FrameworkPropertyMetadata(false));

        public bool IsRootAlwaysExpanded
        {
            get { return (bool)GetValue(IsRootAlwaysExpandedProperty); }
            set { SetValue(IsRootAlwaysExpandedProperty, value); }
        }
        #endregion
        #region Is Item Expanded At Start
        public static readonly DependencyProperty IsItemExpandedAtStartProperty = DependencyProperty.Register("IsItemExpandedAtStart", typeof(bool), typeof(ExtendedTreeViewBase), new FrameworkPropertyMetadata(false));

        public bool IsItemExpandedAtStart
        {
            get { return (bool)GetValue(IsItemExpandedAtStartProperty); }
            set { SetValue(IsItemExpandedAtStartProperty, value); }
        }
        #endregion
        #region Allow Drag Drop
        public static readonly DependencyProperty AllowDragDropProperty = DependencyProperty.Register("AllowDragDrop", typeof(bool), typeof(ExtendedTreeViewBase), new FrameworkPropertyMetadata(false, OnDragDropChanged));

        public bool AllowDragDrop
        {
            get { return (bool)GetValue(AllowDragDropProperty); }
            set { SetValue(AllowDragDropProperty, value); }
        }

        protected static void OnDragDropChanged(DependencyObject modifiedObject, DependencyPropertyChangedEventArgs e)
        {
            if (modifiedObject == null)
                throw new ArgumentNullException(nameof(modifiedObject));

            ExtendedTreeViewBase ctrl = (ExtendedTreeViewBase)modifiedObject;
            ctrl.OnDragDropChanged(e);
        }

        protected virtual void OnDragDropChanged(DependencyPropertyChangedEventArgs e)
        {
            bool NewValue = (bool)e.NewValue;
            if (NewValue)
                AllowDrop = true;
        }
        #endregion
        #region Use Default Cursors
        public static readonly DependencyProperty UseDefaultCursorsProperty = DependencyProperty.Register("UseDefaultCursors", typeof(bool), typeof(ExtendedTreeViewBase), new FrameworkPropertyMetadata(false));

        public bool UseDefaultCursors
        {
            get { return (bool)GetValue(UseDefaultCursorsProperty); }
            set { SetValue(UseDefaultCursorsProperty, value); }
        }
        #endregion
        #region Cursor Forbidden
        public static readonly DependencyProperty CursorForbiddenProperty = DependencyProperty.Register("CursorForbidden", typeof(Cursor), typeof(ExtendedTreeViewBase), new FrameworkPropertyMetadata(null));

        public Cursor CursorForbidden
        {
            get { return (Cursor)GetValue(CursorForbiddenProperty); }
            set { SetValue(CursorForbiddenProperty, value); }
        }
        #endregion
        #region Cursor Move
        public static readonly DependencyProperty CursorMoveProperty = DependencyProperty.Register("CursorMove", typeof(Cursor), typeof(ExtendedTreeViewBase), new FrameworkPropertyMetadata(null));

        public Cursor CursorMove
        {
            get { return (Cursor)GetValue(CursorMoveProperty); }
            set { SetValue(CursorMoveProperty, value); }
        }
        #endregion
        #region Cursor Copy
        public static readonly DependencyProperty CursorCopyProperty = DependencyProperty.Register("CursorCopy", typeof(Cursor), typeof(ExtendedTreeViewBase), new FrameworkPropertyMetadata(null));

        public Cursor CursorCopy
        {
            get { return (Cursor)GetValue(CursorCopyProperty); }
            set { SetValue(CursorCopyProperty, value); }
        }
        #endregion
        #region Drag Starting
        public static readonly RoutedEvent DragStartingEvent = EventManager.RegisterRoutedEvent("DragStarting", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ExtendedTreeViewBase));

        public event RoutedEventHandler DragStarting
        {
            add { AddHandler(DragStartingEvent, value); }
            remove { RemoveHandler(DragStartingEvent, value); }
        }

        protected virtual void NotifyDragStarting(CancellationToken cancellation)
        {
            DragStartingEventArgs Args = new DragStartingEventArgs(DragStartingEvent, DragSource, cancellation);
            RaiseEvent(Args);
        }
        #endregion
        #region Drop Check
        public static readonly RoutedEvent DropCheckEvent = EventManager.RegisterRoutedEvent("DropCheck", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ExtendedTreeViewBase));

        public event RoutedEventHandler DropCheck
        {
            add { AddHandler(DropCheckEvent, value); }
            remove { RemoveHandler(DropCheckEvent, value); }
        }

        protected virtual void NotifyDropCheck(object dropDestinationItem, PermissionToken permission)
        {
            DropCheckEventArgs Args = new DropCheckEventArgs(DropCheckEvent, DragSource, dropDestinationItem, permission);
            RaiseEvent(Args);
        }
        #endregion
        #region Preview Drop Completed
        public static readonly RoutedEvent PreviewDropCompletedEvent = EventManager.RegisterRoutedEvent("PreviewDropCompleted", RoutingStrategy.Tunnel, typeof(RoutedEventHandler), typeof(ExtendedTreeViewBase));

        public event RoutedEventHandler PreviewDropCompleted
        {
            add { AddHandler(PreviewDropCompletedEvent, value); }
            remove { RemoveHandler(PreviewDropCompletedEvent, value); }
        }

        protected virtual void NotifyPreviewDropCompleted(object dropDestinationItem, IList? cloneList)
        {
            DropCompletedEventArgs Args = new DropCompletedEventArgs(PreviewDropCompletedEvent, DragSource, dropDestinationItem, cloneList);
            RaiseEvent(Args);
        }
        #endregion
        #region Drop Completed
        public static readonly RoutedEvent DropCompletedEvent = EventManager.RegisterRoutedEvent("DropCompleted", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ExtendedTreeViewBase));

        public event RoutedEventHandler DropCompleted
        {
            add { AddHandler(DropCompletedEvent, value); }
            remove { RemoveHandler(DropCompletedEvent, value); }
        }

        protected virtual void NotifyDropCompleted(object dropDestinationItem, IList? cloneList)
        {
            DropCompletedEventArgs Args = new DropCompletedEventArgs(DropCompletedEvent, DragSource, dropDestinationItem, cloneList);
            RaiseEvent(Args);
        }
        #endregion
        #region Expand Button Width
        public static readonly DependencyProperty ExpandButtonWidthProperty = DependencyProperty.Register("ExpandButtonWidth", typeof(double), typeof(ExtendedTreeViewBase), new FrameworkPropertyMetadata(0.0), new ValidateValueCallback(IsValidExpandButtonWidth));

        public double ExpandButtonWidth
        {
            get { return (double)GetValue(ExpandButtonWidthProperty); }
            set { SetValue(ExpandButtonWidthProperty, value); }
        }

        public static bool IsValidExpandButtonWidth(object value)
        {
            double Width = (double)value;
            return Width >= 0;
        }
        #endregion
        #region Expand Button Style
        public static readonly DependencyProperty ExpandButtonStyleProperty = DependencyProperty.Register("ExpandButtonStyle", typeof(Style), typeof(ExtendedTreeViewBase));

        public Style ExpandButtonStyle
        {
            get { return (Style)GetValue(ExpandButtonStyleProperty); }
            set { SetValue(ExpandButtonStyleProperty, value); }
        }
        #endregion
        #region Indentation Width
        public static readonly DependencyProperty IndentationWidthProperty = DependencyProperty.Register("IndentationWidth", typeof(double), typeof(ExtendedTreeViewBase), new FrameworkPropertyMetadata(0.0), new ValidateValueCallback(IsValidIndentationWidth));

        public double IndentationWidth
        {
            get { return (double)GetValue(IndentationWidthProperty); }
            set { SetValue(IndentationWidthProperty, value); }
        }

        public static bool IsValidIndentationWidth(object value)
        {
            double Width = (double)value;
            return Width >= 0;
        }
        #endregion
        #region Has Context Menu Open
        private static DependencyPropertyKey HasContextMenuOpenPropertyKey = DependencyProperty.RegisterAttachedReadOnly("HasContextMenuOpen", typeof(bool), typeof(ExtendedTreeViewBase), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty HasContextMenuOpenProperty = HasContextMenuOpenPropertyKey.DependencyProperty;

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
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static ExtendedTreeViewBase()
        {
            OverrideAncestorMetadata();
        }

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
        ///     Called when the control has been initialized and before properties are set.
        /// </summary>
        /// <parameters>
        /// <param name="sender">This parameter is not used.</param>
        /// <param name="e">This parameter is not used.</param>
        /// </parameters>
        protected virtual void OnInitialized(object sender, EventArgs e)
        {
            InitializeDragAndDrop();
        }
        #endregion

        #region Client Interface
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

        public virtual void ScrollIntoView(object item)
        {
            ExtendedTreeViewItemBase ItemContainer = ContainerFromItem(item);
            if (ItemContainer != null)
                ItemContainer.BringIntoView();
        }

        public virtual bool IsExpanded(object item)
        {
            return ExpandedChildren.ContainsKey(item);
        }

        public virtual void Expand(object item)
        {
            ExtendedTreeViewItemBase ItemContainer = ContainerFromItem(item);
            if (ItemContainer != null)
                ItemContainer.IsExpanded = true;
        }

        public virtual void Collapse(object item)
        {
            ExtendedTreeViewItemBase ItemContainer = ContainerFromItem(item);
            if (ItemContainer != null)
                ItemContainer.IsExpanded = false;
        }

        public virtual void ToggleIsExpanded(object item)
        {
            ExtendedTreeViewItemBase ItemContainer = ContainerFromItem(item);
            if (ItemContainer != null)
                ItemContainer.IsExpanded = !ItemContainer.IsExpanded;
        }

        public virtual bool IsSelected(object item)
        {
            return SelectedItems.Contains(item);
        }

        public virtual void SetSelected(object item)
        {
            SelectedItem = item;
        }

        public virtual bool IsItemVisible(object item)
        {
            return VisibleChildren.Contains(item);
        }

        public virtual bool IsCopyPossible
        {
            get { return DragSource.IsDragPossible(); }
        }
        #endregion

        #region Ancestor Interface
        protected static void OverrideAncestorMetadata()
        {
            OverrideMetadataItemsSource();
            OverrideMetadataDefaultStyleKey();
        }

        protected static void OverrideMetadataItemsSource()
        {
            ItemsSourceProperty.OverrideMetadata(typeof(ExtendedTreeViewBase), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.NotDataBindable, null, new CoerceValueCallback(CoerceItemsSource), true));
        }

        protected static void OverrideMetadataDefaultStyleKey()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ExtendedTreeViewBase), new FrameworkPropertyMetadata(typeof(ExtendedTreeViewBase)));
        }

        protected static object? CoerceItemsSource(DependencyObject modifiedObject, object value)
        {
            if (modifiedObject is ExtendedTreeViewBase ctrl)
                return ctrl.CoerceItemsSource(value);
            else
                throw new ArgumentOutOfRangeException(nameof(modifiedObject));
        }

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

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is ExtendedTreeViewItemBase;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return CreateContainerItem();
        }

        protected virtual ExtendedTreeViewItemBase CreateContainerItem()
        {
            return new ExtendedTreeViewItemBase(this);
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);

            UpdateIsDragDropPossible();
        }
        #endregion

        #region Properties
        protected ObservableCollection<object> VisibleChildren { get; } = new ObservableCollection<object>();
        protected Dictionary<object, IList> ExpandedChildren { get; } = new Dictionary<object, IList>();
        #endregion

        #region Implementation
        private void InitializeImplementation()
        {
            ItemsSource = VisibleChildren;
        }

        protected virtual void BuildFlatChildrenTables()
        {
            ResetFlatChildren();
            InsertChildrenFromRoot();
        }

        protected virtual void ResetFlatChildren()
        {
            UninstallAllHandlers();

            VisibleChildren.Clear();
            ExpandedChildren.Clear();
        }

        protected virtual void UninstallAllHandlers()
        {
            foreach (object item in VisibleChildren)
                UninstallHandlers(item);
        }

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

        protected virtual void InternalInsert(int index, object item)
        {
            VisibleChildren.Insert(index, item);
            InstallHandlers(item);
        }

        protected virtual void InternalRemove(int index, object item)
        {
            UninstallHandlers(item);
            VisibleChildren.RemoveAt(index);
        }
        #endregion

        #region Observable Collection
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
        protected virtual ExtendedTreeViewItemBase ContainerFromItem(object item)
        {
            return (ExtendedTreeViewItemBase)ItemContainerGenerator.ContainerFromItem(item);
        }

        protected virtual ExtendedTreeViewItemBase ContainerFromIndex(int index)
        {
            return (ExtendedTreeViewItemBase)ItemContainerGenerator.ContainerFromIndex(index);
        }

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

        public void SetItemExpanded(object item)
        {
            IList Children = GetItemChildren(item);
            ExpandedChildren.Add(item, Children);

            int Index = VisibleChildren.IndexOf(item) + 1;
            Expand(item, Index);
        }

        public bool IsItemExpandable(object item)
        {
            return !IsCtrlDown() && GetItemChildrenCount(item) > 0;
        }

        public bool IsItemCollapsible(object item)
        {
            return !IsCtrlDown() && GetItemChildrenCount(item) > 0 && (item != Content || !IsRootAlwaysExpanded);
        }

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

        public void SetItemCollapsed(object item)
        {
            ExpandedChildren.Remove(item);

            int Index = VisibleChildren.IndexOf(item) + 1;
            Collapse(item, Index);
        }

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

        protected virtual bool IsItemCloneable(object item)
        {
            return (item is ICloneable);
        }

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

        public static bool IsDropOver(ExtendedTreeViewItemBase itemContainer)
        {
            return itemContainer == DropTargetContainer;
        }

        public void ClickPreviousItem(object item)
        {
            if (!IsCtrlDown())
            {
                int ItemIndex = VisibleChildren.IndexOf(item);
                if (ItemIndex > 0)
                    LeftClickSelect(VisibleChildren[ItemIndex - 1]);
            }
        }

        public void ClickNextItem(object item)
        {
            if (!IsCtrlDown())
            {
                int ItemIndex = VisibleChildren.IndexOf(item);
                if (ItemIndex >= 0 && ItemIndex + 1 < VisibleChildren.Count)
                    LeftClickSelect(VisibleChildren[ItemIndex + 1]);
            }
        }

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
                        int LastIndex = (LastSelectedItem != null && IsLastSelectedItemSet ? VisibleChildren.IndexOf(LastSelectedItem) : FirstIndex);

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

        protected virtual ExtendedTreeViewItemBase? GetTarget(DragEventArgs e)
        {
            return GetEventSourceItem(e);
        }

        protected virtual bool IsCtrlDown()
        {
            return (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && !(Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt));
        }

        protected virtual bool IsShiftDown()
        {
            return (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift));
        }

        protected virtual int SortByIndex(object item1, object item2)
        {
            return VisibleChildren.IndexOf(item1) - VisibleChildren.IndexOf(item2);
        }

        protected virtual void AddSelectedItem(object item)
        {
            SelectedItems.Add(item);
        }

        protected virtual void RemoveSelectedItem(object item)
        {
            if (IsLastSelectedItem(item))
                ClearLastSelectedItem();

            SelectedItems.Remove(item);
        }

        protected virtual bool IsLastSelectedItemSet
        {
            get { return LastSelectedItem != null; }
        }

        protected virtual void SetLastSelectedItem(object item)
        {
            LastSelectedItem = item;
        }

        protected virtual bool IsLastSelectedItem(object item)
        {
            return LastSelectedItem == item;
        }

        protected virtual void ClearLastSelectedItem()
        {
            LastSelectedItem = null;
        }

        protected virtual bool IsLastClickedItemSet
        {
            get { return LastClickedItem != null; }
        }

        protected virtual void SetLastClickedItem(object item)
        {
            LastClickedItem = item;
        }

        protected virtual bool IsLastClickedItem(object item)
        {
            return LastClickedItem == item;
        }

        protected virtual void ClearLastClickedItem()
        {
            LastClickedItem = null;
        }

        protected object? LastSelectedItem { get; private set; }
        protected object? LastClickedItem { get; private set; }
        #endregion

        #region Cursors
        protected static Cursor InitializeCursor(int cursorIndex)
        {
            string SystemPath = Environment.GetFolderPath(Environment.SpecialFolder.System);
            string Ole32Path = Path.Combine(SystemPath, "ole32.dll");

            return LoadCursorFromResourceFile(Ole32Path, cursorIndex);
        }

        protected static Cursor LoadCursorFromResourceFile(string filePath, int resourceId)
        {
            CursorResource CursorFromResource = new CursorResource(filePath, (uint)resourceId);
            return CursorFromResource.AsCursor;
        }

        protected static Cursor DefaultCursorForbidden { get; } = InitializeCursor(1);
        protected static Cursor DefaultCursorMove { get; } = InitializeCursor(2);
        protected static Cursor DefaultCursorCopy { get; } = InitializeCursor(3);
        #endregion

        #region Mouse Events
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            DebugCall();

            base.OnPreviewMouseLeftButtonDown(e);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            DebugCall();
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            DebugCall();
            base.OnPreviewMouseLeftButtonUp(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            DebugCall();
            base.OnMouseLeftButtonUp(e);
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            DebugCall();
            base.OnPreviewMouseMove(e);
        }

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

        protected virtual void UpdateIsDragDropPossible()
        {
            CanonicSelection canonicSelectedItemList = new CanonicSelection(CreateItemList());
            if (GetCanonicSelectedItemList(canonicSelectedItemList))
                DragSource.SetIsDragPossible(canonicSelectedItemList);
            else
                DragSource.ClearIsDragPossible();
        }

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

        protected override void OnDragEnter(DragEventArgs e)
        {
            if (e != null)
            {
                UpdateCurrentDropTarget(e, false);

                e.Effects = GetAllowedDropEffects(e);
                e.Handled = true;
            }
        }

        protected override void OnDragLeave(DragEventArgs e)
        {
            if (e != null)
            {
                UpdateCurrentDropTarget(e, true);

                e.Effects = GetAllowedDropEffects(e);
                e.Handled = true;
            }
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            if (e != null)
            {
                UpdateCurrentDropTarget(e, false);

                e.Effects = GetAllowedDropEffects(e);
                e.Handled = true;
            }
        }

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

        protected virtual void ClearCurrentDropTarget()
        {
            Dispatcher.BeginInvoke(new ClearCurrentDropTargetHandler(OnClearCurrentDropTarget));
        }

        protected delegate void ClearCurrentDropTargetHandler();
        protected virtual void OnClearCurrentDropTarget()
        {
            ExtendedTreeViewItemBase? OldDropTarget = DropTargetContainer;
            DropTargetContainer = null;
            DropTargetContainerIndex = -1;

            if (OldDropTarget != null)
                OldDropTarget.UpdateIsDropOver();
        }

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

        public void ContainerLostFocus()
        {
            RemoveKeyboardFocusWithinHandler();
        }

        public void ContainerGotFocus(ExtendedTreeViewItemBase container)
        {
            AddKeyboardFocusWithinHandler(container);
        }

        protected virtual void AddKeyboardFocusWithinHandler(ExtendedTreeViewItemBase container)
        {
            RemoveKeyboardFocusWithinHandler();

            CurrentlyFocusedContainer = container ?? throw new ArgumentNullException(nameof(container));
            CurrentlyFocusedContainer.IsKeyboardFocusWithinChanged += OnIsKeyboardFocusWithinChanged;
        }

        protected virtual void RemoveKeyboardFocusWithinHandler()
        {
            if (CurrentlyFocusedContainer != null)
            {
                CurrentlyFocusedContainer.IsKeyboardFocusWithinChanged -= OnIsKeyboardFocusWithinChanged;
                CurrentlyFocusedContainer = null;
            }
        }

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

        protected virtual void OnContextMenuClosed(object sender, RoutedEventArgs e)
        {
            if (sender is ContextMenu AsContextMenu)
            {
                AsContextMenu.Closed -= OnContextMenuClosed;
                ClearAllContainerTags();
            }
        }

        protected virtual void TagContainer(ExtendedTreeViewItemBase container)
        {
            if (container != null)
            {
                container.SetValue(HasContextMenuOpenPropertyKey, true);
                MarkedContainerList.Add(container);

                //Debug.Print("Tagged: " + Container.Content);
            }
        }

        protected virtual void ClearAllContainerTags()
        {
            foreach (ExtendedTreeViewItemBase Container in MarkedContainerList)
            {
                //Debug.Print("Untagged: " + Container.Content);
                Container.SetValue(HasContextMenuOpenPropertyKey, false);
            }

            MarkedContainerList.Clear();
        }

        protected ExtendedTreeViewItemBase? CurrentlyFocusedContainer { get; private set; }
        protected Collection<ExtendedTreeViewItemBase> MarkedContainerList { get; } = new Collection<ExtendedTreeViewItemBase>();
        #endregion

        #region Descendant Interface
        protected abstract object? GetItemParent(object item);
        protected abstract int GetItemChildrenCount(object item);
        protected abstract IList GetItemChildren(object item);
        protected abstract object GetItemChild(object item, int index);
        protected abstract void InstallHandlers(object item);
        protected abstract void UninstallHandlers(object item);
        protected abstract void DragDropMove(object sourceItem, object destinationItem, IList? itemList);
        protected abstract void DragDropCopy(object sourceItem, object destinationItem, IList? itemList, IList cloneList);

        protected virtual IList CreateItemList()
        {
            return new List<object>();
        }

        protected virtual IInsertItemContext CreateInsertItemContext(object item, int shownIndex)
        {
            return new InsertItemContext(shownIndex);
        }

        protected virtual IRemoveItemContext CreateRemoveItemContext(object item, int shownIndex)
        {
            return new RemoveItemContext(shownIndex);
        }

        protected virtual IDragSourceControl CreateSourceControl()
        {
            return new DragSourceControl(this);
        }
        #endregion

        #region Debugging
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameter is mandatory with [CallerMemberName]")]
        protected virtual void DebugCall([CallerMemberName] string callerName = "")
        {
            bool EnableTraces = ExtendedTreeViewBase.EnableTraces;
            if (EnableTraces)
                System.Diagnostics.Debug.Print(GetType().Name + ": " + callerName);
        }

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
