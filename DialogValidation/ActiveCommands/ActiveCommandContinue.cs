namespace CustomControls
{
    using System.Windows.Input;

    /// <summary>
    /// Represents the <see cref="ActiveCommand"/> object for the Continue command.
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