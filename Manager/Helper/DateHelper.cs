using System;

namespace Manager.Helper
{
    public static class DateHelper
    {
        /// <summary>
        /// Возвращает полночь понедельника недели
        /// </summary>
        /// <param name="first"></param>
        /// <returns></returns>
        public static DateTime GetMonday(DateTime first)
        {
            while (first.DayOfWeek != DayOfWeek.Monday)
                first = first.AddDays(-1);

            return new DateTime(first.Year, first.Month, first.Day);
        }

        public static bool TheSameWeek(DateTime first, DateTime second)
        {
            first = GetMonday(first);
            second = GetMonday(second);

            return first == second;
        }
    }
}