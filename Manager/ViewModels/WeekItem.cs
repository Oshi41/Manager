﻿using System;
using System.Collections.Generic;
using System.Linq;
using Manager.Helper;
using Mvvm;

namespace Manager.ViewModels
{
    public class WeekItem : BindableBase
    {
        #region Fields

        private bool _hasValue;
        private List<LessonViewModel> _lessons;
        private PupilViewModel _pupil;
        private DateTime _date;

        #endregion

        #region Properties

        public bool HasValue
        {
            get => _hasValue;
            set => SetProperty(ref _hasValue, value);
        }

        public List<LessonViewModel> Lessons
        {
            get => _lessons;
            set => SetProperty(ref _lessons, value);
        }

        public PupilViewModel Pupil
        {
            get => _pupil;
            set => SetProperty(ref _pupil, value);
        }

        public DateTime Date
        {
            get => _date;
            set => SetProperty(ref _date, value);
        }

        #endregion


        public WeekItem(PupilViewModel pupil, DateTime date)
        {
            Date = date;
            Pupil = new PupilViewModel(pupil.ToModel());
            Lessons = pupil
                      .Lessons
                      .Where(x => DateHelper
                                 .TheSameWeek(date, x.Date))
                      .ToList();
            
            HasValue = Lessons.Any();
        }
    }
}