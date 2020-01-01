namespace CustomControls
{
    /// <summary>
    /// Represents a context for the option page.
    /// </summary>
    public interface IOptionPageDataContext
    {
        /// <summary>
        /// Gets the page backup.
        /// </summary>
        /// <returns>The page backup.</returns>
        IOptionPageDataContext Backup();

        /// <summary>
        /// Restores the page from a backup.
        /// </summary>
        /// <param name="backupDataContext">The backup.</param>
        void Restore(IOptionPageDataContext backupDataContext);

        /// <summary>
        /// Gets a value indicating whether closing the page is allowed.
        /// </summary>
        bool IsCloseAllowed { get; }
    }
}
