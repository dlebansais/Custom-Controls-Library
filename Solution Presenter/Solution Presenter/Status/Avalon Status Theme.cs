using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Verification;
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
            Assert.ValidateReference(resourceKeys);

            Uri uri = Theme.GetResourceUri();
            if (uri != null)
            {
                ResourceDictionary ThemeResources = Application.LoadComponent(uri) as ResourceDictionary;
                if (ThemeResources != null)
                {
                    string UriName = uri.ToString();

                    foreach (CompositeCollection ThemeResourceKeys in resourceKeys)
                        if (ThemeResourceKeys.Count > 0)
                        {
                            string ThemeUriName = ThemeResourceKeys[0] as string;
                            if (ThemeUriName != null && ThemeUriName == UriName)
                            {
                                int StatusIndex = (int)statusType;
                                if (StatusIndex + 1 < ThemeResourceKeys.Count)
                                {
                                    object Key = ThemeResourceKeys[StatusIndex + 1];
                                    if (Key is SolidColorBrush)
                                        return (Brush)Key;

                                    Brush Resource = ThemeResources[Key] as Brush;
                                    if (Resource != null)
                                        return Resource;

                                    Debug.Print("Resource not found: " + Key);
                                }

                                break;
                            }
                        }
                }
            }

            return null;
        }
        #endregion
    }
}
