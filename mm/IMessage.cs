namespace mm
{
    /// <summary>
    /// Класс сообщения, доставляемого подписанному клиенту
    /// </summary>
    public interface IMessage : System.Collections.IEnumerable, System.IComparable
    {
        /// <summary>
        /// Отправитель сообщения
        /// </summary>
        object Sender { get; set; }

        /// <summary>
        /// Получатель сообщения
        /// </summary>
        object Receiver { get; set; }

        /// <summary>
        /// Тип сообщения
        /// </summary>
        object Type { get; set; }

        /// <summary>
        /// Число параметров
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Индексатор для аргументов сообщения
        /// </summary>
        /// <param name="param">Индекс параметра</param>
        /// <returns>Значение параметра</returns>
        object this[int param] { get; set; }

        /// <summary>
        /// Добавить параметры в сообщение
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns>Текущий экземпляр сообщения</returns>
        IMessage Add(object parameter );

        /// <summary>
        /// Метод присваивает идентификатор адресата
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Текущий экземпляр сообщения</returns>
        IMessage To( object id );

        /// <summary>
        /// Метод присваивает тип сообщения
        /// </summary>
        /// <param name="o"></param>
        /// <returns>Текущий экземпляр сообщения</returns>
        IMessage IsType( object o );
     
        /// <summary>
        /// Вернуть параметр сообщения по индексу
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        T As<T>( int index );
    }
}