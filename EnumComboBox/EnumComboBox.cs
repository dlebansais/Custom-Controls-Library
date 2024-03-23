namespace CustomControls;

using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

/// <summary>
/// Represents a combo box with specific support for enum types.
/// <para>Implemented as a derived class of the <see cref="ComboBox"/> parent.</para>
/// </summary>
/// <remarks>
/// <para>. Retain all features of a standard combo box.</para>
/// <para>. Include an additional property to use for binding specifically on an enum property.</para>
/// <para>. Provide support for localized value names by way of a converter property, to convert all values to their localized content.</para>
/// <para>. Measure the largest selectable value before it reports its own size.</para>
/// </remarks>
public partial class EnumComboBox : ComboBox
{
    #region Init
    /// <summary>
    /// Initializes a new instance of the <see cref="EnumComboBox"/> class.
    /// </summary>
    public EnumComboBox()
    {
        EnumNameCollection = new ObservableCollection<string>();
        IsUserSelecting = false;
    }
    #endregion

    #region Properties
    /// <summary>
    /// Gets the collection of localized value names displayed in the combo box.
    /// </summary>
    public ObservableCollection<string> EnumNameCollection { get; private set; }
    #endregion

    #region Ancestor Interface
    /// <summary>
    /// Override the <see cref="MeasureOverride"/> event handler, to accommodate for the largest enum value name.
    /// </summary>
    /// <param name="constraint">The maximum size that the method can return.</param>
    /// <returns>
    /// The size of the control, up to the maximum specified by constraint.
    /// </returns>
    protected override Size MeasureOverride(Size constraint)
    {
        Size BaseSize = base.MeasureOverride(constraint);

        double AddedWidth = 0;
        double AddedHeight = 0;
        Size SelectedEnumSize;

        if (SelectedIndex >= 0 && SelectedIndex < EnumNameCollection.Count)
            SelectedEnumSize = GetTextSize(EnumNameCollection[SelectedIndex]);
        else
            SelectedEnumSize = BaseSize;

        if (BaseSize.Width > SelectedEnumSize.Width)
            AddedWidth = BaseSize.Width - SelectedEnumSize.Width;
        if (BaseSize.Height > SelectedEnumSize.Height)
            AddedHeight = BaseSize.Height - SelectedEnumSize.Height;

        double MeasuredWidth = 0;
        double MeasuredHeight = 0;
        for (int i = 0; i < EnumNameCollection.Count; i++)
        {
            Size EnumSize = GetTextSize(EnumNameCollection[i]);

            if (MeasuredWidth < EnumSize.Width)
                MeasuredWidth = EnumSize.Width;
            if (MeasuredHeight < EnumSize.Height)
                MeasuredHeight = EnumSize.Height;
        }

        if (MeasuredWidth > 0 && MeasuredHeight > 0)
            return new Size(Math.Min(MeasuredWidth + AddedWidth, constraint.Width), Math.Min(MeasuredHeight + AddedHeight, constraint.Height));
        else
            return BaseSize;
    }

    /// <summary>
    /// Calculates the size of an enum value localized string.
    /// </summary>
    /// <param name="text">The localized string corresponding to one of the enum values.</param>
    /// <returns>The width and height of the string, including trailing whitespaces.</returns>
    private Size GetTextSize(string text)
    {
#pragma warning disable CS0618 // Type or member is obsolete
        FormattedText EnumFormattedText = new(text, ConversionCulture, FlowDirection, new Typeface(FontFamily, FontStyle, FontWeight, FontStretch), FontSize, Foreground);
#pragma warning restore CS0618 // Type or member is obsolete
        double EnumWidth = EnumFormattedText.WidthIncludingTrailingWhitespace;
        double EnumHeight = EnumFormattedText.Height;

        return new Size(EnumWidth, EnumHeight);
    }

    /// <summary>
    /// Override the <see cref="OnSelectionChanged"/> event handler, to update the bound enum property.
    /// </summary>
    /// <param name="e">Provides data for <see cref="SelectionChangedEventArgs"/>.</param>
    protected override void OnSelectionChanged(SelectionChangedEventArgs e)
    {
        base.OnSelectionChanged(e);

        if (EnumBinding is not null)
        {
            Type EnumType = EnumBinding.GetType();
            if (EnumType.IsEnum)
            {
                Array Values = EnumType.GetEnumValues();
                if (SelectedIndex >= 0 && SelectedIndex < Values.Length)
                {
                    IsUserSelecting = true;
                    SetValue(EnumBindingProperty, Values.GetValue(SelectedIndex));
                    IsUserSelecting = false;
                }
            }
        }
    }

    /// <summary>
    /// Gets a value that indicate if the enum bound to the <see cref="EnumBinding"/> dependency property is being modified as a result of a user's action such as selecting a new value.
    /// </summary>
    /// <returns>
    /// <para>True if the user is a author of the change to the enum bound to the <see cref="EnumBinding"/> dependency property.</para>
    /// <para>False if the enum bound to the <see cref="EnumBinding"/> dependency property is being modified by the client or some other code.</para>
    /// </returns>
    private bool IsUserSelecting;
    #endregion

    #region Implementation
    /// <summary>
    /// Gets the zero-based position of the current value of <see cref="EnumBinding"/> among all possible values.
    /// </summary>
    /// <returns>
    /// The index as an integer, -1 if there is no enum bound to <see cref="EnumBinding"/> or if it has an invalid value.
    /// </returns>
    private int EnumBindingAsIndex
    {
        get
        {
            int Result = -1;

            if (EnumBinding is not null)
            {
                Type EnumType = EnumBinding.GetType();
                if (EnumType.IsEnum)
                {
                    Array Values = EnumType.GetEnumValues();
                    object CurrentValue = GetValue(EnumBindingProperty);

                    for (int i = 0; i < Values.Length; i++)
                    {
                        object EnumValue = Values.GetValue(i)!;
                        if (EnumValue.Equals(CurrentValue))
                            Result = i;
                    }
                }
            }

            return Result;
        }
    }

    /// <summary>
    /// Resets properties used to display content in the combo box.
    /// </summary>
    private void ResetContent()
    {
        EnumNameCollection.Clear();
        SelectedIndex = -1;
    }

    /// <summary>
    /// Update properties used to display content in the combo box with new values.
    /// </summary>
    /// <parameters>
    /// <param name="enumType">The type of the enum bound to the <see cref="EnumBinding"/> dependency property.</param>
    /// </parameters>
    /// <remarks>
    /// This will display a new set of localized strings in the drop-down part of the combo box, and select one of them if appropriate.
    /// </remarks>
    private void UpdateContent(Type enumType)
    {
        ObservableCollection<string> NewEnumNameCollection = new();

        if (enumType.IsEnum)
        {
            IValueConverter Converter = NameConverter;
            object ConverterParameter = NameConverterParameter;
            ConversionCulture = NameConverterCulture;

            string[] EnumNames = enumType.GetEnumNames();
            foreach (string EnumName in EnumNames)
            {
                string ConvertedText = (string)Converter.Convert(EnumName, typeof(string), ConverterParameter, ConversionCulture);
                NewEnumNameCollection.Add(ConvertedText);
            }
        }

        EnumNameCollection = NewEnumNameCollection;

        if (SelectedIndex >= EnumNameCollection.Count)
            SelectedIndex = -1;

        ItemsSource = EnumNameCollection;
    }

    /// <summary>
    /// Gets the culture that was used during conversion of enum values to their localized names.
    /// </summary>
    private CultureInfo ConversionCulture = CultureInfo.CurrentCulture;
    #endregion
}
