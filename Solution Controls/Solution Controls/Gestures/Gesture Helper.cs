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

            string GestureText = null;

            ExtendedRoutedCommand AsExtendedCommand;
            RoutedUICommand AsUICommand;

            if ((AsExtendedCommand = command as ExtendedRoutedCommand) != null)
            {
                if (AsExtendedCommand.MenuHeader != null && GestureTable.ContainsKey(AsExtendedCommand.MenuHeader))
                    GestureText = GestureTable[AsExtendedCommand.MenuHeader];
            }

            else if ((AsUICommand = command as RoutedUICommand) != null)
            {
                string DisplayString;
                if (GetCustomGestureText(AsUICommand, GestureTable, out DisplayString) || GetSystemGestureText(AsUICommand, source, out DisplayString))
                    GestureText = DisplayString;
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

            DisplayString = null;
            return false;
        }

        private static bool GetSystemGestureText(RoutedUICommand Command, FrameworkElement Source, out string DisplayString)
        {
            IGestureSource AsGestureSource;
            if ((AsGestureSource = Source as IGestureSource) != null)
            {
                IGestureTranslator Translator = AsGestureSource.GestureTranslator;
                if (Translator != null)
                    foreach (InputGesture Gesture in Command.InputGestures)
                        if (GestureToText(Gesture, Translator, out DisplayString))
                            return true;
            }

            DisplayString = null;
            return false;
        }
        #endregion

        #region Gesture Table
        private static Dictionary<string, string> GetGestureTable(FrameworkElement Source)
        {
            if (Source == null)
                throw new ArgumentNullException(nameof(Source));

            Dictionary<string, string> GestureTable = new Dictionary<string, string>();
            FrameworkElement Element = Source;
            IGestureTranslator Translator = null;

            for (; ; )
            {
                IGestureSource AsGestureSource;
                if ((AsGestureSource = Element as IGestureSource) != null)
                    Translator = AsGestureSource.GestureTranslator;

                ParseKeyBindings(Element.InputBindings, Translator, GestureTable);

                FrameworkElement AsParentElement;
                if ((AsParentElement = Element.Parent as FrameworkElement) != null)
                    Element = AsParentElement;
                else
                    break;
            }

            return GestureTable;
        }

        private static void ParseKeyBindings(InputBindingCollection InputBindings, IGestureTranslator Translator, Dictionary<string, string> GestureTable)
        {
            foreach (InputBinding Binding in InputBindings)
            {
                KeyBinding AsKeyBinding;
                if ((AsKeyBinding = Binding as KeyBinding) != null)
                    ParseKeyBinding(AsKeyBinding, Translator, GestureTable);
            }
        }

        private static void ParseKeyBinding(KeyBinding Binding, IGestureTranslator Translator, Dictionary<string, string> GestureTable)
        {
            string Key;
            ExtendedRoutedCommand AsExtendedCommand;
            RoutedUICommand AsUICommand;

            if ((AsExtendedCommand = Binding.Command as ExtendedRoutedCommand) != null)
                Key = AsExtendedCommand.MenuHeader;

            else if ((AsUICommand = Binding.Command as RoutedUICommand) != null)
                Key = GestureHelper.ApplicationCommandName(AsUICommand);

            else
                Key = null;

            if (Key != null && !GestureTable.ContainsKey(Key))
            {
                string GestureText;
                if (GestureToText(Binding.Gesture, Translator, out GestureText))
                    GestureTable.Add(Key, GestureText);
            }
        }

        private static bool GestureToText(InputGesture Gesture, IGestureTranslator Translator, out string GestureText)
        {
            KeyGesture AsKeyGesture;
            if ((AsKeyGesture = Gesture as KeyGesture) != null)
            {
                string LocalizedText = AsKeyGesture.GetDisplayStringForCulture(CultureInfo.CurrentCulture);
                if (LocalizedText != null && LocalizedText.Length > 0)
                {
                    GestureText = PostTranslateText(Translator, LocalizedText);
                    return true;
                }

                if (AsKeyGesture.DisplayString != null && AsKeyGesture.DisplayString.Length > 0)
                {
                    GestureText = PostTranslateText(Translator, AsKeyGesture.DisplayString);
                    return true;
                }
            }

            GestureText = null;
            return false;
        }

        private static string PostTranslateText(IGestureTranslator Translator, string Text)
        {
            if (Translator == null)
                return Text;
            else
                return Translator.PostTranslate(Text);
        }

        public static string ApplicationCommandName(RoutedCommand command)
        {
            if (command != null)
                return "ApplicationCommand." + command.Name;
            else
                return null;
        }
        #endregion

    }
}
