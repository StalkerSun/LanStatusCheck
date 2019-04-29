using System;

namespace LanStatusCheck.Classes
{
    public class NodePingInfo
    {
        /// <summary>Результат пинговки</summary>
        public bool Result { get; set; }

        /// <summary>Задержка туда-обратно в мс</summary>
        public long Delay_ms { get; set; }

        /// <summary>время</summary>
        public DateTime Dtime { get; set; }

        /// <summary>Количество посылок для получения удачной</summary>
        public int CountTrySend { get; set; }

        public override string ToString()
        {
            return String.Format("Ping Result: {0}, {1}, {2}, {3}", Result, Delay_ms, Dtime.ToString(), CountTrySend);
        }

    }
}
