namespace CustomControls;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;
using System.Windows.Markup;

/// <summary>
/// Represents a command that has a neutral name and a localized name.
/// </summary>
[ContentPropertyAttribute("Name")]
[TypeConverter(typeof(ActiveCommandTypeConverter))]
public abstract class ActiveCommand
{
    #region Global Commands
    /// <summary>
    /// The OK command.
    /// </summary>
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification="No mutable fields in this object")]
    public static readonly ActiveCommand Ok = new ActiveCommandOk();

    /// <summary>
    /// The Cancel command.
    /// </summary>
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "No mutable fields in this object")]
    public static readonly ActiveCommand Cancel = new ActiveCommandCancel();

    /// <summary>
    /// The Abort command.
    /// </summary>
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "No mutable fields in this object")]
    public static readonly ActiveCommand Abort = new ActiveCommandAbort();

    /// <summary>
    /// The Retry command.
    /// </summary>
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "No mutable fields in this object")]
    public static readonly ActiveCommand Retry = new ActiveCommandRetry();

    /// <summary>
    /// The Ignore command.
    /// </summary>
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "No mutable fields in this object")]
    public static readonly ActiveCommand Ignore = new ActiveCommandIgnore();

    /// <summary>
    /// The Yes command.
    /// </summary>
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "No mutable fields in this object")]
    public static readonly ActiveCommand Yes = new ActiveCommandYes();

    /// <summary>
    /// The No command.
    /// </summary>
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "No mutable fields in this object")]
    public static readonly ActiveCommand No = new ActiveCommandNo();

    /// <summary>
    /// The Close command.
    /// </summary>
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "No mutable fields in this object")]
    public static readonly ActiveCommand Close = new ActiveCommandClose();

    /// <summary>
    /// The Help command.
    /// </summary>
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "No mutable fields in this object")]
    public static readonly ActiveCommand Help = new ActiveCommandHelp();

    /// <summary>
    /// The Try Again command.
    /// </summary>
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "No mutable fields in this object")]
    public static readonly ActiveCommand TryAgain = new ActiveCommandTryAgain();

    /// <summary>
    /// The Continue command.
    /// </summary>
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "No mutable fields in this object")]
    public static readonly ActiveCommand Continue = new ActiveCommandContinue();

    /// <summary>
    /// Gets a collection of all known commands.
    /// </summary>
    public static ICollection<ActiveCommand> AllCommands { get { return new Collection<ActiveCommand>(new ActiveCommand[] { Ok, Cancel, Abort, Retry, Ignore, Yes, No, Close, Help, TryAgain, Continue }); } }
    #endregion

    #region Init
    /// <summary>
    /// Initializes a new instance of the <see cref="ActiveCommand"/> class.
    /// Ensure only the static objects exist and no other can be created.
    /// </summary>
    protected ActiveCommand()
    {
    }
    #endregion

    #region Properties
    /// <summary>
    /// Gets the neutral name of the <see cref="ActiveCommand"/> object.
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Gets the localized name of the <see cref="ActiveCommand"/> object.
    /// </summary>
    public abstract string FriendlyName { get; }

    /// <summary>
    /// Gets the routed command of the <see cref="ActiveCommand"/> object.
    /// </summary>
    public abstract RoutedUICommand Command { get; }

    /// <summary>
    /// Gets a value indicating whether a button using this command should be a default button.
    /// </summary>
    public virtual bool IsDefault { get { return false; } }

    /// <summary>
    /// Gets a value indicating whether a button using this command should be a cancel button.
    /// </summary>
    public virtual bool IsCancel { get { return false; } }
    #endregion
}
