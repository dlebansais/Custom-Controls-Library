using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;

namespace CustomControls
{
    public static class DocumentHelper
    {
        public static string GetToolTipText(ICommand command, FrameworkElement source)
        {
            string Text;

            switch (command)
            {
                case ActiveDocumentRoutedCommand AsActiveDocumentCommand:
                    if (source is IActiveDocumentSource AsActiveDocumentSource)
                    {
                        if (AsActiveDocumentSource.ActiveDocument is IDocument ActiveDocument)
                        {
                            string TextFormat = AsActiveDocumentCommand.ButtonToolTip;
                            Text = string.Format(CultureInfo.CurrentCulture, TextFormat, ActiveDocument.Path.HeaderName);
                        }
                        else
                            Text = string.Empty;
                    }
                    else
                        Text = AsActiveDocumentCommand.InactiveButtonToolTip;
                    break;

                case ExtendedRoutedCommand AsExtendedCommand:
                    Text = AsExtendedCommand.ButtonToolTip;
                    break;

                case RoutedUICommand AsUICommand:
                    Text = AsUICommand.Text;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(command));
            }

            return Text;
        }
    }
}
