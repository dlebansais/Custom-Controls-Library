namespace CustomControls
{
    using System;
    using System.Windows;
    using System.Windows.Threading;

    /// <summary>
    /// Represents the event data for a solution presenter event.
    /// </summary>
    /// <typeparam name="TEvent">The event data type.</typeparam>
    public class SolutionPresenterEventArgs<TEvent> : RoutedEventArgs
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionPresenterEventArgs{TEvent}"/> class.
        /// </summary>
        /// <param name="routedEvent">The event that occured.</param>
        /// <param name="eventContext">The event context.</param>
        public SolutionPresenterEventArgs(RoutedEvent routedEvent, object eventContext)
            : base(routedEvent)
        {
            EventContext = eventContext;
        }
        #endregion

        #region Properties and events
        /// <summary>
        /// Gets the event context.
        /// </summary>
        protected object EventContext { get; private set; }

        /// <summary>
        /// Occurs when the event is completed.
        /// </summary>
        public event EventHandler<SolutionPresenterEventCompletedEventArgs>? EventCompleted;

        /// <summary>
        /// Notifies handlers that the operation is completed.
        /// </summary>
        /// <param name="completionArgs">The completion arguments.</param>
        protected virtual void NotifyEventCompleted(object completionArgs)
        {
            EventCompleted?.Invoke(this, new SolutionPresenterEventCompletedEventArgs(EventContext, completionArgs));
        }

        /// <summary>
        /// Notifies handlers that the operation is completed.
        /// </summary>
        /// <param name="dispatcher">The window dispatcher.</param>
        /// <param name="completionArgs">The completion arguments.</param>
        protected virtual void NotifyEventCompletedAsync(Dispatcher dispatcher, object completionArgs)
        {
            if (dispatcher == null)
                throw new ArgumentNullException(nameof(dispatcher));

            dispatcher.BeginInvoke(new Action<object>(OnNotifyEventCompletedAsync), completionArgs);
        }

        /// <summary>
        /// Invokes handlers of the <see cref="EventCompleted"/> event.
        /// </summary>
        /// <param name="completionArgs">The event completion data.</param>
        protected virtual void OnNotifyEventCompletedAsync(object completionArgs)
        {
            EventCompleted?.Invoke(this, new SolutionPresenterEventCompletedEventArgs(EventContext, completionArgs));
        }
        #endregion

        #region Handlers
        /// <summary>
        /// Increments the number of handlers on this event.
        /// </summary>
#pragma warning disable CA1000 // Do not declare static members on generic types
        public static void IncrementHandlerCount()
#pragma warning restore CA1000 // Do not declare static members on generic types
        {
            HandlerCount++;
        }

        /// <summary>
        /// Decrements the number of handlers on this event.
        /// </summary>
#pragma warning disable CA1000 // Do not declare static members on generic types
        public static void DecrementHandlerCount()
#pragma warning restore CA1000 // Do not declare static members on generic types
        {
            HandlerCount--;
        }

        /// <summary>
        /// Gets a value indicating whether the solution presenter event has handlers.
        /// </summary>
#pragma warning disable CA1000 // Do not declare static members on generic types
        public static bool HasHandler { get { return HandlerCount > 0; } }
#pragma warning restore CA1000 // Do not declare static members on generic types

        private static int HandlerCount;
        #endregion
    }
}
