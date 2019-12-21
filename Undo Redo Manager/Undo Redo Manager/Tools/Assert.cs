using System;

namespace Verification
{
    internal static class Assert
    {
        public static void ValidateReference(object referenceValue)
        {
            if (referenceValue == null)
                throw new ArgumentNullException(nameof(referenceValue));
        }

        public static void CheckCondition(bool assertedCondition)
        {
            if (!assertedCondition)
                throw new ArgumentException("Condition not met", nameof(assertedCondition));
        }
    }
}
