using System;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using stickypipes.api.Services;

namespace stickypipes.api.Hubs
{
    public class StickyHub : Hub
    {
        private readonly CacheService _cache;

        public StickyHub(CacheService cache)
        {
            _cache = cache;
        }

        public async Task Register(string username)
        {

            _cache.Create(Context.ConnectionId, username);

            string message = $"{username} : {Context.ConnectionId}";
            await Clients.Caller.SendAsync("registered", message, Context.ConnectionId);
        }

        public async Task WhoAmI()
        {
            if (!_cache.TryGet(Context.ConnectionId, out string username))
            {
                username = $"unable to find anyone matching {Context.ConnectionId}";
            }

            await Clients.All.SendAsync("thisisyou", username);
        }

        public ChannelReader<Session> StreamValues()
        {
            return _cache.StreamValues(Context.ConnectionId).AsChannelReader(10);
        }
    }
}