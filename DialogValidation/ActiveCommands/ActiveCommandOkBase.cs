namespace CustomControls
{
    using System.Windows.Input;

    /// <summary>
    /// Represents the <see cref="ActiveCommand"/> object for the OK command.
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
}
