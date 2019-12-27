using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CustomControls
{
    public class TreeViewMarginConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null && values.Length > 3 && (values[0] is int) && (values[1] is bool) && (values[2] is double) && (values[3] is double))
            {
                int Level = (int)values[0];
                bool IsRootAlwaysExpanded = (bool)values[1];
                double IndentationWidth = (double)values[2];
                double ExpandButtonWidth = (double)values[3];

                double LeftMargin = Level * IndentationWidth;
                if (IsRootAlwaysExpanded && Level > 0)
                    LeftMargin -= ExpandButtonWidth;

                //Debug.Print("Margin: " + Math.Round(LeftMargin, 1));
                return new Thickness(LeftMargin, 0, 0, 0);
            }

            return new Thickness(0);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return Array.Empty<object>();
        }
    }
}
