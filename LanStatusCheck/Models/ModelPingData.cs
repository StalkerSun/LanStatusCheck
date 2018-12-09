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

            PingUnitData a;

            if (AddPingerUnit(new SettingPingUnit() { IpAddress = "10.10.7.1", CountTrySentPing = 3, Description = "123`132", PingPeriod_ms = 500, TimeOut_ms = 250 }, out a))
            {
                a.PingSend += A_PingSend;
                a.PingComplite += A_PingComplite;
                a.UpdateData += A_UpdateData;
                //a.StartPing();
            }

            if (AddPingerUnit(new SettingPingUnit() { IpAddress = "10.10.0.1", CountTrySentPing = 3, Description = "123`132", PingPeriod_ms = 500, TimeOut_ms = 250 }, out a))
            {
                a.PingSend += A_PingSend;
                a.PingComplite += A_PingComplite;
                a.UpdateData += A_UpdateData;
                a.StartPing();
            }

            if (AddPingerUnit(new SettingPingUnit() { IpAddress = "10.10.0.25", CountTrySentPing = 3, Description = "123`132", PingPeriod_ms = 500, TimeOut_ms = 250 }, out a))
            {
                a.PingSend += A_PingSend;
                a.PingComplite += A_PingComplite;
                a.UpdateData += A_UpdateData;
                //a.StartPing();
            }




        }

        private void A_UpdateData(PingUnitData obj)
        {
            Debug.WriteLine(obj.IpAddress + ": "+ obj.PingInfoHistory.Last().ToString() +" " + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss.fff"));

        }

        private void A_PingComplite(PingUnitData obj)
        {
            Debug.WriteLine(obj.IpAddress + ": " + "Пинг закончен " + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss.fff"));
        }

        private void A_PingSend(PingUnitData obj)
        {
            Debug.WriteLine(obj.IpAddress + ": " + "Пинг начат " + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss.fff"));
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

        public bool AddPingerUnit(SettingPingUnit setting, out PingUnitData refPinger)
        {
            refPinger = new PingUnitData(setting);

            var res = refPinger.CheckPingData();

            if (res)
            {
                PingUnits.Add(refPinger);

                return true;
            }
            else
            {
                refPinger = null;

                return false;
            }
        }


        #endregion


    }
}
