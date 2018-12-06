using LanStatusCheck.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Timers;

namespace LanStatusCheck.Classes
{
    public class PingUnitData
    {
        #region varible

        private System.Timers.Timer _timerPing;

        private readonly int _defPingPer = 1000;

        private readonly int _defCountTryPing = 3;

        private SynchronizationContext _context;


        #endregion

        #region prop

        public string IpAddress { get; private set; }

        public string Description { get; set; }

        public int TimeOut_ms { get; set; }

        public int PingPeriod_ms { get; set; }

        public int CountTrySentPing { get; set; }

        public readonly List<NodePingInfo> PingInfoHistory;

        #endregion

        #region events

        public event Action<PingUnitData> PingSend = delegate { };

        public event Action<PingUnitData> PingComplite = delegate { };

        public event Action<PingUnitData> UpdateData = delegate { };

        #endregion

        #region ctor

        public PingUnitData()
        {
            Init();

            PingInfoHistory = new List<NodePingInfo>();
        }

        public PingUnitData(SettingPingUnit setting)
        {

            Init();

            PingInfoHistory = new List<NodePingInfo>();

            IpAddress = setting.IpAddress;

            Description = setting.Description;

            TimeOut_ms = setting.TimeOut_ms;

            PingPeriod_ms = setting.PingPeriod_ms;

            CountTrySentPing = setting.CountTrySentPing;
        }



        #endregion


        #region _timerPing_Elapsed

        private void _timerPing_Elapsed(object sender, ElapsedEventArgs e)
        {
            //_timerPing.Stop();

            Ping();

            _timerPing.Start();
        }

        #endregion

        #region public methods

        public bool StartPing()
        {
            if (!IpHelpers.ParseIp(IpAddress, out var address, out var error))
            {
                return false;
            }

            _timerPing.Interval = PingPeriod_ms;

            _timerPing.Start();

            return true;

        }

        public void StopPing()
        {
            _timerPing.Stop();
        }

        public SettingPingUnit GetSetting()
        {
            return new SettingPingUnit()
            {
                CountTrySentPing = this.CountTrySentPing,
                Description = this.Description,
                IpAddress = this.IpAddress,
                PingPeriod_ms = this.PingPeriod_ms,
                TimeOut_ms = this.TimeOut_ms
            };
        }

        #endregion

        #region local methods

        private void Init()
        {
            _context = SynchronizationContext.Current;

            IpAddress = string.Empty;

            CountTrySentPing = _defCountTryPing;

            PingPeriod_ms = _defPingPer;

            _timerPing = new System.Timers.Timer
            {
                AutoReset = false
            };

            _timerPing.Elapsed += _timerPing_Elapsed;
        }


        private void Ping()
        {
            _context.Post(d => PingSend(this), null);

            var res = PingHelper.PingOk(IpAddress, out var resultArray, CountTrySentPing, TimeOut_ms);

            _context.Post(d => PingComplite(this), null);

            if (!res)
            {
                PingInfoHistory.Add(new NodePingInfo() { Result = false, Dtime = DateTime.Now });

            }
            else
            {
                PingInfoHistory.Add(new NodePingInfo()
                {
                    Result = true,
                    Dtime = DateTime.Now,
                    CountTrySend = resultArray.Length,
                    Delay_ms = resultArray.Where(a => a._pingStatus == IPStatus.Success).ToList().First()._timeDelay_ms
                });
            }
            _context.Post(d => UpdateData(this), null);
        }

        #endregion













    }
}
