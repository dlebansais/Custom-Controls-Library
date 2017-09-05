using Converters;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;
using System.Windows.Markup;

namespace CustomControls
{
    /// <summary>
    ///     Represents a command that has a neutral name and a localized name.
    /// </summary>
    [ContentPropertyAttribute("Name")]
    [TypeConverter(typeof(ActiveCommandTypeConverter))]
    public abstract class ActiveCommand
    {
        #region Global Commands
        /// <summary>
        ///     The OK command.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification="No mutable fields in this object")]
        public static readonly ActiveCommand Ok = new ActiveCommandOk();

        /// <summary>
        ///     The Cancel command.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "No mutable fields in this object")]
        public static readonly ActiveCommand Cancel = new ActiveCommandCancel();

        /// <summary>
        ///     The Abort command.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "No mutable fields in this object")]
        public static readonly ActiveCommand Abort = new ActiveCommandAbort();

        /// <summary>
        ///     The Retry command.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "No mutable fields in this object")]
        public static readonly ActiveCommand Retry = new ActiveCommandRetry();

        /// <summary>
        ///     The Ignore command.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "No mutable fields in this object")]
        public static readonly ActiveCommand Ignore = new ActiveCommandIgnore();

        /// <summary>
        ///     The Yes command.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "No mutable fields in this object")]
        public static readonly ActiveCommand Yes = new ActiveCommandYes();

        /// <summary>
        ///     The No command.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "No mutable fields in this object")]
        public static readonly ActiveCommand No = new ActiveCommandNo();

        /// <summary>
        ///     The Close command.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "No mutable fields in this object")]
        public static readonly ActiveCommand Close = new ActiveCommandClose();

        /// <summary>
        ///     The Help command.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "No mutable fields in this object")]
        public static readonly ActiveCommand Help = new ActiveCommandHelp();

        /// <summary>
        ///     The Try Again command.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "No mutable fields in this object")]
        public static readonly ActiveCommand TryAgain = new ActiveCommandTryAgain();

        /// <summary>
        ///     The Continue command.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "No mutable fields in this object")]
        public static readonly ActiveCommand Continue = new ActiveCommandContinue();

        /// <summary>
        ///     A collection of all known commands.
        /// </summary>
        public static ICollection<ActiveCommand> AllCommands { get { return new Collection<ActiveCommand>(_AllCommands); } }
        private static readonly ActiveCommand[] _AllCommands = new ActiveCommand[] { Ok, Cancel, Abort, Retry, Ignore, Yes, No, Close, Help, TryAgain, Continue };
        #endregion

        #region Init
        /// <summary>
        ///     Ensure only the static objects exist and no other can be created.
        /// </summary>
        protected ActiveCommand()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        ///     Gets the neutral name of the <see cref="ActiveCommand"/> object.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        ///     Gets the localized name of the <see cref="ActiveCommand"/> object.
        /// </summary>
        public abstract string FriendlyName { get; }

        /// <summary>
        ///     Gets the routed command of the <see cref="ActiveCommand"/> object.
        /// </summary>
        public abstract RoutedUICommand Command { get; }

        /// <summary>
        ///     Gets a flag that indicates if a button using this command should be a default button.
        /// </summary>
        public virtual bool IsDefault { get { return false; } }

        /// <summary>
        ///     Gets a flag that indicates if a button using this command should be a cancel button.
        /// </summary>
        public virtual bool IsCancel { get { return false; } }
        #endregion
    }

    /// <summary>
    ///     Represents the <see cref="ActiveCommand"/> object for the OK command.
    /// </summary>
    public class ActiveCommandOkBase : ActiveCommand
    {
        /// <summary>Gets the neutral name of the <see cref="ActiveCommandOkBase"/> object.</summary>
        public override string Name { get { return "Ok"; } }
        /// <summary>Gets the localized name of the <see cref="ActiveCommandOkBase"/> object.</summary>
        public override string FriendlyName { get { return "OK"; } }
        /// <summary>Gets the routed command of the <see cref="ActiveCommandOkBase"/> object.</summary>
        public override RoutedUICommand Command { get { return DialogValidation.DefaultCommandOk; } }
    }

    /// <summary>
    ///     Represents the <see cref="ActiveCommand"/> object for the OK command with a flag to indicate a button associated to this command should be a default button.
    /// </summary>
    public class ActiveCommandOk : ActiveCommandOkBase
    {
        /// <summary>
        ///     Gets a flag that indicates that a button using this command should be a default button.
        /// </summary>
        public override bool IsDefault { get { return true; } }
    }

    /// <summary>Represents the <see cref="ActiveCommand"/> object for the Cancel command.</summary>
    public class ActiveCommandCancelBase : ActiveCommand
    {
        /// <summary>Gets the neutral name of the <see cref="ActiveCommandCancelBase"/> object.</summary>
        public override string Name { get { return "Cancel"; } }
        /// <summary>Gets the localized name of the <see cref="ActiveCommandCancelBase"/> object.</summary>
        public override string FriendlyName { get { return "Cancel"; } }
        /// <summary>Gets the routed command of the <see cref="ActiveCommandCancelBase"/> object.</summary>
        public override RoutedUICommand Command { get { return DialogValidation.DefaultCommandCancel; } }
    }

    /// <summary>
    ///     Represents the <see cref="ActiveCommand"/> object for the Cancel command with a flag to indicate a button associated to this command should be a cancel button.
    /// </summary>
    public class ActiveCommandCancel : ActiveCommandCancelBase
    {
        /// <summary>Gets a flag that indicates that a button using this command should be a cancel button.</summary>
        public override bool IsCancel { get { return true; } }
    }

    /// <summary>
    ///     Represents the <see cref="ActiveCommand"/> object for the Abort command.
    /// </summary>
    public class ActiveCommandAbort : ActiveCommand
    {
        /// <summary>Gets the neutral name of the <see cref="ActiveCommandAbort"/> object.</summary>
        public override string Name { get { return "Abort"; } }
        /// <summary>Gets the localized name of the <see cref="ActiveCommandAbort"/> object.</summary>
        public override string FriendlyName { get { return "_Abort"; } }
        /// <summary>Gets the routed command of the <see cref="ActiveCommandAbort"/> object.</summary>
        public override RoutedUICommand Command { get { return DialogValidation.DefaultCommandAbort; } }
    }

    /// <summary>
    ///     Represents the <see cref="ActiveCommand"/> object for the Retry command.
    /// </summary>
    public class ActiveCommandRetry : ActiveCommand
    {
        /// <summary>Gets the neutral name of the <see cref="ActiveCommandRetry"/> object.</summary>
        public override string Name { get { return "Retry"; } }
        /// <summary>Gets the localized name of the <see cref="ActiveCommandRetry"/> object.</summary>
        public override string FriendlyName { get { return "_Retry"; } }
        /// <summary>Gets the routed command of the <see cref="ActiveCommandRetry"/> object.</summary>
        public override RoutedUICommand Command { get { return DialogValidation.DefaultCommandRetry; } }
    }

    /// <summary>
    ///     Represents the <see cref="ActiveCommand"/> object for the Ignore command.
    /// </summary>
    public class ActiveCommandIgnore : ActiveCommand
    {
        /// <summary>Gets the neutral name of the <see cref="ActiveCommandIgnore"/> object.</summary>
        public override string Name { get { return "Ignore"; } }
        /// <summary>Gets the localized name of the <see cref="ActiveCommandIgnore"/> object.</summary>
        public override string FriendlyName { get { return "_Ignore"; } }
        /// <summary>Gets the routed command of the <see cref="ActiveCommandIgnore"/> object.</summary>
        public override RoutedUICommand Command { get { return DialogValidation.DefaultCommandIgnore; } }
    }

    /// <summary>
    ///     Represents the <see cref="ActiveCommand"/> object for the Yes command.
    /// </summary>
    public class ActiveCommandYes : ActiveCommand
    {
        /// <summary>Gets the neutral name of the <see cref="ActiveCommandYes"/> object.</summary>
        public override string Name { get { return "Yes"; } }
        /// <summary>Gets the localized name of the <see cref="ActiveCommandYes"/> object.</summary>
        public override string FriendlyName { get { return "_Yes"; } }
        /// <summary>Gets the routed command of the <see cref="ActiveCommandYes"/> object.</summary>
        public override RoutedUICommand Command { get { return DialogValidation.DefaultCommandYes; } }
    }

    /// <summary>
    ///     Represents the <see cref="ActiveCommand"/> object for the No command.
    /// </summary>
    public class ActiveCommandNo : ActiveCommand
    {
        /// <summary>Gets the neutral name of the <see cref="ActiveCommandNo"/> object.</summary>
        public override string Name { get { return "No"; } }
        /// <summary>Gets the localized name of the <see cref="ActiveCommandNo"/> object.</summary>
        public override string FriendlyName { get { return "_No"; } }
        /// <summary>Gets the routed command of the <see cref="ActiveCommandNo"/> object.</summary>
        public override RoutedUICommand Command { get { return DialogValidation.DefaultCommandNo; } }
    }

    /// <summary>
    ///     Represents the <see cref="ActiveCommand"/> object for the Close command.
    /// </summary>
    public class ActiveCommandClose : ActiveCommand
    {
        /// <summary>Gets the neutral name of the <see cref="ActiveCommandClose"/> object.</summary>
        public override string Name { get { return "Close"; } }
        /// <summary>Gets the localized name of the <see cref="ActiveCommandClose"/> object.</summary>
        public override string FriendlyName { get { return "_Close"; } }
        /// <summary>Gets the routed command of the <see cref="ActiveCommandClose"/> object.</summary>
        public override RoutedUICommand Command { get { return DialogValidation.DefaultCommandClose; } }
    }

    /// <summary>
    ///     Represents the <see cref="ActiveCommand"/> object for the Help command.
    /// </summary>
    public class ActiveCommandHelp : ActiveCommand
    {
        /// <summary>Gets the neutral name of the <see cref="ActiveCommandHelp"/> object.</summary>
        public override string Name { get { return "Help"; } }
        /// <summary>Gets the localized name of the <see cref="ActiveCommandHelp"/> object.</summary>
        public override string FriendlyName { get { return "Help"; } }
        /// <summary>Gets the routed command of the <see cref="ActiveCommandHelp"/> object.</summary>
        public override RoutedUICommand Command { get { return DialogValidation.DefaultCommandHelp; } }
    }

    /// <summary>
    ///     Represents the <see cref="ActiveCommand"/> object for the Try Again command.
    /// </summary>
    public class ActiveCommandTryAgain : ActiveCommand
    {
        /// <summary>Gets the neutral name of the <see cref="ActiveCommandTryAgain"/> object.</summary>
        public override string Name { get { return "TryAgain"; } }
        /// <summary>Gets the localized name of the <see cref="ActiveCommandTryAgain"/> object.</summary>
        public override string FriendlyName { get { return "_Try Again"; } }
        /// <summary>Gets the routed command of the <see cref="ActiveCommandTryAgain"/> object.</summary>
        public override RoutedUICommand Command { get { return DialogValidation.DefaultCommandTryAgain; } }
    }

    /// <summary>
    ///     Represents the <see cref="ActiveCommand"/> object for the Continue command.
    /// </summary>
    public class ActiveCommandContinue : ActiveCommand
    {
        /// <summary>Gets the neutral name of the <see cref="ActiveCommandContinue"/> object.</summary>
        public override string Name { get { return "Continue"; } }
        /// <summary>Gets the localized name of the <see cref="ActiveCommandContinue"/> object.</summary>
        public override string FriendlyName { get { return "_Continue"; } }
        /// <summary>Gets the routed command of the <see cref="ActiveCommandContinue"/> object.</summary>
        public override RoutedUICommand Command { get { return DialogValidation.DefaultCommandContinue; } }
    }
}
