using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using stickypipes.api.Services;

namespace stickypipes.api.Hubs
{
    public class StickyHub : Hub
    {
        private readonly CacheService _cache;
        private readonly Guid _sessionId;

        public StickyHub(CacheService cache)
        {
            _cache = cache;
            _sessionId = Guid.NewGuid();
        }

        public async Task Register(string username)
        {
            _cache.Create(_sessionId, username);

            string message = $"{username} : {_sessionId}";
            await Clients.Caller.SendAsync("registered", message, _sessionId);
        }

        public async Task WhoAmI()
        {
            await WhoIsThis(_sessionId);
        }

        public async Task WhoIsThis(Guid id)
        {
            if (!_cache.TryGet(id, out string username))
            {
                username = $"unable to find anyone matching {id}";
            }

            await Clients.All.SendAsync("thisisyou", username);
        }
    }
}