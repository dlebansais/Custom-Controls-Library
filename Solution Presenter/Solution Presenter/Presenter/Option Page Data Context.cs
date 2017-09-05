namespace CustomControls
{
    public interface IOptionPageDataContext
    {
        IOptionPageDataContext Backup();
        void Restore(IOptionPageDataContext backupDataContext);
        bool IsCloseAllowed { get; }
    }
}
