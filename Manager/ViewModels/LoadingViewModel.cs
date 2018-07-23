using Manager.Model;
using Microsoft.Win32;
using Mvvm;
using Mvvm.Commands;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Security;
using System.Windows.Input;
using System.Security.Cryptography;
using System.Text;
using Manager.Behaviors;
using Manager.Models;

namespace Manager.ViewModels
{
    public class LoadingViewModel : BindableBase
    {
        #region Fields

        private string _filePath;
        private bool _loadFromFile;
        private SecureString _password;
        private bool _useEncr;

        private CryptoModel _model = new CryptoModel();
        private EncryptionType _encryptionType;

        #endregion

        #region Properties

        public SecureString Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string FilePath
        {
            get => _filePath;
            set => SetProperty(ref _filePath, value);
        }

        public EncryptionType EncryptionType
        {
            get => _encryptionType;
            set => SetProperty(ref _encryptionType, value);
        }


        public bool UseEncr
        {
            get => _useEncr;
            set => SetProperty(ref _useEncr, value);
        }

        public bool IsLoadFromFile
        {
            get => _loadFromFile;
            set => SetProperty(ref _loadFromFile, value);
        }

        public bool HasError { get; set; }

        #endregion

        #region Command


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
            if (EncryptionType == EncryptionType.Password 
                && (Password == null || Password.Length < 4))
            {
                return false;
            }
            
            return IsLoadFromFile
                ? OnCanLoadCommand()
                : OnCanSave();
        }


        private bool OnCanLoadCommand()
        {
            return File.Exists(FilePath);
        }

        private bool OnCanSave()
        {
            return !string.IsNullOrWhiteSpace(FilePath);
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
            try
            {
                if (File.Exists(FilePath))
                {
                    var json = File.ReadAllText(FilePath);

                    EncryptionType? type = EncryptionType;

                    if (!UseEncr)
                        type = null;

                    json = _model.Decrypt(type, json, Password);

                    var pupils = JsonConvert.DeserializeObject<List<Pupil>>(json);
                    Store.Store.Instance.Load(pupils);
                }
            }
            catch
            {
                HasError = true;
            }
        }

        private void OnSaveCommand()
        {
            try
            {
                var json = JsonConvert.SerializeObject(Store.Store.Instance.FindAll());

                EncryptionType? type = EncryptionType;

                if (!UseEncr)
                    type = null;

                json = _model.Encrypt(type, json, Password);

                File.WriteAllText(FilePath, json);
            }
            catch
            {
                HasError = true;
            }

        }

        #endregion

        public LoadingViewModel()
            : this(true)
        {

        }

        public LoadingViewModel(bool isLoadFromFile)
        {
            IsLoadFromFile = isLoadFromFile;

            SelectFilePathCommand = new DelegateCommand(OnOpenFile);
            ActionCommand = new DelegateCommand(OnActionCommand, OnCanActionCommand);
        }
    }

    public enum EncryptionType
    {
        Base64 = 1,
        Password = 2,
        ForMachine = 4,
    }
}
