namespace CustomControls
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using SolutionPresenterInternal.Properties;

    /// <summary>
    /// Represents an application status with localized text.
    /// </summary>
    public class LocalizedApplicationStatus : ApplicationStatus
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizedApplicationStatus"/> class.
        /// </summary>
        /// <param name="key">The text key.</param>
        /// <param name="statusType">The status type.</param>
        public LocalizedApplicationStatus(string key, StatusType statusType)
            : base(LocalizeText(key), statusType)
        {
        }

        /// <summary>
        /// Gets the localized text for a key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The localized text.</returns>
        protected static string LocalizeText(string key)
        {
            try
            {
                return Resources.ResourceManager.GetString(key, CultureInfo.CurrentCulture);
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
                Debug.Print("Key: " + key);

                if (e == null)
                    throw;
                else
                    return string.Empty;
            }
        }
        #endregion
    }
}
