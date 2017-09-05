using System.Diagnostics.CodeAnalysis;

namespace CustomControls
{
    [SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces", Justification="Compile-time markers are allowed")]
    public interface IInsertItemContext : IModifyContext
    {
    }

    public class InsertItemContext : ModifyContext, IInsertItemContext
    {
        public InsertItemContext(int shownIndex)
            : base(shownIndex)
        {
        }

        public override void NextIndex()
        {
            ShownIndex++;
        }
    }
}
