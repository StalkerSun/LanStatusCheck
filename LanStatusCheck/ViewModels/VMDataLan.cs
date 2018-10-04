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
using System.Windows;

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

            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()) == false)
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

                    //UpdateDataNetworkInter();

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
            foreach(var inter in _model.CollectionDataInterface)
            {
                CollectionNetInter.Add(new NetAdapterDataView { DataInterfaceModel = inter });
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
