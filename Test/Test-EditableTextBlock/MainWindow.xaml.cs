using System.Windows;

namespace TestEditableTextBlock
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public string EditableText
        {
            get { return EditableTextInternal; }
            set
            {
                if (EditableTextInternal != value)
                {
                    EditableTextInternal = value;
                }
            }
        }
        private string EditableTextInternal = "Init";
    }
}
