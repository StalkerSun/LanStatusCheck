namespace mm
{
    using System;
    using System.Diagnostics;

    public class EventMessenger: IMessenger
    {
        private Predicate<IMessage> pred;

        private Action<IMessage> action; 

        public object Id { get; private set; }
        
        public EventMessenger()
        {
            QueueProxy.OnAdded += OnMessageAdded;

            if ( QueueProxy.Log == null )
                QueueProxy.Log = o => Debug.WriteLine( string.Format( "[{0}] {1}", QueueProxy.Count, o ) );
        }

        protected virtual void OnMessageAdded(IMessage message)
        {
            // нет обработчика сообщения - выходим
            if (action == null)
                return;

            // проверяем фильтр, если удовлетворяет фильтру - передаем
            if (pred != null && pred(message))
            {
                action(message);

                return;
            }

            // если не удовлетворяет фильтру (или его нет), но адресат верный - передаем
            if (message.CompareTo(Id) == 0)
                action(message);
        }

        public IMessenger Filter( Predicate<IMessage> predicate )
        {
            pred = predicate; return this;
        }

        public IMessenger Abonent( object id )
        {
            Id = id; return this;
        }

        public IMessenger AddHandler( Action<IMessage> handler )
        {            
            action = handler; return this;
        }

        public IMessenger RemoveHandler( )
        {
            action = null; return this;
        }

        public IMessenger Add( IMessage message )
        {
            if ( Id == null)
            {
                throw new Exception( string.Format( "У объекта IMessenger {0} не установлен абонент отправителя!!", 
                    new StackFrame( 2 ).GetMethod().ReflectedType ) );
            }

            message.Sender = Id;

            QueueProxy.Add(message);

            //QueueProxy.Log( message );

            return this;
        }

        public void Dispose()
        {
            QueueProxy.OnAdded -= OnMessageAdded;

            pred   = null;
            action = null;
        }

        public override string ToString()
        {
            return string.Format( "[{0}]", Id);
        }
    }
}