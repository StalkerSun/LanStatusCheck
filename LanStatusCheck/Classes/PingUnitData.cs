using System.Timers;

namespace LanStatusCheck.Classes
{
    public class PingUnitData
    {
        private Timer _timerPing;

        public string IpAddress { get; set; }

        public string Destription { get; set; }

        public int TimeOut_ms { get; set; }

        public int PingPeriod_ms { get; set; }

        public PingUnitData()
        {
            _timerPing = new Timer();

            _timerPing.Elapsed += _timerPing_Elapsed;
        }

        private void _timerPing_Elapsed(object sender, ElapsedEventArgs e)
        {

        }









    }
}
