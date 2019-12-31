namespace CustomControls
{
    using System;
    using System.Collections;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Represents a control containing merged properties.
    /// </summary>
    public partial class MergedProperties : UserControl
    {
        #region Custom properties and events
        #region Document Types
        /// <summary>
        /// Identifies the <see cref="PropertyEntries"/> attached property.
        /// </summary>
        public static readonly DependencyProperty PropertyEntriesProperty = DependencyProperty.Register("PropertyEntries", typeof(IEnumerable), typeof(MergedProperties), new PropertyMetadata(null, OnPropertyEntriesChanged));

        /// <summary>
        /// Gets or sets the propertie entries.
        /// </summary>
        public IEnumerable PropertyEntries
        {
            get { return (IEnumerable)GetValue(PropertyEntriesProperty); }
            set { SetValue(PropertyEntriesProperty, value); }
        }

        /// <summary>
        /// Handles changes of the <see cref="PropertyEntries"/> property.
        /// </summary>
        /// <param name="modifiedObject">The modified object.</param>
        /// <param name="e">An object that contains event data.</param>
        protected static void OnPropertyEntriesChanged(DependencyObject modifiedObject, DependencyPropertyChangedEventArgs e)
        {
            if (modifiedObject == null)
                throw new ArgumentNullException(nameof(modifiedObject));

            MergedProperties ctrl = (MergedProperties)modifiedObject;
            ctrl.OnPropertyEntriesChanged(e);
        }

        /// <summary>
        /// Handles changes of the <see cref="PropertyEntries"/> property.
        /// </summary>
        /// <param name="e">An object that contains event data.</param>
        protected virtual void OnPropertyEntriesChanged(DependencyPropertyChangedEventArgs e)
        {
        }
        #endregion
        #endregion

        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="MergedProperties"/> class.
        /// </summary>
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
