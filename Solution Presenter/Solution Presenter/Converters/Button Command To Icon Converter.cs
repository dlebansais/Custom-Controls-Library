using CustomControls;
using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Converters
{
    [ValueConversion(typeof(ICommand), typeof(Image))]
    public class ButtonCommandToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ICommand AsCommand)
                return GetItemIcon(AsCommand);
            else
                throw new ArgumentOutOfRangeException(nameof(value));
        }

        protected virtual Image GetItemIcon(ICommand command)
        {
            Image ItemIcon;

            switch (command)
            {
                case ExtendedRoutedCommand AsExtendedCommand:
                    Image Icon = new Image() { Source = AsExtendedCommand.ImageSource, Width = 16.0, Height = 16.0 };
                    ItemIcon = Icon;
                    break;

                case RoutedUICommand AsUICommand:
                    string AssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
                    string UriPath = "pack://application:,,,/" + AssemblyName + ";component/Resources/Icons/" + AsUICommand.Name + ".png";

                    try
                    {
                        BitmapImage ImageResource = new BitmapImage(new Uri(UriPath));
                        ItemIcon = new Image { Source = ImageResource, Width = 16.0, Height = 16.0 };
                    }
                    catch (IOException)
                    {
                        throw new ArgumentOutOfRangeException(nameof(command));
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(command));
            }

            return ItemIcon;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
