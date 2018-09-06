namespace mm
{
    using System;
    using System.Linq;

    public static class QueueProxy
    {
        public static Action<IMessage> Log { get; set; }

        public static System.Windows.Threading.Dispatcher Dispatcher { get; set; }

        public static int Count { get { return OnAdded == null ? 0 : OnAdded.GetInvocationList().Count(); } }

        internal static event Action<IMessage> OnAdded;

        internal static void Add( IMessage o )
        {
            if (OnAdded != null)
            {
                if ( Dispatcher == null)
                    OnAdded( o );
                else
                {
                    if ( Dispatcher.CheckAccess() ) OnAdded( o );                        
                    else Dispatcher.Invoke(OnAdded, o);
                }    
            }
        }

        public static string[] GetSubscribers()
        {
            var subscribers = new string[Count];

            if (OnAdded != null)
            {
                var subs = OnAdded.GetInvocationList().Select(sub => sub.Target).ToArray();

                for (var i = 0; i < Count; i++)
                    subscribers[i] = subs[i].ToString();
            }

            return subscribers;
        }
    }
}