using System.Diagnostics.CodeAnalysis;

namespace CustomControls
{
    [SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces", Justification = "Compile-time markers are allowed")]
    public interface IRemoveItemContext : IModifyContext
    {
    }

    public class RemoveItemContext : ModifyContext, IRemoveItemContext
    {
        public RemoveItemContext(int shownIndex)
            : base(shownIndex)
        {
        }

        public override void NextIndex()
        {
        }
    }
}
