namespace UndoRedo
{
    internal class IdentityOperation : IReversibleOperation
    {
        #region Init
        private IdentityOperation()
        {
        }

        public static IdentityOperation Default = new IdentityOperation();
        #endregion

        #region Properties
        public string Name { get; } = string.Empty;
        #endregion

        #region Client Interface
        public void Redo()
        {
        }

        public void Undo()
        {
        }
        #endregion
    }
}
