using System.Windows;
using Manager.ViewModels;

namespace Manager.Windows
{
    public partial class StartWindow
    {
        public StartWindow()
        {
            InitializeComponent();
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            Hide();

            // закрыли самое первое окно
            if (!TryToLoad())
            {
                Close();
                return;
            }

            // если решили сохранять
            if (NeedToSave())
            {
                // если сохранили - выходим
                if (WasSaved())
                {
                    MessageBox.Show("Готово!");
                    
                    Close();
                    return;
                }
                
                // если закрыли сами, вернемся к первому окну
            }
            
            
            Show();
            OnLoad(null, null);
        }

        #region Helping methods

        /// <summary>
        /// Пытаемся загрузить
        /// </summary>
        /// <returns></returns>
        private bool TryToLoad()
        {
            var vm = new LoadingViewModel();
            var window = GetWindow(vm, 320, 400);

            // Closed by user
            if (window.ShowDialog() != true)
                return false;

            if (vm.HasError)
                return TryToLoad();

            return true;
        }

        /// <summary>
        /// Нужно ли сохранить после редактирования
        /// </summary>
        /// <returns></returns>
        private bool NeedToSave()
        {
            var vm = new ScheduleViewModel();
            var window = GetWindow(vm, 600, 400, 3);

            return window.ShowDialog() == true;
        }

        /// <summary>
        /// Пытаемся сохранить
        /// </summary>
        /// <returns></returns>
        private bool WasSaved()
        {
            var vm = new LoadingViewModel(false);
            var window = GetWindow(vm, 320, 400);

            // Closed by user
            if (window.ShowDialog() != true)
                return false;

            if (vm.HasError)
                return WasSaved();

            return true;
        }

        /// <summary>
        /// Настраиваем окошко по размерам
        /// </summary>
        /// <param name="dataContext"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="multiplier"></param>
        /// <returns></returns>
        private Window GetWindow(object dataContext, int width, int height, int multiplier = 2)
        {
            return new AppWindow
            {
                DataContext = dataContext,

                MinHeight = height,
                Height = height,
                MaxHeight = height * multiplier,

                MinWidth = width,
                Width = width,
                MaxWidth = width * multiplier,
            };
        }


        #endregion
    }
}