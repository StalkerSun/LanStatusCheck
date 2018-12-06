using LanStatusCheck.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LanStatusCheck.Models
{
    public class ModelPingData
    {
        public List<PingUnitData> PingUnits;

        private static ModelPingData _instance;

        #region ctor

        private ModelPingData()
        {
            PingUnits = new List<PingUnitData>();

            var a = new PingUnitData(new SettingPingUnit { IpAddress = "10.10.7.1", CountTrySentPing = 3, Description = "123`132", PingPeriod_ms = 1000, TimeOut_ms = 250 });

            a.PingSend += A_PingSend;
            a.PingComplite += A_PingComplite;
            a.UpdateData += A_UpdateData;

            a.StartPing();

            PingUnits.Add(a);
        }

        private void A_UpdateData(PingUnitData obj)
        {
            Debug.WriteLine(obj.PingInfoHistory.Last().ToString());

        }

        private void A_PingComplite(PingUnitData obj)
        {
            Debug.WriteLine("Пинг начат " + DateTime.Now.);
        }

        private void A_PingSend(PingUnitData obj)
        {
            Debug.WriteLine("Пинг закончен " + DateTime.Now.ToString());
        }


        #endregion

        #region singletone getInstance

        public static ModelPingData GetInstanceModel()
        {
            if (_instance == null)
            {
                _instance = new ModelPingData();
            }

            return _instance;
        }

        #endregion

        #region public methods


        #endregion


    }
}
