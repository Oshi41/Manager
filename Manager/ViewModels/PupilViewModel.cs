using Manager.ViewModels.Base;
using Mvvm.Commands;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Xml.Serialization;
using Manager.Parsable;
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

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public ObservableCollection<LessonViewModel> Lessons
        {
            get => _lessons;
            set => SetProperty(ref _lessons, value);
        }

        public LessonViewModel SelectedLesson
        {
            get => _selectedLesson;
            set => SetProperty(ref _selectedLesson, value);
        }

        // Смотрим на правильное значение
        private string GetValidName => HasChanged && BenchMark != null
            ? BenchMark.Name
            : Name;

        #endregion

        #region Command

        public ICommand AddEditLessonCommand { get; set; }
        public ICommand RemoveLessonCommand { get; set; }
        public ICommand DeletePupilCommand { get; set; }
        public ICommand AddChangePupilCommand { get; set; }
        public ICommand RefreshCommand { get; set; }

        #endregion

        #region Command Handlers

        /// <summary>
        /// Удаляю модель из параметра, либо Selected
        /// </summary>
        /// <param name="vm"></param>
        private void OnRemoveLesson(LessonViewModel vm)
        {
            // передали ли что удалять
            var useParam = vm != null;
            
            // получили модель
            var model = useParam
                ? vm?.ToModel()
                : SelectedLesson?.ToModel();

            // модель != null !!!
            if (model == null)
                return;

            // Что мы удаляем из списка
            var toRemove = useParam
                ? Lessons.FirstOrDefault(x => x.TheSame(model))
                : SelectedLesson;
            
            // Нечего удалять
            if (toRemove == null)
                return;
            
            // удаляем из списка и из store
            Lessons.Remove(toRemove);
            Store.Store.Instance.RemoveLesson(model);
        }

        private void OnAddEditLesson(object parameter)
        {
            // параметра нет
            if (parameter == null)
            {
                // если есть что-то в селекте, редактируем
                if (SelectedLesson != null)
                {
                    EditLesson();
                }
            }
            else
            {
                // параметр - модель, её найдем и будем редактировать
                if (parameter is LessonViewModel vm)
                {
                    // модель для поиска
                    var model = vm.ToModel();
                    // найденный элемент
                    var find = Lessons.FirstOrDefault(x => x.TheSame(model));
                    if (find == null)
                        return;

                    // ставим её в выделение
                    SelectedLesson = find;
                    EditLesson();
                    return;
                }

                if (parameter is DateTime monday)
                {
                    AddLesson(Helper.DateHelper.GetMonday(monday));
                }
            }
        }

        private async void EditLesson()
        {
            var old = SelectedLesson?.ToModel();
            
            if (old == null)
                return;

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
            Store.Store.Instance.RemovePupil(BenchMark.Name);
        }

        private async void OnAddChangePupil()
        {
            var old = BenchMark ?? ToModel();
            var vm = new PupilViewModel(old);

            var result = await DialogHost.Show(vm, "PupilHost");

            // диалог успешно завершен
            if (!true.Equals(result)
                // мы что-то изменили
                || !vm.HasChanged
                // это не точная копия 
                || vm.TheSame(old))
            {
                return;
            }

            // создаём персонажа
            if (Store.Store.Instance.FindByName(old.Name) == null)
            {
                Store.Store.Instance.Load(vm.ToModel());
            }
            else
            {
                Store.Store.Instance.ReplacePupil(old.Name, vm.ToModel());
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

            // Создаем только партнеров, которые есть в Store, иначе он не добавится
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

            RemoveLessonCommand = new DelegateCommand<LessonViewModel>(OnRemoveLesson,
                (lesson) => lesson != null || SelectedLesson != null);
            AddEditLessonCommand = new DelegateCommand<object>(OnAddEditLesson);
            DeletePupilCommand = new DelegateCommand(OnDeletePupil);
            AddChangePupilCommand = new DelegateCommand(OnAddChangePupil);
            RefreshCommand = new DelegateCommand(() => OnStoreChanged(null, EventArgs.Empty));
        }

        #region Implemented

        public override Pupil ToModel()
        {
            var model = new Pupil
            {
                Name = Name,
                Lessons = Lessons.Select(x => x.ToModel()).ToList()
            };
            
            model.Invalidate();

            return model;
        }

        protected override void RefreshOverride(Pupil model)
        {
            // сохраняем селект при обновлении
            var selected = SelectedLesson?.ToModel();

            Name = model.Name;
            Lessons = new ObservableCollection<LessonViewModel>(model.Lessons.Select(x => new LessonViewModel(x)));

            // выставляем селект
            if (selected != null)
            {
                SelectedLesson = Lessons.FirstOrDefault(x => x.TheSame(selected));
            }
        }

        #endregion
    }
}
