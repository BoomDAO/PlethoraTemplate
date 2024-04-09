namespace ItsJackAnton.Patterns.Broadcasts
{
    using System.Collections.Generic;
    using UnityEngine.Events;

    public class UnityBroadcast
    {
        private class EventData
        {
            private readonly UnityEvent listeners;
            public uint ListenerCount { get; private set; }

            public EventData(UnityAction listener)
            {
                listeners = new UnityEvent();
                AddListener(listener);
            }


            public void Invoke()
            {
                listeners.Invoke();
            }

            public uint AddListener(UnityAction listener)
            {
                listeners.AddListener(listener);
                ++ListenerCount;
                return ListenerCount;
            }
            public uint RemoveListener(UnityAction listener)
            {
                listeners.RemoveListener(listener);
                --ListenerCount;
                return ListenerCount;
            }
        }

        private readonly Dictionary<uint, EventData> channels = new();

        public uint AddListener(UnityAction listener, uint channel = 0)
        {
            uint key = channel;

            if (channels.TryGetValue(key, out EventData listeners))
            {
                return listeners.AddListener(listener);
            }
            else
            {
                listeners = new EventData(listener);
                channels.Add(key, listeners);
                return 1;
            }
        }
        public uint RemoveListener(UnityAction listener, uint channel = 0)
        {
            uint key = channel;

            if (channels.TryGetValue(key, out EventData listeners))
            {
                return listeners.RemoveListener(listener);
            }
            return 0;
        }
        public void Invoke(uint channel = 0)
        {
            uint key = channel;

            if (channels.TryGetValue(key, out EventData listeners))
            {
                listeners.Invoke();
            }
        }
        public void InvokeAll()
        {
            foreach (var listeners in channels)
            {
                listeners.Value.Invoke();
            }
        }

        public void Dispose()
        {
            channels.Clear();
        }

        public void Dispose(uint channel)
        {
            if (channels.ContainsKey(channel)) channels.Remove(channel);
        }

        public uint GetListenersCount(uint channel)
        {
            if (channels.TryGetValue(channel, out EventData targets)) return targets.ListenerCount;
            else return 0;
        }
    }
}