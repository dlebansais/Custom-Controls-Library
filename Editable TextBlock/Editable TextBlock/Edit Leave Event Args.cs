using System.Windows;

namespace CustomControls
{
    /// <summary>
    /// Contains state information and event data associated with the EditLeave event.
    /// </summary>
    public class EditLeaveEventArgs : EditableTextBlockEventArgs
    {
        #region Init
        /// <summary>
        /// If IsEditCanceled is true, the IsCanceled member will be ignored by the sender.
        /// </summary>
        /// <param name="routedEvent">The event this argument is associated to.</param>
        /// <param name="source">The control from which editing is left.</param>
        /// <param name="Text">The current content of the control.</param>
        /// <param name="Cancellation">A token to hold cancellation information.</param>
        /// <param name="IsEditCanceled">A value that indicates if editing has been canceled.</param>
        internal EditLeaveEventArgs(RoutedEvent routedEvent, EditableTextBlock source, string Text, CancellationToken Cancellation, bool IsEditCanceled)
            : base(routedEvent, source, Text, Cancellation)
        {
            this.IsEditCanceled = IsEditCanceled;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Get a value that indicates if editing has been canceled.
        /// </summary>
        /// <returns>True indicates that editing has been canceled. False indicates that editing has been completed successfully.</returns>
        public bool IsEditCanceled { get; private set; }
        #endregion
    }
}
