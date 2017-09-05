using CustomControls;
using SolutionPresenterInternal.Properties;
using System;
using System.Diagnostics;

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
                return Resources.ResourceManager.GetString(key);
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
