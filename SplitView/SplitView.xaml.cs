namespace CustomControls;

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

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
    /// The maximum number of views that can be visible simultaneously. This control only supports two views.
    /// </summary>
    public const int MaxContent = 2;
    #endregion

    #region Init
    /// <summary>
    /// Initializes a new instance of the <see cref="SplitView"/> class.
    /// </summary>
    public SplitView()
    {
        InitializeComponent();

        CompositeCollection DefaultZoomOptions = (CompositeCollection)FindResource("DefaultZoomOptions");
        Collection<double> ConvertedZoomOptions = new();
        for (int i = 0; i < DefaultZoomOptions.Count; i++)
            ConvertedZoomOptions.Add((double)DefaultZoomOptions[i]);
        SetValue(ZoomOptionsPropertyKey, ConvertedZoomOptions);

        IsTopRowVisible = false;
    }
    #endregion

    #region Properties
    /// <summary>
    /// Gets a value indicating whether the two views are visible.
    /// </summary>
    /// <returns>
    /// True if the two views are visible. False otherwise.
    /// </returns>
    public bool IsTopRowVisible { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the control can be returned to a single view.
    /// </summary>
    /// <returns>
    /// True if the control can be returned to a single view. False otherwise.
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
    /// Gets a value indicating if a view is visible.
    /// </summary>
    /// <param name="rowIndex">The zero-based index of the row. The top row is at index 0.</param>
    /// <returns>
    /// True if the view at index <paramref name="rowIndex"/> is visible. False otherwise.
    /// </returns>
    public bool IsRowVisible(int rowIndex)
    {
        if (IsTopRowVisible)
            return true;
        else
            return rowIndex > 0;
    }

    /// <summary>
    /// Split the control in two views.
    /// </summary>
    public void Split()
    {
        SplitTestOrExecute(true, out _);
    }

    private void SplitTestOrExecute(bool isExecute, out bool isTestSuccessful)
    {
        isTestSuccessful = false;

        if (!IsTopRowVisible)
        {
            if (gridInner.RowDefinitions.Count >= 3 && gridInner.Children.Count > 0 && !double.IsNaN(gridInner.ActualHeight))
            {
                RowDefinition TopRow = gridInner.RowDefinitions[0];
                RowDefinition BottomRow = gridInner.RowDefinitions[2];
                FrameworkElement TopChild = (FrameworkElement)gridInner.Children[0];

                if (TopChild.FindName("comboZoom0") is FrameworkElement ScrollBottom)
                {
                    if (!double.IsNaN(ScrollBottom.ActualHeight) && ScrollBottom.ActualHeight > 0)
                    {
                        if (gridInner.ActualHeight > (2 * (ScrollBottom.ActualHeight + splitterLine.Height)))
                        {
                            isTestSuccessful = true;

                            if (isExecute)
                            {
                                TopRow.Height = new GridLength(1.0, GridUnitType.Star);
                                BottomRow.Height = new GridLength(1.0, GridUnitType.Star);
                                IsTopRowVisible = true;

                                NotifyTopRowVisibilityChanged();
                                NotifyPropertyChanged(nameof(IsTopRowVisible));
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Removes any split and returns the control to a single view state.
    /// </summary>
    public void RemoveSplit()
    {
        RemoveSplitTestOrExecute(true, out _);
    }

    private void RemoveSplitTestOrExecute(bool isExecute, out bool isTestSuccessful)
    {
        isTestSuccessful = false;

        if (IsTopRowVisible)
        {
            if (gridInner.RowDefinitions.Count > 0 && gridInner.Children.Count > 0)
            {
                isTestSuccessful = true;

                if (isExecute)
                {
                    RowDefinition TopRow = gridInner.RowDefinitions[0];
                    TopRow.Height = new GridLength(0, GridUnitType.Star);
                    IsTopRowVisible = false;

                    NotifyTopRowVisibilityChanged();
                    NotifyPropertyChanged(nameof(IsTopRowVisible));
                }
            }
        }
    }

    /// <summary>
    /// Return the control at the specified row.
    /// </summary>
    /// <param name="rowIndex">The row index.</param>
    /// <returns>the control at the specified row.</returns>
    public FrameworkElement? GetRowContent(int rowIndex)
    {
        ScrollViewer scrollViewer = (rowIndex == 0) ? viewer0 : viewer1;
        ContentControl contentControl = (ContentControl)scrollViewer.Content;

        FrameworkElement? rowContent;
        if (VisualTreeHelper.GetChildrenCount(contentControl) > 0)
        {
            ContentPresenter contentPresenter = (ContentPresenter)VisualTreeHelper.GetChild(contentControl, 0);
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
        ContentControl Content = (ContentControl)sender;

        if (VisualTreeHelper.GetChildrenCount(Content) > 0)
        {
            ContentPresenter Presenter = (ContentPresenter)VisualTreeHelper.GetChild(Content, 0);
            if (VisualTreeHelper.GetChildrenCount(Presenter) > 0)
            {
                FrameworkElement ViewContent = (FrameworkElement)VisualTreeHelper.GetChild(Presenter, 0);
                NotifyViewLoaded(ViewContent);
            }
        }
    }

    private void OnContentControlUnloaded(object sender, RoutedEventArgs e)
    {
        ContentControl Content = (ContentControl)sender;

        ContentPresenter Presenter = (ContentPresenter)VisualTreeHelper.GetChild(Content, 0);
        FrameworkElement ViewContent = (FrameworkElement)VisualTreeHelper.GetChild(Presenter, 0);
        NotifyViewUnloaded(ViewContent);
    }

    private delegate void UpdateLengthHandler(FrameworkElement element, Size adjustedSize);

    private void OnBottomScrollBar0SizeChanged(object sender, SizeChangedEventArgs e)
    {
        FixSiblingDimension(sender, "comboZoom0", UpdateHeightAndIndex, OnBottomScrollBar0SizeChanged);
    }

    private void OnBottomScrollBar1SizeChanged(object sender, SizeChangedEventArgs e)
    {
        FixSiblingDimension(sender, "comboZoom1", UpdateHeightAndIndex, OnBottomScrollBar1SizeChanged);
    }

    private void UpdateHeightAndIndex(FrameworkElement element, Size adjustedSize)
    {
        element.Height = adjustedSize.Height;

        if (element is ComboBox AsComboBox)
            _ = Dispatcher.BeginInvoke(DispatcherPriority.Normal, new UpdateIndexHandler(OnUpdateIndex), AsComboBox);
    }

    private delegate void UpdateIndexHandler(ComboBox asComboBox);
    private void OnUpdateIndex(ComboBox asComboBox)
    {
        asComboBox.SelectedIndex = 3;
    }

    private void OnRightScrollBarSizeChanged(object sender, SizeChangedEventArgs e)
    {
        FixSiblingDimension(sender, "buttonSplit", UpdateWidthAndHeight, OnRightScrollBarSizeChanged);
    }

    private void UpdateWidthAndHeight(FrameworkElement element, Size adjustedSize)
    {
        double Length = adjustedSize.Width;
        element.Width = Length;
        element.Height = Length;
    }

    private static void FixSiblingDimension(object sender, string siblingName, UpdateLengthHandler handler, SizeChangedEventHandler eventHandler)
    {
        FrameworkElement SenderElement = (FrameworkElement)sender;
        Size AdjustedSize = new(SenderElement.ActualWidth, SenderElement.ActualHeight);

        if ((!double.IsNaN(AdjustedSize.Width) && AdjustedSize.Width > 0) ||
            (!double.IsNaN(AdjustedSize.Height) && AdjustedSize.Height > 0))
        {
            FrameworkElement CurrentElement = SenderElement;

            while (true)
            {
                if (CurrentElement is Panel AsPanel)
                {
                    if (AsPanel.FindName(siblingName) is FrameworkElement AsSibling)
                        if (double.IsNaN(AsSibling.Height))
                        {
                            SenderElement.SizeChanged -= eventHandler;
                            handler(AsSibling, AdjustedSize);
                        }

                    break;
                }
                else if (CurrentElement.Parent is FrameworkElement AsFrameworkElement)
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
        ComboBox ctrl = (ComboBox)sender;
        int SelectedIndex = ctrl.SelectedIndex;

        if (SelectedIndex >= 0 && SelectedIndex < ZoomOptions.Count)
        {
            double Zoom = ZoomOptions[SelectedIndex];

            if (ZoomChangedEvent is not null)
            {
                ContentControl Content = (ContentControl)viewer.Content;
                if (VisualTreeHelper.GetChildrenCount(Content) > 0)
                {
                    ContentPresenter Presenter = (ContentPresenter)VisualTreeHelper.GetChild(Content, 0);
                    if (VisualTreeHelper.GetChildrenCount(Presenter) > 0)
                    {
                        FrameworkElement ViewContent = (FrameworkElement)VisualTreeHelper.GetChild(Presenter, 0);
                        NotifyZoomChanged(ViewContent, Zoom);
                    }
                }
            }
        }
    }

    private void OnSplitClick(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true;

        MouseButtonEventArgs args = new(InputManager.Current.PrimaryMouseDevice, e.Timestamp, MouseButton.Left);
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
            FrameworkElement TopChild = (FrameworkElement)gridInner.Children[0];

            if (!double.IsNaN(TopRow.ActualHeight) && TopRow.ActualHeight >= 0)
                if (TopChild.FindName("comboZoom0") is FrameworkElement ScrollBottom)
                {
                    if (!double.IsNaN(ScrollBottom.ActualHeight) && ScrollBottom.ActualHeight > 0)
                    {
                        if (TopRow.ActualHeight <= ScrollBottom.ActualHeight + splitterLine.Height)
                        {
                            TopRow.Height = new GridLength(0, GridUnitType.Star);
                            IsTopRowVisible = false;
                        }
                    }
                }
        }

        if (IsTopRowVisible != OldIsVisible)
        {
            NotifyTopRowVisibilityChanged();
            NotifyPropertyChanged(nameof(IsTopRowVisible));
        }
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
