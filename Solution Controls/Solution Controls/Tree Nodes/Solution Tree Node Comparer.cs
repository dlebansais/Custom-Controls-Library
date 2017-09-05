using System.Collections.Generic;
using System.Globalization;

namespace CustomControls
{
    public class SolutionTreeNodeComparer : IComparer<ITreeNodePath>
    {
        public int Compare(ITreeNodePath x, ITreeNodePath y)
        {
            if (x != null && y != null)
            {
                IFolderPath xFolder = x as IFolderPath;
                IFolderPath yFolder = y as IFolderPath;

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
