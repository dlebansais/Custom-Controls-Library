namespace CustomControls
{
    /// <summary>
    /// Represents a context for the <see cref="SolutionTreeLoadedEventArgs"/> event data.
    /// </summary>
    public class SolutionTreeLoadedEventContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionTreeLoadedEventContext"/> class.
        /// </summary>
        /// <param name="isCanceled">True if the load has been canceled.</param>
        public SolutionTreeLoadedEventContext(bool isCanceled)
        {
            IsCanceled = isCanceled;
        }

        /// <summary>
        /// Gets a value indicating whether the load has been canceled.
        /// </summary>
        public bool IsCanceled { get; private set; }
    }
}
