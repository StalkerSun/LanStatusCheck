using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;

namespace LanStatusCheck.Helpers
{
    public static class PingHelper
    {
        private static List<Ping> pingers = new List<Ping>();

        private static int instances = 0;

        private static object @lock = new object();

        private static int result = 0;
        private static int timeOut = 250;

        private static int ttl = 5;

        private static List<string> _activeIp;


        public static List<string> IpAddresesListPing(List<string> ipList)
        {
            _activeIp = new List<string>();

            result = 0;

            CreatePingers(ipList.Count);

            var po = new PingOptions(ttl, true);

            var enc = new ASCIIEncoding();

            byte[] data = enc.GetBytes("abababababababababababababababab");

            var wait = new SpinWait();
            var cnt = 1;

            var watch = Stopwatch.StartNew();

            for (int i = 0; i < pingers.Count; i++)
            {
                lock (@lock)
                {
                    instances += 1;
                }

                var contact = ipList[i];

                //Console.WriteLine(contact);

                pingers[i].SendAsync(contact, timeOut, data, po);
                cnt += 1;
            }

            while (instances > 0)
            {
                wait.SpinOnce();
            }

            watch.Stop();

            DestroyPingers();

            Console.WriteLine("Finished in {0}. Found {1} active IP-addresses.", watch.Elapsed.ToString(), result);

            return _activeIp;
        }

        public static void Ping_completed(object s, PingCompletedEventArgs e)
        {
            lock (@lock)
            {
                instances -= 1;
            }

            if (e.Reply != null && e.Reply.Status == IPStatus.Success)
            {
                string ip = e.Reply.Address.ToString();

                _activeIp.Add(ip);

                Console.WriteLine(string.Concat("Active IP: ", ip));
                result += 1;
            }
            else
            {
                //Console.WriteLine(String.Concat("Non-active IP: ", e.Reply.Address.ToString()))
            }
        }

        private static void CreatePingers(int cnt)
        {
            for (int i = 1; i <= cnt; i++)
            {
                Ping p = new Ping();
                p.PingCompleted += Ping_completed;
                pingers.Add(p);
            }
        }

        private static void DestroyPingers()
        {
            foreach (Ping p in pingers)
            {
                p.PingCompleted -= Ping_completed;
                p.Dispose();
            }

            pingers.Clear();

        }

    }
}
