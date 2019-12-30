namespace CustomControls
{
    using System.Windows.Input;

    /// <summary>
    /// Represents the <see cref="ActiveCommand"/> object for the Retry command.
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
}