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
            ICommand AsCommand;
            if ((AsCommand = value as ICommand) != null)
                return GetItemIcon(AsCommand);
            else
                return null;
        }

        protected virtual Image GetItemIcon(ICommand command)
        {
            Image ItemIcon = null;

            ExtendedRoutedCommand AsExtendedCommand;
            RoutedUICommand AsUICommand;

            if ((AsExtendedCommand = command as ExtendedRoutedCommand) != null)
            {
                Image Icon = new Image() { Source = AsExtendedCommand.ImageSource, Width = 16.0, Height = 16.0 };
                ItemIcon = Icon;
            }

            else if ((AsUICommand = command as RoutedUICommand) != null)
            {
                string AssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
                string UriPath = "pack://application:,,,/" + AssemblyName + ";component/Resources/Icons/" + AsUICommand.Name + ".png";

                try
                {
                    BitmapImage ImageResource = new BitmapImage(new Uri(UriPath));
                    ItemIcon = new Image { Source = ImageResource, Width = 16.0, Height = 16.0 };
                }
                catch (IOException)
                {
                    ItemIcon = null;
                }
            }

            return ItemIcon;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
