using System;
using System.Collections;
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

        private static void Ping_completed(object s, PingCompletedEventArgs e)
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
        /// <summary> Проверка доступа к абоненту с адресом IP_address  через Ping (ICMP-пакеты) ==================================</summary>
        /// <param name="IP_address">IP адрес абонента </param>
        /// <param name="timeout_msec">Таймаут [мсек]. По умолчанию = 120мсек</param>
        /// <param name="RoutingNodeCount">Максимальное число пересылок до достижения узла назначения. По умолчанию = 128</param>
        /// <returns>true-абонент отвечает, false-абонент не отвечает </returns>
        public static bool PingOk(string IP_address, int timeout_msec = 250, int RoutingNodeCount = 128)
        {
            if (( IP_address == "" )
                  || ( IP_address == "0.0.0.0" )
                  || ( IP_address == "255.255.255.255" ))
            {
                return false; //----------------->
            }

            System.Net.NetworkInformation.Ping pingSender = new System.Net.NetworkInformation.Ping();
            try
            {
                System.Net.NetworkInformation.PingOptions options = new System.Net.NetworkInformation.PingOptions();
                options.DontFragment = true;     // Не фрагментировать 
                options.Ttl = RoutingNodeCount;  // Максимальное число пересылок до достижения узла назначения.

                // Тестовые данные для передачи
                string data = "abababababababababababababababab";
                byte[] buffer = Encoding.ASCII.GetBytes(data);

                if (IP_address == "")
                {
                    return false; //-------------------->
                }

                // Попытка связи
                try
                {
                    System.Net.NetworkInformation.PingReply reply = pingSender.Send(IP_address, timeout_msec, buffer, options);
                    if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }
            }
            finally
            {
                pingSender.Dispose();
            }
        }  // PingOk

        /// <summary> Проверка доступа к абоненту с адресом IP_address ==================================</summary>
        /// <param name="IP_address">IP адрес абонента.</param>
        /// <param name="pings_Res">Результат попыток пигновки - Массив элементов типа S_PingResult.
        ///                     Массив содержит ping_count элементов или меньше.
        ///                     Если ошибка произошла до пингования, длина массива = 0.
        ///                     После удачной попытки пинговка завершается. Результат удачной попытки также заносится в массив.
        ///                     </param>
        /// <param name="ping_count">Максимальное количество попыток пинговки.</param>
        /// <param name="timeout_msec">Таймаут [мсек]. По умолчанию = 120мсек.</param>
        /// <param name="RoutingNodeCount">Максимальное число пересылок до достижения узла назначения. По умолчанию = 128.</param>
        /// <returns>true-абонент отвечает, false-абонент не отвечает.</returns>
        public static bool PingOk(string IP_address, out S_PingResult[] pings_Res, int ping_count = 1, int timeout_msec = 250, int RoutingNodeCount = 128)
        {
            //lock(Sync_Ping)
            {
                bool result = false;         // Результат работы функции PingOk
                pings_Res = new S_PingResult[0];       // Массив Результатов попыток Пингования
                ArrayList results = new ArrayList();   // Список (Массив) Результатов попыток Пингования
                if (ping_count > 10)
                {
                    ping_count = 10;
                }

                System.Net.NetworkInformation.Ping pingSender = new System.Net.NetworkInformation.Ping();
                try
                {
                    System.Net.NetworkInformation.PingOptions options = new System.Net.NetworkInformation.PingOptions();
                    options.DontFragment = true; // Не фрагментировать 
                    options.Ttl = RoutingNodeCount; // Максимальное число пересылок до достижения узла назначения.

                    // Тестовые данные для передачи
                    string data = "abababababababababababababababab";
                    byte[] buffer = Encoding.ASCII.GetBytes(data);

                    // Попытка связи
                    if (IP_address == "")
                    {
                        return result = false; //-------------------->
                    }

                    try
                    {
                        result = false;
                        for (int i = 0; i < ping_count; i++)
                        {
                            // Пингование
                            System.Net.NetworkInformation.PingReply reply = pingSender.Send(IP_address, timeout_msec, buffer, options);

                            // Результат - в массив
                            S_PingResult s = new S_PingResult();
                            s._pingStatus = reply.Status;
                            if (reply.Status == IPStatus.Success)
                            {
                                s._timeDelay_ms = reply.RoundtripTime;
                            }
                            else
                            {
                                s._timeDelay_ms = timeout_msec;
                            }

                            results.Add(s);
                            if (reply.Status == IPStatus.Success)
                            {
                                result = true;
                                break; //---------->
                            }
                        }
                    }
                    catch //(Exception ex)
                    {
                        result = false;
                    }
                }
                finally
                {
                    if (pingSender != null)
                    {
                        pingSender.Dispose();
                    }

                    pings_Res = ( S_PingResult[] ) results.ToArray(typeof(S_PingResult));
                }
                return result;
            }
        }  // PingOk

        // Результат запроса Ping
        public struct S_PingResult
        {
            public IPStatus _pingStatus;    // if  (pingStatus == IPStatus.Success)   => Пинг удачно прошел
            public long _timeDelay_ms;  // Время, затраченное на прохождение сигнала  туда+обратно.
        }

        /// <summary>Обработка результатов Ping-а.</summary>
        /// <param name="pings_Result">Входной параметр - результат вызова функции PingOk().</param>
        /// <param name="sError">"" или текст ошибки. "Не проходит Ping канала связи.\r\n"
        ///                                            + в скобках - английский текст неудачных попыток Ping.</param>
        /// <param name="timeDelay_ms">Время "туда-обратно" при удачной попытке Ping-а</param>
        /// <returns>=true - пингование завершилость успешно, false - Не проходит Ping канала связи.</returns>
        public static bool PingProcess(S_PingResult[] pings_Result, out string sError, out long timeDelay_ms)
        {
            sError = "";
            timeDelay_ms = 0;
            bool result = false;
            if (pings_Result == null)
            {
                sError = "Невозможно произвести Ping.";
                return result; //-------------->
            }
            int count = pings_Result.Length;
            if (count == 0)
            {
                sError = "Невозможно произвести Ping.";
                return result; //-------------->
            }

            for (int i = 0; i < count; i++)
            {
                S_PingResult pngRes = ( S_PingResult ) pings_Result[i];
                timeDelay_ms = pngRes._timeDelay_ms;
                result = ( pngRes._pingStatus == IPStatus.Success );
                if (result == false)
                {
                    sError += "\r\n  Ping №" + i.ToString() + ": " + pngRes._pingStatus.ToString();
                }
            }
            if (result == false)
            {
                sError = "Не проходит Ping канала связи (" + sError + ").";
            }
            return result;
        }  // PingProcess

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
