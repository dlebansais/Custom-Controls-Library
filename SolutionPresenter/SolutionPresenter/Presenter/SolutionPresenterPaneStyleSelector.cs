namespace CustomControls
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Represents a selector for solution presenter panes.
    /// </summary>
    public class SolutionPresenterPaneStyleSelector : StyleSelector
    {
        /// <summary>
        /// Gets or sets the pane style.
        /// </summary>
        public Style DocumentPaneStyle { get; set; } = new Style();

        /// <summary>
        /// Selects a style.
        /// </summary>
        /// <param name="item">The item for which to select the style.</param>
        /// <param name="container">The item container.</param>
        /// <returns>The selected style.</returns>
        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is IDocument)
                return DocumentPaneStyle;

            return base.SelectStyle(item, container);
        }
    }
}
