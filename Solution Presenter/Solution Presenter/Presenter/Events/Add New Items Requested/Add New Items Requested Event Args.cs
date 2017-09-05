using System.Collections.Generic;
using System.Windows;

namespace CustomControls
{
    public class AddNewItemsRequestedEventArgs : SolutionPresenterEventArgs
    {
        private static int HandlerCount = 0;
        public static void IncrementHandlerCount() { HandlerCount++; }
        public static void DecrementHandlerCount() { HandlerCount--; }
        public static bool HasHandler { get { return HandlerCount > 0; } }

        public AddNewItemsRequestedEventArgs(RoutedEvent routedEvent, IAddNewItemsRequestedEventContext eventContext)
            : base(routedEvent, eventContext)
        {
        }

        public IFolderPath DestinationFolderPath { get { return ((IAddNewItemsRequestedEventContext)EventContext).DestinationFolderPath; } }

        public virtual void NotifyCompleted(IList<IDocumentPath> documentPathList)
        {
            IAddNewItemsRequestedCompletionArgs CompletionArgs = new AddNewItemsRequestedCompletionArgs(documentPathList);
            NotifyEventCompleted(CompletionArgs);
        }
    }
}
