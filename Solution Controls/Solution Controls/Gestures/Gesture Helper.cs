using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Input;

namespace CustomControls
{
    public static class GestureHelper
    {
        #region Gesture Text
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

        private static bool GetCustomGestureText(RoutedUICommand Command, Dictionary<string, string> GestureTable, out string DisplayString)
        {
            string CommandName = GestureHelper.ApplicationCommandName(Command);

            if (GestureTable.ContainsKey(CommandName))
            {
                DisplayString = GestureTable[CommandName];
                return true;
            }

            DisplayString = string.Empty;
            return false;
        }

        private static bool GetSystemGestureText(RoutedUICommand Command, FrameworkElement Source, out string DisplayString)
        {
            if (Source is IGestureSource AsGestureSource)
            {
                IGestureTranslator? Translator = AsGestureSource.GestureTranslator;
                if (Translator != null)
                    foreach (InputGesture Gesture in Command.InputGestures)
                        if (GestureToText(Gesture, Translator, out DisplayString))
                            return true;
            }

            DisplayString = string.Empty;
            return false;
        }
        #endregion

        #region Gesture Table
        private static Dictionary<string, string> GetGestureTable(FrameworkElement Source)
        {
            Dictionary<string, string> GestureTable = new Dictionary<string, string>();
            FrameworkElement Element = Source;
            IGestureTranslator? Translator = null;

            for (;;)
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

        private static void ParseKeyBindings(InputBindingCollection InputBindings, IGestureTranslator? Translator, Dictionary<string, string> GestureTable)
        {
            foreach (InputBinding Binding in InputBindings)
                if (Binding is KeyBinding AsKeyBinding)
                    ParseKeyBinding(AsKeyBinding, Translator, GestureTable);
        }

        private static void ParseKeyBinding(KeyBinding Binding, IGestureTranslator? Translator, Dictionary<string, string> GestureTable)
        {
            string Key = string.Empty;

            switch (Binding.Command)
            {
                case ExtendedRoutedCommand AsExtendedCommand:
                    Key = AsExtendedCommand.MenuHeader;
                    break;

                case RoutedUICommand AsUICommand:
                    Key = GestureHelper.ApplicationCommandName(AsUICommand);
                    break;
            }

            if (Key.Length > 0 && !GestureTable.ContainsKey(Key))
                if (GestureToText(Binding.Gesture, Translator, out string GestureText))
                    GestureTable.Add(Key, GestureText);
        }

        private static bool GestureToText(InputGesture Gesture, IGestureTranslator? Translator, out string GestureText)
        {
            if (Gesture is KeyGesture AsKeyGesture)
            {
                string LocalizedText = AsKeyGesture.GetDisplayStringForCulture(CultureInfo.CurrentCulture);
                if (LocalizedText.Length > 0)
                {
                    GestureText = PostTranslateText(Translator, LocalizedText);
                    return true;
                }

                if (AsKeyGesture.DisplayString.Length > 0 && AsKeyGesture.DisplayString.Length > 0)
                {
                    GestureText = PostTranslateText(Translator, AsKeyGesture.DisplayString);
                    return true;
                }
            }

            GestureText = string.Empty;
            return false;
        }

        private static string PostTranslateText(IGestureTranslator? Translator, string Text)
        {
            if (Translator != null)
                return Translator.PostTranslate(Text);
            else
                return Text;
        }

        public static string ApplicationCommandName(RoutedCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            return "ApplicationCommand." + command.Name;
        }
        #endregion

    }
}
