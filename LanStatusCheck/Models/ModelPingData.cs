namespace LanStatusCheck.Models
{
    public class ModelPingData
    {
        private static ModelPingData _instance;

        private ModelPingData()
        {

        }

        public static ModelPingData GetInstanceModel()
        {
            if (_instance == null)
            {
                _instance = new ModelPingData();
            }

            return _instance;
        }

    }
}
