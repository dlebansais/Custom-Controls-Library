namespace CustomControls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Represents a selector for property entry templates.
    /// </summary>
    public class PropertyEntrySelector : DataTemplateSelector
    {
        /// <summary>
        /// Returns a template.
        /// </summary>
        /// <param name="item">The data object for which to select the template.</param>
        /// <param name="container">The data-bound object.</param>
        /// <returns>The template.</returns>
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
