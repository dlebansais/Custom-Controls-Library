namespace CustomControls
{
    /// <summary>
    /// Represents the status of an application.
    /// </summary>
    public interface IApplicationStatus
    {
        /// <summary>
        /// Gets the status text.
        /// </summary>
        string Text { get; }

        /// <summary>
        /// Gets the status type.
        /// </summary>
        StatusType StatusType { get; }
    }

    /// <summary>
    /// Represents the status of an application.
    /// </summary>
    public class ApplicationStatus : IApplicationStatus
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationStatus"/> class.
        /// </summary>
        /// <param name="text">The status text.</param>
        /// <param name="statusType">The status type.</param>
        public ApplicationStatus(string text, StatusType statusType)
        {
            Text = text;
            StatusType = statusType;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the status text.
        /// </summary>
        public string Text { get; protected set; }

        /// <summary>
        /// Gets or sets the status type.
        /// </summary>
        public StatusType StatusType { get; protected set; }
        #endregion
    }
}
