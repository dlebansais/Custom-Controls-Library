using CustomControls;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Converters
{
    /// <summary>
    ///     Converter from a <see cref="ExtendedToolBarButton"/> to an image source.
    /// </summary>
    [ValueConversion(typeof(ExtendedToolBarButton), typeof(ImageSource))]
    public class ExtendedButtonToImageSourceConverter : IValueConverter
    {
        /// <summary>
        ///     Converts from a <see cref="ExtendedToolBarButton"/> to an image source.
        /// </summary>
        /// <param name="value">The <see cref="ExtendedToolBarButton"/> object to convert.</param>
        /// <param name="targetType">This parameter is not used.</param>
        /// <param name="parameter">This parameter is not used.</param>
        /// <param name="culture">This parameter is not used.</param>
        /// <returns>
        ///     If <paramref name="value"/> is a valid <see cref="ExtendedToolBarButton"/> object, the converter returns the image source associated to its command.
        ///     Otherwise, this method returns null.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ExtendedToolBarButton Button)
                return GetItemImageSource(Button.Command, Button.Reference);
            else
                throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        ///     Converts from a <see cref="ExtendedToolBarButton"/> to an image source.
        ///     Uses the reference from <paramref name="command"/> first, and if null uses the <paramref name="reference"/> object.
        /// <returns>
        ///     If both are null, returns null.
        /// </returns>
        /// </summary>
        private static ImageSource GetItemImageSource(ICommand command, CommandResourceReference reference)
        {
            switch (command)
            {
                case ExtendedRoutedCommand AsExtendedCommand:
                    return AsExtendedCommand.ImageSource;

                case RoutedUICommand AsUICommand:
                    Assembly ReferenceAssembly = Assembly.Load(reference.AssemblyName);
                    ImageSource ItemImageSource = ThemeIcons.GetImageSource(ReferenceAssembly, AsUICommand.Name);
                    return ItemImageSource;

                default:
                    throw new ArgumentOutOfRangeException(nameof(command));
            }
        }

        /// <summary>
        ///     This method is not used and will always return null.
        /// </summary>
        /// <param name="value">This parameter is not used.</param>
        /// <param name="targetType">This parameter is not used.</param>
        /// <param name="parameter">This parameter is not used.</param>
        /// <param name="culture">This parameter is not used.</param>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
