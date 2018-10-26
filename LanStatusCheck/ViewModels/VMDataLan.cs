using LanStatusCheck.Classes;
using LanStatusCheck.Contract;
using LanStatusCheck.Enums;
using LanStatusCheck.Models;
using mm;
using msg;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LanStatusCheck.ViewModels
{
    public class VMDataLan : NotifyPropertyBase
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
            {
                _messenger = IoC.Get<IMessenger>().Abonent(Abonent.VModelNetworkAdapters).AddHandler(HandleMessage);

                _model = new ModelDataLan();
            }

            

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
                var data = new NetAdapterDataView
                {
                    DataInterfaceModel = inter,
                    IsUpButtonEnabled = true,
                    IsDownButtonEnabled = true,
                    IsFavoriteButtonEnabled = true,
                    IsDeleteButtonEnabled = true,
                    IsPlayButtonEnabled = true,
                    IsElementFavorite = false,
                    StatusItem = EnumStatusItem.Normal
                };

                data.ItemAction += VMDataLan_ItemAction;

                CollectionNetInter.Add(data);
            }
        }

        private void VMDataLan_ItemAction(string id, EnumTypeOperationNaviPanel operation)
        {
            var element = CollectionNetInter.Where(a => a.DataInterfaceModel.Interface.Id == id).First();

            switch (operation)
            {
                case EnumTypeOperationNaviPanel.Up:
                    UpDownElement(element,1);
                    break;
                case EnumTypeOperationNaviPanel.Down:
                    UpDownElement(element, -1);
                    break;
                case EnumTypeOperationNaviPanel.Favorite:
                    SetFavorite(element);
                    break;
                case EnumTypeOperationNaviPanel.PlayDelete:
                    SetPlayDelete(element);
                    break;
                default:
                    break;
            }

        }

        /// <summary>Движение по коллекции в сторону увеличения индекса</summary>
        /// <param name="elem">объект для движения</param>
        /// <param name="offset">Индекс смещения Плюс - смещение в сторону увеличения индекса, Минус - в сторону уменьшения</param>
        private void UpDownElement(NetAdapterDataView elem, int offset)
        {

            if (elem == null) throw new NullReferenceException(elem.ToString());

            var positionElement = CollectionNetInter.IndexOf(elem);

            var newPositionElement = positionElement + offset;

            if(newPositionElement<0 || newPositionElement>=CollectionNetInter.Count)
            {
                throw new IndexOutOfRangeException("Индекс за пределами диапозона. Проверьте правильность смещения");
            }

            CollectionNetInter.Move(positionElement, newPositionElement);

            SetStateUpDownArrow();

        }

        private void SetStateUpDownArrow()
        {
            var tmpCol = CollectionNetInter.Where(a => a.StatusItem == EnumStatusItem.Normal).ToList();

            for (int i = 0; i < tmpCol.Count; i++)
            {

                var indexInSource = CollectionNetInter.IndexOf(tmpCol[i]);

                if (i == 0)
                {
                    CollectionNetInter[indexInSource].IsUpButtonEnabled = true;
                    CollectionNetInter[indexInSource].IsDownButtonEnabled = false;
                    continue;
                }

                if (i == tmpCol.Count - 1)
                {
                    CollectionNetInter[indexInSource].IsUpButtonEnabled = false;
                    CollectionNetInter[indexInSource].IsDownButtonEnabled = true;
                    continue;
                }

                CollectionNetInter[indexInSource].IsUpButtonEnabled = true;

                CollectionNetInter[indexInSource].IsDownButtonEnabled = true;
            }
        }


        private void SetFavorite(NetAdapterDataView elem)
        {
            if (elem == null) throw new NullReferenceException(elem.ToString());

            if(elem.StatusItem==EnumStatusItem.Favorite)
            {
                var newIndex = GetNewIndex(elem, EnumStatusItem.Normal, CollectionNetInter, _model.CollectionDataInterface);

                var oldIndex = CollectionNetInter.IndexOf(elem);

                CollectionNetInter.Move(oldIndex, newIndex);

                elem.StatusItem = EnumStatusItem.Normal;

            }
            else
            {
                var newIndex = GetNewIndex(elem, EnumStatusItem.Favorite, CollectionNetInter, _model.CollectionDataInterface);

                var oldIndex = CollectionNetInter.IndexOf(elem);

                CollectionNetInter.Move(oldIndex, newIndex);

                elem.StatusItem=EnumStatusItem.Favorite;
            }

            SetStateUpDownArrow();
        }

        private void SetPlayDelete(NetAdapterDataView elem)
        {
            if (elem == null) throw new NullReferenceException(elem.ToString());

            if(elem.StatusItem==EnumStatusItem.Deleted)
            {
                var newIndex = GetNewIndex(elem, EnumStatusItem.Normal, CollectionNetInter, _model.CollectionDataInterface);

                var oldIndex = CollectionNetInter.IndexOf(elem);

                CollectionNetInter.Move(oldIndex, newIndex);

                elem.StatusItem = EnumStatusItem.Normal;

                _model.BlockListInterface.TryRemove(elem.DataInterfaceModel.Interface.Id.GetHashCode(), out string str);
            }
            else
            {
                var newIndex = GetNewIndex(elem, EnumStatusItem.Deleted, CollectionNetInter, _model.CollectionDataInterface);

                var oldIndex = CollectionNetInter.IndexOf(elem);

                CollectionNetInter.Move(oldIndex, newIndex);

                elem.StatusItem = EnumStatusItem.Deleted;

                _model.BlockListInterface.TryAdd(elem.DataInterfaceModel.Interface.Id.GetHashCode(), elem.DataInterfaceModel.Interface.Id);
            }


        }

        private int GetNewIndex(NetAdapterDataView elem, EnumStatusItem status, IEnumerable<NetAdapterDataView> viewList, IEnumerable<NetworkInterfaceData> sourceList)
        {

            var list = viewList.Where(a => a.StatusItem == status).ToList();

            switch (status)
            {
                case EnumStatusItem.Favorite:
                    {

                        if (list.Count == 0) return 0;

                        var newIndex = viewList.ToList().IndexOf(list.Last()) + 1;

                        return newIndex;
                    }
                    
                case EnumStatusItem.Deleted:
                    {
                        return viewList.Count()-1;
                    }

                case EnumStatusItem.Normal:
                    {
                        var listFavorite = viewList.Where(a => a.StatusItem == EnumStatusItem.Favorite).ToList();

                        var listDeleted = viewList.Where(a => a.StatusItem == EnumStatusItem.Deleted).ToList();

                        var downIndex = listFavorite.Count() == 0 ? 0 : viewList.ToList().IndexOf(listFavorite.Last());

                        var upIndex = listDeleted.Count() == 0 ? viewList.Count() - 1 : viewList.ToList().IndexOf(listDeleted.First());

                        var indexElemInSource = sourceList.ToList().IndexOf(elem.DataInterfaceModel);

                        if (indexElemInSource > downIndex && indexElemInSource < upIndex)
                            return indexElemInSource;
                        else
                            return upIndex - 1;
                    }

                default:
                    return -1;

            }
        }



        #endregion
    }
}
