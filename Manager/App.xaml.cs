using System;
using System.Windows;
using System.Windows.Navigation;
using Manager.ViewModels;
using Manager.Windows;

namespace Manager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            //InitialLoading();
        }

        /// <summary>
        /// Открываем окно загрузки. Начало программы.
        /// </summary>
        private void InitialLoading()
        {
            var vm = new LoadingViewModel();

            var window = new AppWindow
            {
                MinWidth = 320,
                MaxWidth = 320 * 2,
                MinHeight = 400,
                MaxHeight = 400 * 2,

                Content = vm
            };

            // загружаем данные в Store
            var result = window.ShowDialog();

            // загрузили, что-то отредактировали и готовы сохранить
            if (true.Equals(result)
                || vm.WasLoaded
                || AfterLoading())
            {
                if (OnSaving())
                {
                    App.Current.Shutdown();
                }
            }

            InitialLoading();
        }

        /// <summary>
        /// Открываем окно редактирования (расписания)
        /// </summary>
        private bool AfterLoading()
        {
            // открываем окно редактирования
            var editWindow = new AppWindow
            {
                MinWidth = 600,
                MaxWidth = 600 * 3,
                MinHeight = 400,
                MaxHeight = 400 * 3,

                Content = new ScheduleViewModel()
            };

            return editWindow.ShowDialog() == true;
        }

        /// <summary>
        /// Сохраняем результаты
        /// </summary>
        /// <returns></returns>
        private bool OnSaving()
        {
            var vm = new LoadingViewModel(false);
            
            var window = new AppWindow
            {
                MinWidth = 320,
                MaxWidth = 320 * 2,
                MinHeight = 400,
                MaxHeight = 400 * 2,

                Content = vm
            };

            var result = window.ShowDialog();

            return result == true || vm.WasLoaded;
        }
    }
}
