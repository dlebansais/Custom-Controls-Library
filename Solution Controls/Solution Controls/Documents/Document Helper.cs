using System.Globalization;
using System.Windows;
using System.Windows.Input;

namespace CustomControls
{
    public static class DocumentHelper
    {
        public static string GetToolTipText(ICommand command, FrameworkElement source)
        {
            string Text = null;
            IDocument ActiveDocument;

            IActiveDocumentSource AsActiveDocumentSource;
            if ((AsActiveDocumentSource = source as IActiveDocumentSource) != null)
                ActiveDocument = AsActiveDocumentSource.ActiveDocument;
            else
                ActiveDocument = null;

            ActiveDocumentRoutedCommand AsActiveDocumentCommand;
            ExtendedRoutedCommand AsExtendedCommand;
            RoutedUICommand AsUICommand;

            if ((AsActiveDocumentCommand = command as ActiveDocumentRoutedCommand) != null)
            {
                if (ActiveDocument == null)
                    //Text = AsActiveDocumentCommand.GetLocalizedText(AsActiveDocumentCommand.InactiveToolTipKey);
                    Text = AsActiveDocumentCommand.InactiveButtonToolTip;
                else
                {
                    //string TextFormat = AsActiveDocumentCommand.GetLocalizedText(AsActiveDocumentCommand.ToolTipKey);
                    string TextFormat = AsActiveDocumentCommand.ButtonToolTip;
                    if (TextFormat != null)
                        Text = string.Format(CultureInfo.CurrentCulture, TextFormat, ActiveDocument.Path.HeaderName);
                }
            }

            else if ((AsExtendedCommand = command as ExtendedRoutedCommand) != null)
                //Text = AsExtendedCommand.GetLocalizedText(AsExtendedCommand.ToolTipKey);
                Text = AsExtendedCommand.ButtonToolTip;

            else if ((AsUICommand = command as RoutedUICommand) != null)
                Text = AsUICommand.Text;

            return Text;
        }
    }
}
