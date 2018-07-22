using System;
using System.Collections.Generic;
using System.Windows;
using Manager.Model;
using Manager.ViewModels;
using MaterialDesignThemes.Wpf;

namespace Manager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ScheduleView_OnLoaded(object sender, RoutedEventArgs e)
        {
            var me = new Pupil
            {
                Name = "Arkady",
                Lessons = new List<Lesson>
                {
                    new Lesson
                    {
                      Date  = DateTime.Today,
                        IsMain = true,
                        Number = 1,
                        LessonType = LessonTypes.Reading,
                        Name = "Arkady"
                    },
                    
                    new Lesson
                    {
                        Date  = DateTime.Today.AddDays(-7 * 2),
                        IsMain = true,
                        Number = 50,
                        LessonType = LessonTypes.Study,
                        Name = "Arkady",
                        Partner = "Balor"
                    },
                    
                    new Lesson
                    {
                        Date  = DateTime.Today.AddDays(-7 * 2),
                        IsMain = false,
                        LessonType = LessonTypes.Study,
                        Name = "Arkady",
                        Partner = "Balor1"
                    },
                    
                    new Lesson
                    {
                        Date  = DateTime.Today.AddDays(-7 * 2),
                        IsMain = false,
                        LessonType = LessonTypes.Study,
                        Name = "Arkady",
                        Partner = "Balor2"
                    },
                }
            };
            
            var balor = new Pupil
            {
                Name = "Balor",
                Lessons = new List<Lesson>
                {
                    new Lesson
                    {
                        Date  = DateTime.Today.AddDays(7),
                        IsMain = true,
                        Number = 12,
                        LessonType = LessonTypes.Reading,
                        Name = "Balor"
                    },
                    
                    new Lesson
                    {
                        Date  = DateTime.Today.AddDays(-7 * 2),
                        IsMain = true,
                        LessonType = LessonTypes.Study,
                        Name = "Balor",
                        Number = 10,
                        Partner = "Arkady"
                    }
                }
            };
            
            var student = new Pupil
            {
                Name = "student",
                Lessons = new List<Lesson>
                {
                    new Lesson
                    {
                        Date  = DateTime.Today.AddDays(-7 * 2),
                        IsMain = false,
                        LessonType = LessonTypes.Study,
                        Name = "student",
                        Partner = "Arkady"
                    }
                }
            };
            
            Store.Store.Instance.Load(me);
            Store.Store.Instance.Load(balor);
            Store.Store.Instance.Load(student);

//            for (int i = 0; i < 10; i++)
//            {
//                var lesson = new Lesson
//                {
//                    Date = DateTime.Today,
//                    IsMain = i % 2 == 0,
//                    LessonType = (LessonTypes) (i % 4),
//                    Number = i * 2,
//                    Name = "Name " + i,
//                };
//                
//                var pupil = new Pupil
//                {
//                    Name = "Name " + i,
//                };
//                
//                pupil.Lessons.Add(lesson);
//
//                lesson.Number /= 2;
//                lesson.Partner = "Another name";
//                lesson.Date = lesson.Date.AddDays(-7);
//                    
//                
//                pupil.Lessons.Add(lesson);
//                
//                Store.Store.Instance.Load(pupil);
//            }
//
            var dataContext = new ScheduleViewModel();
            ScheduleView.DataContext = dataContext;
        }

        private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
        {
//            var lesson = new LessonViewModel
//            {
//                Name = "Name",
//                LessonType = LessonTypes.Reading,
//                Date = DateTime.Today,
//                Number = 12,
//                IsMain = true
//            };
//            
//            var lesson1 = new LessonViewModel
//            {
//                Name = "Name",
//                LessonType = LessonTypes.Study,
//                Date = DateTime.Today.AddDays(-50),
//                Number = 50,
//                IsMain = false,
//                Partner = "Family",
//            };
//            
//            var vm = new PupilViewModel
//            {
//                Name = "Name"
//            };
//            vm.Lessons.Add(lesson);
//            vm.Lessons.Add(lesson1);
//
//            ((FrameworkElement) sender).DataContext = vm;
//            
//            Store.Store.Instance.Load(vm.ToModel());
        }
    }
}