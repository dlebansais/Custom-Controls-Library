using System.Windows;
using System.Windows.Controls;

namespace CustomControls
{
    public static class PrettyItemsControl
    {
        public static void MakeMenuPretty(ItemsControl itemsCollection)
        {
            RestoreVisible(itemsCollection);
            ModifyAllSubmenus(itemsCollection, HideEmpty);
            HideSeparators(itemsCollection, FromTop);
            HideSeparators(itemsCollection, FromBottom);
            HideDuplicateSeparators(itemsCollection);
        }

        public static void MakeToolBarTrayPretty(ToolBarTray toolBarTray)
        {
            if (toolBarTray != null)
                foreach (ToolBar ToolBar in toolBarTray.ToolBars)
                {
                    RestoreSeparators(ToolBar);
                    HideSeparators(ToolBar, FromTop);
                    HideSeparators(ToolBar, FromBottom);
                    HideDuplicateSeparators(ToolBar);
                }
        }

        private static void RestoreVisible(ItemsControl ItemsCollection)
        {
            for (int i = 0; i < ItemsCollection.Items.Count; i++)
            {
                MenuItem AsMenuItem;
                FrameworkElement AsFrameworkElement;

                if ((AsMenuItem = ItemsCollection.Items[i] as MenuItem) != null)
                {
                    if (AsMenuItem.Items.Count > 0)
                        AsMenuItem.Visibility = Visibility.Visible;
                }

                else if ((AsFrameworkElement = ItemsCollection.Items[i] as FrameworkElement) != null)
                    AsFrameworkElement.Visibility = Visibility.Visible;
            }
        }

        private static void ModifyAllSubmenus(ItemsControl ItemsCollection, ModifyMenuHandler Handler)
        {
            for (int i = 0; i < ItemsCollection.Items.Count; i++)
            {
                MenuItem AsMenuItem;
                if ((AsMenuItem = ItemsCollection.Items[i] as MenuItem) != null)
                    if (AsMenuItem.Items.Count > 0)
                    {
                        ModifyAllSubmenus(AsMenuItem, Handler);
                        Handler(AsMenuItem);
                    }
            }
        }

        private delegate void ModifyMenuHandler(MenuItem MenuItem);

        private static void HideEmpty(MenuItem MenuItem)
        {
            bool AllCollapsed = true;
            for (int i = 0; i < MenuItem.Items.Count; i++)
            {
                MenuItem Submenu;
                if ((Submenu = MenuItem.Items[i] as MenuItem) != null)
                    if (Submenu.Visibility == Visibility.Visible)
                    {
                        AllCollapsed = false;
                        break;
                    }
            }

            if (AllCollapsed)
                MenuItem.Visibility = Visibility.Collapsed;
            else
                MenuItem.Visibility = Visibility.Visible;
        }

        private static void RestoreSeparators(ItemsControl ItemsCollection)
        {
            for (int i = 0; i < ItemsCollection.Items.Count; i++)
            {
                Separator AsSeparator;
                if ((AsSeparator = ItemsCollection.Items[i] as Separator) != null)
                    AsSeparator.Visibility = Visibility.Visible;
            }
        }

        private delegate object ItemOfBrowser(ItemCollection Items, int Index);

        private static object FromTop(ItemCollection Items, int Index)
        {
            return Items[Index];
        }

        private static object FromBottom(ItemCollection Items, int Index)
        {
            return Items[Items.Count - Index - 1];
        }

        private static void HideSeparators(ItemsControl ItemCollection, ItemOfBrowser Browser)
        {
            ItemCollection Items = ItemCollection.Items;
            bool Exit = false;

            for (int i = 0; i < Items.Count && !Exit; i++)
            {
                FrameworkElement AsFrameworkElement;
                if ((AsFrameworkElement = Browser(Items, i) as FrameworkElement) != null)
                {
                    if (AsFrameworkElement.Visibility == Visibility.Visible)
                    {
                        Separator AsSeparator;
                        if ((AsSeparator = AsFrameworkElement as Separator) != null)
                            AsSeparator.Visibility = Visibility.Collapsed;
                        else
                            Exit = true;
                    }
                }
                else
                    Exit = true;
            }
        }

        private static void HideDuplicateSeparators(ItemsControl ItemCollection)
        {
            ItemCollection Items = ItemCollection.Items;
            Separator PreviousVisibleSeparator = null;

            for (int i = 0; i < Items.Count; i++)
            {
                FrameworkElement ElementItem = (FrameworkElement)Items[i];
                if (ElementItem.Visibility == Visibility.Visible)
                {
                    Separator AsSeparator;
                    if ((AsSeparator = Items[i] as Separator) != null)
                    {
                        if (PreviousVisibleSeparator != null)
                            PreviousVisibleSeparator.Visibility = Visibility.Collapsed;

                        PreviousVisibleSeparator = AsSeparator;
                    }
                    else
                        PreviousVisibleSeparator = null;
                }
            }
        }
    }
}
