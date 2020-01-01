namespace CustomControls
{
    using System;
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// Represents a window to display documents to save.
    /// </summary>
    public partial class SaveAllWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SaveAllWindow"/> class.
        /// </summary>
        public SaveAllWindow()
        {
            InitializeComponent();
            DataContext = this;

            Loaded += OnLoaded;
        }

        /// <summary>
        /// Called when the window is loaded.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnLoaded(object sender, EventArgs e)
        {
            Title = Owner.Title;
            Icon = Owner.Icon;
        }

        /// <summary>
        /// Gets the list of title.
        /// </summary>
        public ObservableCollection<string> TitleList { get; } = new ObservableCollection<string>();

        /// <summary>
        /// Gets or sets the name of the modified solution.
        /// </summary>
        public string DirtySolutionName { get; set; } = string.Empty;

        /// <summary>
        /// Gets the result of the user choice.
        /// </summary>
        public MessageBoxResult Result { get; private set; } = MessageBoxResult.Cancel;

        /// <summary>
        /// Gets the text for items to save.
        /// </summary>
        public string SaveText
        {
            get
            {
                string Text = string.Empty;
                string TabText = string.Empty;

                if (DirtySolutionName.Length > 0)
                {
                    Text += DirtySolutionName;
                    Text += "*";
                    TabText = "  ";
                }

                foreach (string Title in TitleList)
                {
                    if (Text.Length > 0)
                        Text += "\r\n";

                    Text += TabText + Title;
                }

                return Text;
            }
        }

        private void OnYes(object sender, ExecutedRoutedEventArgs e)
        {
            Result = MessageBoxResult.Yes;
            Close();
        }

        private void OnNo(object sender, ExecutedRoutedEventArgs e)
        {
            Result = MessageBoxResult.No;
            Close();
        }

        private void OnCancel(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }
    }
}
