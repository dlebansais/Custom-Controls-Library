using CustomControls;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Converters
{
    public class StatusTypesToThemeForegroundBrushConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null && values.Length > 1 && (values[0] is StatusType) && (values[1] is IStatusTheme))
            {
                StatusType StatusType = (StatusType)values[0];
                IStatusTheme Theme = (IStatusTheme)values[1];
                if (Theme != null)
                    return Theme.GetForegroundBrush(StatusType);
            }

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
