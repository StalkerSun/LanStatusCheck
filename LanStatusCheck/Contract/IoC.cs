using Ninject;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanStatusCheck.Contract
{
    public static class IoC
    {
        private static StandardKernel kernel;

        /// <summary>
        /// Метод производит инициализацию библиотеки Ninject 
        /// осуществляет привязку интерфейса к конкретной реализации
        /// </summary>
        /// <param name="modules"></param>
        public static void Initialization(params INinjectModule[] modules) => kernel = new StandardKernel(modules);

        /// <summary>
        /// Метод возвращает экземпляр класса реализующего интерфейс
        /// </summary>
        /// <typeparam name="T">Требуемый экземпляр</typeparam>
        /// <returns></returns>
        public static T Get<T>() => kernel == null ? default(T) : kernel.Get<T>();
    }
}
