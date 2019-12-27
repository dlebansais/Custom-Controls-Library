using System;
using System.Windows;
using System.Windows.Controls;

namespace CustomControls
{
    public class PropertyEntrySelector : DataTemplateSelector
    {
        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            FrameworkElement ControlContainer = (FrameworkElement)container;
            DataTemplate? Result = null;

            switch (item)
            {
                case IStringPropertyEntry AsStringPropertyEntry:
                    Result = ControlContainer.FindResource("StringPropertyKey") as DataTemplate;
                    break;

                case IEnumPropertyEntry AsEnumPropertyEntry:
                    Result = ControlContainer.FindResource("EnumPropertyKey") as DataTemplate;
                    break;
            }

            return Result;
        }
    }
}
