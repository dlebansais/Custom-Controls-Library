namespace CustomControls
{
    /// <summary>
    /// Specifies how to proceed to a commit.
    /// </summary>
    public enum CommitOption
    {
        /// <summary>
        /// Stop the commit.
        /// </summary>
        Stop,

        /// <summary>
        /// Continue the commit with saving.
        /// </summary>
        Continue,

        /// <summary>
        /// Commit on file and continue.
        /// </summary>
        CommitAndContinue,
    }
}
