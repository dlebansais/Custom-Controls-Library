namespace CustomControls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Contains a set of tools to make collections of controls pretty.
    /// </summary>
    public static class PrettyItemsControl
    {
        /// <summary>
        /// Makes a menu pretty.
        /// </summary>
        /// <param name="itemsCollection">The menu.</param>
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

        /// <summary>
        /// Makes a toolbar tray pretty.
        /// </summary>
        /// <param name="toolBarTray">The toolbar tray.</param>
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

        private static void RestoreVisible(ItemsControl itemsCollection)
        {
            for (int i = 0; i < itemsCollection.Items.Count; i++)
            {
                switch (itemsCollection.Items[i])
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

        private static void ModifyAllSubmenus(ItemsControl itemsCollection, ModifyMenuHandler handler)
        {
            for (int i = 0; i < itemsCollection.Items.Count; i++)
                if (itemsCollection.Items[i] is MenuItem AsMenuItem)
                    if (AsMenuItem.Items.Count > 0)
                    {
                        ModifyAllSubmenus(AsMenuItem, handler);
                        handler(AsMenuItem);
                    }
        }

        private delegate void ModifyMenuHandler(MenuItem menuItem);

        private static void HideEmpty(MenuItem menuItem)
        {
            bool AllCollapsed = true;
            for (int i = 0; i < menuItem.Items.Count; i++)
            {
                if (menuItem.Items[i] is MenuItem Submenu)
                    if (Submenu.Visibility == Visibility.Visible)
                    {
                        AllCollapsed = false;
                        break;
                    }
            }

            if (AllCollapsed)
                menuItem.Visibility = Visibility.Collapsed;
            else
                menuItem.Visibility = Visibility.Visible;
        }

        private static void RestoreSeparators(ItemsControl itemsCollection)
        {
            for (int i = 0; i < itemsCollection.Items.Count; i++)
            {
                if (itemsCollection.Items[i] is Separator AsSeparator)
                    AsSeparator.Visibility = Visibility.Visible;
            }
        }

        private delegate object ItemOfBrowser(ItemCollection items, int index);

        private static object FromTop(ItemCollection items, int index)
        {
            return items[index];
        }

        private static object FromBottom(ItemCollection items, int index)
        {
            return items[items.Count - index - 1];
        }

        private static void HideSeparators(ItemsControl itemCollection, ItemOfBrowser browser)
        {
            ItemCollection Items = itemCollection.Items;
            bool Exit = false;

            for (int i = 0; i < Items.Count && !Exit; i++)
                if (browser(Items, i) is FrameworkElement AsFrameworkElement)
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

        private static void HideDuplicateSeparators(ItemsControl itemCollection)
        {
            ItemCollection Items = itemCollection.Items;
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
