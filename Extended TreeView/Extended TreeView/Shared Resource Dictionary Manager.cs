using System;
using System.Windows;

namespace CustomControls
{
    internal static class SharedResourceDictionaryManager
    {
        internal static ResourceDictionary SharedDictionary
        {
            get
            {
                if (_SharedDictionary.Count == 0)
                {
                    Uri ResourceLocater = new Uri("/ExtendedTreeView;component/themes/generic.xaml", UriKind.Relative);
                    _SharedDictionary = (ResourceDictionary)Application.LoadComponent(ResourceLocater);
                }

                return _SharedDictionary;
            }
        }

        private static ResourceDictionary _SharedDictionary = new ResourceDictionary();
    }
}
