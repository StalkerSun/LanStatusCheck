using System.ComponentModel;

namespace msg
{
    /// <summary>
    /// Перечислитель для модулей проекта
    /// </summary>
    public enum Abonent
    {
        [Description( "Абонент не присвоен" )]
        None,

        [Description( "Все диалоги являются приемниками сообщения" )]
        All,

        [Description( "Модель данных сетевых адаптеров" )]
        ModelNetworkAdapters,

        [Description("Модель отображения данных сетевых адаптеров")]
        VModelNetworkAdapters,


    }
}
