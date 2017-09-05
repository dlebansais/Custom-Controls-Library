namespace CustomControls
{
    public interface IApplicationStatus
    {
        string Text { get; }
        StatusType StatusType { get; }
    }

    public class ApplicationStatus : IApplicationStatus
    {
        #region Init
        public ApplicationStatus(string text, StatusType statusType)
        {
            this.Text = text;
            this.StatusType = statusType;
        }
        #endregion

        #region Properties
        public string Text { get; protected set; }
        public StatusType StatusType { get; protected set; }
        #endregion
    }
}
