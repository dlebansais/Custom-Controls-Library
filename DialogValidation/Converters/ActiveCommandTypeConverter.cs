namespace CustomControls;

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;

/// <summary>
/// Converter from a string to a <see cref="ActiveCommand"/>.
/// </summary>
#pragma warning disable CA1812
internal class ActiveCommandTypeConverter : TypeConverter
#pragma warning restore CA1812
{
    /// <summary>
    /// Compares a name with known command names and execute a handler if found.
    /// </summary>
    /// <param name="name">The command name to compare.</param>
    /// <param name="handler">A handler to execute if a command is found.</param>
    /// <returns>
    /// <see langword="true"/> if a match is found; Otherwise, <see langword="false"/>.
    /// </returns>
    public static bool TryParseName(string name, Action<ActiveCommand> handler)
    {
        foreach (ActiveCommand Command in ActiveCommand.AllCommands)
            if (name == Command.Name)
            {
                handler(Command);
                return true;
            }

        return false;
    }

    /// <summary>
    /// Tell the system this converter can convert a string to a <see cref="ActiveCommand"/>.
    /// </summary>
    /// <param name="context">A <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
    /// <param name="sourceType">A <see cref="Type"/> that represents the type you want to convert from.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="sourceType"/> is <see cref="string"/>, or if the base converter can perform the conversion; Otherwise, <see langword="false"/>.
    /// </returns>
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        if (sourceType == typeof(string))
            return true;

        return base.CanConvertFrom(context, sourceType);
    }

    /// <summary>
    /// Converts the given object to the type of this converter, using the specified context and culture information.
    /// </summary>
    /// <param name="context">A <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
    /// <param name="culture">The <see cref="CultureInfo"/> to use as the current culture.</param>
    /// <param name="value">The <see cref="object"/> to convert.</param>
    /// <returns>
    /// An <see cref="ActiveCommand"/> that represents the converted value.
    /// </returns>
    /// <remarks>
    /// Compare a name with known command names and return the corresponding <see cref="ActiveCommand"/>.
    /// </remarks>
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is string AsString)
        {
            object? Result = null;

            if (TryParseName(AsString, (ActiveCommand command) => Result = command))
                return Result;
            else
                return DependencyProperty.UnsetValue;
        }
        else
        {
            return base.ConvertFrom(context, culture, value);
        }
    }

    /// <summary>
    /// Converts a <see cref="ActiveCommand"/> to a string, using the specified context and culture information.
    /// </summary>
    /// <param name="context">A <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
    /// <param name="culture">A <see cref="CultureInfo"/>. If <see langword="null"/> is passed, the current culture is assumed.</param>
    /// <param name="value">The <see cref="ActiveCommand"/> to convert.</param>
    /// <param name="destinationType">The <see cref="Type"/> to convert the value parameter to.</param>
    /// <returns>
    /// A string that represents the converted value.
    /// </returns>
    /// <remarks>
    /// <para>This method will convert to a string which is the name of the command.</para>
    /// </remarks>
#if NET6_0_OR_GREATER
    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (value is ActiveCommand ActiveCommand && destinationType == typeof(string))
            return ActiveCommand.Name;
        else
            return base.ConvertTo(context, culture, value, destinationType);
    }
#else
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        ActiveCommand ActiveCommand = (ActiveCommand)value;

        if (destinationType == typeof(string))
            return ActiveCommand.Name;
        else
            return base.ConvertTo(context, culture, value, destinationType);
    }
#endif
}
