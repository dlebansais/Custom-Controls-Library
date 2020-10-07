namespace CustomControls
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// Contains a set of tools for documents.
    /// </summary>
    public static class DocumentHelper
    {
        /// <summary>
        /// Gets the text of the tooltip of a command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="source">The control to which the command applies.</param>
        /// <returns>The tooltip text.</returns>
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
