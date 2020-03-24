using System;
using System.Collections.Generic;

namespace stickypipes.api
{
    public static class SessionCache
    {
        public static Dictionary<Guid, string> SessionIdToOtherCacheLookups { get; set; } = new Dictionary<Guid, string>();
    }
}
