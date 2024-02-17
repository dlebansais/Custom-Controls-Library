namespace CustomControls;

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
                const int MaxPath = 260;
                char[] ThemeFileName = new char[MaxPath];
                char[] ColorBuff = new char[MaxPath];
                char[] SizeBuff = new char[MaxPath];
                NativeMethods.GetCurrentThemeName(ThemeFileName, MaxPath, ColorBuff, MaxPath, SizeBuff, MaxPath);

                string FileName = Path.GetFileName(new string(ThemeFileName).ToUpperInvariant());
                string ThemeChoice = FileName switch
                {
                    "AERO.MSSTYLES" => "aero.normalcolor",
                    "AERO2.MSSTYLES" => "aero2.normalcolor",
                    "AEROLITE.MSSTYLES" => "aerolite.normalcolor",
                    _ => "generic",
                };
                Uri ResourceLocater = new($"/ExtendedTreeView;component/themes/{ThemeChoice}.xaml", UriKind.Relative);
                SharedDictionaryInternal = (ResourceDictionary)Application.LoadComponent(ResourceLocater);
            }

            return SharedDictionaryInternal;
        }
    }

    private static ResourceDictionary SharedDictionaryInternal = new();
}
