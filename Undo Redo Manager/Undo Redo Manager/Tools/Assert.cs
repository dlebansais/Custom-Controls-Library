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

        public static void CheckCondition(bool assertedCondition)
        {
            if (!assertedCondition)
                throw new ArgumentException("Condition not met", "assertedCondition");
        }
    }
}
