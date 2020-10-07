using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;

namespace ExtendedTreeViewDemo
{
    public partial class TreeViewSettingsWindow : Window
    {
        #region Constants
        private static IReadOnlyDictionary<TreeViewSize, int> SizeTable = new Dictionary<TreeViewSize, int>()
        {
            { TreeViewSize.Small, 10 },
            { TreeViewSize.Medium, 10000 },
            { TreeViewSize.Large, 10000000 }
        };
        private static IReadOnlyDictionary<TreeViewDepth, int> AverageChildrenCountTable = new Dictionary<TreeViewDepth, int>()
        {
            { TreeViewDepth.Small, 5 },
            { TreeViewDepth.Medium, 500},
            { TreeViewDepth.Large, 50000 }
        };
        #endregion

        #region Init
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
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

        public TreeViewSettingsWindow()
        {
            InitializeComponent();
            DataContext = this;
        }
        #endregion

        #region Properties
        public static bool IsDeferredScrollingEnabled { get; set; }
        public static bool IsVirtualizing { get; set; }
        public static VirtualizationMode VirtualizationMode { get; set; }
        public static SelectionMode SelectionMode { get; set; }
        public static bool AllowDragDrop { get; set; }
        public static TreeViewType TreeViewType { get; set; }
        public static TreeViewSize TreeViewSize { get; set; }
        public static TreeViewDepth TreeViewDepth { get; set; }
        public static TreeViewItemType TreeViewItemType { get; set; }
        public static bool IsRootAlwaysExpanded { get; set; }
        public static bool IsItemExpandedAtStart { get; set; }
        public static bool IsCloneable { get; set; }
        public static bool AreLeavesSealed { get; set; }
        public static int ItemCount { get { return SizeTable[TreeViewSize]; } }
        public static int AverageChildrenCount { get { return AverageChildrenCountTable[TreeViewDepth]; } }
        #endregion

        #region Implementation
        protected virtual void OnOk(object sender, RoutedEventArgs e)
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
}
