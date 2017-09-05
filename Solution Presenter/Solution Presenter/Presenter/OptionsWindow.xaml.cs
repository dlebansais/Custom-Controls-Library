using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace CustomControls
{
    public partial class OptionsWindow : Window
    {
        #region Init
        public OptionsWindow(int optionPageIndex, ThemeOption theme, bool saveBeforeCompiling, ICollection<TabItem> optionPages)
        {
            this.OptionPageIndex = optionPageIndex;
            this.Theme = theme;
            this.SaveBeforeCompiling = saveBeforeCompiling;
            this.UnusedCtrl = null;

            InitializeComponent();
            DataContext = this;

            Loaded += OnLoaded;

            List<TabItem> TemplateList = new List<TabItem>();
            BackupTable = new Dictionary<IOptionPageDataContext, IOptionPageDataContext>();
            
            TemplateList.Add(FindResource("PageTheme") as TabItem);
            TemplateList.Add(FindResource("PageCompiler") as TabItem);
            if (optionPages != null)
            {
                foreach (TabItem Page in optionPages)
                {
                    IOptionPageDataContext AsOptionPageDataContext;
                    if ((AsOptionPageDataContext = Page.DataContext as IOptionPageDataContext) != null)
                        BackupTable.Add(AsOptionPageDataContext, AsOptionPageDataContext.Backup());

                    TemplateList.Add(Page);
                }
            }

            Pages = TemplateList;
        }

        protected virtual void OnLoaded(object sender, EventArgs e)
        {
            Title = Owner.Title;
            Icon = Owner.Icon;
        }

        protected Dictionary<IOptionPageDataContext, IOptionPageDataContext> BackupTable { get; private set; }
        #endregion

        #region Properties
        public ICollection<TabItem> Pages { get; private set; }
        public int OptionPageIndex { get; set; }
        public ThemeOption Theme { get; set; }
        public bool SaveBeforeCompiling { get; set; }
        public TightfittingTabControl UnusedCtrl { get; private set; } // Don't remove this, it forces the load of the corresponding assembly. Otherwise, an exception is thrown before this window is displayed.
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
