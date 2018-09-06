using System.ComponentModel;

namespace msg
{
    /// <summary>
    ///     Перечислитель для типов сообщений при взаимодействии между модулями проекта
    /// </summary>
    public enum MsgType
    {
        [Description( "Инициализация приложения" )]
        InitApp,

        [Description("Новые данные по сетевым адаптерам")]
        UpdateDataModelNetInter,

        [Description("Инициализация сетевых интерфейсов окончена")]
        InitNetInterFinished,


    }
}