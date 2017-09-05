using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;

namespace CustomControls
{
    public class SolutionTreeCommittedEventArgs : SolutionPresenterEventArgs
    {
        private static int HandlerCount = 0;
        public static void IncrementHandlerCount() { HandlerCount++; }
        public static void DecrementHandlerCount() { HandlerCount--; }
        public static bool HasHandler { get { return HandlerCount > 0; } }

        public SolutionTreeCommittedEventArgs(RoutedEvent routedEvent, SolutionTreeCommittedEventContext eventContext)
            : base(routedEvent, eventContext)
        {
        }

        public IReadOnlyList<ITreeNodePath> DirtyItemList { get { return (IReadOnlyList<ITreeNodePath>)((SolutionTreeCommittedEventContext)EventContext).Info.DirtyItemList; } }
        public IReadOnlyList<ITreeNodePath> DirtyPropertiesList { get { return (IReadOnlyList<ITreeNodePath>)((SolutionTreeCommittedEventContext)EventContext).Info.DirtyPropertiesList; } }
        public IReadOnlyList<IDocument> DirtyDocumentList { get { return (IReadOnlyList<IDocument>)((SolutionTreeCommittedEventContext)EventContext).Info.DirtyDocumentList; } }

        public virtual void NotifyCompletedAsync(Dispatcher dispatcher)
        {
            ISolutionTreeCommittedCompletionArgs CompletionArgs = new SolutionTreeCommittedCompletionArgs();
            NotifyEventCompletedAsync(dispatcher, CompletionArgs);
        }
    }
}
