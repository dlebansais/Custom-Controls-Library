using System.Windows.Media;

namespace CustomControls
{
    /// <summary>
    ///     Represents a command that extracts associated resources from localized standard resources.
    /// </summary>
    public class LocalizedRoutedCommand : ExtendedRoutedCommand
    {
        #region Constants
        /// <summary>
        ///     The pattern found in localized resource that can be replaced with the name of the application using the command.
        /// </summary>
        public static readonly string ApplicationNamePattern = "{ApplicationName}";
        #endregion

        #region Properties
        /// <summary>
        ///     Gets or sets the <see cref="CommandResourceReference"/> this command is associated to.
        /// </summary>
        public CommandResourceReference Reference { get; set; }
        /// <summary>
        ///     Gets or sets the key to use to find a localized menu header in standard resource files.
        /// </summary>
        public string HeaderKey { get; set; }
        /// <summary>
        ///     Gets or sets the key to use to find a localized tooltip in standard resource files.
        /// </summary>
        public string ToolTipKey { get; set; }
        /// <summary>
        ///     Gets or sets the key to use to find an image in standard resource files.
        /// </summary>
        public string IconKey { get; set; }

        /// <summary>
        ///     Gets the localized menu header.
        /// </summary>
        public override string MenuHeader { get { return Reference.GetString(HeaderKey); } }
        /// <summary>
        ///     Gets the localized tooltip.
        /// </summary>
        public override string ButtonToolTip { get { return Reference.GetString(ToolTipKey); } }
        /// <summary>
        ///     Gets the localized image.
        /// </summary>
        public override ImageSource ImageSource { get { return Reference.GetImageSource(IconKey); } }
        #endregion
    }
}
