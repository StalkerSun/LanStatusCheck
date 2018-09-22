using LanStatusCheck.Contract;
using mm;
using msg;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace LanStatusCheck
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            IoC.Initialization(new ModuleBinding());
        }

    }

    public class ModuleBinding : NinjectModule
    {
        public override void Load()
        {
            // Подписываемся на обработку справочной информации 
            // при передаче сообщения между адресатами
            Configuration.OnLog = m =>
            {
                

                // при обмене сообщениями отображаем информацию в Output
                Debug.WriteLine(m);
            };

            // условия для дополнительной логики обработки сообщений (разослать всем)
            // если абонент = Abonent.All
            Configuration.Criteria.Add(m => m.Receiver.Equals(Abonent.All));

            // Ниже происходит процесс привязки интерфейсов к конкретным реализациям
            // Работает так (на примере IMessage):
            // 1. Запрашивается конкретный интерфейс через IoC.Get<IMessage>()
            // 2. Внутри контейнера IoC проверяется есть ли привязка к интерфейсу
            // 3. Если привязка есть - возвращается конкретный экземпляр реализующий IMessage
            //    в данном случае Message (остальные методы как работают см. документацию по Ninject)
            Bind<IMessage>().To<Message>();

            Bind<IMessenger>().ToMethod(context => new WeakMessenger().Abonent(Abonent.None));
        }
    }
}
