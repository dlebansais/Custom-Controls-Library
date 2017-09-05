using System.Collections.Generic;
using System.Windows;

namespace CustomControls
{
    public class FolderEnumeratedEventArgs : SolutionPresenterEventArgs
    {
        private static int HandlerCount = 0;
        public static void IncrementHandlerCount() { HandlerCount++; }
        public static void DecrementHandlerCount() { HandlerCount--; }
        public static bool HasHandler { get { return HandlerCount > 0; } }

        public FolderEnumeratedEventArgs(RoutedEvent routedEvent, FolderEnumeratedEventContext eventContext)
            : base(routedEvent, eventContext)
        {
        }

        public IFolderPath ParentPath { get { return (IFolderPath)((FolderEnumeratedEventContext)EventContext).ParentPath; } }
        public IRootProperties RootProperties { get { return ((FolderEnumeratedEventContext)EventContext).RootProperties; } }
        public ICollection<IFolderPath> ExpandedFolderList { get { return ((FolderEnumeratedEventContext)EventContext).ExpandedFolderList; } }
        public object Context { get { return ((FolderEnumeratedEventContext)EventContext).Context; } }

        public virtual void NotifyCompleted(IReadOnlyList<ITreeNodePath> children, IReadOnlyDictionary<ITreeNodePath, ITreeNodeProperties> childrenProperties)
        {
            IFolderEnumeratedCompletionArgs CompletionArgs = new FolderEnumeratedCompletionArgs(children, childrenProperties);
            NotifyEventCompleted(CompletionArgs);
        }
    }
}
