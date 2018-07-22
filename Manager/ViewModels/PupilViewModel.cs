using Manager.Model;
using Manager.ViewModels.Base;
using Mvvm.Commands;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;

namespace Manager.ViewModels
{
    public class PupilViewModel : UpdatableViewModelBase<Pupil>
    {
        #region Fields

        private string _name;
        private ObservableCollection<LessonViewModel> _lessons = new ObservableCollection<LessonViewModel>();
        private LessonViewModel _selectedLesson;

        #endregion

        #region Properties

        public string Name { get => _name; set => SetProperty(ref _name, value); }
        public ObservableCollection<LessonViewModel> Lessons { get => _lessons; set => SetProperty(ref _lessons, value); }

        public LessonViewModel SelectedLesson { get => _selectedLesson; set => SetProperty(ref _selectedLesson, value); }

        #endregion

        #region Command

        public ICommand AddEditLessonCommand { get; set; }
        public ICommand RemoveLessonCommand { get; set; }
        public ICommand DeletePupilCommand { get; set; }
        public ICommand ChangePupilCommand { get; set; }
        public ICommand RefreshCommand { get; set; }

        #endregion

        #region Command Handlers

        private void OnRemoveLesson(LessonViewModel vm)
        {
            var model = vm.ToModel();
            
            // сначала удаляем модель из параметра
            if (model != null && Lessons.Any(x => x.TheSame(model)))
            {
                Store.Store.Instance.RemoveLesson(model);
                return;
            }
            
            if (SelectedLesson == null)
                return;

            Store.Store.Instance.RemoveLesson(SelectedLesson.ToModel());
        }

        private void OnAddEditLesson(object parameter)
        {
            // Можем передать модель
            if (parameter is LessonViewModel vm
                && Lessons.Any(x => x.TheSame(vm.ToModel())))
            {
                SelectedLesson = vm;
                EditLesson();
                return;
            }

            // либо передаем дату
            if (SelectedLesson == null && parameter is DateTime time)
            {
                AddLesson(time);
                return;
            }
            
            // в иных случаях запускаем создание нового правила
            EditLesson();
        }

        private async void EditLesson()
        {
            var old = SelectedLesson?.ToModel();

            var newViewModel = new LessonViewModel(old);
            
            var result = await DialogHost.Show(newViewModel, "LessonHost");
            
            if (!true.Equals(result))
                return;

            Store.Store.Instance.Replace(old, newViewModel.ToModel());
        }

        private async void AddLesson(DateTime monday)
        {
            var model = new Lesson
            {
                Date = monday,
                Name = Name
            };

            var vm = new LessonViewModel(model);

            var result = await DialogHost.Show(vm, "LessonHost");
            
            if (!true.Equals(result))
                return;

            var lesson = vm.ToModel();
            
            Lessons.Add(vm);
            Store.Store.Instance.AddLesson(lesson);
            
            // создаем таск для партнера
            TryToCreatePartnerLesson(lesson);
        }

        private void OnDeletePupil()
        {
            Clear();
            Store.Store.Instance.RemovePupil(benchMark.Name);
        }

        private async void OnChangePupil()
        {
            var old = ToModel();

            var vm = new PupilViewModel(old);

            var result = await DialogHost.Show(vm, "PupilHost");

            if (!true.Equals(result)
                || !vm.hasChanged
                || Equals(old, vm.ToModel()))
                return;

            if (benchMark == null 
                || Store.Store.Instance.FindByName(benchMark.Name) == null)
            {
                Store.Store.Instance.Load(vm.ToModel());
            }
            else
            {
                Store.Store.Instance.ReplacePupil(benchMark.Name, vm.ToModel());
            }
        }

        #endregion

        #region Event handlers

        private void OnStoreChanged(object sender, EventArgs e)
        {
            Refresh(Store.Store.Instance.FindByName(Name));
        }

        #endregion

        #region Methods

        public void Clear()
        {
            Store.Store.Instance.StoreChanged -= OnStoreChanged;
        }

        /// <summary>
        /// Создаёт парное задание для компаньона
        /// </summary>
        /// <param name="lesson"></param>
        private void TryToCreatePartnerLesson(Lesson lesson)
        {
            // Можем задать side-task только если мы главные, и есть имя партнёра
            if (!lesson.IsMain || string.IsNullOrWhiteSpace(lesson?.Partner))
                return;

            var find = Store.Store.Instance.FindByName(lesson.Partner);
            if (find == null)
                return;
            
            // навык не трогаем, нам не нужен
            var parnterLesson = new Lesson
            {
                Date = lesson.Date,
                IsMain = false,
                LessonType = lesson.LessonType,
                Name = find.Name,
                // в данном случа патнер - основной выступающий
                Partner = lesson.Name
            };
            
            Store.Store.Instance.AddLesson(parnterLesson);
        }

        #endregion

        public PupilViewModel(Pupil model = null) : base(model)
        {
            Store.Store.Instance.StoreChanged += OnStoreChanged;

            RemoveLessonCommand = new DelegateCommand<LessonViewModel>(OnRemoveLesson, (lesson) => lesson != null || SelectedLesson != null);
            AddEditLessonCommand = new DelegateCommand<object>(OnAddEditLesson);
            DeletePupilCommand = new DelegateCommand(OnDeletePupil);
            ChangePupilCommand = new DelegateCommand(OnChangePupil);
            RefreshCommand = new DelegateCommand(() => OnStoreChanged(null, EventArgs.Empty));
        }

        #region Implemented
        public override Pupil ToModel()
        {
            return new Pupil
            {
                Name = Name,
                Lessons = Lessons.Select(x => x.ToModel()).ToList()
            };
        }

        protected override void RefreshOverride(Pupil model)
        {
            var selected = SelectedLesson?.ToModel();
            
            Name = model.Name;
            Lessons = new ObservableCollection<LessonViewModel>(model.Lessons.Select(x => new LessonViewModel(x)));
            
            if (selected != null)
                SelectedLesson = new LessonViewModel(selected);
        }
        #endregion
    }
}
