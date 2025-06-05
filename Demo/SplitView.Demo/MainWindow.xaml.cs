namespace SplitView.Demo;

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using CustomControls;

/// <summary>
/// Interaction logic for MainWindow.xaml.
/// </summary>
internal partial class MainWindow : Window, INotifyPropertyChanged
{
    #region Init
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;

        TestContentInternal = "test";
    }
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets the test content string.
    /// </summary>
    public string TestContent
    {
        get => TestContentInternal;
        set
        {
            if (TestContentInternal != value)
            {
                TestContentInternal = value;
                NotifyThisPropertyChanged();
            }
        }
    }

    private string TestContentInternal;
    #endregion

    #region Events
    private void OnViewLoaded(object sender, RoutedEventArgs args)
    {
        ViewLoadedEventArgs Args = (ViewLoadedEventArgs)args;
        FrameworkElement View = Args.ViewContent;

        View.LayoutTransform = new ScaleTransform();
    }

    private void OnZoomChanged(object sender, RoutedEventArgs args)
    {
        ZoomChangedEventArgs Args = (ZoomChangedEventArgs)args;
        FrameworkElement View = Args.ViewContent;
        double Zoom = Args.Zoom;

        if (View.LayoutTransform is ScaleTransform ZoomTransform)
        {
            ZoomTransform.ScaleX = Zoom;
            ZoomTransform.ScaleY = Zoom;
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
    protected void NotifyPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    /// <summary>
    /// Invoke handlers of the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="propertyName">Name of the property that changed.</param>
    protected void NotifyThisPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    #endregion
}
