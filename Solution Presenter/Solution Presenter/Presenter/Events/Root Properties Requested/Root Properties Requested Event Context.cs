using System.Collections.Generic;

namespace CustomControls
{
    public class RootPropertiesRequestedEventContext
    {
        public RootPropertiesRequestedEventContext(IRootProperties properties)
        {
            this.Properties = properties;
        }

        public IRootProperties Properties { get; private set; }
    }
}
