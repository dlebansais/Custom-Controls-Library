namespace CustomControls
{
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Represents a button with additional properties indicating if it should be displayed in a tool bar.
    /// </summary>
    public class ExtendedToolBarButton : Button
    {
        #region Custom properties and events
        #region Reference
        /// <summary>
        /// Identifies the <see cref="Reference"/> dependency property.
        /// </summary>
        /// <returns>
        /// The identifier for the <see cref="Reference"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty ReferenceProperty = DependencyProperty.Register("Reference", typeof(CommandResourceReference), typeof(ExtendedToolBarButton), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets or sets The reference to an assembly where to find resources associated to the button.
        /// Used when the command does not have enough information to locate these resources. For instance, if the command is one of the ApplicationCommands.
        /// Can be null otherwise.
        /// </summary>
        public CommandResourceReference Reference
        {
            get { return (CommandResourceReference)GetValue(ReferenceProperty); }
            set { SetValue(ReferenceProperty, value); }
        }
        #endregion
        #region Is Checkable
        /// <summary>
        /// Identifies the <see cref="IsCheckable"/> dependency property.
        /// </summary>
        /// <returns>
        /// The identifier for the <see cref="IsCheckable"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty IsCheckableProperty = DependencyProperty.Register("IsCheckable", typeof(bool), typeof(ExtendedToolBarButton), new FrameworkPropertyMetadata(true, OnIsCheckableChanged));

        /// <summary>
        /// Gets or sets a value indicating whether the user can check the button as visible.
        /// True, the user can check the button as visible or uncheck it as hidden in the tool bar.
        /// False, the button is always visible.
        /// </summary>
        public bool IsCheckable
        {
            get { return (bool)GetValue(IsCheckableProperty); }
            set { SetValue(IsCheckableProperty, value); }
        }

        /// <summary>
        /// Called when the <see cref="IsCheckable"/> dependency property is changed on <paramref name="modifiedObject"/>.
        /// </summary>
        /// <param name="modifiedObject">The object that had its property modified.</param>
        /// <param name="e">Information about the change.</param>
        private static void OnIsCheckableChanged(DependencyObject modifiedObject, DependencyPropertyChangedEventArgs e)
        {
            ExtendedToolBarButton ctrl = (ExtendedToolBarButton)modifiedObject;
            ctrl.OnIsCheckableChanged();
        }

        /// <summary>
        /// Called when the <see cref="IsCheckable"/> dependency property is changed.
        /// </summary>
        private void OnIsCheckableChanged()
        {
            UpdateVisibility();
        }
        #endregion
        #region Is Default Active
        /// <summary>
        /// Identifies the <see cref="IsDefaultActive"/> dependency property.
        /// </summary>
        /// <returns>
        /// The identifier for the <see cref="IsDefaultActive"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty IsDefaultActiveProperty = DependencyProperty.Register("IsDefaultActive", typeof(bool), typeof(ExtendedToolBarButton), new FrameworkPropertyMetadata(true, OnIsDefaultActiveChanged));

        /// <summary>
        /// Gets or sets a value indicating whether the default setting  is True.
        /// True, the default setting for the <see cref="IsActive"/> property is True.
        /// False, the default setting for the <see cref="IsActive"/> property is False.
        /// </summary>
        public bool IsDefaultActive
        {
            get { return (bool)GetValue(IsDefaultActiveProperty); }
            set { SetValue(IsDefaultActiveProperty, value); }
        }

        /// <summary>
        /// Called when the <see cref="IsDefaultActive"/> dependency property is changed on <paramref name="modifiedObject"/>.
        /// </summary>
        /// <param name="modifiedObject">The object that had its property modified.</param>
        /// <param name="e">Information about the change.</param>
        private static void OnIsDefaultActiveChanged(DependencyObject modifiedObject, DependencyPropertyChangedEventArgs e)
        {
            ExtendedToolBarButton ctrl = (ExtendedToolBarButton)modifiedObject;
            ctrl.OnIsDefaultActiveChanged();
        }

        /// <summary>
        /// Called when the <see cref="IsDefaultActive"/> dependency property is changed.
        /// </summary>
        private void OnIsDefaultActiveChanged()
        {
            InitializeIsActive();
        }
        #endregion
        #region Is Active
        /// <summary>
        /// Identifies the <see cref="IsActive"/> dependency property.
        /// </summary>
        /// <returns>
        /// The identifier for the <see cref="IsActive"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register("IsActive", typeof(bool), typeof(ExtendedToolBarButton), new FrameworkPropertyMetadata(true, OnIsActiveChanged));

        /// <summary>
        /// Gets or sets a value indicating whether the the button is visible.
        /// True, the button is visible in the tool bar.
        /// False, the button is hidden.
        /// </summary>
        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        /// <summary>
        /// Called when the <see cref="IsActive"/> dependency property is changed on <paramref name="modifiedObject"/>.
        /// </summary>
        /// <param name="modifiedObject">The object that had its property modified.</param>
        /// <param name="e">Information about the change.</param>
        private static void OnIsActiveChanged(DependencyObject modifiedObject, DependencyPropertyChangedEventArgs e)
        {
            ExtendedToolBarButton ctrl = (ExtendedToolBarButton)modifiedObject;
            ctrl.OnIsActiveChanged();
        }

        /// <summary>
        /// Called when the <see cref="IsActive"/> dependency property is changed.
        /// </summary>
        private void OnIsActiveChanged()
        {
            UpdateVisibility();
            NotifyIsActiveChanged();
        }
        #endregion
        #region Is Active Changed
        /// <summary>
        /// Identifies the <see cref="IsActiveChanged"/> routed event.
        /// </summary>
        /// <returns>
        /// The identifier for the <see cref="IsActiveChanged"/> routed event.
        /// </returns>
        public static readonly RoutedEvent IsActiveChangedEvent = EventManager.RegisterRoutedEvent("IsActiveChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ExtendedToolBarButton));

        /// <summary>
        /// Sent when the <see cref="IsActive"/> has changed.
        /// </summary>
        public event RoutedEventHandler IsActiveChanged
        {
            add { AddHandler(IsActiveChangedEvent, value); }
            remove { RemoveHandler(IsActiveChangedEvent, value); }
        }

        /// <summary>
        /// Sends a <see cref="IsActiveChanged"/> event.
        /// </summary>
        protected virtual void NotifyIsActiveChanged()
        {
            RaiseEvent(new RoutedEventArgs(IsActiveChangedEvent));
        }
        #endregion
        #endregion

        #region Init
        /// <summary>
        /// Initializes static members of the <see cref="ExtendedToolBarButton"/> class.
        /// </summary>
        static ExtendedToolBarButton()
        {
            OverrideAncestorMetadata();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedToolBarButton"/> class.
        /// </summary>
        public ExtendedToolBarButton()
        {
            InitializeIsActive();
            ToolTipService.SetShowOnDisabled(this, true);
        }
        #endregion

        #region Ancestor Interface
        /// <summary>
        /// Overrides metadata associated to the ancestor control with new ones associated to this control specifically.
        /// </summary>
        private static void OverrideAncestorMetadata()
        {
            OverrideMetadataDefaultStyleKey();
            OverrideMetadataCommandKey();
        }

        /// <summary>
        /// Overrides the DefaultStyleKey metadata associated to the ancestor control with a new one associated to this control specifically.
        /// </summary>
        private static void OverrideMetadataDefaultStyleKey()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ExtendedToolBarButton), new FrameworkPropertyMetadata(typeof(ExtendedToolBarButton)));
        }

        /// <summary>
        /// Overrides the CommandKey metadata associated to the ancestor control with a new one associated to this control specifically.
        /// </summary>
        private static void OverrideMetadataCommandKey()
        {
            CommandProperty.OverrideMetadata(typeof(ExtendedToolBarButton), new FrameworkPropertyMetadata(OnCommandChanged));
        }

        /// <summary>
        /// Called when the Command dependency property is changed on <paramref name="modifiedObject"/>.
        /// </summary>
        /// <param name="modifiedObject">The object that had its property modified.</param>
        /// <param name="e">Information about the change.</param>
        private static void OnCommandChanged(DependencyObject modifiedObject, DependencyPropertyChangedEventArgs e)
        {
            ExtendedToolBarButton ctrl = (ExtendedToolBarButton)modifiedObject;
            ctrl.OnCommandChanged();
        }

        /// <summary>
        /// Called when the Command dependency property is changed.
        /// </summary>
        private void OnCommandChanged()
        {
            UpdateVisibility();
        }
        #endregion

        #region Client Interface
        /// <summary>
        /// Returns the <see cref="IsActive"/> dependency property to its default value.
        /// </summary>
        public virtual void ResetIsActive()
        {
            IsActive = IsDefaultActive;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Initializes the <see cref="IsActive"/> dependency property to its default value.
        /// </summary>
        private void InitializeIsActive()
        {
            IsActive = IsDefaultActive;
        }

        /// <summary>
        /// Updates the button visibility according to the current value of its properties.
        /// </summary>
        private void UpdateVisibility()
        {
            bool IsCommandGroupEnabled = true;

            if (Command is ExtendedRoutedCommand AsExtendedCommand)
                if (!AsExtendedCommand.CommandGroup.IsEnabled)
                    IsCommandGroupEnabled = false;

            Visibility = (IsCommandGroupEnabled && (!IsCheckable || IsActive)) ? Visibility.Visible : Visibility.Collapsed;
        }
        #endregion
    }
}
