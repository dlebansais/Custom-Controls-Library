namespace Converters
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using CustomControls;

    /// <summary>
    /// Converter from a string to a collection of <see cref="ActiveCommand"/>.
    /// </summary>
#pragma warning disable CA1812
    internal class ActiveCommandCollectionTypeConverter : TypeConverter
#pragma warning restore CA1812
    {
        /// <summary>
        /// Tell the system this converter can convert a string to a <see cref="ActiveCommandCollection"/>.
        /// </summary>
        /// <param name="context">A System.ComponentModel.ITypeDescriptorContext that provides a format context.</param>
        /// <param name="sourceType">A System.Type that represents the type you want to convert from.</param>
        /// <returns>
        /// True if <paramref name="sourceType"/> is <see cref="string"/>, or if the base converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Converts the given object to the type of this converter, using the specified context and culture information.
        /// </summary>
        /// <param name="context">A System.ComponentModel.ITypeDescriptorContext that provides a format context.</param>
        /// <param name="culture">The System.Globalization.CultureInfo to use as the current culture.</param>
        /// <param name="value">The System.Object to convert.</param>
        /// <returns>
        /// An <see cref="ActiveCommandCollection"/> that represents the converted value.
        /// </returns>
        /// <remarks>
        /// <para>This method will convert a string with format "&lt;commandname&gt;, ..." where &lt;commandname&gt; is the name of one of the supported commands.</para>
        /// </remarks>
        /// <example>
        /// <code>
        /// "OK, Cancel, Yes, No"
        /// </code>
        /// </example>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string AsString)
            {
                string[] StringList = AsString.Split(',');

                ActiveCommandCollection Result = new ActiveCommandCollection();
                foreach (string Name in StringList)
                    ActiveCommandTypeConverter.TryParseName(Name.Trim(), (ActiveCommand command) => Result.Add(command));

                return Result;
            }

            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// Converts a <see cref="ActiveCommandCollection"/> to a string, using the specified context and culture information.
        /// </summary>
        /// <param name="context">A System.ComponentModel.ITypeDescriptorContext that provides a format context.</param>
        /// <param name="culture">A System.Globalization.CultureInfo. If null is passed, the current culture is assumed.</param>
        /// <param name="value">The <see cref="ActiveCommandCollection"/> to convert.</param>
        /// <param name="destinationType">The System.Type to convert the value parameter to.</param>
        /// <returns>
        /// A string that represents the converted value.
        /// </returns>
        /// <remarks>
        /// <para>This method will convert to a string with format "&lt;commandname&gt;, ..." where &lt;commandname&gt; is the name of each command in the collection.</para>
        /// </remarks>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            ActiveCommandCollection ActiveCommandCollection = (ActiveCommandCollection)value;

            if (destinationType == typeof(string))
            {
                string Result = string.Empty;

                foreach (ActiveCommand Command in ActiveCommandCollection)
                {
                    if (Result.Length > 0)
                        Result += ",";

                    Result += Command.Name;
                }

                return Result;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
