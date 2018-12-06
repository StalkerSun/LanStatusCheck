using LanStatusCheck.Classes;
using LanStatusCheck.Models;

namespace LanStatusCheck.ViewModels
{
    public class VMDataPing : NotifyPropertyBase
    {
        private ModelPingData _model;

        public VMDataPing()
        {
            _model = ModelPingData.GetInstanceModel();
        }
    }
}
