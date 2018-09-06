namespace mm
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Text;

    public class EventMessage : IMessage
    {
        private readonly ArrayList args = new ArrayList();

        public object Recipient { get; set; }

        public object Sender { get; set; }

        public object Type { get; set; }

        public int Count
        {
            get { return args.Count; }
        }

        public object this[int param]
        {
            get { return args[param]; }
            set { args[param] = value; }
        }

        public IEnumerator GetEnumerator()
        {
            return args.GetEnumerator();
        }

        public IMessage Add(object parameter)
        {
            args.Add(parameter);
            return this;
        }

        public IMessage To(object id)
        {
            Recipient = id;
            return this;
        }

        public IMessage From(object id)
        {
            Sender = id;
            return this;
        }

        public IMessage IsType(object o)
        {
            Type = o;
            return this;
        }

        public T As<T>(int index)
        {
            return (T)this[0];
        }

        public override string ToString()
        {
            var stringParameters = string.Empty;

            if (args.Count > 0)
            {
                var sb = new StringBuilder();

                sb.Append(": ");

                for (var i = 0; i < args.Count; i++)
                    sb.Append((i) + ") " + args[i] + " ");

                stringParameters = sb.ToString().Trim();
            }

            return string.Format( "{0} [{1}] -> [{2}] => {3} {4} {5}",
                DateTime.Now.ToString( "HH:mm:ss.fff" ),
                Sender,
                Recipient,
                Type,
                Count == 0 ? string.Empty : "[" + Count.ToString(CultureInfo.InvariantCulture) + "]",
                stringParameters).Trim();
        }

        public int CompareTo(object obj)
        {
            return Recipient.Equals(obj) ? 0 : -1;
        }
    }
}