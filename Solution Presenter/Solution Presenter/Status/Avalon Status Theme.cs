using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Xceed.Wpf.AvalonDock.Themes;

namespace CustomControls
{
    public class AvalonStatusTheme : StatusTheme
    {
        #region Init
        public AvalonStatusTheme(Theme theme, CompositeCollection backgroundResourceKeys, CompositeCollection foregroundResourceKeys)
        {
            this.Theme = theme;
            this.BackgroundResourceKeys = backgroundResourceKeys;
            this.ForegroundResourceKeys = foregroundResourceKeys;
        }

        protected Theme Theme { get; private set; }
        protected CompositeCollection BackgroundResourceKeys { get; private set; }
        protected CompositeCollection ForegroundResourceKeys { get; private set; }
        #endregion

        #region Implementation
        protected override Brush GetStatusTypeBackgroundBrush(StatusType statusType)
        {
            return GetBrush(statusType, BackgroundResourceKeys);
        }

        protected override Brush GetStatusTypeForegroundBrush(StatusType statusType)
        {
            return GetBrush(statusType, ForegroundResourceKeys);
        }

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
