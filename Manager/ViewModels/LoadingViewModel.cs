using Manager.Model;
using Microsoft.Win32;
using Mvvm;
using Mvvm.Commands;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Manager.ViewModels
{
    public class LoadingViewModel : BindableBase
    {
        #region Fields
        private string _filePath;
        private bool _loadFromFile;
        private SecureString _password;
        private bool _useBase64;
        private bool _noEncr;

        #endregion

        #region Properties

        public SecureString Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string FilePath { get => _filePath; set => SetProperty(ref _filePath, value); }
        private EncryptionType EncrType { get; set; }

        public bool NoEncr
        {
            get => _noEncr;
            set => SetProperty(ref _noEncr ,value);
        }

        public bool UseBase64
        {
            get => EncrType.HasFlag(EncryptionType.Base64);
            set
            {
                if (value == UseBase64)
                    return;
                
                if (value)
                    EncrType ^= EncryptionType.Base64;
                else
                {
                    EncrType &= ~EncryptionType.Base64;
                }
                
                OnPropertyChanged();
            }
        }

        public bool UsePassword
        {
            get => EncrType.HasFlag(EncryptionType.Password);
            set
            {
                if (value == UsePassword)
                    return;
                
                if (value)
                    EncrType ^= EncryptionType.Password;
                else
                {
                    EncrType &= ~EncryptionType.Password;
                }
                
                OnPropertyChanged();
            }
        }
        
        public bool UseForMachine
        {
            get => EncrType.HasFlag(EncryptionType.ForMachine);
            set
            {
                if (value == UseForMachine)
                    return;
                
                if (value)
                    EncrType ^= EncryptionType.ForMachine;
                else
                {
                    EncrType &= ~EncryptionType.ForMachine;
                }
                
                OnPropertyChanged();
            }
        }

        #endregion

        #region Command

        public bool IsLoadFromFile { get => _loadFromFile; set => SetProperty(ref _loadFromFile, value); }

        public ICommand SelectFilePathCommand { get; set; }
        public ICommand ActionCommand { get; set; }

        #endregion

        #region Command handlers

        private void OnActionCommand()
        {
            if (IsLoadFromFile)
                 OnLoadCommand();
            else
                OnSaveCommand();
        }

        private bool OnCanActionCommand()
        {
            return IsLoadFromFile
                ? File.Exists(FilePath)
                : !string.IsNullOrWhiteSpace(FilePath);

        }

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

        public LoadingViewModel()
             :this(true)
        {
            
        }
        
        public LoadingViewModel(bool isLoadFromFile)
        {
            IsLoadFromFile = isLoadFromFile;
            
            SelectFilePathCommand = new DelegateCommand(OnOpenFile);
            ActionCommand = new DelegateCommand(OnActionCommand, OnCanActionCommand);
        }
    }

    [Flags]
    public enum EncryptionType
    {
        Base64 = 1,
        Password = 2,
        ForMachine = 4,
    }
}
