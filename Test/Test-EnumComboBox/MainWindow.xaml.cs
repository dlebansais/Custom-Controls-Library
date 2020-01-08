﻿namespace TestEnumComboBox
{
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            ctrl.NameConverterParameter = string.Empty;
            ctrl.NameConverterCulture = CultureInfo.InvariantCulture;
        }

        public TestEnum1 TestProperty { get; set; }
        public TestEnum2 OtherTestProperty { get; set; }

        private void OnNullSet(object sender, RoutedEventArgs e)
        {
            OldEnumBinding = ctrl.EnumBinding;

            ctrl.EnumBinding = OtherTestProperty;
            ctrl.SelectedIndex = -1;
            ctrl.SelectedIndex = 0;

            ctrl.EnumBinding = null;
            OldNameConverter = ctrl.NameConverter;
            ctrl.SelectedIndex = -1;
        }

        private void OnNullCleared(object sender, RoutedEventArgs e)
        {
            ctrl.SelectedIndex = 0;
            ctrl.EnumBinding = OldEnumBinding;
            if (OldNameConverter != null)
                ctrl.NameConverter = OldNameConverter;
        }

        private void OnBadSet(object sender, RoutedEventArgs e)
        {
            OldEnumBinding = ctrl.EnumBinding;

            ctrl.EnumBinding = BadBinding;
            ctrl.SelectedIndex = -1;
        }

        private void OnBadCleared(object sender, RoutedEventArgs e)
        {
            ctrl.SelectedIndex = 0;
            ctrl.EnumBinding = OldEnumBinding;
        }

        private object? OldEnumBinding = null;
        private IValueConverter? OldNameConverter = null;
        private int BadBinding = 0;
    }
}
