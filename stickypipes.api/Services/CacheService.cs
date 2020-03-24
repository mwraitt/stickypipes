using System;
using System.Collections.Generic;
using System.Linq;

namespace stickypipes.api.Services
{
    public class CacheService
    {
        public void Create(Guid sessionId, string userName)
        {
            SessionCache.SessionIdToOtherCacheLookups.Add(sessionId, userName);
        }

        public void Delete()
        {

        }

        public bool TryGet(Guid id, out string username)
        {
            if (SessionCache.SessionIdToOtherCacheLookups.TryGetValue(id, out username))
            {
                return true;
            }

            return false;
        }

        public List<string> GetAll()
        {
            return SessionCache.SessionIdToOtherCacheLookups.Values.ToList();
        }
    }
}