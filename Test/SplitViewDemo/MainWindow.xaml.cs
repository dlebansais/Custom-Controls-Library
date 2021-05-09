namespace CustomControls
{
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Media;

    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Init
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            TestContentInternal = "test";
        }
        #endregion

        #region Properties
        public string TestContent
        {
            get { return TestContentInternal; }
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
        private void OnViewLoaded(object sender, RoutedEventArgs e)
        {
            SplitView Ctrl = (SplitView)sender;
            ViewLoadedEventArgs Args = (ViewLoadedEventArgs)e;
            FrameworkElement View = Args.ViewContent;

            View.LayoutTransform = new ScaleTransform();
        }

        private void OnZoomChanged(object sender, RoutedEventArgs e)
        {
            SplitView Ctrl = (SplitView)sender;
            ZoomChangedEventArgs Args = (ZoomChangedEventArgs)e;
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
        public event PropertyChangedEventHandler? PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "With [CallerMemberName] a default parameter is mandatory")]
        public void NotifyThisPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
