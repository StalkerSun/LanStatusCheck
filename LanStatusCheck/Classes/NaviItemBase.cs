using LanStatusCheck.Enums;
using LanStatusCheck.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LanStatusCheck.Classes
{
    public class NaviItemBase : INaviItemInList, INotifyPropertyChanged
    {
        private string _childId;

        private bool _isElementFavorite;

        public EnumStatusItem _statusItem;

        #region events

        public event Action<string, EnumTypeOperationNaviPanel> ItemAction = delegate { };

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Prop

        public bool IsUpButtonEnabled { get; set; }

        public bool IsDownButtonEnabled { get; set; }

        public bool IsFavoriteButtonEnabled { get; set; }

        public bool IsDeleteButtonEnabled { get; set; }

        public bool IsPlayButtonEnabled { get; set; }

        public bool IsElementFavorite
        {
            get { return _isElementFavorite; }
            set
            {
                _isElementFavorite = value;

                OnPropertyChanged();
            }
        }

        public EnumStatusItem StatusItem
        {
            get { return _statusItem; }
            set
            {
                _statusItem = value;

                OnPropertyChanged();
            }
        }



        #endregion

        public void SetIdInterface(string id)
        {
            _childId = id;

            
        }



        public RelayCommand UpItemCommand
        {
            get { return new RelayCommand(()=> { ItemAction(_childId, EnumTypeOperationNaviPanel.Up); }, () => IsUpButtonActive()); }
        }

        private bool IsUpButtonActive()
        {
            if (StatusItem == EnumStatusItem.Favorite || StatusItem == EnumStatusItem.Deleted)
                return false;

            return IsUpButtonEnabled;
        }

        public RelayCommand DownItemCommand
        {
            get { return new RelayCommand(() => { ItemAction(_childId, EnumTypeOperationNaviPanel.Down); }, () => IsDownButtonActive()); }
        }

        private bool IsDownButtonActive()
        {
            if (StatusItem == EnumStatusItem.Favorite || StatusItem == EnumStatusItem.Deleted)
                return false;

            return IsDownButtonEnabled;
        }


        public RelayCommand FavoritItemCommand
        {
            get { return new RelayCommand(() => { ItemAction(_childId, EnumTypeOperationNaviPanel.Favorite); }, () => true); }
        }

        public RelayCommand DeleteItemCommand
        {
            get { return new RelayCommand(() => { ItemAction(_childId, EnumTypeOperationNaviPanel.Delete); }, () => IsDeleteButtonEnabled); }
        }

        public RelayCommand PlayItemCommand
        {
            get { return new RelayCommand(() => { ItemAction(_childId, EnumTypeOperationNaviPanel.Play); }, () => IsPlayButtonEnabled); }
        }


        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
