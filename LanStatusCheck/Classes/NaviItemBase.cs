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
        public RelayCommand UpItemCommand
        {
            get { return new RelayCommand(TapUpItemButton, () => true); }
        }

        private void TapUpItemButton()
        {

        }

        public RelayCommand DownItemCommand => throw new NotImplementedException();
    }
}
