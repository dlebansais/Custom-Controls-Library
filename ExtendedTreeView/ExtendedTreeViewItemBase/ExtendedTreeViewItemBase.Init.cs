namespace CustomControls
{
    using System.ComponentModel;
    using System.Windows.Controls;

    /// <summary>
    /// Represents an item in a tree view control.
    /// </summary>
    public partial class ExtendedTreeViewItemBase : ContentControl, INotifyPropertyChanged
    {
        static ExtendedTreeViewItemBase()
        {
            OverrideAncestorMetadata();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedTreeViewItemBase"/> class.
        /// </summary>
        /// <param name="host">The tree to which this item belongs.</param>
        public ExtendedTreeViewItemBase(ExtendedTreeViewBase host)
        {
            Host = host;

            object DefaultStyle = TryFindResource(typeof(ExtendedTreeViewItemBase));
            if (DefaultStyle == null)
                Resources.MergedDictionaries.Add(SharedResourceDictionaryManager.SharedDictionary);
        }

        /// <summary>
        /// Gets the tree to which this item belongs.
        /// </summary>
        protected ExtendedTreeViewBase Host { get; }
    }
}
