using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanStatusCheck.Classes
{
    public static class HelpersDataTransform
    {
        /// <summary>Определение выбросов в последовательности данных</summary>
        /// <param name="sequens">Исходная последовательность (минимум 6 эллементов)</param>
        /// <param name="minVal">Возвращает: Минимальное заначение внутренних границ</param>
        /// <param name="maxVal">Возвращает: Максимальное заначение внутренних границ</param>
        /// <returns>True - все прошло ок, False - произошла ошибка</returns>
        public static bool DetectEmissinsFromSequence( List<double> sequens, out double minVal, out double maxVal)
        {
            minVal = maxVal = 0;

            if (sequens==null || sequens.Count < 6) return false;

            var localSeq = new List<double>(sequens).OrderBy(i=>i).ToList();

            //вычисляем медиану последовательности Q2

            double q2;

            List<int> q2Index = new List<int>();

            if((localSeq.Count % 2) == 0)
            {

                int ind1 = localSeq.Count / 2 - 1;

                int ind2 = localSeq.Count / 2;

                q2 = (localSeq[ind1] + localSeq[ind2]) / 2;

                q2Index.Add(ind1);

                q2Index.Add(ind2);

            }
            else
            {
                int ind1 = localSeq.Count / 2;

                q2 = localSeq[ind1];

                q2Index.Add(ind1);

            }

            //вычисляем нижний квартиль

            


            return true;
        }

        public static List<double> DeleteEmissinsFromSequence(List<double> sequens)
        {
            if (sequens == null || sequens.Count < 6)
                return new List<double>();


            if (!DetectEmissinsFromSequence(sequens, out double min, out double max))
                return new List<double>();



            return sequens;
        }

    }
}
