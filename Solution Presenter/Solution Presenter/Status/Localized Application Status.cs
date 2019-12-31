﻿namespace CustomControls
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using SolutionPresenterInternal.Properties;

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
                    return string.Empty;
            }
        }
        #endregion
    }
}
