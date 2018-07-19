using Manager.Model;
using Microsoft.Win32;
using Mvvm;
using Mvvm.Commands;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Manager.ViewModels
{
    public class LoadingViewModel : BindableBase
    {
        #region Fields
        private string _filePath;
        private EncryptionType _encrType;
        private bool _loadFromFile;

        #endregion

        #region Properties
        public string FilePath { get => _filePath; set => SetProperty(ref _filePath, value); }
        public EncryptionType EncrType { get => _encrType; set => SetProperty(ref _encrType, value); }

        #endregion

        #region Command

        public ICommand OpenFile { get; set; }

        public bool IsLoadFromFile { get => _loadFromFile; set => SetProperty(ref _loadFromFile, value); }

        public ICommand LoadCommand { get; set; }
        public ICommand SaveCommand { get; set; }

        #endregion

        #region Command handlers

        private void OnOpenFile()
        {
            FileDialog dlg;
            if (IsLoadFromFile)
                dlg = new OpenFileDialog();
            else
                dlg = new SaveFileDialog();

            if (dlg.ShowDialog() == true)
                FilePath = dlg.FileName;
        }

        private void OnLoadCommand()
        {
            var json = string.Empty;

            if (File.Exists(FilePath))
            {
                json = File.ReadAllText(FilePath);

                if (EncrType.HasFlag(EncryptionType.ForMachine))
                {

                }

                if (EncrType.HasFlag(EncryptionType.Password))
                {

                }

                if (EncrType.HasFlag(EncryptionType.Base64))
                {

                }
            }

            try
            {
                var pupils = JsonConvert.DeserializeObject<List<Pupil>>(json);
                Store.Store.Instance.Load(pupils);
            }
            catch 
            {

            }
        }

        private void OnSaveCommand()
        {
            var json = JsonConvert.SerializeObject(Store.Store.Instance.FindAll());

            // порядок обратный загрузке!

            if (EncrType.HasFlag(EncryptionType.Base64))
            {

            }

            if (EncrType.HasFlag(EncryptionType.Password))
            {

            }

            if (EncrType.HasFlag(EncryptionType.ForMachine))
            {

            }


            File.WriteAllText(FilePath, json);            
        }

        #endregion

        #region Helping methods

        #endregion

        public LoadingViewModel(bool loadFromFile)
        {
            IsLoadFromFile = loadFromFile;
            LoadCommand = new DelegateCommand(OnLoadCommand, () => File.Exists(FilePath));
            SaveCommand = new DelegateCommand(OnSaveCommand, () => !string.IsNullOrWhiteSpace(FilePath));
        }
    }

    [Flags]
    public enum EncryptionType
    {
        None = 0,
        Base64 = 1,
        Password = 2,
        ForMachine = 4,
    }
}
