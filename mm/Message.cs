namespace mm
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Text;

    public class Message : IMessage
    {
        private readonly ArrayList args = new ArrayList();
        public object Receiver { get; set; }
        public object Sender { get; set; }
        public object Type { get; set; }
        public int Count => args.Count;
        public object this[int param]
        {
            get { return args[param]; }
            set { args[param] = value; }
        }
        public IEnumerator GetEnumerator() => args.GetEnumerator();
        public IMessage Add(object parameter) { args.Add(parameter); return this; }
        public IMessage To(object id) { Receiver = id; return this; }
        public IMessage From(object id) { Sender = id; return this; }
        public IMessage IsType(object o) { Type = o; return this; }
        public T As<T>(int index) => (T)this[index];

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

            return string.Format("{0} [{1}] -> [{2}] => {3} {4} {5}",
                DateTime.Now.ToString("HH:mm:ss.fff"),
                Sender,
                Receiver,
                Type,
                Count == 0 ? string.Empty : "[" + Count.ToString(CultureInfo.InvariantCulture) + "]",
                stringParameters).Trim();
        }

        public int CompareTo(object obj) => Receiver.Equals(obj) ? 0 : -1;
    }
}