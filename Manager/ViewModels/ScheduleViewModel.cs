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
        private ObservableCollection<DateTime> _dates;

        #endregion

        #region Properties

        public ObservableCollection<PupilViewModel> Pupils
        {
            get => _pupils;
            set => SetProperty(ref _pupils, value);
        }

        public ObservableCollection<WeekItem> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }

        public ObservableCollection<DateTime> Dates
        {
            get => _dates;
            set => SetProperty(ref _dates, value);
        }

        public int WeeksCount
        {
            get => _weeksCount;
            set
            {
                // Всегда нечет!
                var val = value;
                if (val % 2 == 0)
                    val++;

                SetProperty(ref _weeksCount, val);
            }
        }

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
            var time = DateTime.Now;
            var middle = Dates[Dates.Count / 2];
            if (!Store.Helper.TheSameWeek(time, middle))
                RefreshFromStore(DateTime.Now);
        }

        #endregion

        public ScheduleViewModel(int weekcount = 9)
        {
            MoveNextCommand = new DelegateCommand(OnMoveNext);
            MovePrevCommand = new DelegateCommand(OnMovePrev);
            RefreshCommand = new DelegateCommand(OnRefresh);

            WeeksCount = weekcount;
            RefreshFromStore(DateTime.Today);

            Store.Store.Instance.StoreChanged += OnRefreshByStore;
        }

        #region Methods

        private void OnRefreshByStore(object sender, EventArgs e)
        {
            var time = Dates[Dates.Count / 2];

            RefreshFromStore(time);
        }

        private void RefreshFromStore(DateTime middle)
        {
            Pupils = new ObservableCollection<PupilViewModel>(Store
                                                              .Store
                                                              .Instance
                                                              .FindAll()
                                                              .Select(x => new PupilViewModel(x)));
            RefreshSchedule(middle);
        }

        private void RefreshSchedule(DateTime middle)
        {
            Items.Clear();

            var list = new List<WeekItem>();

            middle = Store.Helper.GetMonday(middle);

            FillDates(middle);

            foreach (var pupil in Pupils)
            {
                foreach (var date in Dates)
                {
                    list.Add(new WeekItem(pupil, date));
                }
            }

            Items = new ObservableCollection<WeekItem>(list);
        }

        private void FillDates(DateTime middle)
        {
            // заполнили текующую дату
            var dates = new List<DateTime> {middle};

            for (int i = 1; i <= WeeksCount / 2; i++)
            {
                dates.Add(middle.AddDays(7 * i));
                dates.Add(middle.AddDays(7 * -i));
            }

            dates.Sort();

            Dates = new ObservableCollection<DateTime>(dates);
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

        public bool HasValue
        {
            get => _hasValue;
            set => SetProperty(ref _hasValue, value);
        }

        public LessonViewModel Lesson
        {
            get => _lesson;
            set => SetProperty(ref _lesson, value);
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
            Lesson = pupil.Lessons.FirstOrDefault(x => Store.Helper.TheSameWeek(date, x.Date));
            HasValue = Lesson != null;

            // Костыль для команд в расписании.
            // Ссылаемся на команды PupilViewModel
            Pupil.SelectedLesson = Lesson;
        }
    }
}