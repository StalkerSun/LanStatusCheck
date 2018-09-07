using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace LanStatusCheck.Classes
{
    public class NetworkInterfaceData
    {
        #region Fields

        /// <summary>Объект интерфейса</summary>
        public NetworkInterface Interface;

        /// <summary>Имя интерфейса</summary>
        public string NameInterface;

        /// <summary>Скорость отдачи</summary>
        public double UploadSpeedKBitS;

        /// <summary>Скорость загрузки</summary>
        public double DownloadSpeedKBitS;

        /// <summary>Ip адреса интерфейса</summary>
        public List<string> InterfaceIPAdresses;

        #endregion

        #region Variable

        private long _oldReceivedBytes = -1;

        private long _oldSentBytes = -1;

        #endregion


        #region ctor
        public NetworkInterfaceData(NetworkInterface inter)
        {
            Interface = inter;

            NameInterface = Interface.Description;

            InterfaceIPAdresses = new List<string>(GetIpAddresses(inter));
        }

        #endregion

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

            DownloadSpeedKBitS = ((currentReceivedB * 8) / 1024) / interval;

            UploadSpeedKBitS = ((currentSentB * 8) / 1024) / interval;

            _oldReceivedBytes = totalReceivedB;

            _oldSentBytes = totalSentB;
        }

        private List<string> GetIpAddresses(NetworkInterface inter)
        {
            var ipCollection = inter.GetIPProperties().UnicastAddresses.Where(a=>a.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToList();

            var listStr = ipCollection.Select(a => a.Address.ToString()).ToList();

            return listStr;
        }





    }
}
