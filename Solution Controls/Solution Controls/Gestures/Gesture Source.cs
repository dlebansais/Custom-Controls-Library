using System.Collections.Generic;

namespace CustomControls
{
    public interface IGestureSource
    {
        IGestureTranslator GestureTranslator { get; }
    }
}
