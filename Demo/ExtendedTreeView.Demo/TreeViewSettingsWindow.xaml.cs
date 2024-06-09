namespace ExtendedTreeView.Demo;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;

/// <summary>
/// Interaction logic for TreeViewSettingsWindow.xaml.
/// </summary>
public partial class TreeViewSettingsWindow : Window
{
    #region Constants
    private static readonly IReadOnlyDictionary<TreeViewSize, int> SizeTable = new Dictionary<TreeViewSize, int>()
    {
        { TreeViewSize.Small, 10 },
        { TreeViewSize.Medium, 10000 },
        { TreeViewSize.Large, 10000000 },
    };
    private static readonly IReadOnlyDictionary<TreeViewDepth, int> AverageChildrenCountTable = new Dictionary<TreeViewDepth, int>()
    {
        { TreeViewDepth.Small, 5 },
        { TreeViewDepth.Medium, 500 },
        { TreeViewDepth.Large, 50000 },
    };
    #endregion

    #region Init
    static TreeViewSettingsWindow()
    {
        IsDeferredScrollingEnabled = true;
        IsVirtualizing = true;
        VirtualizationMode = VirtualizationMode.Standard;
        SelectionMode = SelectionMode.Extended;
        AllowDragDrop = true;
        TreeViewType = TreeViewType.VS2013;
        TreeViewSize = TreeViewSize.Small;
        TreeViewDepth = TreeViewDepth.Small;
        TreeViewItemType = TreeViewItemType.FixedHeightImage;
        IsRootAlwaysExpanded = true;
        IsItemExpandedAtStart = false;
        IsCloneable = true;
        AreLeavesSealed = true;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TreeViewSettingsWindow"/> class.
    /// </summary>
    public TreeViewSettingsWindow()
    {
        InitializeComponent();
        DataContext = this;
    }
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets a value indicating whether scrolling is deferred.
    /// </summary>
    public static bool IsDeferredScrollingEnabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether virtualizing is enabled.
    /// </summary>
    public static bool IsVirtualizing { get; set; }

    /// <summary>
    /// Gets or sets the virtualization mode.
    /// </summary>
    public static VirtualizationMode VirtualizationMode { get; set; }

    /// <summary>
    /// Gets or sets the selection mode.
    /// </summary>
    public static SelectionMode SelectionMode { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether Drag and Drop is allowed.
    /// </summary>
    public static bool AllowDragDrop { get; set; }

    /// <summary>
    /// Gets or sets the tree view type.
    /// </summary>
    public static TreeViewType TreeViewType { get; set; }

    /// <summary>
    /// Gets or sets the tree view size.
    /// </summary>
    public static TreeViewSize TreeViewSize { get; set; }

    /// <summary>
    /// Gets or sets the tree view depth.
    /// </summary>
    public static TreeViewDepth TreeViewDepth { get; set; }

    /// <summary>
    /// Gets or sets the tree view item type.
    /// </summary>
    public static TreeViewItemType TreeViewItemType { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether root is always expanded.
    /// </summary>
    public static bool IsRootAlwaysExpanded { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether items are expanded at start..
    /// </summary>
    public static bool IsItemExpandedAtStart { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the tree view is cloneable.
    /// </summary>
    public static bool IsCloneable { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether leaves are sealed.
    /// </summary>
    public static bool AreLeavesSealed { get; set; }

    /// <summary>
    /// Gets the count of items.
    /// </summary>
    public static int ItemCount { get { return SizeTable[TreeViewSize]; } }

    /// <summary>
    /// Gets the average children count.
    /// </summary>
    public static int AverageChildrenCount { get { return AverageChildrenCountTable[TreeViewDepth]; } }
    #endregion

    #region Implementation
    /// <summary>
    /// Handles the OK command.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="args">Arguments.</param>
    protected virtual void OnOk(object sender, RoutedEventArgs args)
    {
        MessageBoxResult Result = MessageBoxResult.OK;

        if (TreeViewSize == TreeViewSize.Large)
        {
            if (!IsVirtualizing)
                Result = MessageBox.Show("Generating a large tree view with virtualization off is a slow operation. Continue?", Title, MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            else if (TreeViewType == TreeViewType.Custom && TreeViewItemType == TreeViewItemType.VariableHeightImageAndText)
                Result = MessageBox.Show("Generating a large tree view with variable height items is a slow operation. Continue?", Title, MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            else if (IsItemExpandedAtStart)
                Result = MessageBox.Show("Generating a large tree view with all items expanded is a slow operation. Continue?", Title, MessageBoxButton.OKCancel, MessageBoxImage.Warning);
        }
        else if (TreeViewSize == TreeViewSize.Medium && IsItemExpandedAtStart)
        {
            if (!IsVirtualizing)
                Result = MessageBox.Show("Generating a tree view with all items expanded and no virtualization is a slow operation. Continue?", Title, MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            else if (TreeViewType == TreeViewType.Custom && TreeViewItemType == TreeViewItemType.VariableHeightImageAndText)
                Result = MessageBox.Show("Generating a tree view with variable height items and all of them expanded is a slow operation. Continue?", Title, MessageBoxButton.OKCancel, MessageBoxImage.Warning);
        }

        if (Result == MessageBoxResult.OK)
        {
            DialogResult = true;
            Close();
        }
    }
    #endregion
}
