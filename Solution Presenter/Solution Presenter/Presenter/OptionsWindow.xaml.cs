namespace CustomControls
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Represents the window for options of a solution.
    /// </summary>
    public partial class OptionsWindow : Window
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionsWindow"/> class.
        /// </summary>
        /// <param name="optionPageIndex">The index of the page to display.</param>
        /// <param name="theme">The theme to use.</param>
        /// <param name="saveBeforeCompiling">True if documents must be saved before compiling.</param>
        /// <param name="optionPages">The list of controls for each option category.</param>
        public OptionsWindow(int optionPageIndex, ThemeOption theme, bool saveBeforeCompiling, ICollection<TabItem> optionPages)
        {
            OptionPageIndex = optionPageIndex;
            Theme = theme;
            SaveBeforeCompiling = saveBeforeCompiling;

            InitializeComponent();
            DataContext = this;

            Loaded += OnLoaded;

            List<TabItem> TemplateList = new List<TabItem>();
            BackupTable = new Dictionary<IOptionPageDataContext, IOptionPageDataContext>();

            TemplateList.Add((TabItem)FindResource("PageTheme"));
            TemplateList.Add((TabItem)FindResource("PageCompiler"));
            if (optionPages != null)
            {
                foreach (TabItem Page in optionPages)
                {
                    if (Page.DataContext is IOptionPageDataContext AsOptionPageDataContext)
                        BackupTable.Add(AsOptionPageDataContext, AsOptionPageDataContext.Backup());

                    TemplateList.Add(Page);
                }
            }

            Pages = TemplateList;
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
        /// Gets the table of backup data for each option category.
        /// </summary>
        protected Dictionary<IOptionPageDataContext, IOptionPageDataContext> BackupTable { get; private set; }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the list of controls for option categories.
        /// </summary>
        public ICollection<TabItem> Pages { get; private set; }

        /// <summary>
        /// Gets or sets the selected tab.
        /// </summary>
        public int OptionPageIndex { get; set; }

        /// <summary>
        /// Gets or sets the display theme.
        /// </summary>
        public ThemeOption Theme { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether documents must be saved before compiling.
        /// </summary>
        public bool SaveBeforeCompiling { get; set; }

        /// <summary>
        /// Gets a dummy object.
        /// Don't remove this, it forces the load of the corresponding assembly. Otherwise, an exception is thrown before this window is displayed.
        /// </summary>
        public TightfittingTabControl? UnusedCtrl { get; }
        #endregion

        #region Events
        private void CanOK(object sender, CanExecuteRoutedEventArgs e)
        {
            if (IsCloseAllowed())
                e.CanExecute = true;
        }

        private void OnOK(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CanCancel(object sender, CanExecuteRoutedEventArgs e)
        {
            if (IsCloseAllowed())
                e.CanExecute = true;
        }

        private void OnCancel(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            if (!IsCloseAllowed())
                e.Cancel = true;
            else if (!DialogResult.HasValue || !DialogResult.Value)
            {
                foreach (KeyValuePair<IOptionPageDataContext, IOptionPageDataContext> Entry in BackupTable)
                {
                    IOptionPageDataContext PageDataContext = Entry.Key;
                    IOptionPageDataContext BackupDataContext = Entry.Value;
                    PageDataContext.Restore(BackupDataContext);
                }
            }
        }

        private bool IsCloseAllowed()
        {
            foreach (KeyValuePair<IOptionPageDataContext, IOptionPageDataContext> Entry in BackupTable)
            {
                IOptionPageDataContext PageDataContext = Entry.Key;
                if (!PageDataContext.IsCloseAllowed)
                    return false;
            }

            return true;
        }
        #endregion

        private void OnThemePageLoaded(object sender, RoutedEventArgs e)
        {
        }

        private void OnCompilerPageLoaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
