namespace mm
{
    using System;
    using System.Linq;

    /// <summary>
    /// Реализация класса оправителя сообщения с подпиской в глобальной очереди сообщений Proxy
    /// </summary>
    public sealed class ProxyMessenger : IMessenger
    {
        private bool isDisposed;

        private Action<IMessage> action;

        public object Id { get; private set; }

        public ProxyMessenger()
        {
            Proxy.OnAdded += Proccess;
        }

        private void Proccess(IMessage message)
        {
            // нет обработчика сообщения - выходим
            if( action == null )
                return;

            // проверяем фильтр, если удовлетворяет фильтру - передаем
            if( Configuration.Criteria.Count > 0 && Configuration.Criteria.Any(pr => pr(message)) )
            {
                action(message);
                return;
            }

            // если не удовлетворяет фильтру (или его нет), но адресат верный - передаем
            if( message.CompareTo(Id) == 0 )
                action(message);
        }

        public IMessenger Abonent(object id)
        {
            Id = id;
            return this;
        }

        public IMessenger AddHandler(Action<IMessage> handler)
        {
            action = handler;
            return this;
        }

        public IMessenger Add(IMessage message)
        {
            if( isDisposed )
                return this;
            message.Sender = Id;
            Proxy.Add(message);
            Configuration.OnLog?.Invoke(message);
            return this;
        }

        public void Dispose()
        {
            Proxy.OnAdded -= Proccess;
            Id = null;
            action = null;
            isDisposed = true;
        }

        internal static class Proxy
        {
            internal static event Action<IMessage> OnAdded;
            internal static void Add(IMessage o)
            {
                if( OnAdded != null )
                {
                    if( Configuration.Context == null ) OnAdded(o);
                    else Configuration.Context.Post(state => OnAdded(o), null);
                }
            }
        }
    }
}