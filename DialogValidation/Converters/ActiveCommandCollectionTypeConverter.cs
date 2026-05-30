namespace CustomControls;

using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

/// <summary>
/// Converter from a string to a collection of <see cref="ActiveCommand"/>.
/// </summary>
#pragma warning disable CA1812
internal class ActiveCommandCollectionTypeConverter : TypeConverter
#pragma warning restore CA1812
{
    /// <inheritdoc />
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        if (sourceType == typeof(string))
            return true;

        return base.CanConvertFrom(context, sourceType);
    }

    /// <inheritdoc />
    /// <remarks>
    /// <para>This method will convert a string with format "&lt;commandname&gt;, ..." where &lt;commandname&gt; is the name of one of the supported commands.</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// "OK, Cancel, Yes, No"
    /// </code>
    /// </example>
#if NET6_0_OR_GREATER
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
#else
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
#endif
    {
        if (value is string AsString)
        {
            string[] StringList = AsString.Split(',');

            ActiveCommandCollection Result = new();
            foreach (string Name in StringList)
                _ = ActiveCommandTypeConverter.TryParseName(Name.Trim(), Result.Add);

            return Result;
        }
        else
        {
            return base.ConvertFrom(context, culture, value);
        }
    }

    /// <inheritdoc />
    /// <remarks>
    /// <para>This method will convert to a string with format "&lt;commandname&gt;, ..." where &lt;commandname&gt; is the name of each command in the collection.</para>
    /// </remarks>
#if NET6_0_OR_GREATER
    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (value is ActiveCommandCollection ActiveCommandCollection && destinationType == typeof(string))
            return string.Join(',', ActiveCommandCollection.Select(command => command.Name));
        else
            return base.ConvertTo(context, culture, value, destinationType);
    }
#else
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
        else
        {
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
#endif
}
