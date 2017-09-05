using System.Collections.Generic;

namespace CustomControls
{
    public interface ISolutionRoot : ISolutionFolder
    {
    }

    public class SolutionRoot : SolutionFolder, ISolutionRoot
    {
        #region Init
        public SolutionRoot(IRootPath path, IRootProperties properties, IComparer<ITreeNodePath> nodeComparer)
            : base(path, properties, nodeComparer)
        {
        }
        #endregion
    }
}
