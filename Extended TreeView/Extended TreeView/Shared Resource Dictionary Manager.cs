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
                if (_SharedDictionary == null)
                {
                    Uri ResourceLocater = new Uri("/ExtendedTreeView;component/themes/generic.xaml", UriKind.Relative);
                    _SharedDictionary = Application.LoadComponent(ResourceLocater) as ResourceDictionary;
                }

                return _SharedDictionary;
            }
        }

        private static ResourceDictionary _SharedDictionary;
    }
}
