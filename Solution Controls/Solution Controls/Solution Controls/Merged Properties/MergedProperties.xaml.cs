using System;
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
            if (modifiedObject == null)
                throw new ArgumentNullException(nameof(modifiedObject));

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
            EditableTextBlock Ctrl = (EditableTextBlock)sender;
            StringPropertyEntry Entry = (StringPropertyEntry)Ctrl.DataContext;
            Entry.UpdateText(Ctrl.Text);
        }

        private void OnSelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox Ctrl = (ComboBox)sender;
            EnumPropertyEntry Entry = (EnumPropertyEntry)Ctrl.DataContext;
            Entry.UpdateSelectedIndex(Ctrl.SelectedIndex);
        }
        #endregion
    }
}
