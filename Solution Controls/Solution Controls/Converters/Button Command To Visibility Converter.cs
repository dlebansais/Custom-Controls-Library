using CustomControls;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Converters
{
    public class ButtonCommandToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null && values.Length > 1 && (values[0] is ICommand) && (values[1] is bool))
            {
                ICommand Command = (ICommand)values[0];
                bool CanShow = (bool)values[1];
                IDocument ActiveDocument = (values.Length > 2) ? (IDocument)values[2] : null;

                bool IsVisible = CanShow && GetItemVisibility(Command, ActiveDocument);
                return IsVisible ? Visibility.Visible : Visibility.Collapsed;
            }

            return null;
        }

        protected virtual bool GetItemVisibility(ICommand command, object activeDocument)
        {
            ExtendedRoutedCommand AsExtendedCommand;
            if ((AsExtendedCommand = command as ExtendedRoutedCommand) != null)
            {
                if (AsExtendedCommand.CommandGroup != null)
                    if (!AsExtendedCommand.CommandGroup.IsEnabled)
                        return false;
            }

            if (command is ActiveDocumentRoutedCommand)
            {
                if (activeDocument == null)
                    return true;
            }

            return true;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
