namespace CustomControls
{
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Represents a menu item with additional properties indicating if it should be displayed in a menu.
    /// </summary>
    public class ExtendedToolBarMenuItem : MenuItem
    {
        #region Custom properties and events
        #region Can Show
        /// <summary>
        /// Identifies the <see cref="CanShow"/> dependency property.
        /// </summary>
        /// <returns>
        /// The identifier for the <see cref="CanShow"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty CanShowProperty = DependencyProperty.Register("CanShow", typeof(bool), typeof(ExtendedToolBarMenuItem), new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets a value indicating whether the button can be shown in any menu.
        /// True, the button can be shown in any menu.
        /// False, the button should not appear in any menu.
        /// <para>Note: this allows to specify the menu generic visibility when multi binding is used to decide if it is visible.</para>
        /// </summary>
        public bool CanShow
        {
            get { return (bool)GetValue(CanShowProperty); }
            set { SetValue(CanShowProperty, value); }
        }
        #endregion
        #endregion

        #region Init
        /// <summary>
        /// Initializes static members of the <see cref="ExtendedToolBarMenuItem"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Can't be done inline - too complex")]
        static ExtendedToolBarMenuItem()
        {
            OverrideAncestorMetadata();
        }
        #endregion

        #region Ancestor Interface
        /// <summary>
        /// Overrides metadata associated to the ancestor control with new ones associated to this control specifically.
        /// </summary>
        private static void OverrideAncestorMetadata()
        {
            OverrideMetadataDefaultStyleKey();
        }

        /// <summary>
        /// Overrides the DefaultStyleKey metadata associated to the ancestor control with a new one associated to this control specifically.
        /// </summary>
        private static void OverrideMetadataDefaultStyleKey()
        {
            MenuItem.DefaultStyleKeyProperty.OverrideMetadata(typeof(ExtendedToolBarMenuItem), new FrameworkPropertyMetadata(typeof(ExtendedToolBarMenuItem)));
        }
        #endregion
    }
}
