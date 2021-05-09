namespace ExtendedTreeViewDemo
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Media.Animation;
    using System.Windows.Threading;
    using CustomControls;

    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            IsGenerating = false;
            GenerateCount = 0;
            GenerateProgress = 0.0;

            Collection<ResourceDictionary> MergedDictionaries = Application.Current.Resources.MergedDictionaries;
            VS2013Dictionnary = MergedDictionaries[0];
            ExplorerDictionnary = MergedDictionaries[1];
            CustomFixedHeightImageDictionnary = MergedDictionaries[2];
            CustomTextImageDictionnary = MergedDictionaries[3];
            CustomVariableHeightImageAndTextDictionnary = MergedDictionaries[4];
            MergedDictionaries.Clear();
        }

        public bool IsGenerating { get; private set; }
        public int GenerateCount { get; private set; }
        public double GenerateProgress { get; private set; }
        private ResourceDictionary VS2013Dictionnary;
        private ResourceDictionary ExplorerDictionnary;
        private ResourceDictionary CustomFixedHeightImageDictionnary;
        private ResourceDictionary CustomTextImageDictionnary;
        private ResourceDictionary CustomVariableHeightImageAndTextDictionnary;

        protected virtual void UpdateProgress(int count, int total)
        {
            GenerateCount = count;
            GenerateProgress = total > 0 ? Math.Round((double)count / (double)total, 2) : 0;

            NotifyPropertyChanged(nameof(GenerateCount));
            NotifyPropertyChanged(nameof(GenerateProgress));
        }

        protected virtual void OnNewTreeView(object sender, ExecutedRoutedEventArgs e)
        {
            TreeViewSettingsWindow Dlg = new TreeViewSettingsWindow();
            Dlg.Owner = this;
            Dlg.ShowDialog();

            if (Dlg.DialogResult.HasValue && Dlg.DialogResult.Value)
                GenerateTreeView();
        }

        protected virtual void OnExit(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        protected virtual void GenerateTreeView()
        {
            IsGenerating = true;
            NotifyPropertyChanged(nameof(IsGenerating));

            panelMain.Children.Clear();
            UpdateProgress(0, 0);

            ExtendedTreeView treeviewSample = new ExtendedTreeView();
            treeviewSample.SetValue(ScrollViewer.IsDeferredScrollingEnabledProperty, TreeViewSettingsWindow.IsDeferredScrollingEnabled);
            treeviewSample.SetValue(VirtualizingPanel.IsVirtualizingProperty, TreeViewSettingsWindow.IsVirtualizing);
            treeviewSample.SetValue(VirtualizingPanel.VirtualizationModeProperty, TreeViewSettingsWindow.VirtualizationMode);
            treeviewSample.SelectionMode = TreeViewSettingsWindow.SelectionMode;
            treeviewSample.AllowDragDrop = TreeViewSettingsWindow.AllowDragDrop;
            treeviewSample.IsRootAlwaysExpanded = TreeViewSettingsWindow.IsRootAlwaysExpanded;
            treeviewSample.IsItemExpandedAtStart = TreeViewSettingsWindow.IsItemExpandedAtStart;
            treeviewSample.Content = GenerateRoot(TreeViewSettingsWindow.IsCloneable);
            treeviewSample.DropCheck += OnDropCheck;

            Collection<ResourceDictionary> MergedDictionaries = Application.Current.Resources.MergedDictionaries;
            MergedDictionaries.Clear();

            bool IsHandled = false;

            switch (TreeViewSettingsWindow.TreeViewType)
            {
                case TreeViewType.VS2013:
                    MergedDictionaries.Add(VS2013Dictionnary);
                    treeviewSample.ExpandButtonStyle = (Style)FindResource("VS2013ExpandButtonStyle");
                    treeviewSample.ExpandButtonWidth = 16;
                    treeviewSample.IndentationWidth = 16;
                    IsHandled = true;
                    break;

                case TreeViewType.Explorer:
                    MergedDictionaries.Add(ExplorerDictionnary);
                    ButtonList.Clear();
                    Style ButtonStyle = (Style)FindResource("ExplorerExpandButtonStyle");
                    Style NewButtonStyle = new Style(typeof(ToggleButton), ButtonStyle);
                    NewButtonStyle.Setters.Add(new EventSetter(ToggleButton.LoadedEvent, new RoutedEventHandler(OnButtonLoaded)));
                    NewButtonStyle.Setters.Add(new EventSetter(ToggleButton.UnloadedEvent, new RoutedEventHandler(OnButtonUnloaded)));
                    treeviewSample.ExpandButtonStyle = NewButtonStyle;
                    treeviewSample.ExpandButtonWidth = 12;
                    treeviewSample.IndentationWidth = 8;
                    treeviewSample.MouseEnter += OnTreeViewMouseEnter;
                    treeviewSample.MouseLeave += OnTreeViewMouseLeave;
                    IsHandled = true;
                    break;

                case TreeViewType.Custom:
                    switch (TreeViewSettingsWindow.TreeViewItemType)
                    {
                        case TreeViewItemType.FixedHeightImage:
                            MergedDictionaries.Add(CustomFixedHeightImageDictionnary);
                            IsHandled = true;
                            break;

                        case TreeViewItemType.Text:
                            MergedDictionaries.Add(CustomTextImageDictionnary);
                            IsHandled = true;
                            break;

                        case TreeViewItemType.VariableHeightImageAndText:
                            MergedDictionaries.Add(CustomVariableHeightImageAndTextDictionnary);
                            IsHandled = true;
                            break;
                    }

                    break;
            }

            Debug.Assert(IsHandled);

            panelMain.Children.Add(treeviewSample);
        }

        protected virtual IExtendedTreeNode GenerateRoot(bool isCloneable)
        {
            int ItemCount = TreeViewSettingsWindow.ItemCount;
            int AverageChildrenCount = TreeViewSettingsWindow.AverageChildrenCount;
            Random RNG = new Random(0);
            Collection<IExtendedTreeNode> ParentChain = new Collection<IExtendedTreeNode>();
            IExtendedTreeNode Root = CreateRootNode(isCloneable);
            ParentChain.Add(Root);

            Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new GenerateNextHandler(GenerateNext), isCloneable, ItemCount, AverageChildrenCount, RNG, Root, 1, ParentChain, 0, null, 0, 0);

            return Root;
        }

        protected virtual IExtendedTreeNode CreateRootNode(bool isCloneable)
        {
            if (isCloneable)
                return new CloneableRootTestNode();
            else
                return new RootTestNode();
        }

        protected virtual IExtendedTreeNode CreateNode(bool isCloneable, IExtendedTreeNode parent, int generated)
        {
            if (isCloneable)
                return new CloneableTestNode((CloneableTestNode)parent, generated);
            else
                return new TestNode((TestNode)parent, generated);
        }

        protected delegate void GenerateNextHandler(bool isCloneable, int itemCount, int averageChildrenCount, Random randomNumberGenerator, IExtendedTreeNode root, int generated, Collection<IExtendedTreeNode> parentChain, int parentIndex, IExtendedTreeNode? parentTreeNode, int childIndex, int childrenCount);
        protected virtual void GenerateNext(bool isCloneable, int itemCount, int averageChildrenCount, Random randomNumberGenerator, IExtendedTreeNode root, int generated, Collection<IExtendedTreeNode> parentChain, int parentIndex, IExtendedTreeNode? parentTreeNode, int childIndex, int childrenCount)
        {
            if (randomNumberGenerator == null || parentChain == null)
                return;

            int ThisPassCount = 0;
            int MaxPassCount = TreeViewSettingsWindow.IsItemExpandedAtStart ? 100 : 1000;

            while (generated < itemCount && ThisPassCount++ < MaxPassCount)
            {
                if (childIndex >= childrenCount)
                {
                    parentIndex = randomNumberGenerator.Next(parentChain.Count);
                    parentTreeNode = parentChain[parentIndex];
                    childIndex = 0;
                    childrenCount = randomNumberGenerator.Next(averageChildrenCount) + 1;
                }

                IExtendedTreeNode NewItem = CreateNode(isCloneable, parentTreeNode !, generated);
                parentTreeNode?.Children.Add(NewItem);

                if (childIndex == 0)
                    parentChain[parentIndex] = NewItem;
                else
                    parentChain.Add(NewItem);

                generated++;
                childIndex++;
            }

            UpdateProgress(generated, itemCount);
            if (IsGenerating && generated < itemCount)
                Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new GenerateNextHandler(GenerateNext), isCloneable, itemCount, averageChildrenCount, randomNumberGenerator, root, generated, parentChain, parentIndex, parentTreeNode, childIndex, childrenCount);
            else
            {
                IsGenerating = false;
                NotifyPropertyChanged(nameof(IsGenerating));
            }
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            IsGenerating = false;
            NotifyPropertyChanged(nameof(IsGenerating));
        }

        protected virtual void OnDropCheck(object sender, RoutedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            DropCheckEventArgs Args = (DropCheckEventArgs)e;
            IExtendedTreeNode DropDestinationItem = (IExtendedTreeNode)Args.DropDestinationItem;

            if (DropDestinationItem != null && ((ICollection<IExtendedTreeNode>)DropDestinationItem.Children).Count == 0 && TreeViewSettingsWindow.AreLeavesSealed)
                Args.Deny();
        }

        protected virtual void OnTreeViewMouseEnter(object sender, MouseEventArgs e)
        {
            Storyboard res = (Storyboard)FindResource("ShowButton");
            foreach (FrameworkElement Button in ButtonList)
                Button.BeginStoryboard(res);
        }

        protected virtual void OnTreeViewMouseLeave(object sender, MouseEventArgs e)
        {
            Storyboard res = (Storyboard)FindResource("HideButton");
            foreach (FrameworkElement Button in ButtonList)
                Button.BeginStoryboard(res);
        }

        protected virtual void OnButtonLoaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement ElementSender = (FrameworkElement)sender;
            ButtonList.Add(ElementSender);
        }

        protected virtual void OnButtonUnloaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement ElementSender = (FrameworkElement)sender;
            ButtonList.Remove(ElementSender);
        }

        private List<FrameworkElement> ButtonList = new List<FrameworkElement>();

        #region Implementation of INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameter is mandatory with [CallerMemberName]")]
        public void NotifyThisPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
