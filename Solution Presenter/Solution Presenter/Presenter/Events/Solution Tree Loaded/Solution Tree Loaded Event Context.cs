namespace CustomControls
{
    public class SolutionTreeLoadedEventContext
    {
        public SolutionTreeLoadedEventContext(bool isCanceled)
        {
            this.IsCanceled = isCanceled;
        }

        public bool IsCanceled { get; private set; }
    }
}
