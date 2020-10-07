namespace CustomControls
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// Contains a set of tools to manipulate gestures.
    /// </summary>
    public static class GestureHelper
    {
        #region Gesture Text
        /// <summary>
        /// Gets the gesture text associated to a command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="source">The control to which the command applies.</param>
        /// <returns>The gesture text.</returns>
        public static string GetGestureText(ICommand command, FrameworkElement source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            Dictionary<string, string> GestureTable = GetGestureTable(source);

            string GestureText = string.Empty;

            switch (command)
            {
                case ExtendedRoutedCommand AsExtendedCommand:
                    if (AsExtendedCommand.MenuHeader != null && GestureTable.ContainsKey(AsExtendedCommand.MenuHeader))
                        GestureText = GestureTable[AsExtendedCommand.MenuHeader];
                    break;

                case RoutedUICommand AsUICommand:
                    string DisplayString;
                    if (GetCustomGestureText(AsUICommand, GestureTable, out DisplayString) || GetSystemGestureText(AsUICommand, source, out DisplayString))
                        GestureText = DisplayString;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(command));
            }

            return GestureText;
        }

        private static bool GetCustomGestureText(RoutedUICommand command, Dictionary<string, string> gestureTable, out string displayString)
        {
            string CommandName = GestureHelper.ApplicationCommandName(command);

            if (gestureTable.ContainsKey(CommandName))
            {
                displayString = gestureTable[CommandName];
                return true;
            }

            displayString = string.Empty;
            return false;
        }

        private static bool GetSystemGestureText(RoutedUICommand command, FrameworkElement source, out string displayString)
        {
            if (source is IGestureSource AsGestureSource)
            {
                IGestureTranslator? Translator = AsGestureSource.GestureTranslator;
                if (Translator != null)
                    foreach (InputGesture Gesture in command.InputGestures)
                        if (GestureToText(Gesture, Translator, out displayString))
                            return true;
            }

            displayString = string.Empty;
            return false;
        }
        #endregion

        #region Gesture Table
        private static Dictionary<string, string> GetGestureTable(FrameworkElement source)
        {
            Dictionary<string, string> GestureTable = new Dictionary<string, string>();
            FrameworkElement Element = source;
            IGestureTranslator? Translator = null;

            while (true)
            {
                if (Element is IGestureSource AsGestureSource)
                    Translator = AsGestureSource.GestureTranslator;

                ParseKeyBindings(Element.InputBindings, Translator, GestureTable);

                if (Element.Parent is FrameworkElement AsParentElement)
                    Element = AsParentElement;
                else
                    break;
            }

            return GestureTable;
        }

        private static void ParseKeyBindings(InputBindingCollection inputBindings, IGestureTranslator? translator, Dictionary<string, string> gestureTable)
        {
            foreach (InputBinding Binding in inputBindings)
                if (Binding is KeyBinding AsKeyBinding)
                    ParseKeyBinding(AsKeyBinding, translator, gestureTable);
        }

        private static void ParseKeyBinding(KeyBinding binding, IGestureTranslator? translator, Dictionary<string, string> gestureTable)
        {
            string Key = string.Empty;

            switch (binding.Command)
            {
                case ExtendedRoutedCommand AsExtendedCommand:
                    Key = AsExtendedCommand.MenuHeader;
                    break;

                case RoutedUICommand AsUICommand:
                    Key = GestureHelper.ApplicationCommandName(AsUICommand);
                    break;
            }

            if (Key.Length > 0 && !gestureTable.ContainsKey(Key))
                if (GestureToText(binding.Gesture, translator, out string GestureText))
                    gestureTable.Add(Key, GestureText);
        }

        private static bool GestureToText(InputGesture gesture, IGestureTranslator? translator, out string gestureText)
        {
            if (gesture is KeyGesture AsKeyGesture)
            {
                string LocalizedText = AsKeyGesture.GetDisplayStringForCulture(CultureInfo.CurrentCulture);
                if (LocalizedText.Length > 0)
                {
                    gestureText = PostTranslateText(translator, LocalizedText);
                    return true;
                }

                if (AsKeyGesture.DisplayString.Length > 0 && AsKeyGesture.DisplayString.Length > 0)
                {
                    gestureText = PostTranslateText(translator, AsKeyGesture.DisplayString);
                    return true;
                }
            }

            gestureText = string.Empty;
            return false;
        }

        private static string PostTranslateText(IGestureTranslator? translator, string text)
        {
            if (translator != null)
                return translator.PostTranslate(text);
            else
                return text;
        }

        /// <summary>
        /// Gets the name of an application command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>The command name.</returns>
        public static string ApplicationCommandName(RoutedCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            return "ApplicationCommand." + command.Name;
        }
        #endregion
    }
}
