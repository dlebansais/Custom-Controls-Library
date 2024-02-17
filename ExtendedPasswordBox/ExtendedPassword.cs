namespace CustomControls;

using System.Security;

/// <summary>
/// Represents a password with security information.
/// </summary>
public class ExtendedPassword
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExtendedPassword"/> class.
    /// </summary>
    /// <param name="password">A plain text password.</param>
    /// <remarks>
    /// The resulting object is considered not secure and the <see cref="IsSecure"/> property will return False.
    /// </remarks>
    internal ExtendedPassword(string password)
    {
        Password = password;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtendedPassword"/> class.
    /// </summary>
    /// <param name="securePassword">A secure password.</param>
    /// <remarks>
    /// The resulting object is considered secure and the <see cref="IsSecure"/> property will return True.
    /// </remarks>
    internal ExtendedPassword(SecureString securePassword)
    {
        IsSecure = true;
        Password = string.Empty;
        SecurePassword = securePassword;
    }

    /// <summary>
    /// Gets a value indicating whether the password is secure and which property to use to read the password.
    /// </summary>
    /// <returns>
    /// True if the password is secure, and then password should then be read using the <see cref="SecurePassword"/> property.
    /// False if the password is not secure, and then password should then be read using the <see cref="Password"/> property.
    /// </returns>
    public bool IsSecure { get; private set; }

    /// <summary>
    /// Gets a plain text password.
    /// </summary>
    /// <returns>
    /// The password as a plain text string. Null if the <see cref="IsSecure"/> property is True.
    /// </returns>
    public string Password { get; private set; } = string.Empty;

    /// <summary>
    /// Gets a secure password.
    /// </summary>
    /// <returns>
    /// The password as a secure string. Null if the <see cref="IsSecure"/> property is False.
    /// </returns>
    public SecureString SecurePassword { get; private set; } = new SecureString();
}
