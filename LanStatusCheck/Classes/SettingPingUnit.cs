using System;

namespace LanStatusCheck.Classes
{
    [Serializable]
    public class SettingPingUnit
    {
        public string IpAddress;

        public string Description;

        public int TimeOut_ms;

        public int PingPeriod_ms;

        public int CountTrySentPing;

    }
}
