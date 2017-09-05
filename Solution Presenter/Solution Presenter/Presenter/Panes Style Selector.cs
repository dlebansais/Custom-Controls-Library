using System.Windows;
using System.Windows.Controls;

namespace CustomControls
{
    public class SolutionPresenterPaneStyleSelector : StyleSelector
    {
        public Style DocumentPaneStyle { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is IDocument)
                return DocumentPaneStyle;

            return base.SelectStyle(item, container);
        }
    }
}
