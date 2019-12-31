namespace CustomControls
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a path for exchange with the clipboard.
    /// </summary>
    [Serializable]
    public class ClipboardPathData
    {
        #region Constants
        /// <summary>
        /// The clipboard data format.
        /// </summary>
        public static readonly string SolutionExplorerClipboardPathFormat = "185F4C03-D513-4F86-ADDB-C13C87417E81";
        #endregion

        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ClipboardPathData"/> class.
        /// </summary>
        /// <param name="pathGroup">The path group.</param>
        internal ClipboardPathData(IPathGroup pathGroup)
        {
            if (pathGroup == null)
                throw new ArgumentNullException(nameof(pathGroup));

            PathTable = pathGroup.PathTable;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the path table.
        /// </summary>
        public IReadOnlyDictionary<ITreeNodePath, IPathConnection> PathTable { get; private set; }
        #endregion
    }
}
