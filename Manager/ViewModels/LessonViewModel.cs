using Manager.Model;
using Manager.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Manager.ViewModels
{
    public class LessonViewModel : UpdatableViewModelBase<Lesson>
    {
        #region Fields

        private string _name;
        private string _partner;
        private int? _number;
        private DateTime _date;
        private bool _isMain = true;
        private LessonTypes _lessonType;
        private List<string> _allPupils;


        #endregion

        public LessonViewModel()
            : this(null)
        {

        }

        public LessonViewModel(Lesson model = null) : base(model)
        {
            AllPupils = Store
                        .Store
                        .Instance
                        .FindAll()
                        .Where(x => !string.Equals(x.Name, Name))
                        .Select(x => x.Name).ToList();

            if (Date == default(DateTime))
                Date = DateTime.Today;
        }

        #region Properties

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Partner
        {
            get => _partner;
            set => SetProperty(ref _partner, value);
        }

        public int? Number
        {
            get => _number;
            set => SetProperty(ref _number, value);
        }

        public DateTime Date
        {
            get => _date;
            set => SetProperty(ref _date, value);
        }

        public bool IsMain
        {
            get => _isMain;
            set
            {
                if (SetProperty(ref _isMain, value))
                    Number = null;
            }
        }

        public LessonTypes LessonType
        {
            get => _lessonType;
            set => SetProperty(ref _lessonType, value);
        }

        public List<string> AllPupils
        {
            get => _allPupils;
            set => SetProperty(ref _allPupils, value);
        }

        public static List<int> AllLessons { get; } = Enumerable.Range(1, 53).ToList();

        #endregion

        #region Implemented

        public override Lesson ToModel()
        {
            return new Lesson
            {
                Name = Name,
                Partner = Partner,
                Number = Number ?? 0,
                Date = Date,
                IsMain = IsMain,
                LessonType = LessonType,
            };
        }

        protected override void RefreshOverride(Lesson model)
        {
            Name = model.Name;
            Partner = model.Partner;
            Number = model.Number;
            Date = model.Date;
            IsMain = model.IsMain;
            LessonType = model.LessonType;
        }

        #endregion
    }
}