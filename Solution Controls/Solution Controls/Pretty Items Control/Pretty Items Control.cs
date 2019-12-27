using System;
using System.Windows;
using System.Windows.Controls;

namespace CustomControls
{
    public static class PrettyItemsControl
    {
        public static void MakeMenuPretty(ItemsControl itemsCollection)
        {
            if (itemsCollection == null)
                throw new ArgumentNullException(nameof(itemsCollection));

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
                switch (ItemsCollection.Items[i])
                {
                    case MenuItem AsMenuItem:
                        if (AsMenuItem.Items.Count > 0)
                            AsMenuItem.Visibility = Visibility.Visible;
                        break;

                    case FrameworkElement AsFrameworkElement:
                        AsFrameworkElement.Visibility = Visibility.Visible;
                        break;
                }
            }
        }

        private static void ModifyAllSubmenus(ItemsControl ItemsCollection, ModifyMenuHandler Handler)
        {
            for (int i = 0; i < ItemsCollection.Items.Count; i++)
                if (ItemsCollection.Items[i] is MenuItem AsMenuItem)
                    if (AsMenuItem.Items.Count > 0)
                    {
                        ModifyAllSubmenus(AsMenuItem, Handler);
                        Handler(AsMenuItem);
                    }
        }

        private delegate void ModifyMenuHandler(MenuItem MenuItem);

        private static void HideEmpty(MenuItem MenuItem)
        {
            bool AllCollapsed = true;
            for (int i = 0; i < MenuItem.Items.Count; i++)
            {
                if (MenuItem.Items[i] is MenuItem Submenu)
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
                if (ItemsCollection.Items[i] is Separator AsSeparator)
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
                if (Browser(Items, i) is FrameworkElement AsFrameworkElement)
                {
                    if (AsFrameworkElement.Visibility == Visibility.Visible)
                    {
                        if (AsFrameworkElement is Separator AsSeparator)
                            AsSeparator.Visibility = Visibility.Collapsed;
                        else
                            Exit = true;
                    }
                }
                else
                    Exit = true;
        }

        private static void HideDuplicateSeparators(ItemsControl ItemCollection)
        {
            ItemCollection Items = ItemCollection.Items;
            Separator? PreviousVisibleSeparator = null;

            for (int i = 0; i < Items.Count; i++)
            {
                FrameworkElement ElementItem = (FrameworkElement)Items[i];
                if (ElementItem.Visibility == Visibility.Visible)
                    if (Items[i] is Separator AsSeparator)
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
