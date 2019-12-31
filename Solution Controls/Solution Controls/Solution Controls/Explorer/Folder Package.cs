namespace CustomControls
{
    /// <summary>
    /// Represents a folder package.
    /// </summary>
    public class FolderPackage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FolderPackage"/> class.
        /// </summary>
        /// <param name="key">The folder key.</param>
        /// <param name="value">The folder value.</param>
        public FolderPackage(IFolderPath key, IFolderPath value)
        {
            Key = key;
            Value = value;
        }

        /// <summary>
        /// Gets the folder key.
        /// </summary>
        public IFolderPath Key { get; private set; }

        /// <summary>
        /// Gets the folder value.
        /// </summary>
        public IFolderPath Value { get; private set; }
    }
}
