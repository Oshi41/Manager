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

        private void OnRemoveLesson()
        {
            if (SelectedLesson == null)
                return;

            Store.Store.Instance.RemoveLesson(SelectedLesson.ToModel());
        }

        private void OnAddEditLesson(DateTime? monday)
        {
            if (SelectedLesson == null && monday.HasValue)
            {
                AddLesson(monday.Value);
            }
            else
            {
                EditLesson();
            }
        }

        private async void EditLesson()
        {
            var old = SelectedLesson.ToModel();

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

            Store.Store.Instance.AddLesson(vm.ToModel());
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
            
            Store.Store.Instance.ReplacePupil(benchMark.Name, ToModel());
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

        #endregion

        public PupilViewModel(Pupil model = null) : base(model)
        {
            Store.Store.Instance.StoreChanged += OnStoreChanged;

            RemoveLessonCommand = new DelegateCommand(OnRemoveLesson, () => SelectedLesson != null);
            AddEditLessonCommand = new DelegateCommand<DateTime?>(OnAddEditLesson);
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
            Name = model.Name;
            Lessons = new ObservableCollection<LessonViewModel>(model.Lessons.Select(x => new LessonViewModel(x)));
        }
        #endregion
    }
}
