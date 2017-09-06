using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CustomControls
{
    public partial class MergedProperties : UserControl
    {
        #region Custom properties and events
        #region Document Types
        public static readonly DependencyProperty PropertyEntriesProperty = DependencyProperty.Register("PropertyEntries", typeof(IEnumerable), typeof(MergedProperties), new PropertyMetadata(null, OnPropertyEntriesChanged));

        public IEnumerable PropertyEntries
        {
            get { return (IEnumerable)GetValue(PropertyEntriesProperty); }
            set { SetValue(PropertyEntriesProperty, value); }
        }

        protected static void OnPropertyEntriesChanged(DependencyObject modifiedObject, DependencyPropertyChangedEventArgs e)
        {
            MergedProperties ctrl = (MergedProperties)modifiedObject;
            ctrl.OnPropertyEntriesChanged(e);
        }

        protected virtual void OnPropertyEntriesChanged(DependencyPropertyChangedEventArgs e)
        {
        }
        #endregion
        #endregion

        #region Init
        public MergedProperties()
        {
            InitializeComponent();
        }
        #endregion

        #region Events
        private void OnTextMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            EditableTextBlock Ctrl = (EditableTextBlock)sender;
            Ctrl.IsEditing = true;
        }

        private void OnTextChanged(object sender, RoutedEventArgs e)
        {
            EditableTextBlock Ctrl = sender as EditableTextBlock;
            if (Ctrl != null)
            {
                StringPropertyEntry Entry = Ctrl.DataContext as StringPropertyEntry;
                if (Entry != null)
                    Entry.UpdateText(Ctrl.Text);
            }
        }

        private void OnSelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox Ctrl = sender as ComboBox;
            if (Ctrl != null)
            {
                EnumPropertyEntry Entry = Ctrl.DataContext as EnumPropertyEntry;
                if (Entry != null)
                    Entry.UpdateSelectedIndex(Ctrl.SelectedIndex);
            }
        }
        #endregion
    }
}
