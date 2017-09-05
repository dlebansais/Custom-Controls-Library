using System;
using System.Collections.Generic;
using Verification;

namespace CustomControls
{
    [Serializable]
    public class ClipboardPathData
    {
        #region Constants
        public static readonly string SolutionExplorerClipboardPathFormat = "185F4C03-D513-4F86-ADDB-C13C87417E81";
        #endregion

        #region Init
        internal ClipboardPathData(IPathGroup pathGroup)
        {
            Assert.ValidateReference(pathGroup);

            PathTable = pathGroup.PathTable;
        }
        #endregion

        #region Properties
        public IReadOnlyDictionary<ITreeNodePath, IPathConnection> PathTable { get; private set; }
        #endregion
    }
}
