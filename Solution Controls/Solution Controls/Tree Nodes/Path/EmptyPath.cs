namespace CustomControls
{
    public class EmptyPath : IRootPath
    {
        public string FriendlyName { get; } = string.Empty;

        public bool IsEqual(ITreeNodePath other)
        {
            return false;
        }

        public void ChangeFriendlyName(string newName)
        {
        }
    }
}
