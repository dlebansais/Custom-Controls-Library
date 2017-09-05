namespace CustomControls
{
    public interface ITreeNodePath
    {
        string FriendlyName { get; }
        bool IsEqual(ITreeNodePath other);
        void ChangeFriendlyName(string newName);
    }
}
