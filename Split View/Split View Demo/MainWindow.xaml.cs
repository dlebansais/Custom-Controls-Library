using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;

namespace CustomControls
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Init
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            _TestContent = "test";
        }
        #endregion

        #region Properties
        public string TestContent 
        {
            get { return _TestContent; }
            set
            {
                if (_TestContent != value)
                {
                    _TestContent = value;
                    NotifyThisPropertyChanged();
                }
            }
        }
        private string _TestContent;
        #endregion

        #region Events
        private void OnViewLoaded(object sender, RoutedEventArgs e)
        {
            SplitView Ctrl = sender as SplitView;
            ViewLoadedEventArgs Args = e as ViewLoadedEventArgs;
            FrameworkElement View = Args.ViewContent;

            View.LayoutTransform = new ScaleTransform();
        }

        private void OnZoomChanged(object sender, RoutedEventArgs e)
        {
            SplitView Ctrl = sender as SplitView;
            ZoomChangedEventArgs Args = e as ZoomChangedEventArgs;
            FrameworkElement View = Args.ViewContent;
            double Zoom = Args.Zoom;

            ScaleTransform ZoomTransform;
            if ((ZoomTransform = View.LayoutTransform as ScaleTransform) != null)
            {
                ZoomTransform.ScaleX = Zoom;
                ZoomTransform.ScaleY = Zoom;
            }
        }
        #endregion

        #region Implementation of INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "With [CallerMemberName] a default parameter is mandatory")]
        public void NotifyThisPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
