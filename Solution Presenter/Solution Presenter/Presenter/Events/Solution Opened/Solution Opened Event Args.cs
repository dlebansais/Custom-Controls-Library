using System.Collections.Generic;
using System.Windows;

namespace CustomControls
{
    public class SolutionOpenedEventArgs : SolutionPresenterEventArgs
    {
        private static int HandlerCount = 0;
        public static void IncrementHandlerCount() { HandlerCount++; }
        public static void DecrementHandlerCount() { HandlerCount--; }
        public static bool HasHandler { get { return HandlerCount > 0; } }

        public SolutionOpenedEventArgs(RoutedEvent routedEvent, SolutionOpenedEventContext eventContext)
            : base(routedEvent, eventContext)
        {
        }

        public IRootPath OpenedRootPath { get { return ((SolutionOpenedEventContext)EventContext).OpenedRootPath; } }

        public virtual void NotifyCompleted(IRootProperties openedRootProperties, IComparer<ITreeNodePath> openedRootComparer, IList<IFolderPath> expandedFolderList, object context)
        {
            ISolutionOpenedCompletionArgs CompletionArgs = new SolutionOpenedCompletionArgs(openedRootProperties, openedRootComparer, expandedFolderList, context);
            NotifyEventCompleted(CompletionArgs);
        }
    }
}
