namespace CustomControls
{
    public interface IAddNewItemsRequestedEventContext
    {
        IFolderPath DestinationFolderPath { get; }
    }

    public class AddNewItemsRequestedEventContext : IAddNewItemsRequestedEventContext
    {
        public AddNewItemsRequestedEventContext(IFolderPath destinationFolderPath)
        {
            this.DestinationFolderPath = destinationFolderPath;
        }

        public IFolderPath DestinationFolderPath { get; private set; }
    }
}
