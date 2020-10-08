namespace CustomControls
{
    /// <summary>
    /// Represents a context for the <see cref="AddNewItemsRequestedEventArgs"/> event data.
    /// </summary>
    public interface IAddNewItemsRequestedEventContext
    {
        /// <summary>
        /// Gets the destination folder path.
        /// </summary>
        IFolderPath DestinationFolderPath { get; }
    }

    /// <summary>
    /// Represents a context for the <see cref="AddNewItemsRequestedEventArgs"/> event data.
    /// </summary>
    public class AddNewItemsRequestedEventContext : IAddNewItemsRequestedEventContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddNewItemsRequestedEventContext"/> class.
        /// </summary>
        /// <param name="destinationFolderPath">The destination folder path.</param>
        public AddNewItemsRequestedEventContext(IFolderPath destinationFolderPath)
        {
            DestinationFolderPath = destinationFolderPath;
        }

        /// <summary>
        /// Gets the destination folder path.
        /// </summary>
        public IFolderPath DestinationFolderPath { get; private set; }
    }
}
