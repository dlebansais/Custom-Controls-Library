namespace CustomControls
{
    public interface IModifyContext
    {
        int ShownIndex { get; }
        void Start();
        void NextIndex();
        void Complete();
        void Close();
    }

    public abstract class ModifyContext : IModifyContext
    {
        protected ModifyContext(int shownIndex)
        {
            this.ShownIndex = shownIndex;
        }

        public int ShownIndex { get; protected set; }

        public virtual void Start()
        {
        }

        public abstract void NextIndex();

        public virtual void Complete()
        {
        }

        public virtual void Close()
        {
        }
    }
}
