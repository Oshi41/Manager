using System;
using System.IO.Packaging;
using System.Security;
using System.Windows;
using System.Windows.Navigation;
using Manager.Models;
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

            //CryptTest();
        }

        private void CryptTest()
        {
            var password = "1234567890123456";
            var pass = new SecureString();
            foreach (var c in password)
            {
                pass.AppendChar(c);
            }

            EncryptionType type = EncryptionType.ForMachine;

            var model = new CryptoModel();


            var text = "Очень сложный и запустыннй текст? event using the Edlish letters";

            var encrypted = model.Encrypt(type, text, pass);

            var decrypted = model.Decrypt(type, encrypted, pass);

            var equals = string.Equals(decrypted, text);
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
