using System;

namespace LanStatusCheck.Classes
{
    public class NodeActiveNetInterface
    {
        /// <summary>Скорость отдачи в KBit в секунду</summary>
        public double UpSpeedKBitSec { get; set; }

        /// <summary>Скорость загрузки в KBit в секунду</summary>
        public double DownSpeedKBitSec { get; set; }

        /// <summary>Нагрузка на канал ОТДАЧИ в процентах</summary>
        public int LoadPerSecUp { get; set; }

        /// <summary>Нагрузка на канал ЗАГРУЗКИ в процентах</summary>
        public int LoadPerSecDown { get; set; }

        /// <summary>Всего получено Байт</summary>
        public long TotalRecivedBytes { get; set; }

        /// <summary>Всего передано Байт</summary>
        public long TotalTransmiteBytes { get; set; }

        /// <summary>Время точки</summary>
        public DateTime Time { get; set; }

        public NodeActiveNetInterface()
        {

        }

        public override string ToString()
        {
            return string.Format("UpSpeedKBitSec = {0}, DownSpeedKBitSec = {1}, LoadPerSecUp = {2}, LoadPerSecDown = {3}, TotalRecivedBytes = {4}, TotalTransmiteBytes = {5}, Time = {6}",
                UpSpeedKBitSec, DownSpeedKBitSec, LoadPerSecUp, LoadPerSecDown, TotalRecivedBytes, TotalTransmiteBytes, Time);
        }
    }
}
