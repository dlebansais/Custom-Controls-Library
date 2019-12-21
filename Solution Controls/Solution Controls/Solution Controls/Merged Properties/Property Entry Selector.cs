using System;
using System.Windows;
using System.Windows.Controls;

namespace CustomControls
{
    public class PropertyEntrySelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            FrameworkElement ControlContainer = (FrameworkElement)container;
            DataTemplate Result;

            if (item is IStringPropertyEntry)
                Result = ControlContainer.FindResource("StringPropertyKey") as DataTemplate;

            else if (item is IEnumPropertyEntry)
                Result = ControlContainer.FindResource("EnumPropertyKey") as DataTemplate;

            else
                Result = null;

            return Result;
        }
    }
}
