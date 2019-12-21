using SolutionPresenterInternal.Properties;
using System;
using System.Diagnostics;
using System.Globalization;

namespace CustomControls
{
    public class LocalizedApplicationStatus : ApplicationStatus
    {
        #region Init
        public LocalizedApplicationStatus(string key, StatusType statusType)
            : base(LocalizeText(key), statusType)
        {
        }

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
                    return "";
            }
        }
        #endregion
    }
}
