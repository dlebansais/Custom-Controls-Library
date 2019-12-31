namespace CustomControls
{
    using System.Windows;
    using System.Windows.Controls;

    public class SolutionPresenterPaneStyleSelector : StyleSelector
    {
        public Style DocumentPaneStyle { get; set; } = new Style();

        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is IDocument)
                return DocumentPaneStyle;

            return base.SelectStyle(item, container);
        }
    }
}
