using Mvvm;
using Mvvm.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Manager.Helper;
using Manager.Parsable;

namespace Manager.ViewModels
{
    public class ScheduleViewModel : BindableBase
    {
        #region Fields

        private ObservableCollection<PupilViewModel> _pupils = new ObservableCollection<PupilViewModel>();
        private ObservableCollection<WeekItem> _items = new ObservableCollection<WeekItem>();
        private int _weeksCount;
        private ObservableCollection<DateHeader> _dates;

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

        public ObservableCollection<DateHeader> Dates
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

                if (SetProperty(ref _weeksCount, val))
                    OnRefresh();
            }
        }

        public StringFilter Filter { get; }

        #endregion

        #region Commands

        public ICommand MoveNextCommand { get; set; }
        public ICommand MovePrevCommand { get; set; }
        public ICommand RefreshCommand { get; set; }

        public ICommand AddPupil { get; set; }

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

        private void OnReset()
        {
            if (!DateHelper.TheSameWeek(DateTime.Now, FindMiddle()))
                RefreshFromStore(DateTime.Now);
        }

        private void OnRefresh()
        {
            RefreshFromStore(FindMiddle());
        }

        private void OnCreatePupil()
        {
            var vm = new PupilViewModel();
            vm.AddChangePupilCommand.Execute(null);
        }

        #endregion

        public ScheduleViewModel(int weekcount = 9)
        {
            _weeksCount = weekcount;
            
            MoveNextCommand = new DelegateCommand(OnMoveNext);
            MovePrevCommand = new DelegateCommand(OnMovePrev);
            RefreshCommand = new DelegateCommand(OnReset);
            AddPupil = new DelegateCommand(OnCreatePupil);
            
            Filter = new StringFilter(new DelegateCommand(OnRefresh));
            
            RefreshFromStore(DateTime.Today);

            Store.Store.Instance.StoreChanged += OnRefreshByStore;
        }

        #region Methods

        private void OnRefreshByStore(object sender, EventArgs e)
        {
            RefreshFromStore(FindMiddle());
        }

        private void RefreshFromStore(DateTime middle)
        {
            RefreshPupils();
            
            RefreshSchedule(middle);
        }

        private void RefreshPupils()
        {
            IEnumerable<Pupil> all = Store.Store.Instance.FindAll();

            // используем фильтр
            if (Filter.IsEnabled)
                all = all.Where(x => Filter.IsMatch(x.Name));

            // сортируем по имени
            var result = all
                .OrderBy(x => x.Name)
                .ToList();

            Pupils = new ObservableCollection<PupilViewModel>(
                result.Select(x => new PupilViewModel(x)));

        }

        private void RefreshSchedule(DateTime middle)
        {
            Items.Clear();

            var list = new List<WeekItem>();

            middle = DateHelper.GetMonday(middle);

            FillDates(middle);

            foreach (var pupil in Pupils)
            {
                foreach (var date in Dates)
                {
                    list.Add(new WeekItem(pupil, (DateTime)date));
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

            Dates = new ObservableCollection<DateHeader>(dates.Select(x => new DateHeader(x)));
        }

        private DateTime FindMiddle()
        {
            if (Dates?.Any() == true)
            {
                return Dates[Dates.Count / 2].DateTime;
            }

            return DateTime.Today;
        }

        #endregion

    }
}