namespace ExtendedTreeView.Demo;

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
using Contracts;
using CustomControls;

/// <summary>
/// Interaction logic for MainWindow.xaml.
/// </summary>
public partial class MainWindow : Window, INotifyPropertyChanged
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
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

    /// <summary>
    /// Gets a value indicating whether the application is generating nodes.
    /// </summary>
    public bool IsGenerating { get; private set; }

    /// <summary>
    /// Gets the count of generated nodes.
    /// </summary>
    public int GenerateCount { get; private set; }

    /// <summary>
    /// Gets the progress in generating nodes.
    /// </summary>
    public double GenerateProgress { get; private set; }

    private readonly ResourceDictionary VS2013Dictionnary;
    private readonly ResourceDictionary ExplorerDictionnary;
    private readonly ResourceDictionary CustomFixedHeightImageDictionnary;
    private readonly ResourceDictionary CustomTextImageDictionnary;
    private readonly ResourceDictionary CustomVariableHeightImageAndTextDictionnary;

    /// <summary>
    /// Updates the generation progress.
    /// </summary>
    /// <param name="count">The current count.</param>
    /// <param name="total">The total count.</param>
    protected virtual void UpdateProgress(int count, int total)
    {
        GenerateCount = count;
        GenerateProgress = total > 0 ? Math.Round(count / (double)total, 2) : 0;

        NotifyPropertyChanged(nameof(GenerateCount));
        NotifyPropertyChanged(nameof(GenerateProgress));
    }

    /// <summary>
    /// Handles the creation of a new tree view.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="args">Ignored.</param>
    protected virtual void OnNewTreeView(object sender, ExecutedRoutedEventArgs args)
    {
        TreeViewSettingsWindow Dlg = new();
        Dlg.Owner = this;
        _ = Dlg.ShowDialog();

        if (Dlg.DialogResult.HasValue && Dlg.DialogResult.Value)
            GenerateTreeView();
    }

    /// <summary>
    /// Handles the exit event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="args">Ignored.</param>
    protected virtual void OnExit(object sender, ExecutedRoutedEventArgs args)
    {
        Close();
    }

    /// <summary>
    /// Generates a tree view.
    /// </summary>
    protected virtual void GenerateTreeView()
    {
        IsGenerating = true;
        NotifyPropertyChanged(nameof(IsGenerating));

        panelMain.Children.Clear();
        UpdateProgress(0, 0);

        ExtendedTreeView TreeviewSample = new();
        TreeviewSample.SetValue(ScrollViewer.IsDeferredScrollingEnabledProperty, TreeViewSettingsWindow.IsDeferredScrollingEnabled);
        TreeviewSample.SetValue(VirtualizingPanel.IsVirtualizingProperty, TreeViewSettingsWindow.IsVirtualizing);
        TreeviewSample.SetValue(VirtualizingPanel.VirtualizationModeProperty, TreeViewSettingsWindow.VirtualizationMode);
        TreeviewSample.SelectionMode = TreeViewSettingsWindow.SelectionMode;
        TreeviewSample.AllowDragDrop = TreeViewSettingsWindow.AllowDragDrop;
        TreeviewSample.IsRootAlwaysExpanded = TreeViewSettingsWindow.IsRootAlwaysExpanded;
        TreeviewSample.IsItemExpandedAtStart = TreeViewSettingsWindow.IsItemExpandedAtStart;
        TreeviewSample.Content = GenerateRoot(TreeViewSettingsWindow.IsCloneable);
        TreeviewSample.DropCheck += OnDropCheck;

        Collection<ResourceDictionary> MergedDictionaries = Application.Current.Resources.MergedDictionaries;
        MergedDictionaries.Clear();

        bool IsHandled = false;

        switch (TreeViewSettingsWindow.TreeViewType)
        {
            case TreeViewType.VS2013:
                MergedDictionaries.Add(VS2013Dictionnary);
                TreeviewSample.ExpandButtonStyle = (Style)FindResource("VS2013ExpandButtonStyle");
                TreeviewSample.ExpandButtonWidth = 16;
                TreeviewSample.IndentationWidth = 16;
                IsHandled = true;
                break;

            case TreeViewType.Explorer:
                MergedDictionaries.Add(ExplorerDictionnary);
                ButtonList.Clear();
                Style ButtonStyle = (Style)FindResource("ExplorerExpandButtonStyle");
                Style NewButtonStyle = new(typeof(ToggleButton), ButtonStyle);
                NewButtonStyle.Setters.Add(new EventSetter(LoadedEvent, new RoutedEventHandler(OnButtonLoaded)));
                NewButtonStyle.Setters.Add(new EventSetter(UnloadedEvent, new RoutedEventHandler(OnButtonUnloaded)));
                TreeviewSample.ExpandButtonStyle = NewButtonStyle;
                TreeviewSample.ExpandButtonWidth = 12;
                TreeviewSample.IndentationWidth = 8;
                TreeviewSample.MouseEnter += OnTreeViewMouseEnter;
                TreeviewSample.MouseLeave += OnTreeViewMouseLeave;
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

        _ = panelMain.Children.Add(TreeviewSample);
    }

    /// <summary>
    /// Generates a tree root.
    /// </summary>
    /// <param name="isCloneable">Indicates whether nodes can be cloned.</param>
    protected virtual IExtendedTreeNode GenerateRoot(bool isCloneable)
    {
        int ItemCount = TreeViewSettingsWindow.ItemCount;
        int AverageChildrenCount = TreeViewSettingsWindow.AverageChildrenCount;
        RandomNumberGenerator RNG = new() { Seed = 0 };
        Collection<IExtendedTreeNode> ParentChain = new();
        IExtendedTreeNode Root = CreateRootNode(isCloneable);
        ParentChain.Add(Root);

        _ = Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action<bool, int, int, RandomNumberGenerator, IExtendedTreeNode, int, Collection<IExtendedTreeNode>, int, IExtendedTreeNode?, int, int>(GenerateNext), isCloneable, ItemCount, AverageChildrenCount, RNG, Root, 1, ParentChain, 0, null, 0, 0);

        return Root;
    }

    /// <summary>
    /// Creates a root node.
    /// </summary>
    /// <param name="isCloneable">Indicates whether the node can be cloned.</param>
    protected virtual IExtendedTreeNode CreateRootNode(bool isCloneable)
    {
        if (isCloneable)
            return new CloneableRootTestNode();
        else
            return new RootTestNode();
    }

    /// <summary>
    /// Creates a new node.
    /// </summary>
    /// <param name="isCloneable">Indicates whether the node can be cloned.</param>
    /// <param name="parent">The parent node.</param>
    /// <param name="generated">A generated value associated to the node.</param>
    protected virtual IExtendedTreeNode CreateNode(bool isCloneable, IExtendedTreeNode parent, int generated)
    {
        if (isCloneable)
            return new CloneableTestNode((CloneableTestNode)parent, generated);
        else
            return new TestNode((TestNode)parent, generated);
    }

    /// <summary>
    /// Generates the next node.
    /// </summary>
    /// <param name="isCloneable">Indicates whether the node can be cloned.</param>
    /// <param name="itemCount">The node cunt so far.</param>
    /// <param name="averageChildrenCount">The average number of child nodes.</param>
    /// <param name="randomNumberGenerator">The number generator to use.</param>
    /// <param name="root">The root.</param>
    /// <param name="generated">The generated number.</param>
    /// <param name="parentChain">The chain of parents.</param>
    /// <param name="parentIndex">The parent idnex.</param>
    /// <param name="parentTreeNode">The parent tree node.</param>
    /// <param name="childIndex">The child index.</param>
    /// <param name="childrenCount">The count of children.</param>
    protected virtual void GenerateNext(bool isCloneable, int itemCount, int averageChildrenCount, RandomNumberGenerator randomNumberGenerator, IExtendedTreeNode root, int generated, Collection<IExtendedTreeNode> parentChain, int parentIndex, IExtendedTreeNode? parentTreeNode, int childIndex, int childrenCount)
    {
        if (randomNumberGenerator is null || parentChain is null)
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
            _ = Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action<bool, int, int, RandomNumberGenerator, IExtendedTreeNode, int, Collection<IExtendedTreeNode>, int, IExtendedTreeNode?, int, int>(GenerateNext), isCloneable, itemCount, averageChildrenCount, randomNumberGenerator, root, generated, parentChain, parentIndex, parentTreeNode, childIndex, childrenCount);
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

    /// <summary>
    /// Handles drop.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="args">Arguments.</param>
    [Access("protected", "virtual")]
    [RequireNotNull(nameof(args), Type = "RoutedEventArgs")]
    private void OnDropCheckVerified(object sender, DropCheckEventArgs args)
    {
        IExtendedTreeNode DropDestinationItem = (IExtendedTreeNode)args.DropDestinationItem;

        if (DropDestinationItem is not null && ((ICollection<IExtendedTreeNode>)DropDestinationItem.Children).Count == 0 && TreeViewSettingsWindow.AreLeavesSealed)
            args.Deny();
    }

    /// <summary>
    /// Handles mouse enter in tree view.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="args">Arguments.</param>
    protected virtual void OnTreeViewMouseEnter(object sender, MouseEventArgs args)
    {
        Storyboard Resource = (Storyboard)FindResource("ShowButton");
        foreach (FrameworkElement Button in ButtonList)
            Button.BeginStoryboard(Resource);
    }

    /// <summary>
    /// Handles mouse leave in tree view.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="args">Arguments.</param>
    protected virtual void OnTreeViewMouseLeave(object sender, MouseEventArgs args)
    {
        Storyboard Resource = (Storyboard)FindResource("HideButton");
        foreach (FrameworkElement Button in ButtonList)
            Button.BeginStoryboard(Resource);
    }

    /// <summary>
    /// Handles click on the Loaded button.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="args">Arguments.</param>
    protected virtual void OnButtonLoaded(object sender, RoutedEventArgs args)
    {
        FrameworkElement ElementSender = (FrameworkElement)sender;
        ButtonList.Add(ElementSender);
    }

    /// <summary>
    /// Handles click on the Unloaded button.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="args">Arguments.</param>
    protected virtual void OnButtonUnloaded(object sender, RoutedEventArgs args)
    {
        FrameworkElement ElementSender = (FrameworkElement)sender;
        _ = ButtonList.Remove(ElementSender);
    }

    private readonly List<FrameworkElement> ButtonList = new();

    #region Implementation of INotifyPropertyChanged
    /// <summary>
    /// The PropertyChanged event.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Notifies that a property is changed.
    /// </summary>
    /// <param name="propertyName">The property name.</param>
    public void NotifyPropertyChanged(string propertyName)
    {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
#pragma warning restore CS8602 // Dereference of a possibly null reference.
    }
    #endregion
}
