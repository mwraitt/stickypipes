using System;
using System.Collections.Concurrent;
using System.Reactive.Subjects;
using System.Threading;

namespace stickypipes.api.Services
{
    public class CacheService
    {
        private readonly SemaphoreSlim _updateValueLock = new SemaphoreSlim(1, 1);

        private readonly ConcurrentDictionary<string, Session> _sessions = new ConcurrentDictionary<string, Session>();
        private readonly ConcurrentDictionary<string, Subject<Session>> _subjects = new ConcurrentDictionary<string, Subject<Session>>();

        private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(1);

        private Timer _timer;
        private volatile bool _updatingValues;

        public void Create(string id, string user)
        {
            var session = new Session() { Id = Guid.NewGuid(), User = user };
            _sessions.TryAdd(id, session);
            _subjects.TryAdd(id, new Subject<Session>());

            if (_timer == null)
            {
                _timer = new Timer(UpdateValues, null, _updateInterval, _updateInterval);
            }
        }

        public bool TryGet(string id, out string username)
        {
            username = "";
            if (!_sessions.TryGetValue(id, out Session session))
            {
                return false;
            }
            username = session.User;
            return true;
        }

        public IObservable<Session> StreamValues(string id)
        {
            return _subjects[id];
        }

        private async void UpdateValues(object state)
        {
            await _updateValueLock.WaitAsync();
            try
            {
                if (!_updatingValues)
                {
                    _updatingValues = true;

                    foreach (var session in _sessions)
                    {
                        session.Value.Value++;
                        if (_subjects.TryGetValue(session.Key, out Subject<Session> subject))
                        {
                            subject.OnNext(session.Value);
                        }
                    }

                    _updatingValues = false;
                }
            }
            finally
            {
                _updateValueLock.Release();
            }
        }
    }
}