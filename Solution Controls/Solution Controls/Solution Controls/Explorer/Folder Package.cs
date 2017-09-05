namespace CustomControls
{
    public class FolderPackage
    {
        public FolderPackage(IFolderPath key, IFolderPath value)
        {
            this.Key = key;
            this.Value = value;
        }

        public IFolderPath Key { get; private set; }
        public IFolderPath Value { get; private set; }
    }
}
