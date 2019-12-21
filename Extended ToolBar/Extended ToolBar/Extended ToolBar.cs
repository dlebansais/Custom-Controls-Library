using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Xaml;

namespace CustomControls
{
    /// <summary>
    ///     Represents a tool bar with a localized name.
    /// </summary>
    public class ExtendedToolBar : ToolBar
    {
        #region Custom properties and events
        #region ToolBar Name
        /// <summary>
        ///     Identifies the <see cref="ToolBarName"/> dependency property.
        /// </summary>
        /// <returns>
        ///     The identifier for the <see cref="ToolBarName"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty ToolBarNameProperty = DependencyProperty.Register("ToolBarName", typeof(string), typeof(ExtendedToolBar), new FrameworkPropertyMetadata(null));

        /// <summary>
        ///     The localized name of the toolbar. Can be null.
        /// </summary>
        public string ToolBarName
        {
            get { return (string)GetValue(ToolBarNameProperty); }
            set { SetValue(ToolBarNameProperty, value); }
        }
        #endregion
        #endregion

        #region Init
        /// <summary>
        ///     Initializes the <see cref="ExtendedToolBar"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Can't be done inline - too complex")]
        static ExtendedToolBar()
        {
            OverrideAncestorMetadata();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExtendedToolBar"/> class.
        /// </summary>
        public ExtendedToolBar()
        {
            InitializeCheckedButtons();
            InitializeHandlers();
        }
        #endregion

        #region Ancestor Interface
        /// <summary>
        ///     Overrides metadata associated to the ancestor control with new ones associated to this control specifically.
        /// </summary>
        protected static void OverrideAncestorMetadata()
        {
            OverrideMetadataDefaultStyleKey();
        }

        /// <summary>
        ///     Overrides the DefaultStyleKey metadata associated to the ancestor control with a new one associated to this control specifically.
        /// </summary>
        protected static void OverrideMetadataDefaultStyleKey()
        {
            ToolBar.DefaultStyleKeyProperty.OverrideMetadata(typeof(ExtendedToolBar), new FrameworkPropertyMetadata(typeof(ExtendedToolBar)));
        }
        #endregion

        #region Client Interface
        /// <summary>
        ///     Returns all buttons in the toolbar to their default active value.
        /// </summary>
        public virtual void Reset()
        {
            foreach (ExtendedToolBarItem Item in AllButtons)
                Item.Button.ResetIsActive();
        }

        /// <summary>
        ///     Serializes the active state of buttons in the toolbar.
        /// </summary>
        /// <returns>
        ///     A string containing the toolbar state.
        /// </returns>
        public virtual string SerializeActiveButtons()
        {
            try
            {
                bool[] ActiveTable = new bool[AllButtons.Count];
                for (int i = 0; i < AllButtons.Count && i < ActiveTable.Length; i++)
                    ActiveTable[i] = AllButtons[i].Button.IsActive;

                return XamlServices.Save(ActiveTable);
            }
            catch (Exception e)
            {
                if (e.Message == null) // To make the code analyzer happy. Since the doc of XamlServices.Save() doesn't specify any exception, this should safe, right?...
                    throw;

                return "";
            }
        }

        /// <summary>
        ///     Deserializes the active state of buttons in the toolbar.
        /// </summary>
        /// <param name="xamlData">A string containing the new state of the toolbar.</param>
        public virtual void DeserializeActiveButtons(string xamlData)
        {
            if (xamlData == null)
                throw new ArgumentNullException(nameof(xamlData));

            try
            {
                bool[] ActiveTable = (bool[])XamlServices.Parse(xamlData);
                for (int i = 0; i < AllButtons.Count && i < ActiveTable.Length; i++)
                    AllButtons[i].Button.IsActive = ActiveTable[i];
            }
            catch (Exception e)
            {
                if (e.Message == null) // To make the code analyzer happy. Since the doc of XamlServices.Parse() doesn't specify any exception other than NullException, this should safe, right?...
                    throw;
            }
        }
        #endregion

        #region Checked Buttons
        /// <summary>
        ///     Initializes the list of all <see cref="ExtendedToolBarButton"/> objects in the toolbar.
        /// </summary>
        private void InitializeCheckedButtons()
        {
            AllButtons = new ObservableCollection<ExtendedToolBarItem>();
        }

        /// <summary>
        ///     Updates the list of all <see cref="ExtendedToolBarButton"/> objects in the toolbar.
        /// </summary>
        /// <param name="e">This parameter is not used.</param>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            AllButtons.Clear();

            foreach (object Item in Items)
            {
                ExtendedToolBarButton AsExtendedToolBarButton;
                if ((AsExtendedToolBarButton = Item as ExtendedToolBarButton) != null)
                {
                    bool IsCommandGroupEnabled = ExtendedToolBar.IsCommandGroupEnabled(AsExtendedToolBarButton.Command);
                    if (IsCommandGroupEnabled)
                    {
                        ExtendedToolBarItem NewMenuItem = new ExtendedToolBarItem(AsExtendedToolBarButton);
                        AllButtons.Add(NewMenuItem);
                    }
                }
            }
        }

        /// <summary>
        ///     Checks if a command belongs to a group associated to an enabled feature.
        /// </summary>
        /// <param name="command">The <see cref="ICommand"/> command object.</param>
        /// <returns>
        ///     True if the command is not associated to any feature, or if that feature is enabled.
        ///     False if the command belongs to a group associated to a feature, and that feature is disabled.
        /// </returns>
        public static bool IsCommandGroupEnabled(ICommand command)
        {
            ExtendedRoutedCommand AsExtendedCommand;
            if ((AsExtendedCommand = command as ExtendedRoutedCommand) != null)
                if (AsExtendedCommand.CommandGroup != null)
                    return AsExtendedCommand.CommandGroup.IsEnabled;

            return true;
        }

        /// <summary>
        ///     Gets the collection of all <see cref="ExtendedToolBarButton"/> objects in the toolbar.
        /// </summary>
        /// <returns>
        ///     The collection of all <see cref="ExtendedToolBarButton"/> objects in the toolbar.
        /// </returns>
        public ObservableCollection<ExtendedToolBarItem> AllButtons { get; private set; }
        #endregion

        #region Implementation
        private void InitializeHandlers()
        {
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (VisualTreeHelper.GetChildrenCount(this) > 0)
            {
                FrameworkElement FirstChild;
                if ((FirstChild = VisualTreeHelper.GetChild(this, 0) as FrameworkElement) != null)
                {
                    ToggleButton AddRemoveButton;
                    if ((AddRemoveButton = FirstChild.FindName("AddRemoveButton") as ToggleButton) != null)
                        AddRemoveButton.Checked += OnAddRemoveButtonChecked;

                    ToggleButton OverflowButton;
                    if ((OverflowButton = FirstChild.FindName("OverflowButton") as ToggleButton) != null)
                        OverflowButton.Unchecked += OnOverflowButtonUnchecked;

                    MenuItem ResetToolBarMenuItem;
                    if ((ResetToolBarMenuItem = FirstChild.FindName("ResetToolBarMenuItem") as MenuItem) != null)
                        ResetToolBarMenuItem.Click += OnResetToolBarClicked;
                }
            }
        }

        /// <summary>
        ///     Called when the "Add or Remove Button" button is checked.
        /// </summary>
        /// <param name="sender">The button object</param>
        /// <param name="e">This parameter is not used</param>
        protected virtual void OnAddRemoveButtonChecked(object sender, RoutedEventArgs e)
        {
            ToggleButton AddRemoveButton;
            if ((AddRemoveButton = sender as ToggleButton) != null)
                AddRemoveButton.IsEnabled = false;
        }

        /// <summary>
        ///     Called when the toolbar overflow button is unchecked.
        /// </summary>
        /// <param name="sender">The button object</param>
        /// <param name="e">This parameter is not used</param>
        protected virtual void OnOverflowButtonUnchecked(object sender, RoutedEventArgs e)
        {
            ToggleButton OverflowButton;
            if ((OverflowButton = sender as ToggleButton) != null)
            {
                FrameworkElement ParentControl;
                if ((ParentControl = OverflowButton.Parent as FrameworkElement) != null)
                {
                    ToggleButton AddRemoveButton;
                    if ((AddRemoveButton = ParentControl.FindName("AddRemoveButton") as ToggleButton) != null)
                    {
                        AddRemoveButton.IsChecked = false;
                        AddRemoveButton.IsEnabled = true;
                    }
                }
            }
        }

        /// <summary>
        ///     Called when the "Reset ToolBar" button is clicked.
        /// </summary>
        /// <param name="sender">The button object</param>
        /// <param name="e">This parameter is not used</param>
        protected virtual void OnResetToolBarClicked(object sender, RoutedEventArgs e)
        {
            if (IsResetConfirmedByUser())
                Reset();
        }

        /// <summary>
        ///     Called when the user has confirmed or canceled the reset.
        /// </summary>
        /// <returns>
        ///     True if confirmed.
        ///     False if canceled.
        /// </returns>
        private bool IsResetConfirmedByUser()
        {
            string Title = Application.Current.MainWindow.Title;

            string Question;
            if (ToolBarName == null || ToolBarName.Length == 0)
                Question = ExtendedToolBarInternal.Properties.Resources.ConfirmResetThisToolBarQuestion;
            else
            {
                string QuestionFormat = ExtendedToolBarInternal.Properties.Resources.ConfirmResetToolBarQuestion;
                Question = string.Format(CultureInfo.CurrentCulture, QuestionFormat, ToolBarName);
            }

            MessageBoxResult Result = MessageBox.Show(Question, Title, MessageBoxButton.YesNo, MessageBoxImage.Question);
            return (Result == MessageBoxResult.Yes);
        }
        #endregion
    }
}
