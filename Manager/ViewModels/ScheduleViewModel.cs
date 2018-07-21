using Mvvm;
using Mvvm.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Input;

namespace Manager.ViewModels
{
    public class ScheduleViewModel : BindableBase
    {
        #region Fields

        private ObservableCollection<PupilViewModel> _pupils = new ObservableCollection<PupilViewModel>();
        private ObservableCollection<WeekItem> _items = new ObservableCollection<WeekItem>();
        private int _weeksCount;

        #endregion

        #region Properties

        public ObservableCollection<PupilViewModel> Pupils { get => _pupils; set => SetProperty(ref _pupils, value); }
        public ObservableCollection<WeekItem> Items { get => _items; set => SetProperty(ref _items, value); }
        public int WeeksCount { get => _weeksCount; set => SetProperty(ref _weeksCount, value); }

        #endregion

        #region Commands

        public ICommand MoveNextCommand { get; set; }
        public ICommand MovePrevCommand { get; set; }
        public ICommand RefreshCommand { get; set; }

        public ICommand SaveCommand { get; set; }
        public ICommand LoadCommand { get; set; }

        #endregion

        #region Command handlers

        private void OnMoveNext()
        {
            var last = Items.LastOrDefault();
            var time = last?.Date ?? DateTime.Today;

            RefreshFromStore(time);
        }

        private void OnMovePrev()
        {
            var first = Items.FirstOrDefault();
            var time = first?.Date ?? DateTime.Today;

            RefreshFromStore(time);
        }

        private void OnRefresh()
        {
            var first = Items.FirstOrDefault();
            var time = first?.Date.AddDays(7 * WeeksCount / 2) ?? DateTime.Today;

            RefreshFromStore(time);
        }

        #endregion

        public ScheduleViewModel(int weekcount = 8)
        {
            MoveNextCommand = new DelegateCommand(OnMoveNext);
            MovePrevCommand = new DelegateCommand(OnMovePrev);
            RefreshCommand = new DelegateCommand(OnRefresh);

            WeeksCount = weekcount;
            RefreshFromStore(DateTime.Today);
        }

        #region Methods

        private void RefreshFromStore(DateTime middle)
        {
            Pupils = new ObservableCollection<PupilViewModel>(Store.Store.Instance.FindAll().Select(x => new PupilViewModel(x)));
            RefreshSchedule(middle);
        }

        private void RefreshSchedule(DateTime middle)
        {            
            Items.Clear();

            var list = new List<WeekItem>();

            middle = Store.Helper.GetMonday(middle);
            
            var dates = new List<DateTime>();
            for (int i = 0; i < WeeksCount / 2; i++)
            {
                dates.Add(middle.AddDays(7 * i));
                dates.Add(middle.AddDays(7 * -i));
            }
            
            dates.Sort();

            foreach (var pupil in Pupils)
            {
                foreach (var date in dates)
                {
                    list.Add(new WeekItem(pupil, date));
                }
//                var time = middle.AddDays(-7 * (WeeksCount / 2));
//
//                for (int i = 0; i < WeeksCount; i++)
//                {
//                    list.Add(new WeekItem(pupil, time));
//                    time.AddDays(7);
//                }
            }

            Items = new ObservableCollection<WeekItem>(list);
        }

        #endregion

    }

    public class WeekItem : BindableBase
    {
        #region Fields

        private bool _hasValue;
        private LessonViewModel _lesson;
        private PupilViewModel _pupil;
        private DateTime _date;

        #endregion

        #region Properties

        public bool HasValue { get => _hasValue; set => SetProperty(ref _hasValue, value); }
        public LessonViewModel Lesson { get => _lesson; set => SetProperty(ref _lesson, value); }
        public PupilViewModel Pupil { get => _pupil; set => SetProperty(ref _pupil, value); }
        public DateTime Date { get => _date; set => SetProperty(ref _date, value); }

        #endregion


        public WeekItem(PupilViewModel pupil, DateTime date)
        {
            Date = date;
            Pupil = new PupilViewModel(pupil.ToModel());
            Lesson = pupil.Lessons.FirstOrDefault(x => Store.Helper.TheSameWeek(date, x.Date));
            HasValue = Lesson != null;

            // Костыль для команд в расписании.
            // Ссылаемся на команды PupilViewModel
            Pupil.SelectedLesson = Lesson;
        }
    }
}
