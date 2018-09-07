using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanStatusCheck.Classes
{
    public static class HelpersDataTransform
    {

        /// <summary>Удаление выбросов из исхходной последовательности данных</summary>
        /// <param name="sequens">Исходная последовательность</param>
        /// <returns>Возвращает очищенную от выбросов исходную коллекцию</returns>
        public static List<double> DeleteEmissinsFromSequence( List<double> sequens)
        {


            var localSeq = new List<double>(sequens).OrderBy(i=>i).ToList();

            return localSeq;
        }

    }
}
