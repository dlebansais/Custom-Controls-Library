namespace CustomControls
{
    using System;
    using System.IO;
    using System.Text;
    using System.Windows;

    /// <summary>
    /// Represents the manager of a dictionary of shared resources.
    /// </summary>
    internal static class SharedResourceDictionaryManager
    {
        /// <summary>
        /// Gets the resource dictionary.
        /// </summary>
        internal static ResourceDictionary SharedDictionary
        {
            get
            {
                if (SharedDictionaryInternal.Count == 0)
                {
                    StringBuilder ThemeFileName = new StringBuilder(0x200);
                    StringBuilder ColorBuff = new StringBuilder(0x200);
                    StringBuilder SizeBuff = new StringBuilder(0x200);
                    NativeMethods.GetCurrentThemeName(ThemeFileName, ThemeFileName.Capacity, ColorBuff, ColorBuff.Capacity, SizeBuff, SizeBuff.Capacity);
                    string FileName = Path.GetFileName(ThemeFileName.ToString().ToLower());

                    string ThemeChoice;
                    switch (FileName)
                    {
                        case "aero.msstyles":
                            ThemeChoice = "aero.normalcolor";
                            break;
                        case "aero2.msstyles":
                            ThemeChoice = "aero2.normalcolor";
                            break;
                        case "aerolite.msstyles":
                            ThemeChoice = "aerolite.normalcolor";
                            break;
                        default:
                            ThemeChoice = "generic";
                            break;
                    }

                    Uri ResourceLocater = new Uri($"/ExtendedTreeView;component/themes/{ThemeChoice}.xaml", UriKind.Relative);
                    SharedDictionaryInternal = (ResourceDictionary)Application.LoadComponent(ResourceLocater);
                }

                return SharedDictionaryInternal;
            }
        }

        private static ResourceDictionary SharedDictionaryInternal = new ResourceDictionary();
    }
}
