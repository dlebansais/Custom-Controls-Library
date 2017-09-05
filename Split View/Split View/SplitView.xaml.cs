using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace CustomControls
{
    /// <summary>
    /// <para>Represents a view of an arbitrary content that can be split horizontally in two views.</para>
    /// <para>Implemented as a user control with a <see cref="ViewTemplate"/> template for views.</para>
    /// </summary>
    /// <remarks>
    /// <para>Features:</para>
    /// <para>. Begins as a single view with a grip on the top right corner to start splitting.</para>
    /// <para>. Uses a <see cref="GridSplitter"/> to separate the views.</para>
    /// <para>. Hides one of the views if it becomes too small to be visible.</para>
    /// </remarks>
    public partial class SplitView : UserControl, INotifyPropertyChanged
    {
        #region Constants
        /// <summary>
        ///     The maximum number of views that can be visible simultaneously. This control only supports two views.
        /// </summary>
        public const int MaxContent = 2;
        #endregion

        #region Custom properties and events
        #region Cell Template
        /// <summary>
        ///     Identifies the <see cref="ViewTemplate"/> dependency property.
        /// </summary>
        /// <returns>
        ///     The identifier for the <see cref="ViewTemplate"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty ViewTemplateProperty = DependencyProperty.Register("ViewTemplate", typeof(DataTemplate), typeof(SplitView), new PropertyMetadata(null));

        /// <summary>
        ///     Gets or sets the <see cref="DataTemplate"/> used to display each view.
        /// </summary>
        public DataTemplate ViewTemplate
        {
            get { return (DataTemplate)GetValue(ViewTemplateProperty); }
            set { SetValue(ViewTemplateProperty, value); }
        }
        #endregion
        #region Zoom Options
        private static readonly DependencyPropertyKey ZoomOptionsPropertyKey = DependencyProperty.RegisterReadOnly("ZoomOptions", typeof(Collection<double>), typeof(SplitView), new PropertyMetadata(null));
        /// <summary>
        ///     Identifies the <see cref="ZoomOptions"/> dependency property.
        /// </summary>
        /// <returns>
        ///     The identifier for the <see cref="ZoomOptions"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty ZoomOptionsProperty = ZoomOptionsPropertyKey.DependencyProperty;

        /// <summary>
        ///     Gets a collection of zooms that can be applied to each view separately.
        /// </summary>
        public Collection<double> ZoomOptions
        {
            get { return (Collection<double>)GetValue(ZoomOptionsProperty); }
        }
        #endregion
        #region Content Loaded
        /// <summary>
        ///     Identifies the ViewLoaded routed event.
        /// </summary>
        public static readonly RoutedEvent ViewLoadedEvent = EventManager.RegisterRoutedEvent("ViewLoaded", RoutingStrategy.Bubble, typeof(RoutedEvent), typeof(SplitView));

        /// <summary>
        ///     Sent when a view has been loaded.
        /// </summary>
        public event RoutedEventHandler ViewLoaded
        {
            add { AddHandler(ViewLoadedEvent, value); }
            remove { RemoveHandler(ViewLoadedEvent, value); }
        }

        /// <summary>
        ///     Sends the <see cref="ViewLoaded"/> event.
        /// </summary>
        /// <parameters>
        /// <param name="viewContent">The control representing the view that has been loaded.</param>
        /// </parameters>
        protected virtual void NotifyViewLoaded(FrameworkElement viewContent)
        {
            ViewLoadedEventArgs Args = new ViewLoadedEventArgs(ViewLoadedEvent, viewContent);
            RaiseEvent(Args);
        }
        #endregion
        #region Content Unloaded
        /// <summary>
        ///     Identifies the ViewUnloaded routed event.
        /// </summary>
        public static readonly RoutedEvent ViewUnloadedEvent = EventManager.RegisterRoutedEvent("ViewUnloaded", RoutingStrategy.Bubble, typeof(RoutedEvent), typeof(SplitView));

        /// <summary>
        ///     Sent when a view has been unloaded.
        /// </summary>
        public event RoutedEventHandler ViewUnloaded
        {
            add { AddHandler(ViewUnloadedEvent, value); }
            remove { RemoveHandler(ViewUnloadedEvent, value); }
        }

        /// <summary>
        ///     Sends the <see cref="ViewUnloaded"/> event.
        /// </summary>
        /// <parameters>
        /// <param name="viewContent">The control representing the view that has been unloaded.</param>
        /// </parameters>
        protected virtual void NotifyViewUnloaded(FrameworkElement viewContent)
        {
            ViewUnloadedEventArgs Args = new ViewUnloadedEventArgs(ViewUnloadedEvent, viewContent);
            RaiseEvent(Args);
        }
        #endregion
        #region Top Row Visibility Changed
        /// <summary>
        ///     Identifies the TopRowVisibilityChanged routed event.
        /// </summary>
        public static readonly RoutedEvent TopRowVisibilityChangedEvent = EventManager.RegisterRoutedEvent("TopRowVisibilityChanged", RoutingStrategy.Bubble, typeof(RoutedEvent), typeof(SplitView));

        /// <summary>
        ///     Sent when the top row view has becomed visible or collapsed.
        /// </summary>
        public event RoutedEventHandler TopRowVisibilityChanged
        {
            add { AddHandler(TopRowVisibilityChangedEvent, value); }
            remove { RemoveHandler(TopRowVisibilityChangedEvent, value); }
        }

        /// <summary>
        ///     Sends the <see cref="TopRowVisibilityChanged"/> event.
        /// </summary>
        protected virtual void NotifyTopRowVisibilityChanged()
        {
            RoutedEventArgs Args = new RoutedEventArgs(TopRowVisibilityChangedEvent, this);
            RaiseEvent(Args);
        }
        #endregion
        #region Zoom Changed
        /// <summary>
        ///     Identifies the ZoomChanged routed event.
        /// </summary>
        public static readonly RoutedEvent ZoomChangedEvent = EventManager.RegisterRoutedEvent("ZoomChanged", RoutingStrategy.Bubble, typeof(RoutedEvent), typeof(SplitView));

        /// <summary>
        ///     Sent when the zoom of one of the views has changed.
        /// </summary>
        public event RoutedEventHandler ZoomChanged
        {
            add { AddHandler(ZoomChangedEvent, value); }
            remove { RemoveHandler(ZoomChangedEvent, value); }
        }

        /// <summary>
        ///     Sends the <see cref="ViewUnloaded"/> event.
        /// </summary>
        /// <parameters>
        /// <param name="viewContent">The control representing the view to which the new zoom applies.</param>
        /// <param name="zoom">The new zoom value.</param>
        /// </parameters>
        /// <remarks>
        ///     A value of <paramref name="zoom"/>=1.0 means no zoom (or 100%).
        /// </remarks>
        protected virtual void NotifyZoomChanged(FrameworkElement viewContent, double zoom)
        {
            ZoomChangedEventArgs Args = new ZoomChangedEventArgs(ZoomChangedEvent, viewContent, zoom);
            RaiseEvent(Args);
        }
        #endregion
        #endregion

        #region Init
        /// <summary>
        ///     Initializes a new instance of the <see cref="SplitView"/> class.
        /// </summary>
        public SplitView()
        {
            InitializeComponent();

            CompositeCollection DefaultZoomOptions = FindResource("DefaultZoomOptions") as CompositeCollection;
            Collection<double> ConvertedZoomOptions = new Collection<double>();
            for (int i = 0; i < DefaultZoomOptions.Count; i++)
                ConvertedZoomOptions.Add((double)DefaultZoomOptions[i]);
            SetValue(ZoomOptionsPropertyKey, ConvertedZoomOptions);

            IsTopRowVisible = false;
        }
        #endregion

        #region Properties
        /// <summary>
        ///     Gets a flag indicating if the two views are visible.
        /// </summary>
        /// <returns>
        ///     True if the two views are visible. False otherwise.
        /// </returns>
        public bool IsTopRowVisible { get; private set; }

        /// <summary>
        ///     Gets a value indicating if the control can be returned to a single view.
        /// </summary>
        /// <returns>
        ///     True if the control can be returned to a single view. False otherwise.
        /// </returns>
        public bool IsSplitRemovable
        {
            get
            {
                bool IsTestSuccessful;
                RemoveSplitTestOrExecute(false, out IsTestSuccessful);

                return IsTestSuccessful;
            }
        }
        #endregion

        #region Client Interface
        /// <summary>
        ///     Gets a value indicating if a view is visible.
        /// </summary>
        /// <parameters>
        /// <param name="rowIndex">The zero-based index of the row. The top row is at index 0.</param>
        /// </parameters>
        /// <returns>
        ///     True if the view at index <paramref name="rowIndex"/> is visible. False otherwise.
        /// </returns>
        public bool IsRowVisible(int rowIndex)
        {
            if (IsTopRowVisible)
                return true;
            else
                return rowIndex > 0;
        }

        /// <summary>
        ///     Split the control in two views.
        /// </summary>
        public void Split()
        {
            bool IsTestSuccessful;
            SplitTestOrExecute(true, out IsTestSuccessful);
        }

        private void SplitTestOrExecute(bool IsExecute, out bool IsTestSuccessful)
        {
            IsTestSuccessful = false;

            if (!IsTopRowVisible)
            {
                if (gridInner.RowDefinitions.Count >= 3 && gridInner.Children.Count > 0 && !double.IsNaN(gridInner.ActualHeight))
                {
                    RowDefinition TopRow = gridInner.RowDefinitions[0];
                    RowDefinition BottomRow = gridInner.RowDefinitions[2];
                    FrameworkElement TopChild = gridInner.Children[0] as FrameworkElement;

                    FrameworkElement scrollBottom;
                    if ((scrollBottom = TopChild.FindName("comboZoom0") as FrameworkElement) != null)
                    {
                        if (!double.IsNaN(scrollBottom.ActualHeight) && scrollBottom.ActualHeight > 0)
                        {
                            if (gridInner.ActualHeight > (2 * (scrollBottom.ActualHeight + splitterLine.Height)))
                            {
                                IsTestSuccessful = true;

                                if (IsExecute)
                                {
                                    TopRow.Height = new GridLength(1.0, GridUnitType.Star);
                                    BottomRow.Height = new GridLength(1.0, GridUnitType.Star);
                                    IsTopRowVisible = true;

                                    NotifyTopRowVisibilityChanged();
                                    NotifyPropertyChanged("IsTopRowVisible");
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Removes any split and returns the control to a single view state.
        /// </summary>
        public void RemoveSplit()
        {
            bool IsTestSuccessful;
            RemoveSplitTestOrExecute(true, out IsTestSuccessful);
        }

        private void RemoveSplitTestOrExecute(bool IsExecute, out bool IsTestSuccessful)
        {
            IsTestSuccessful = false;

            if (IsTopRowVisible)
            {
                if (gridInner.RowDefinitions.Count > 0 && gridInner.Children.Count > 0)
                {
                    IsTestSuccessful = true;

                    if (IsExecute)
                    {
                        RowDefinition TopRow = gridInner.RowDefinitions[0];
                        TopRow.Height = new GridLength(0, GridUnitType.Star);
                        IsTopRowVisible = false;

                        NotifyTopRowVisibilityChanged();
                        NotifyPropertyChanged("IsTopRowVisible");
                    }
                }
            }
        }

        /// <summary>
        ///     Return the control at the specified row.
        /// </summary>
        public FrameworkElement GetRowContent(int rowIndex)
        {
            ScrollViewer scrollViewer = (rowIndex == 0) ? viewer0 : viewer1;
            ContentControl contentControl = scrollViewer.Content as ContentControl;

            FrameworkElement rowContent;
            if (VisualTreeHelper.GetChildrenCount(contentControl) > 0)
            {
                ContentPresenter contentPresenter = VisualTreeHelper.GetChild(contentControl, 0) as ContentPresenter;
                if (VisualTreeHelper.GetChildrenCount(contentPresenter) > 0)
                {
                    rowContent = VisualTreeHelper.GetChild(contentPresenter, 0) as FrameworkElement;
                }
                else
                    rowContent = null;
            }
            else
                rowContent = null;

            return rowContent;
        }
        #endregion

        #region Events
        private void OnContentControlLoaded(object sender, RoutedEventArgs e)
        {
            ContentControl Content = sender as ContentControl;

            if (VisualTreeHelper.GetChildrenCount(Content) > 0)
            {
                ContentPresenter Presenter = VisualTreeHelper.GetChild(Content, 0) as ContentPresenter;
                if (VisualTreeHelper.GetChildrenCount(Presenter) > 0)
                {
                    FrameworkElement ViewContent = VisualTreeHelper.GetChild(Presenter, 0) as FrameworkElement;
                    NotifyViewLoaded(ViewContent);
                }
            }
        }

        private void OnContentControlUnloaded(object sender, RoutedEventArgs e)
        {
            ContentControl Content = sender as ContentControl;

            ContentPresenter Presenter = VisualTreeHelper.GetChild(Content, 0) as ContentPresenter;
            FrameworkElement ViewContent = VisualTreeHelper.GetChild(Presenter, 0) as FrameworkElement;
            NotifyViewUnloaded(ViewContent);
        }

        private delegate void UpdateLengthHandler(FrameworkElement Element, Size AdjustedSize);

        private void OnBottomScrollBar0SizeChanged(object sender, SizeChangedEventArgs e)
        {
            FixSiblingDimension(sender, "comboZoom0", UpdateHeightAndIndex, OnBottomScrollBar0SizeChanged);
        }

        private void OnBottomScrollBar1SizeChanged(object sender, SizeChangedEventArgs e)
        {
            FixSiblingDimension(sender, "comboZoom1", UpdateHeightAndIndex, OnBottomScrollBar1SizeChanged);
        }

        private void UpdateHeightAndIndex(FrameworkElement Element, Size AdjustedSize)
        {
            Element.Height = AdjustedSize.Height;

            ComboBox AsComboBox;
            if ((AsComboBox = Element as ComboBox) != null)
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new UpdateIndexHandler(OnUpdateIndex), AsComboBox);
        }

        private delegate void UpdateIndexHandler(ComboBox AsComboBox);
        private void OnUpdateIndex(ComboBox AsComboBox)
        {
            AsComboBox.SelectedIndex = 3;
        }

        private void OnRightScrollBarSizeChanged(object sender, SizeChangedEventArgs e)
        {
            FixSiblingDimension(sender, "buttonSplit", UpdateWidthAndHeight, OnRightScrollBarSizeChanged);
        }

        private void UpdateWidthAndHeight(FrameworkElement Element, Size AdjustedSize)
        {
            double Length = AdjustedSize.Width;
            Element.Width = Length;
            Element.Height = Length;
        }

        private static void FixSiblingDimension(object sender, string SiblingName, UpdateLengthHandler Handler, SizeChangedEventHandler EventHandler)
        {
            FrameworkElement SenderElement = sender as FrameworkElement;
            Size AdjustedSize = new Size(SenderElement.ActualWidth, SenderElement.ActualHeight);

            if ((!double.IsNaN(AdjustedSize.Width) && AdjustedSize.Width > 0) ||
                (!double.IsNaN(AdjustedSize.Height) && AdjustedSize.Height > 0))
            {
                FrameworkElement CurrentElement = SenderElement;

                while (CurrentElement != null)
                {
                    Panel AsPanel;
                    FrameworkElement AsFrameworkElement;

                    if ((AsPanel = CurrentElement as Panel) != null)
                    {
                        FrameworkElement AsSibling;
                        if ((AsSibling = AsPanel.FindName(SiblingName) as FrameworkElement) != null)
                        {
                            if (double.IsNaN(AsSibling.Height))
                            {
                                SenderElement.SizeChanged -= EventHandler;
                                Handler(AsSibling, AdjustedSize);
                            }
                        }

                        break;
                    }

                    else if ((AsFrameworkElement = CurrentElement.Parent as FrameworkElement) != null)
                        CurrentElement = AsFrameworkElement;

                    else
                        break;
                }
            }
        }

        private void OnMainGridSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Grid MainGrid = (Grid)sender;
            if ((!double.IsNaN(MainGrid.ActualWidth) && MainGrid.ActualWidth > 0) &&
                (!double.IsNaN(MainGrid.ActualHeight) && MainGrid.ActualHeight > 0))
            {
                Grid InnerGrid = (Grid)MainGrid.FindName("gridInner");
                InnerGrid.Width = MainGrid.ActualWidth;
                InnerGrid.Height = MainGrid.ActualHeight + (splitterLine.Height * 2);
            }
        }

        private void OnZoom0Changed(object sender, SelectionChangedEventArgs e)
        {
            OnZoomChanged(sender, viewer0);
        }

        private void OnZoom1Changed(object sender, SelectionChangedEventArgs e)
        {
            OnZoomChanged(sender, viewer1);
        }

        private void OnZoomChanged(object sender, ScrollViewer viewer)
        {
            ComboBox ctrl = sender as ComboBox;
            int SelectedIndex = ctrl.SelectedIndex;

            if (SelectedIndex >= 0 && SelectedIndex < ZoomOptions.Count)
            {
                double Zoom = ZoomOptions[SelectedIndex];

                if (ZoomChangedEvent != null)
                {
                    ContentControl Content = viewer.Content as ContentControl;
                    if (VisualTreeHelper.GetChildrenCount(Content) > 0)
                    {
                        ContentPresenter Presenter = VisualTreeHelper.GetChild(Content, 0) as ContentPresenter;
                        if (VisualTreeHelper.GetChildrenCount(Presenter) > 0)
                        {
                            FrameworkElement ViewContent = VisualTreeHelper.GetChild(Presenter, 0) as FrameworkElement;
                            NotifyZoomChanged(ViewContent, Zoom);
                        }
                    }
                }
            }
        }

        private void OnSplitClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            MouseButtonEventArgs args = new MouseButtonEventArgs(InputManager.Current.PrimaryMouseDevice, e.Timestamp, MouseButton.Left);
            args.RoutedEvent = e.RoutedEvent;
            args.Source = splitterLine;
            splitterLine.RaiseEvent(args);
        }

        private void OnDragCompleted(object sender, DragCompletedEventArgs e)
        {
            bool OldIsVisible = IsTopRowVisible;

            IsTopRowVisible = true;

            if (gridInner.RowDefinitions.Count > 0 && gridInner.Children.Count > 0)
            {
                RowDefinition TopRow = gridInner.RowDefinitions[0];
                FrameworkElement TopChild = gridInner.Children[0] as FrameworkElement;

                if (!double.IsNaN(TopRow.ActualHeight) && TopRow.ActualHeight >= 0)
                {
                    FrameworkElement scrollBottom;
                    if ((scrollBottom = TopChild.FindName("comboZoom0") as FrameworkElement) != null)
                    {
                        if (!double.IsNaN(scrollBottom.ActualHeight) && scrollBottom.ActualHeight > 0)
                        {
                            if (TopRow.ActualHeight <= scrollBottom.ActualHeight + splitterLine.Height)
                            {
                                TopRow.Height = new GridLength(0, GridUnitType.Star);
                                IsTopRowVisible = false;
                            }
                        }
                    }
                }
            }

            if (IsTopRowVisible != OldIsVisible)
            {
                NotifyTopRowVisibilityChanged();
                NotifyPropertyChanged("IsTopRowVisible");
            }
        }
        #endregion

        #region Implementation of INotifyPropertyChanged
        /// <summary>
        ///     Implements the PropertyChanged event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        internal void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameter is mandatory with [CallerMemberName]")]
        internal void NotifyThisPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
