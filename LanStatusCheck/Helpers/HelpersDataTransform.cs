﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanStatusCheck.Helpers
{
    public static class HelpersDataTransform
    {
        public static int MinNodeInSequence = 6;

        #region public

        /// <summary>Определение выбросов в последовательности данных</summary>
        /// <param name="sequens">Исходная последовательность (минимум 6 эллементов)</param>
        /// <param name="minVal">Возвращает: Минимальное заначение внутренних границ</param>
        /// <param name="maxVal">Возвращает: Максимальное заначение внутренних границ</param>
        /// <returns>True - все прошло ок, False - произошла ошибка</returns>
        public static bool DetectEmissinsFromSequence(List<double> sequens, out double minVal, out double maxVal)
        {
            minVal = maxVal = 0;

            if (sequens == null || sequens.Count < MinNodeInSequence) return false;

            var localSeq = new List<double>(sequens).OrderBy(i => i).ToList();

            //вычисляем медиану последовательности Q2

            if (!GetValueAndIndexMedianSequence(localSeq, out double q2, out List<int> q2Index)) return false;

            double q1, q3 = double.NaN;

            bool res;

            if(q2Index.Count==1)
            {

                //вычисляем нижний квартиль
                res = GetValueMedianSequence(localSeq.GetRange(0, q2Index[0]), out q1);

                //вычисляем верхний квартиль
                if (res)
                    res = GetValueMedianSequence(localSeq.GetRange(q2Index[0]+1, q2Index[0]), out q3);
            }
            else
            {
                //вычисляем нижний квартиль
                res = GetValueMedianSequence(localSeq.GetRange(0, q2Index[1]), out q1);

                //вычисляем верхний квартиль
                if (res)
                    res = GetValueMedianSequence(localSeq.GetRange(q2Index[1], q2Index[1]), out q3);
            }

            if (!res) return false;

            //вычисляем межквартильный диапазон

            var deltaQ = q3 - q1;

            //Вычисляем внутренние границы

            minVal = q1 - (deltaQ * 1.5);

            maxVal = q3 + (deltaQ * 1.5);


            return true;
        }


        public static List<double> DeleteEmissinsFromSequence(List<double> sequens)
        {
            if (sequens == null || sequens.Count < MinNodeInSequence)
                return new List<double>();

            if (!DetectEmissinsFromSequence(sequens, out double min, out double max))
                return new List<double>();

            var localList = new List<double>(sequens);

            var resSeq = localList.RemoveAll(a => (a < min || a > max));

            return localList;
        }

        #endregion

        #region private

        private static bool GetValueAndIndexMedianSequence (List<double> seq, out double valMedian, out List<int> indexMedian)
        {
            valMedian = double.NaN;

            indexMedian = new List<int>();

            if (seq == null )
                return false;

            if ((seq.Count % 2) == 0)
            {

                int ind1 = seq.Count / 2 - 1;

                int ind2 = seq.Count / 2;

                valMedian = (seq[ind1] + seq[ind2]) / 2;

                indexMedian.Add(ind1);

                indexMedian.Add(ind2);

            }
            else
            {
                int ind1 = seq.Count / 2;

                valMedian = seq[ind1];

                indexMedian.Add(ind1);

            }

            return true;
        }

        private static bool GetValueMedianSequence(List<double> seq, out double valMedian)
        {
            valMedian = double.NaN;

            if (seq == null)
                return false;

            if ((seq.Count % 2) == 0)
            {

                int ind1 = seq.Count / 2 - 1;

                int ind2 = seq.Count / 2;

                valMedian = (seq[ind1] + seq[ind2]) / 2;
            }
            else
            {
                int ind1 = seq.Count / 2;

                valMedian = seq[ind1];
            }

            return true;
        }

        #endregion

    }
}
