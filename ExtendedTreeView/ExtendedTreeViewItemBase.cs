namespace CustomControls
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;

    /// <summary>
    /// Represents an item in a tree view control.
    /// </summary>
    public class ExtendedTreeViewItemBase : ContentControl, INotifyPropertyChanged
    {
        #region Custom properties and events
        #region Is Selected
        /// <summary>
        /// Identifies the <see cref="IsSelected"/> attached property.
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty = Selector.IsSelectedProperty.AddOwner(typeof(ExtendedTreeViewItemBase));

        /// <summary>
        /// Gets or sets a value indicating whether the item is selected.
        /// </summary>
        public bool IsSelected
        {
            get { return Selector.GetIsSelected(this); }
            set { Selector.SetIsSelected(this, value); }
        }
        #endregion
        #region Selected
        /// <summary>
        /// Identifies the <see cref="Selected"/> routed event.
        /// </summary>
        public static readonly RoutedEvent SelectedEvent = Selector.SelectedEvent;

        /// <summary>
        /// Occurs after the item is selected.
        /// </summary>
        public event RoutedEventHandler Selected
        {
            add { AddHandler(SelectedEvent, value); }
            remove { RemoveHandler(SelectedEvent, value); }
        }

        /// <summary>
        /// Invokes handlers of the <see cref="Selected"/> event.
        /// </summary>
        protected virtual void NotifySelected()
        {
            RoutedEventArgs Args = new RoutedEventArgs(SelectedEvent, this);
            RaiseEvent(Args);
        }
        #endregion
        #region Unselected
        /// <summary>
        /// Identifies the <see cref="Unselected"/> routed event.
        /// </summary>
        public static readonly RoutedEvent UnselectedEvent = Selector.UnselectedEvent;

        /// <summary>
        /// Occurs after the item is unselected.
        /// </summary>
        public event RoutedEventHandler Unselected
        {
            add { AddHandler(UnselectedEvent, value); }
            remove { RemoveHandler(UnselectedEvent, value); }
        }

        /// <summary>
        /// Invokes handlers of the <see cref="Unselected"/> event.
        /// </summary>
        protected virtual void NotifyUnselected()
        {
            RoutedEventArgs Args = new RoutedEventArgs(UnselectedEvent, this);
            RaiseEvent(Args);
        }
        #endregion
        #region Is Expanded
        /// <summary>
        /// Identifies the <see cref="IsExpanded"/> attached property.
        /// </summary>
        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register("IsExpanded", typeof(bool), typeof(ExtendedTreeViewItemBase), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal, new PropertyChangedCallback(OnIsExpandedChanged)));

        /// <summary>
        /// Gets or sets a value indicating whether the item is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        /// <summary>
        /// Handles changes of the <see cref="IsExpanded"/> property.
        /// </summary>
        /// <param name="modifiedObject">The modified object.</param>
        /// <param name="e">An object that contains event data.</param>
        protected static void OnIsExpandedChanged(DependencyObject modifiedObject, DependencyPropertyChangedEventArgs e)
        {
            if (modifiedObject == null)
                throw new ArgumentNullException(nameof(modifiedObject));

            ExtendedTreeViewItemBase ctrl = (ExtendedTreeViewItemBase)modifiedObject;
            ctrl.OnIsExpandedChanged(e);
        }

        /// <summary>
        /// Handles changes of the <see cref="IsExpanded"/> property.
        /// </summary>
        /// <param name="e">An object that contains event data.</param>
        protected virtual void OnIsExpandedChanged(DependencyPropertyChangedEventArgs e)
        {
            if (!IsContentInitializing)
            {
                bool NewIsExpanded = (bool)e.NewValue;
                if (NewIsExpanded)
                    Host.SetItemExpanded(Content);
                else
                    Host.SetItemCollapsed(Content);
            }
        }

        /// <summary>
        /// Begins initialization of the item content.
        /// </summary>
        protected virtual void BeginInitializeContent()
        {
            IsContentInitializing = true;
        }

        /// <summary>
        /// Ends initialization of the item content.
        /// </summary>
        protected virtual void EndInitializeContent()
        {
            IsContentInitializing = false;
        }

        /// <summary>
        /// Gets a value indicating whether the content is being initialized.
        /// </summary>
        protected bool IsContentInitializing { get; private set; }
        #endregion
        #region Expanded
        /// <summary>
        /// Identifies the <see cref="Expanded"/> routed event.
        /// </summary>
        public static readonly RoutedEvent ExpandedEvent = EventManager.RegisterRoutedEvent("Expanded", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ExtendedTreeViewItemBase));

        /// <summary>
        /// Occurs after the item is expanded.
        /// </summary>
        public event RoutedEventHandler Expanded
        {
            add { AddHandler(ExpandedEvent, value); }
            remove { RemoveHandler(ExpandedEvent, value); }
        }

        /// <summary>
        /// Invokes handlers of the <see cref="Expanded"/> event.
        /// </summary>
        protected virtual void NotifyExpanded()
        {
            RoutedEventArgs Args = new RoutedEventArgs(ExpandedEvent, this);
            RaiseEvent(Args);
        }
        #endregion
        #region Collapsed
        /// <summary>
        /// Identifies the <see cref="Collapsed"/> routed event.
        /// </summary>
        public static readonly RoutedEvent CollapsedEvent = EventManager.RegisterRoutedEvent("Collapsed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ExtendedTreeViewItemBase));

        /// <summary>
        /// Occurs after the item is collapsed.
        /// </summary>
        public event RoutedEventHandler Collapsed
        {
            add { AddHandler(CollapsedEvent, value); }
            remove { RemoveHandler(CollapsedEvent, value); }
        }

        /// <summary>
        /// Invokes handlers of the <see cref="Collapsed"/> event.
        /// </summary>
        protected virtual void NotifyCollapsed()
        {
            RoutedEventArgs Args = new RoutedEventArgs(CollapsedEvent, this);
            RaiseEvent(Args);
        }
        #endregion
        #region Is Drop Over
        /// <summary>
        /// Identifies the <see cref="IsDropOver"/> attached property.
        /// </summary>
        public static readonly DependencyProperty IsDropOverProperty = IsDropOverPropertyKey?.DependencyProperty !;
        private static DependencyPropertyKey IsDropOverPropertyKey = DependencyProperty.RegisterReadOnly("IsDropOver", typeof(bool), typeof(ExtendedTreeViewItemBase), new PropertyMetadata(false));

        /// <summary>
        /// Gets a value indicating whether the item is the destination of a drop.
        /// </summary>
        public bool IsDropOver
        {
            get { return (bool)GetValue(IsDropOverProperty); }
        }
        #endregion
        #endregion

        #region Init
        static ExtendedTreeViewItemBase()
        {
            OverrideAncestorMetadata();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedTreeViewItemBase"/> class.
        /// </summary>
        /// <param name="host">The tree to which this item belongs.</param>
        public ExtendedTreeViewItemBase(ExtendedTreeViewBase host)
        {
            Host = host;

            object DefaultStyle = TryFindResource(typeof(ExtendedTreeViewItemBase));
            if (DefaultStyle == null)
                Resources.MergedDictionaries.Add(SharedResourceDictionaryManager.SharedDictionary);
        }

        /// <summary>
        /// Gets the tree to which this item belongs.
        /// </summary>
        protected ExtendedTreeViewBase Host { get; }
        #endregion

        #region Ancestor Interface
        /// <summary>
        /// Overrides inherited metadata.
        /// </summary>
        protected static void OverrideAncestorMetadata()
        {
            OverrideMetadataContent();
            OverrideMetadataDefaultStyleKey();
        }

        /// <summary>
        /// Override metadata for the <see cref="ContentControl.Content"/> property.
        /// </summary>
        protected static void OverrideMetadataContent()
        {
            ContentProperty.OverrideMetadata(typeof(ExtendedTreeViewItemBase), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnContentChanged)));
        }

        /// <summary>
        /// Override metadata for the <see cref="FrameworkElement.DefaultStyleKey"/> property.
        /// </summary>
        protected static void OverrideMetadataDefaultStyleKey()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ExtendedTreeViewItemBase), new FrameworkPropertyMetadata(typeof(ExtendedTreeViewItemBase)));
        }

        /// <summary>
        /// Called when the <see cref="ContentControl.Content"/> property has changed.
        /// </summary>
        /// <param name="modifiedObject">The object for which the property changed.</param>
        /// <param name="e">The event data.</param>
        protected static void OnContentChanged(DependencyObject modifiedObject, DependencyPropertyChangedEventArgs e)
        {
            if (modifiedObject == null)
                throw new ArgumentNullException(nameof(modifiedObject));

            ExtendedTreeViewItemBase ctrl = (ExtendedTreeViewItemBase)modifiedObject;
            ctrl.OnContentChanged(e);
        }

        /// <summary>
        /// Called when the <see cref="ContentControl.Content"/> property has changed.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected virtual void OnContentChanged(DependencyPropertyChangedEventArgs e)
        {
            BeginInitializeContent();

            object NewContent = e.NewValue;
            IsExpanded = Host.IsExpanded(NewContent);

            EndInitializeContent();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the indentation level of the item.
        /// </summary>
        public int Level { get { return Host.ItemLevel(Content); } }

        /// <summary>
        /// Updates the <see cref="IsDropOver"/> property.
        /// </summary>
        public void UpdateIsDropOver()
        {
            SetValue(IsDropOverPropertyKey, ExtendedTreeViewBase.IsDropOver(this));
        }

        /// <summary>
        /// Updates the disconnected item.
        /// </summary>
        /// <param name="value">Candidate value for the disconnected object.</param>
        public static void UpdateDisconnectedItem(object value)
        {
            if (DisconnectedItem == null && value != null)
            {
                Type ItemType = value.GetType();
                if (ItemType.FullName == "MS.Internal.NamedObject")
                    DisconnectedItem = value;
            }
        }

        private static object? DisconnectedItem;
        #endregion

        #region Implementation
        /// <summary>
        /// Called when the <see cref="ContentControl.Content"/> property changes.
        /// </summary>
        /// <param name="oldContent">The old value of the <see cref="ContentControl.Content"/> property.</param>
        /// <param name="newContent">The new value of the <see cref="ContentControl.Content"/> property.</param>
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            UpdateDisconnectedItem(newContent);

            if (DisconnectedItem != null && newContent != DisconnectedItem)
                NotifyPropertyChanged(nameof(Level));
        }

        /// <summary>
        /// Raises the <see cref="UIElement.GotFocus"/> routed event by using the event data that is provided.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);

            Host.ContainerGotFocus(this);
        }

        /// <summary>
        /// Raises the <see cref="UIElement.LostFocus"/> routed event by using the event data that is provided.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            Host.ContainerLostFocus();

            base.OnLostFocus(e);
        }
        #endregion

        #region Mouse Events
        /// <summary>
        /// Invoked when an unhandled <see cref="Mouse.MouseEnterEvent"/> attached event is raised on this element.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            DebugCall();
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="Mouse.MouseLeaveEvent"/> attached event is raised on this element.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            DebugCall();
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="UIElement.PreviewMouseLeftButtonDown"/> attached event is raised on this element.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            DebugCall();
            base.OnPreviewMouseLeftButtonDown(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="UIElement.MouseLeftButtonDown"/> attached event is raised on this element.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            DebugCall();
            SelectItemOnLeftButtonDown();
            base.OnMouseLeftButtonDown(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="UIElement.PreviewMouseLeftButtonUp"/> attached event is raised on this element.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            DebugCall();
            base.OnPreviewMouseLeftButtonUp(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="UIElement.MouseLeftButtonUp"/> attached event is raised on this element.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            DebugCall();
            UnselectItemOnLeftButtonUp();
            base.OnMouseLeftButtonUp(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="UIElement.PreviewMouseRightButtonDown"/> attached event is raised on this element.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
        {
            DebugCall();
            base.OnPreviewMouseRightButtonDown(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="UIElement.MouseRightButtonDown"/> attached event is raised on this element.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            DebugCall();
            SelectItemOnRightButtonDown();
            base.OnMouseRightButtonDown(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="UIElement.PreviewMouseRightButtonUp"/> attached event is raised on this element.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnPreviewMouseRightButtonUp(MouseButtonEventArgs e)
        {
            DebugCall();
            base.OnPreviewMouseRightButtonUp(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="UIElement.MouseRightButtonUp"/> attached event is raised on this element.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            DebugCall();
            UnselectItemOnRightButtonUp();
            base.OnMouseRightButtonUp(e);
        }
        #endregion

        #region Keyboard Events
        /// <summary>
        /// Invoked when an unhandled <see cref="System.Windows.Input.Keyboard.KeyDownEvent"/> attached event reaches an element in its route that is derived from this class.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e != null)
            {
                switch (e.Key)
                {
                    case Key.Left:
                        if (IsExpanded && Host.IsItemCollapsible(Content))
                        {
                            IsExpanded = false;
                            e.Handled = true;
                            return;
                        }

                        break;

                    case Key.Right:
                        if (!IsExpanded && Host.IsItemExpandable(Content))
                        {
                            IsExpanded = true;
                            e.Handled = true;
                            return;
                        }

                        break;

                    case Key.Up:
                        Host.ClickPreviousItem(Content);
                        break;

                    case Key.Down:
                        Host.ClickNextItem(Content);
                        break;

                    default:
                        break;
                }
            }

            base.OnKeyDown(e);
        }
        #endregion

        #region Selection
        /// <summary>
        /// Selects the item because a left button down event occured.
        /// </summary>
        protected void SelectItemOnLeftButtonDown()
        {
            Focus();
            Host.LeftClickSelect(Content);
        }

        /// <summary>
        /// Unselects the item because a left button up event occured.
        /// </summary>
        protected void UnselectItemOnLeftButtonUp()
        {
            Host.LeftClickUnselect(Content);
        }

        /// <summary>
        /// Selects the item because a right button down event occured.
        /// </summary>
        protected void SelectItemOnRightButtonDown()
        {
            Focus();
            Host.RightClickSelect(Content);
        }

        /// <summary>
        /// Unselects the item because a right button up event occured.
        /// </summary>
        protected void UnselectItemOnRightButtonUp()
        {
            Host.RightClickUnselect(Content);
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
