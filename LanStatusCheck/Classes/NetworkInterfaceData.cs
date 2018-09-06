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
        public NetworkInterface Interface;

        public string NameInterface;

        public double UploadSpeedKBitS;

        public double DownloadSpeedKBitS;

        private long _oldReceivedBytes = -1;

        private long _oldSentBytes = -1;


        public NetworkInterfaceData(NetworkInterface inter)
        {
            Interface = inter;

            NameInterface = Interface.Description;

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

            DownloadSpeedKBitS = ((currentReceivedB * 8) / 1024) / interval;

            UploadSpeedKBitS = ((currentSentB * 8) / 1024) / interval;

            _oldReceivedBytes = totalReceivedB;

            _oldSentBytes = totalSentB;
        }



    }
}
