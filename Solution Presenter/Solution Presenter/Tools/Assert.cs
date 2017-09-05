using System;

namespace Verification
{
    internal static class Assert
    {
        public static void ValidateReference(object referenceValue)
        {
            if (referenceValue == null)
                throw new ArgumentNullException("referenceValue", "Invalid null reference");
        }
    }
}
