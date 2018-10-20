using LanStatusCheck.Classes;
using LanStatusCheck.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LanStatusCheck.Interfaces
{
    internal interface INaviItemInList
    {
        event Action<string, EnumTypeOperationNaviPanel> ItemAction;

        RelayCommand UpItemCommand { get; }

        RelayCommand DownItemCommand { get; }

        RelayCommand FavoritItemCommand { get; }

        RelayCommand DeleteItemCommand { get; }

        RelayCommand PlayItemCommand { get; }
    }
}
