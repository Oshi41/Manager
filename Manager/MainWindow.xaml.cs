using System;
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

            for (int i = 0; i < 10; i++)
            {
                var lesson = new Lesson
                {
                    Date = DateTime.Today.AddDays(-i * 7),
                    IsMain = i % 2 == 0,
                    LessonType = (LessonTypes) (i % 4),
                    Number = i * 2,
                    Name = "Name " + i,
                };
                
                var pupil = new Pupil
                {
                    Name = "Name " + i,
                };
                
                pupil.Lessons.Add(lesson);

                lesson.Number /= 2;
                lesson.Partner = "Another name";
                
                pupil.Lessons.Add(lesson);
                
                Store.Store.Instance.Load(pupil);
            }
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