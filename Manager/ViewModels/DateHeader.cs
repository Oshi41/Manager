using System;
using Manager.Helper;
using Mvvm;

namespace Manager.ViewModels
{
    public class DateHeader : BindableBase
    {
        private DateTime _dateTime;

        public DateTime DateTime
        {
            get => _dateTime;
            set => SetProperty(ref _dateTime ,value);
        }

        public bool IsPresentWeek { get; set; }

        public DateHeader(DateTime? time = null)
        {
            DateTime = time ?? DateTime.Now;

            IsPresentWeek = DateHelper.TheSameWeek(DateTime, DateTime.Today);
        }

        #region Work with DateTime

        public static implicit operator DateHeader(DateTime time)
        {
            return new DateHeader(time);
        }

        public static explicit operator DateTime(DateHeader item)
        {
            return item.DateTime;
        } 

        #endregion
    }
}