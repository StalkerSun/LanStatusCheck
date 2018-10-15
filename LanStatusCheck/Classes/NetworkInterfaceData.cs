using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LanStatusCheck.Classes
{
    public class NetworkInterfaceData
    {
        #region Fields

        /// <summary>Объект интерфейса</summary>
        public NetworkInterface Interface { get; private set; }

        /// <summary>Скорость отдачи</summary>
        public double UploadSpeedKBitS { get; private set; }

        /// <summary>Скорость загрузки</summary>
        public double DownloadSpeedKBitS { get; private set; }

        /// <summary>Ip адреса интерфейса</summary>
        public List<string> InterfaceIPAdresses { get; private set; }

        /// <summary>Данные по истории измеения сосояния</summary> //TODO:Назвал как-то хреново, при рефакторинге ПЕРЕИМЕНОВАТЬ
        public readonly List<NodeActiveNetInterface> HistoryDataActivity;

        public event Action UpdateData = delegate { };



        #endregion

        #region Variable

        private long _oldReceivedBytes = int.MinValue;

        private long _oldSentBytes = int.MinValue;

        private int _countPointInHistory = 600;

        private SynchronizationContext _context;
        
        #endregion
        
        #region ctor
        public NetworkInterfaceData(NetworkInterface inter)
        {
            _context = SynchronizationContext.Current;

            Interface = inter;

            InterfaceIPAdresses = new List<string>(GetIpAddresses(inter));

            HistoryDataActivity = new List<NodeActiveNetInterface>();
        }

        #endregion

        public void CulculateParameters( int interval)
        {
            var totalReceived = Interface.GetIPv4Statistics().BytesReceived;

            var totalSent = Interface.GetIPv4Statistics().BytesSent;

            if(_oldReceivedBytes == int.MinValue || _oldSentBytes == int.MinValue)
            {
                _oldReceivedBytes = totalReceived;

                _oldSentBytes = totalSent;

                return;
            }

            var speedUpKBitS = GetSpeedKBitS(interval, _oldSentBytes, totalSent);

            var speedDownKBitS = GetSpeedKBitS(interval, _oldReceivedBytes, totalReceived);

            _oldReceivedBytes = totalReceived;

            _oldSentBytes = totalSent;

            var loadUp = GetPercentLoadOnInterface(speedUpKBitS, Interface.Speed / 1000);

            var loadDown = GetPercentLoadOnInterface(speedDownKBitS, Interface.Speed / 1000);

            if (HistoryDataActivity.Count > _countPointInHistory)
                HistoryDataActivity.RemoveAt(0);

            HistoryDataActivity.Add(new NodeActiveNetInterface()
            {
                DownSpeedKBitSec = speedDownKBitS,
                UpSpeedKBitSec = speedUpKBitS,
                LoadPerSecDown = loadDown,
                LoadPerSecUp = loadUp,
                TotalRecivedBytes = totalReceived,
                TotalTransmiteBytes = totalSent,
                Time = DateTime.Now


            });

            Debug.WriteLineIf(Interface.Name.Contains("Интернет"), String.Format("Rx-{0}, Tx-{1}", totalReceived, totalSent), "Интерфейс Интернет");

            if(Interface.Name.Contains("Интернет"))
            { }

            _context.Post(d=>UpdateData(), null);
        }

        /// <summary>
        /// Посчитать текущуюю скорость на интерфейсе
        /// </summary>
        /// <param name="interval">Интервал времени между подсчётами</param>
        public void CulcCurrentSpeed(int interval)
        {
            long totalReceivedB = Interface.GetIPv4Statistics().BytesReceived;

            long totalSentB = Interface.GetIPv4Statistics().BytesSent;

            if(_oldReceivedBytes == -1 ||_oldSentBytes == -1 )
            {
                _oldReceivedBytes = totalReceivedB;

                _oldSentBytes = totalSentB;

                return;
            }

            var currentReceivedB = totalReceivedB - _oldReceivedBytes;

            var currentSentB = totalSentB - _oldSentBytes;

            DownloadSpeedKBitS = ((currentReceivedB * 8) / 1000) / interval;

            UploadSpeedKBitS = ((currentSentB * 8) / 1000) / interval;

            _oldReceivedBytes = totalReceivedB;

            _oldSentBytes = totalSentB;
        }

        private double GetSpeedKBitS(int interval, long oldTotalBytes, long newTotalBytes)
        {
            var delta = newTotalBytes - oldTotalBytes;

            return ((delta * 8) / 1000) / interval;
        }


        private List<string> GetIpAddresses(NetworkInterface inter)
        {
            var ipCollection = inter.GetIPProperties().UnicastAddresses.Where(a=>a.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToList();

            var listStr = ipCollection.Select(a => a.Address.ToString()).ToList();

            return listStr;
        }

        private int GetPercentLoadOnInterface(double speedkbS, double maxSpeedInterfaceKbS)
        {
            var currentLoad = (speedkbS * 100.0) / maxSpeedInterfaceKbS;

            return Convert.ToInt32(currentLoad);
        }




    }
}
