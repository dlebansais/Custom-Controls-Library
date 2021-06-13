namespace CustomControls
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows.Controls.Primitives;

    /// <summary>
    /// Represents a control with a tree of nodes that can be moved around with Drag and Drop.
    /// </summary>
    public abstract partial class ExtendedTreeViewBase : MultiSelector
    {
        /// <summary>
        /// Logs a call entry.
        /// </summary>
        /// <param name="callerName">Name of the caller.</param>
        protected virtual void DebugCall([CallerMemberName] string callerName = "")
        {
            bool EnableTraces = ExtendedTreeViewBase.EnableTraces;
            if (EnableTraces)
                System.Diagnostics.Debug.Print(GetType().Name + ": " + callerName);
        }

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        [Localizable(false)]
        protected virtual void DebugMessage(string message)
        {
            bool EnableTraces = ExtendedTreeViewBase.EnableTraces;
            if (EnableTraces)
                System.Diagnostics.Debug.Print(GetType().Name + ": " + message);
        }
    }
}
