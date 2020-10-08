namespace CustomControls
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media;
    using Xceed.Wpf.AvalonDock.Themes;

    /// <summary>
    /// Represents a status theme for AvalonDock.
    /// </summary>
    public class AvalonStatusTheme : StatusTheme
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="AvalonStatusTheme"/> class.
        /// </summary>
        /// <param name="theme">The theme.</param>
        /// <param name="backgroundResourceKeys">The list of background brushes.</param>
        /// <param name="foregroundResourceKeys">The list of foreground brushes.</param>
        public AvalonStatusTheme(Theme theme, CompositeCollection backgroundResourceKeys, CompositeCollection foregroundResourceKeys)
        {
            Theme = theme;
            BackgroundResourceKeys = backgroundResourceKeys;
            ForegroundResourceKeys = foregroundResourceKeys;
        }

        /// <summary>
        /// Gets the theme.
        /// </summary>
        protected Theme Theme { get; private set; }

        /// <summary>
        /// Gets the list of background brushes.
        /// </summary>
        protected CompositeCollection BackgroundResourceKeys { get; private set; }

        /// <summary>
        /// Gets the list of foreground brushes.
        /// </summary>
        protected CompositeCollection ForegroundResourceKeys { get; private set; }
        #endregion

        #region Implementation
        /// <summary>
        /// Gets the background brush for a status type.
        /// </summary>
        /// <param name="statusType">The status type.</param>
        /// <returns>The brush.</returns>
        protected override Brush GetStatusTypeBackgroundBrush(StatusType statusType)
        {
            return GetBrush(statusType, BackgroundResourceKeys);
        }

        /// <summary>
        /// Gets the foreground brush for a status type.
        /// </summary>
        /// <param name="statusType">The status type.</param>
        /// <returns>The brush.</returns>
        protected override Brush GetStatusTypeForegroundBrush(StatusType statusType)
        {
            return GetBrush(statusType, ForegroundResourceKeys);
        }

        /// <summary>
        /// Gets the brush for a status type within a collection.
        /// </summary>
        /// <param name="statusType">The status type.</param>
        /// <param name="resourceKeys">The collection of brushes by their resource key.</param>
        /// <returns>The brush.</returns>
        protected virtual Brush GetBrush(StatusType statusType, CompositeCollection resourceKeys)
        {
            if (resourceKeys == null)
                throw new ArgumentNullException(nameof(resourceKeys));

            if (Theme.GetResourceUri() is Uri Uri)
                if (Application.LoadComponent(Uri) is ResourceDictionary ThemeResources)
                {
                    string UriName = Uri.ToString();

                    foreach (CompositeCollection ThemeResourceKeys in resourceKeys)
                        if (ThemeResourceKeys.Count > 0)
                        {
                            string ThemeUriName = (string)ThemeResourceKeys[0];
                            if (ThemeUriName == UriName)
                            {
                                int StatusIndex = (int)statusType;
                                if (StatusIndex + 1 < ThemeResourceKeys.Count)
                                {
                                    object Key = ThemeResourceKeys[StatusIndex + 1];
                                    if (Key is SolidColorBrush AsSolidColorBrush)
                                        return AsSolidColorBrush;

                                    if (ThemeResources[Key] is Brush Resource)
                                        return Resource;

                                    Debug.Print("Resource not found: " + Key);
                                }

                                break;
                            }
                        }
                }

            throw new ArgumentOutOfRangeException(nameof(resourceKeys));
        }
        #endregion
    }
}
