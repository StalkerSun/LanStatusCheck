namespace mm
{
    /// <summary>
    /// Класс для осуществления отправки сообщения абоненту
    /// </summary>
    public interface IMessenger : System.IDisposable
    {
        /// <summary>
        /// Свойство возвращает текущего абонента отправителя сообщений
        /// </summary>
        object Id { get; }

        /// <summary>
        /// Метод вставляет сообщение в очередь
        /// </summary>
        IMessenger Add(IMessage message);

        /// <summary>
        ///  Метод устанавливает идентификатор абонента 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IMessenger Abonent(object id);

        /// <summary>
        ///  Метод добавляет делегат обработки сообщений
        /// </summary>
        /// <returns></returns>
        IMessenger AddHandler(System.Action<IMessage> handler);
    }
}