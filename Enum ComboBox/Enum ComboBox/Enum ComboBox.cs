using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace CustomControls
{
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
    public class EnumComboBox : ComboBox
    {
        #region Custom properties and events
        #region Enum Binding
        /// <summary>
        /// Identifies the <see cref="EnumBinding"/> dependency property.
        /// </summary>
        ///
        /// <returns>
        /// The identifier for the <see cref="EnumBinding"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty EnumBindingProperty = DependencyProperty.Register("EnumBinding", typeof(object), typeof(EnumComboBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnEnumBindingChanged));

        /// <summary>
        /// Gets or sets the enum property to bind on.
        /// <para>This is a replacement for the ComboBox.ItemsSource property.</para>
        /// </summary>
        /// <example>
        /// Example: for a type called <code>MyEnum { MyFirstValue, MySecondValue }</code> and a property <code>MyEnum MyEnumValue { get; set; }</code> one can use the following Xaml code:
        /// <code>
        /// <para>&lt;EnumComboBox EnumBinding="{Binding Path=MyEnumValue}"/&gt;</para>
        /// </code>
        /// </example>
        [Bindable(true)]
        public object EnumBinding
        {
            get { return (object)GetValue(EnumBindingProperty); }
            set { SetValue(EnumBindingProperty, value); }
        }

        private static void OnEnumBindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            EnumComboBox Ctrl = (EnumComboBox)d;
            Ctrl.OnEnumBindingChanged(e);
        }

        private void OnEnumBindingChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsUserSelecting)
                return;

            if (e.NewValue == null)
                ResetContent();

            else if (e.OldValue == null || e.OldValue.GetType() != e.NewValue.GetType())
                UpdateContent(e.NewValue.GetType());

            int Index = EnumBindingAsIndex;
            if (SelectedIndex != Index)
                SelectedIndex = Index;
        }
        #endregion
        #region Name Converter
        /// <summary>
        /// Identifies the <see cref="NameConverter"/> dependency property.
        /// </summary>
        ///
        /// <returns>
        /// The identifier for the <see cref="NameConverter"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty NameConverterProperty = DependencyProperty.Register("NameConverter", typeof(IValueConverter), typeof(EnumComboBox), new PropertyMetadata(new Converters.IdentityStringConverter()));

        /// <summary>
        /// Gets or sets the converter to use to convert an enum value to its localized content (usually a string).
        /// </summary>
        public IValueConverter NameConverter
        {
            get { return (IValueConverter)GetValue(NameConverterProperty); }
            set { SetValue(NameConverterProperty, value); }
        }
        #endregion
        #region Name Converter Parameter
        /// <summary>
        /// Identifies the <see cref="NameConverterParameter"/> dependency property.
        /// </summary>
        ///
        /// <returns>
        /// The identifier for the <see cref="NameConverterParameter"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty NameConverterParameterProperty = DependencyProperty.Register("NameConverterParameter", typeof(object), typeof(EnumComboBox), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the converter parameter to use when converting an enum value to its localized content.
        /// </summary>
        public object NameConverterParameter
        {
            get { return GetValue(NameConverterParameterProperty); }
            set { SetValue(NameConverterParameterProperty, value); }
        }
        #endregion
        #region Name Converter Culture
        /// <summary>
        /// Identifies the <see cref="NameConverterCulture"/> dependency property.
        /// </summary>
        ///
        /// <returns>
        /// The identifier for the <see cref="NameConverterCulture"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty NameConverterCultureProperty = DependencyProperty.Register("NameConverterCulture", typeof(CultureInfo), typeof(EnumComboBox), new PropertyMetadata(CultureInfo.CurrentCulture));

        /// <summary>
        /// Gets or sets the converter culture to use when converting an enum value to its localized content.
        /// </summary>
        public CultureInfo NameConverterCulture
        {
            get { return (CultureInfo)GetValue(NameConverterCultureProperty); }
            set { SetValue(NameConverterCultureProperty, value); }
        }
        #endregion
        #endregion

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
        /// <parameters>
        /// <param name="constraint">The maximum size that the method can return.</param>
        /// </parameters>
        /// <returns>
        /// The size of the control, up to the maximum specified by constraint.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            Size BaseSize = base.MeasureOverride(constraint);

            double AddedWidth, AddedHeight;
            if (SelectedIndex >= 0 && SelectedIndex < EnumNameCollection.Count)
            {
                Size SelectedEnumSize = GetTextSize(EnumNameCollection[SelectedIndex]);

                if (BaseSize.Width > SelectedEnumSize.Width)
                    AddedWidth = BaseSize.Width - SelectedEnumSize.Width;
                else
                    AddedWidth = 0;
                if (BaseSize.Height > SelectedEnumSize.Height)
                    AddedHeight = BaseSize.Height - SelectedEnumSize.Height;
                else
                    AddedHeight = 0;
            }
            else
            {
                AddedWidth = 0;
                AddedHeight = 0;
            }

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
        /// <parameters>
        /// <param name="text">The localized string corresponding to one of the enum values.</param>
        /// </parameters>
        /// <returns>
        /// The width and height of the string, including trailing whitespaces.
        /// </returns>
        private Size GetTextSize(string text)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            FormattedText EnumFormattedText = new FormattedText(text, ConversionCulture, FlowDirection, new Typeface(FontFamily, FontStyle, FontWeight, FontStretch), FontSize, Foreground);
#pragma warning restore CS0618 // Type or member is obsolete
            double EnumWidth = EnumFormattedText.WidthIncludingTrailingWhitespace;
            double EnumHeight = EnumFormattedText.Height;

            return new Size(EnumWidth, EnumHeight);
        }

        /// <summary>
        /// Override the <see cref="OnSelectionChanged"/> event handler, to update the bound enum property.
        /// </summary>
        /// <parameters>
        /// <param name="e">Provides data for <see cref="SelectionChangedEventArgs"/>.</param>
        /// </parameters>
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);

            if (EnumBinding != null)
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
                if (EnumBinding != null)
                {
                    Type EnumType = EnumBinding.GetType();
                    if (EnumType.IsEnum)
                    {
                        Array Values = EnumType.GetEnumValues();
                        object CurrentValue = GetValue(EnumBindingProperty);

                        for (int i = 0; i < Values.Length; i++)
                            if (Values.GetValue(i).Equals(CurrentValue))
                                return i;
                    }
                }

                return -1;
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
            EnumNameCollection.Clear();

            if (enumType != null && enumType.IsEnum)
            {
                IValueConverter Converter = NameConverter;
                object ConverterParameter = NameConverterParameter;
                ConversionCulture = NameConverterCulture;

                string[] EnumNames = enumType.GetEnumNames();
                foreach (string EnumName in EnumNames)
                {
                    string ConvertedText = (string)Converter.Convert(EnumName, typeof(string), ConverterParameter, ConversionCulture);
                    EnumNameCollection.Add(ConvertedText);
                }
            }

            ItemsSource = EnumNameCollection;

            if (SelectedIndex >= EnumNameCollection.Count)
                SelectedIndex = -1;
        }

        /// <summary>
        /// Gets the culture that was used during conversion of enum values to their localized names.
        /// </summary>
        private CultureInfo ConversionCulture = CultureInfo.CurrentCulture;
        #endregion
    }
}
