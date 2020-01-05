namespace CustomControls
{
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// Represents a comparer of tree nodes.
    /// </summary>
    public class SolutionTreeNodeComparer : IComparer<ITreeNodePath>
    {
        /// <summary>
        /// Compares tree nodes of a solution.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>A signed integer that indicates the relative values of x and y.</returns>
        public int Compare(ITreeNodePath x, ITreeNodePath y)
        {
            if (x != null && y != null)
            {
                IFolderPath? xFolder = x as IFolderPath;
                IFolderPath? yFolder = y as IFolderPath;

                if (xFolder != null && yFolder == null)
                    return -1;
                else if (xFolder == null && yFolder != null)
                    return +1;
                else
                    return string.Compare(x.FriendlyName, y.FriendlyName, false, CultureInfo.CurrentCulture);
            }
            else
                return 0;
        }
    }
}
