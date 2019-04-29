using LanStatusCheck.Classes;
using LanStatusCheck.Contract;
using mm;
using msg;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Timers;
using System.Windows;

namespace LanStatusCheck.Models
{
    public class ModelDataLan
    {

        private static ModelDataLan _instance;


        #region local variable

        List<NetworkInterface> _collectionInterface;

        readonly int _periondTestLanSpeed = 1000; //Интервал подсчёта скорости обмена по сети

        Timer _timerTestSpeed;

        private IMessenger _messenger;

        #endregion

        #region public

        public readonly List<NetworkInterfaceData> CollectionDataInterface;

        public readonly ConcurrentDictionary<int, string> BlockListInterface;


        #endregion

        #region ctor

        private ModelDataLan()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()) == false)
            {
                _messenger = IoC.Get<IMessenger>().Abonent(Abonent.ModelNetworkAdapters).AddHandler(HandleMessage);
            }

            _collectionInterface = new List<NetworkInterface>(GetAllUpLanInterface());

            CollectionDataInterface = new List<NetworkInterfaceData>(_collectionInterface.Select(a => new NetworkInterfaceData(a)));

            BlockListInterface = new ConcurrentDictionary<int, string>();

            _timerTestSpeed = new Timer();

            CreateAndSendMessage(Abonent.VModelNetworkAdapters, MsgType.InitNetInterFinished, null);

            _timerTestSpeed.Interval = _periondTestLanSpeed;

            _timerTestSpeed.Elapsed += _timerTestSpeed_Elapsed;

            _timerTestSpeed.Start();

        }

        #endregion

        #region HandleMessage

        private void HandleMessage(IMessage obj)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region public region

        public static ModelDataLan GetInstanceModel()
        {
            if (_instance == null)
            {
                _instance = new ModelDataLan();
            }

            return _instance;
        }

        #endregion


        #region Local Methods

        private void _timerTestSpeed_Elapsed(object sender, ElapsedEventArgs e)
        {
            foreach (var inter in CollectionDataInterface)
            {
                var res = BlockListInterface.TryGetValue(inter.Interface.Id.GetHashCode(), out string val);
                if (!res)
                {
                    inter.CulculateParameters(_periondTestLanSpeed / 1000);
                }
            }

            //CreateAndSendMessage(Abonent.VModelNetworkAdapters, MsgType.UpdateDataModelNetInter);
        }

        private List<NetworkInterface> GetAllUpLanInterface()
        {
            var listInterface = NetworkInterface.GetAllNetworkInterfaces().ToList();

            var onlyUpInterface = listInterface.Where(a => ( a.OperationalStatus == OperationalStatus.Up )).ToList();

            foreach (var node in onlyUpInterface)
            {
                GetInformationFromAdapter(node);
            }

            return onlyUpInterface;
        }

        private void GetInformationFromAdapter(NetworkInterface adapter)
        {
            Debug.WriteLine("\nDescription: {0} \nId: {1} \nIsReceiveOnly: {2} \nName: {3} \nNetworkInterfaceType: {4} " +
                    "\nOperationalStatus: {5} " +
                    "\nSpeed (bits per second): {6} " +
                    "\nSpeed (kilobits per second): {7} " +
                    "\nSpeed (megabits per second): {8} " +
                    "\nSpeed (gigabits per second): {9} " +
                    "\nSupportsMulticast: {10}",
                    adapter.Description,
                    adapter.Id,
                    adapter.IsReceiveOnly,
                    adapter.Name,
                    adapter.NetworkInterfaceType,
                    adapter.OperationalStatus,
                    adapter.Speed,
                    adapter.Speed / 1000,
                    adapter.Speed / 1000 / 1000,
                    adapter.Speed / 1000 / 1000 / 1000,
                    adapter.SupportsMulticast);

            var ipv4Info = adapter.GetIPv4Statistics();
            Console.WriteLine("OutputQueueLength: {0}", ipv4Info.OutputQueueLength);
            Console.WriteLine("BytesReceived: {0}", ipv4Info.BytesReceived);
            Console.WriteLine("BytesSent: {0}", ipv4Info.BytesSent);

        }


        /// <summary>Собрать и отправить сообщение через службу сообщений</summary>
        /// <param name="abonent">Абонент для которого предназначено сообщение</param>
        /// <param name="type">Тип сообщения</param>
        /// <param name="args">Данные, которые необходимо передать</param>
        private void CreateAndSendMessage(Abonent abonent, MsgType type, IEnumerable<object> args = null)
        {
            var msg = IoC.Get<IMessage>().To(abonent).IsType(type);

            if (args != null)
            {
                foreach (var argument in args)
                {
                    msg.Add(argument);
                }
            }

            _messenger.Add(msg);
        }

        #endregion





    }
}
