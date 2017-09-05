using System.Security;

namespace CustomControls
{
    /// <summary>
    ///     Represents a password with security information.
    /// </summary>
    public class ExtendedPassword
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ExtendedPassword"/> class.
        /// </summary>
        /// <parameters>
        /// <param name="Password">A plain text password.</param>
        /// </parameters>
        /// <remarks>
        ///     The resulting object is considered not secure and the <see cref="IsSecure"/> property will return False.
        /// </remarks>
        internal ExtendedPassword(string Password)
        {
            this.IsSecure = false;
            this.Password = Password;
            this.SecurePassword = null;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExtendedPassword"/> class.
        /// </summary>
        /// <parameters>
        /// <param name="SecurePassword">A secure password.</param>
        /// </parameters>
        /// <remarks>
        ///     The resulting object is considered secure and the <see cref="IsSecure"/> property will return True.
        /// </remarks>
        internal ExtendedPassword(SecureString SecurePassword)
        {
            this.IsSecure = true;
            this.Password = null;
            this.SecurePassword = SecurePassword;
        }

        /// <summary>
        ///     Indicates if the password is secure and which property to use to read the password.
        /// </summary>
        /// <returns>
        ///     True if the password is secure, and then password should then be read using the <see cref="SecurePassword"/> property.
        ///     False if the password is not secure, and then password should then be read using the <see cref="Password"/> property.
        /// </returns>
        public bool IsSecure { get; private set; }

        /// <summary>
        ///     A plain text password.
        /// </summary>
        /// <returns>
        ///     The password as a plain text string. Null if the <see cref="IsSecure"/> property is True.
        /// </returns>
        public string Password { get; private set; }

        /// <summary>
        ///     A secure password.
        /// </summary>
        /// <returns>
        ///     The password as a secure string. Null if the <see cref="IsSecure"/> property is False.
        /// </returns>
        public SecureString SecurePassword { get; private set; }
    }
}
