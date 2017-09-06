using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace CustomControls
{
    public class ExtendedTreeViewItemBase : ContentControl, INotifyPropertyChanged
    {
        #region Custom properties and events
        #region Is Selected
        public static readonly DependencyProperty IsSelectedProperty = Selector.IsSelectedProperty.AddOwner(typeof(ExtendedTreeViewItemBase));

        public bool IsSelected
        {
            get { return Selector.GetIsSelected(this); }
            set { Selector.SetIsSelected(this, value); }
        }
        #endregion
        #region Selected
        public static readonly RoutedEvent SelectedEvent = Selector.SelectedEvent;

        public event RoutedEventHandler Selected
        {
            add { AddHandler(SelectedEvent, value); }
            remove { RemoveHandler(SelectedEvent, value); }
        }

        protected virtual void NotifySelected()
        {
            RoutedEventArgs Args = new RoutedEventArgs(SelectedEvent, this);
            RaiseEvent(Args);
        }
        #endregion
        #region Unselected
        public static readonly RoutedEvent UnselectedEvent = Selector.UnselectedEvent;

        public event RoutedEventHandler Unselected
        {
            add { AddHandler(UnselectedEvent, value); }
            remove { RemoveHandler(UnselectedEvent, value); }
        }

        protected virtual void NotifyUnselected()
        {
            RoutedEventArgs Args = new RoutedEventArgs(UnselectedEvent, this);
            RaiseEvent(Args);
        }
        #endregion
        #region Is Expanded
        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register("IsExpanded", typeof(bool), typeof(ExtendedTreeViewItemBase), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal, new PropertyChangedCallback(OnIsExpandedChanged)));

        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        protected static void OnIsExpandedChanged(DependencyObject modifiedObject, DependencyPropertyChangedEventArgs e)
        {
            ExtendedTreeViewItemBase ctrl = (ExtendedTreeViewItemBase)modifiedObject;
            ctrl.OnIsExpandedChanged(e);
        }

        protected virtual void BeginInitializeContent()
        {
            IsContentInitializing = true;
        }

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

        protected virtual void EndInitializeContent()
        {
            IsContentInitializing = false;
        }

        protected bool IsContentInitializing { get; private set; }
        #endregion
        #region Expanded
        public static readonly RoutedEvent ExpandedEvent = EventManager.RegisterRoutedEvent("Expanded", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ExtendedTreeViewItemBase));

        public event RoutedEventHandler Expanded
        {
            add { AddHandler(ExpandedEvent, value); }
            remove { RemoveHandler(ExpandedEvent, value); }
        }

        protected virtual void NotifyExpanded()
        {
            RoutedEventArgs Args = new RoutedEventArgs(ExpandedEvent, this);
            RaiseEvent(Args);
        }
        #endregion
        #region Collapsed
        public static readonly RoutedEvent CollapsedEvent = EventManager.RegisterRoutedEvent("Collapsed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ExtendedTreeViewItemBase));

        public event RoutedEventHandler Collapsed
        {
            add { AddHandler(CollapsedEvent, value); }
            remove { RemoveHandler(CollapsedEvent, value); }
        }

        protected virtual void NotifyCollapsed()
        {
            RoutedEventArgs Args = new RoutedEventArgs(CollapsedEvent, this);
            RaiseEvent(Args);
        }
        #endregion
        #region Is Drop Over
        private static DependencyPropertyKey IsDropOverPropertyKey = DependencyProperty.RegisterReadOnly("IsDropOver", typeof(bool), typeof(ExtendedTreeViewItemBase), new PropertyMetadata(false));
        public static readonly DependencyProperty IsDropOverProperty = IsDropOverPropertyKey.DependencyProperty;

        public bool IsDropOver
        {
            get { return (bool)GetValue(IsDropOverProperty); }
        }
        #endregion
        #endregion

        #region Init
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static ExtendedTreeViewItemBase()
        {
            OverrideAncestorMetadata();
        }

        public ExtendedTreeViewItemBase(ExtendedTreeViewBase host)
        {
            this.Host = host;

            object DefaultStyle = TryFindResource(typeof(ExtendedTreeViewItemBase));
            if (DefaultStyle == null)
                Resources.MergedDictionaries.Add(SharedResourceDictionaryManager.SharedDictionary);
        }

        protected ExtendedTreeViewBase Host { get; private set; }
        #endregion

        #region Ancestor Interface
        protected static void OverrideAncestorMetadata()
        {
            OverrideMetadataContent();
            OverrideMetadataDefaultStyleKey();
        }

        protected static void OverrideMetadataContent()
        {
            ContentControl.ContentProperty.OverrideMetadata(typeof(ExtendedTreeViewItemBase), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnContentChanged)));
        }

        protected static void OverrideMetadataDefaultStyleKey()
        {
            ItemsControl.DefaultStyleKeyProperty.OverrideMetadata(typeof(ExtendedTreeViewItemBase), new FrameworkPropertyMetadata(typeof(ExtendedTreeViewItemBase)));
        }

        protected static void OnContentChanged(DependencyObject modifiedObject, DependencyPropertyChangedEventArgs e)
        {
            ExtendedTreeViewItemBase ctrl = (ExtendedTreeViewItemBase)modifiedObject;
            ctrl.OnContentChanged(e);
        }

        protected virtual void OnContentChanged(DependencyPropertyChangedEventArgs e)
        {
            BeginInitializeContent();

            object NewContent = e.NewValue;
            IsExpanded = Host.IsExpanded(NewContent);

            EndInitializeContent();
        }
        #endregion

        #region Properties
        public int Level { get { return Host.ItemLevel(Content); } }

        public void UpdateIsDropOver()
        {
            SetValue(IsDropOverPropertyKey, ExtendedTreeViewBase.IsDropOver(this));
        }

        public static void UpdateDisconnectedItem(object value)
        {
            if (DisconnectedItem == null && value != null)
            {
                Type ItemType = value.GetType();
                if (ItemType.FullName == "MS.Internal.NamedObject")
                    DisconnectedItem = value;
            }
        }

        private static object DisconnectedItem = null;
        #endregion

        #region Implementation
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            UpdateDisconnectedItem(newContent);

            if (DisconnectedItem != null && newContent != DisconnectedItem)
                NotifyPropertyChanged("Level");
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);

            Host.ContainerGotFocus(this);
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            Host.ContainerLostFocus();

            base.OnLostFocus(e);
        }
        #endregion

        #region Mouse Events
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            DebugCall();
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            DebugCall();
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            DebugCall();
            base.OnPreviewMouseLeftButtonDown(e);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            DebugCall();
            SelectItemOnLeftButtonDown();
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
            UnselectItemOnLeftButtonUp();
            base.OnMouseLeftButtonUp(e);
        }

        protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
        {
            DebugCall();
            base.OnPreviewMouseRightButtonDown(e);
        }

        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            DebugCall();
            SelectItemOnRightButtonDown();
            base.OnMouseRightButtonDown(e);
        }

        protected override void OnPreviewMouseRightButtonUp(MouseButtonEventArgs e)
        {
            DebugCall();
            base.OnPreviewMouseRightButtonUp(e);
        }

        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            DebugCall();
            UnselectItemOnRightButtonUp();
            base.OnMouseRightButtonUp(e);
        }
        #endregion

        #region Keyboard Events
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
        protected void SelectItemOnLeftButtonDown()
        {
            Focus();
            Host.LeftClickSelect(Content);
        }

        protected void UnselectItemOnLeftButtonUp()
        {
            Host.LeftClickUnselect(Content);
        }

        protected void SelectItemOnRightButtonDown()
        {
            Focus();
            Host.RightClickSelect(Content);
        }

        protected void UnselectItemOnRightButtonUp()
        {
            Host.RightClickUnselect(Content);
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

        protected virtual void DebugMessage(string message)
        {
            bool EnableTraces = ExtendedTreeViewBase.EnableTraces;
            if (EnableTraces)
                System.Diagnostics.Debug.Print(GetType().Name + ": " + message);
        }
        #endregion

        #region Implementation of INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameter is mandatory with [CallerMemberName]")]
        public void NotifyThisPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
