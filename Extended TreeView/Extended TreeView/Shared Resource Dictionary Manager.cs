namespace CustomControls
{
    using System;
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
                    Uri ResourceLocater = new Uri("/ExtendedTreeView;component/themes/generic.xaml", UriKind.Relative);
                    SharedDictionaryInternal = (ResourceDictionary)Application.LoadComponent(ResourceLocater);
                }

                return SharedDictionaryInternal;
            }
        }

        private static ResourceDictionary SharedDictionaryInternal = new ResourceDictionary();
    }
}
