using LanStatusCheck.Enums;
using LanStatusCheck.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LanStatusCheck.Classes
{
    public class NaviItemBase : INaviItemInList
    {
        private string _childId;

        public  event Action<string, EnumTypeOperationNaviPanel> ItemAction = delegate { };

        public void SetIdInterface(string id)
        {
            _childId = id;
        }

        public RelayCommand UpItemCommand
        {
            get { return new RelayCommand(()=> { ItemAction(_childId, EnumTypeOperationNaviPanel.Up); }, () => true); }
        }


        public RelayCommand DownItemCommand
        {
            get { return new RelayCommand(() => { ItemAction(_childId, EnumTypeOperationNaviPanel.Down); }, () => true); }
        }

        public RelayCommand FavoritItemCommand
        {
            get { return new RelayCommand(() => { ItemAction(_childId, EnumTypeOperationNaviPanel.Favorite); }, () => true); }
        }

        public RelayCommand DeleteItemCommand
        {
            get { return new RelayCommand(() => { ItemAction(_childId, EnumTypeOperationNaviPanel.Delete); }, () => true); }
        }

        public RelayCommand PlayItemCommand
        {
            get { return new RelayCommand(() => { ItemAction(_childId, EnumTypeOperationNaviPanel.Play); }, () => true); }
        }

        
    }
}
