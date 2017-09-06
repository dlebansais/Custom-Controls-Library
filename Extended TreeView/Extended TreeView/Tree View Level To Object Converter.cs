using System;
using System.Globalization;
using System.Windows.Data;

namespace CustomControls
{
    public class TreeViewLevelToObjectConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            CompositeCollection CollectionOfItems = parameter as CompositeCollection;

            if (values != null && values.Length > 2 && (values[0] is int) && (values[1] is bool) && (values[2] is int))
            {
                int Level = (int)values[0];
                bool IsRootAlwaysExpanded = (bool)values[1];
                int ChildCount = (int)values[2];

                //Debug.Print("Level: " + Level);

                if (Level == 0 && IsRootAlwaysExpanded)
                    return CollectionOfItems[0];

                else if (ChildCount == 0)
                    return CollectionOfItems[1];
            }

            return CollectionOfItems[2];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
