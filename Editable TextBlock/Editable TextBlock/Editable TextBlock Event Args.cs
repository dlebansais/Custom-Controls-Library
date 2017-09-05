using System.Windows;

namespace CustomControls
{
    /// <summary>
    ///     Contains state information and event data associated with events from the <see cref="EditableTextBlock"/> control.
    /// </summary>
    public class EditableTextBlockEventArgs : RoutedEventArgs
    {
        #region Init
        /// <param name="routedEvent">The event this argument is associated to.</param>
        /// <param name="source">The control from which editing is left.</param>
        /// <param name="Text">The current content of the control.</param>
        /// <param name="Cancellation">A token to hold cancellation information.</param>
        internal EditableTextBlockEventArgs(RoutedEvent routedEvent, EditableTextBlock source, string Text, CancellationToken Cancellation)
            : base(routedEvent, source)
        {
            this.Text = Text;
            this.Cancellation = Cancellation;
        }
        #endregion

        #region Properties
        /// <summary>
        ///     Get a value that is the current content of the control.
        /// </summary>
        /// <returns>The current content of the control.</returns>
        public string Text { get; private set; }

        /// <summary>
        ///     Gets a token that indicates if the associated operation has been canceled by any handler.
        /// </summary>
        private CancellationToken Cancellation;
        #endregion

        #region Client Interface
        /// <summary>
        ///     Allows a handler to cancel the operation notified by the event associated to this object.
        /// </summary>
        public virtual void Cancel()
        {
            Cancellation.Cancel();
        }
        #endregion
    }
}
