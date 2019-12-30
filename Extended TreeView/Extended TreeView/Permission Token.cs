namespace CustomControls
{
    /// <summary>
    /// Contains permission information.
    /// </summary>
    public class PermissionToken
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the PermissionToken class.
        /// </summary>
        public PermissionToken()
        {
            this.IsAllowed = true;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Get a value that indicates if the associated operation has been authorized by any handler.
        /// </summary>
        /// <returns>True indicates that a handler has allowed the operation. False indicates that the operation associated to the event is denied.</returns>
        public bool IsAllowed { get; private set; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Allows a handler to deny the operation notified by the event associated to this object.
        /// </summary>
        public virtual void Deny()
        {
            IsAllowed = false;
        }
        #endregion
    }
}
