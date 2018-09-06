namespace mm
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Реализация класса оправителя сообщения с хранением слабой ссылки WeakReference
    /// </summary>
    public sealed class WeakMessenger : IMessenger
    {
        private Action<IMessage> action;

        public object Id { get; private set; }

        public WeakMessenger()
        {
            Weak<WeakMessenger>.Add(this);
        }

        public IMessenger Abonent(object id) { Id = id; return this; }

        public IMessenger AddHandler(Action<IMessage> handler) { action = handler; return this; }

        public IMessenger Add(IMessage message)
        {
            message.Sender = Id;
            Configuration.OnLog?.Invoke(message);
            if( Configuration.Context == null ) Proccess(message);
            else Configuration.Context.Post(state => Proccess(message), null);
            return this;
        }

        private static void Proccess(IMessage message)
        {
            foreach( var messenger in Weak<WeakMessenger>.Get().Where(messenger => messenger.action != null) )
            {
                // проверка на выполнение общих предикатов (сообщение для всех и т.п.)
                if( Configuration.CheckCriteries(message) )
                    messenger.action(message);
                else if( message.CompareTo(messenger.Id) == 0 )
                    messenger.action(message);
            }
        }

        public void Dispose()
        {
            Weak<WeakMessenger>.Remove(this);
            action = null;
        }

        public override string ToString() => $"[{Weak<WeakMessenger>.Count}] {DateTime.Now:HH:mm:ss.fff} Count messengers";

        private static class Weak<T> where T : class
        {
            static readonly ConcurrentDictionary<int, WeakReference<T>> references = new ConcurrentDictionary<int, WeakReference<T>>();

            public static void Add(T o) => references.TryAdd(o.GetHashCode(), new WeakReference<T>(o, false));

            public static void Remove(T o)
            {
                WeakReference<T> reference;
                references.TryRemove(o.GetHashCode(), out reference);
            }

            public static List<T> Get()
            {
                var targets = new List<T>();
                var removeKeys = new List<int>();
                foreach( var reference in references )
                {
                    T target;
                    if( reference.Value.TryGetTarget(out target) )
                        targets.Add(target);
                    else
                        removeKeys.Add(reference.Key);
                }
                foreach( var index in removeKeys )
                {
                    WeakReference<T> reference;
                    references.TryRemove(index, out reference);
                }
                return targets;
            }

            public static int Count => references.Count;
        }
    }
}