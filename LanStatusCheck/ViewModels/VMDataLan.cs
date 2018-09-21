using LanStatusCheck.Classes;
using LanStatusCheck.Contract;
using LanStatusCheck.Models;
using mm;
using msg;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanStatusCheck.ViewModels
{
    public class VMDataLan : INotifyPropertyChanged
    {
        #region local

        private IMessenger _messenger;

        private ModelDataLan _model;



        #endregion

        #region collections
        public ObservableCollection<NetAdapterDataView> CollectionNetInter { get; set; }


        #endregion

        #region ctor
        public VMDataLan()
        {

            CollectionNetInter = new ObservableCollection<NetAdapterDataView>();

            _messenger = IoC.Get<IMessenger>().Abonent(Abonent.VModelNetworkAdapters).AddHandler(HandleMessage);

            _model = new ModelDataLan();

        }

        #endregion

        #region HandleMessage

        private void HandleMessage(IMessage obj)
        {
            switch ((MsgType)obj.Type)
            {
                case MsgType.UpdateDataModelNetInter:

                    UpdateDataNetworkInter();

                    break;
                case MsgType.InitNetInterFinished:

                    InitInterface();

                    break;

                default:
                    break;
            }
        }

        #endregion

        #region local methods

        private void InitInterface()
        {
            var tmpList = new List<NetAdapterDataView>(_model.CollectionDataInterface.Select(a => new NetAdapterDataView()
            {
                DownSpeed = a.DownloadSpeedKBitS,
                UpSpeed = a.UploadSpeedKBitS,
                DataInterface = a
                
            }));

            foreach(var data in tmpList)
            {
                CollectionNetInter.Add(new NetAdapterDataView(data));
            }
        }

        private void UpdateDataNetworkInter()
        {
            foreach (var node in CollectionNetInter)
            {
                var res = _model.CollectionDataInterface.First(a => a.Interface.Name == node.NameInter);

                node.SetParamData(res.UploadSpeedKBitS, res.DownloadSpeedKBitS);
            }
        }


        #endregion





        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
