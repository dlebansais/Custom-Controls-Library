namespace CustomControls
{
    using System.Windows.Input;
    using System.Windows.Media;

    /// <summary>
    /// Represents a command with information to find associated resources.
    /// </summary>
    public abstract class ExtendedRoutedCommand : RoutedCommand
    {
        #region Properties
        /// <summary>
        /// Gets or sets the <see cref="CommandGroup"/> this command is associated to.
        /// </summary>
        public SolutionCommandGroup CommandGroup { get; set; } = SolutionCommandGroup.Default;
        /// <summary>
        /// Gets the name of the resource for menu headers using this command.
        /// </summary>
        public abstract string MenuHeader { get; }
        /// <summary>
        /// Gets the source of the resource for buttons using this command.
        /// </summary>
        public abstract ImageSource ImageSource { get; }
        /// <summary>
        /// Gets the name of the resource for tooltips on controls using this command.
        /// </summary>
        public abstract string ButtonToolTip { get; }
        #endregion
    }
}
