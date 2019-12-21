using CustomControls;
using System;
using System.ComponentModel;
using System.Globalization;

namespace Converters
{
    /// <summary>
    ///     Converter from a string to a <see cref="ActiveCommand"/>.
    /// </summary>
#pragma warning disable CA1812
    internal class ActiveCommandTypeConverter : TypeConverter
#pragma warning restore CA1812
    {
        /// <summary>
        ///     Compare a name with known command names and return the courresponding <see cref="ActiveCommand"/>.
        /// </summary>
        /// <param name="Name">The command name to compare.</param>
        /// <param name="Result">A <see cref="ActiveCommand"/> with this name, null if no command name matches <paramref name="Name"/>.</param>
        /// <returns>
        ///     True if a match is found; otherwise, false.
        /// </returns>
        public static bool TryParseName(string Name, out ActiveCommand Result)
        {
            foreach (ActiveCommand Command in ActiveCommand.AllCommands)
                if (Name == Command.Name)
                {
                    Result = Command;
                    return true;
                }

            Result = null;
            return false;
        }

        /// <summary>
        ///     Tell the system this converter can convert a string to a <see cref="ActiveCommand"/>.
        /// </summary>
        /// <param name="context">A System.ComponentModel.ITypeDescriptorContext that provides a format context.</param>
        /// <param name="sourceType">A System.Type that represents the type you want to convert from.</param>
        /// <returns>
        ///     True if <paramref name="sourceType"/> is <see cref="String"/>, or if the base converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        ///     Converts the given object to the type of this converter, using the specified context and culture information.
        /// </summary>
        /// <param name="context">A System.ComponentModel.ITypeDescriptorContext that provides a format context.</param>
        /// <param name="culture">The System.Globalization.CultureInfo to use as the current culture.</param>
        /// <param name="value">The System.Object to convert.</param>
        /// <returns>
        ///     An <see cref="ActiveCommand"/> that represents the converted value.
        /// </returns>
        /// <remarks>
        ///     Compare a name with known command names and return the courresponding <see cref="ActiveCommand"/>.
        /// </remarks>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string AsString;
            if ((AsString = value as string) != null)
            {
                ActiveCommand Result;
                if (ActiveCommandTypeConverter.TryParseName(AsString, out Result))
                    return Result;
            }

            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// Converts a <see cref="ActiveCommand"/> to a string, using the specified context and culture information.
        /// </summary>
        /// <param name="context">A System.ComponentModel.ITypeDescriptorContext that provides a format context.</param>
        /// <param name="culture">A System.Globalization.CultureInfo. If null is passed, the current culture is assumed.</param>
        /// <param name="value">The <see cref="ActiveCommand"/> to convert.</param>
        /// <param name="destinationType">The System.Type to convert the value parameter to.</param>
        /// <returns>
        /// A string that represents the converted value.
        /// </returns>
        /// <remarks>
        /// <para>This method will convert to a string which is the name of the command.</para>
        /// </remarks>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            ActiveCommand AsActiveCommand;
            if ((AsActiveCommand = value as ActiveCommand) != null)
                if (destinationType == typeof(string))
                    return AsActiveCommand.Name;

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
