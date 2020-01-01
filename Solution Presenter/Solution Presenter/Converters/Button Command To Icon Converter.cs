namespace Converters
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Media.Imaging;
    using CustomControls;

    /// <summary>
    /// Represents a converter from a button command to an image.
    /// </summary>
    [ValueConversion(typeof(ICommand), typeof(Image))]
    public class ButtonCommandToIconConverter : IValueConverter
    {
        /// <summary>
        /// Converts from a button command to an image.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>The converted value.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ICommand AsCommand)
                return GetItemIcon(AsCommand);
            else
                throw new ArgumentOutOfRangeException(nameof(value));
        }

        /// <summary>
        /// Gets the icon from the command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>The icon.</returns>
        protected virtual Image GetItemIcon(ICommand command)
        {
            Image ItemIcon;

            switch (command)
            {
                case ExtendedRoutedCommand AsExtendedRoutedCommand:
                    Image Icon = new Image() { Source = AsExtendedRoutedCommand.ImageSource, Width = 16.0, Height = 16.0 };
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

        /// <summary>
        /// Converts a binding target value to the source binding value.
        /// </summary>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use..</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>The converted value.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
