namespace CustomControls
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents the root folder of a solution.
    /// </summary>
    public interface ISolutionRoot : ISolutionFolder
    {
    }

    /// <summary>
    /// Represents the root folder of a solution.
    /// </summary>
    public class SolutionRoot : SolutionFolder, ISolutionRoot
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionRoot"/> class.
        /// </summary>
        /// <param name="path">The folder path.</param>
        /// <param name="properties">The folder properties.</param>
        /// <param name="nodeComparer">The node comparer.</param>
        public SolutionRoot(IRootPath path, IRootProperties properties, IComparer<ITreeNodePath> nodeComparer)
            : base(path, properties, nodeComparer)
        {
        }
        #endregion
    }
}
