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
        
        private RijndaelManaged _crypter = new RijndaelManaged();

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

        private EncryptionType EncrType { get; set; }

        public bool NoEncr
        {
            get => _noEncr;
            set => SetProperty(ref _noEncr, value);
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

        public bool IsLoadFromFile
        {
            get => _loadFromFile;
            set => SetProperty(ref _loadFromFile, value);
        }


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
            return IsLoadFromFile
                ? OnCanLoadCommand()
                : OnCanSave();
        }

        private bool OnCanLoadCommand()
        {
            if (File.Exists(FilePath))
                return false;

            if (UsePassword && Password.Length < 4)
                return false;

            return true;
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
            var json = string.Empty;

            if (File.Exists(FilePath))
            {
                json = File.ReadAllText(FilePath);

                if (!NoEncr)
                {
                    json = TryEncrypt(json);
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

            if (!NoEncr)
            {
                json = TryCrypt(json);
            }


            File.WriteAllText(FilePath, json);
        }

        #endregion

        #region Helping methods

        /// <summary>
        /// Пытаемся зашифровать
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string TryEncrypt(string text)
        {
            if (EncrType.HasFlag(EncryptionType.Base64))
            {
                text = ToBase64(text);
            }
            
            if (EncrType.HasFlag(EncryptionType.Password))
            {
                text = WithPass(PasswordBindBehavior.SecureStringToString(Password));
            }
            
            if (EncrType.HasFlag(EncryptionType.ForMachine))
            {
                text = WithPass(GetUniqueKey());
                
            }

            return text;
        }

        /// <summary>
        /// Пытаемся расшифровать. Порядок обратный шифровке!!!
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string TryCrypt(string text)
        {
            if (EncrType.HasFlag(EncryptionType.ForMachine))
            {
                text = FromPass(GetUniqueKey());
            }

            if (EncrType.HasFlag(EncryptionType.Password))
            {
                text = FromPass(PasswordBindBehavior.SecureStringToString(Password));
            }

            if (EncrType.HasFlag(EncryptionType.Base64))
            {
                text = FromBase64(text);
            }

            return text;
        }

        // заполняем остальное единицами
        private byte[] GetValidKey()
        {
            var bytes = new byte[_crypter.KeySize / 8];
            
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = byte.MaxValue;
            }

            var password = Encoding.Unicode.GetBytes(PasswordBindBehavior.SecureStringToString(Password));
            for (int i = 0; i < password.Length && i < 16; i++)
            {
                bytes[i] |= password[i];
            }

            return bytes;
        }

        public string GetUniqueKey()
        {
            var names = new[]
            {
                "UniqueId",
                "ProcessorId",
                "SerialNumber",
            };

            
            var mc = new ManagementClass("Win32_processor");
            var moc = mc.GetInstances();

            foreach (var mo in moc)
            {
                var properties = mo.Properties.OfType<PropertyData>().ToList();
                
                foreach (var propName in names)
                {
                    var temp = properties.FirstOrDefault(x => string.Equals(x.Name, propName));

                    if (!string.IsNullOrEmpty(temp?.Value?.ToString()))
                        return temp.Value.ToString();
                }
            }

            return string.Empty;
        }

        private string ToBase64(string text)
        {
            return Convert.ToBase64String(Encoding.Unicode.GetBytes(text));
        }

        private string FromBase64(string text)
        {
            return Encoding.Unicode.GetString(Convert.FromBase64String(text));
        }

        private string FromPass(string text)
        {
            var bytes = Encoding.Unicode.GetBytes(text);
            var copy = new byte[bytes.Length];
            
            using (var memoryStream = new MemoryStream(bytes))
            {
                // получил нужный размер ключа
                _crypter.Key = GetValidKey();
                
                // создал дешифратор
                using (var decrypter = _crypter.CreateDecryptor())
                {
                    // создал поток дешифратора
                    using (var cryptoStream = new CryptoStream(memoryStream, decrypter, CryptoStreamMode.Read))
                    {
                        
                        cryptoStream.Read(copy, 0, copy.Length);
                    }
                }
            }

            return Encoding.Unicode.GetString(copy);
        }

        private string WithPass(string text)
        {
            var bytes = Encoding.Unicode.GetBytes(text);
            
            using (var memoryStream = new MemoryStream())
            {
                // получил нужный размер ключа
                _crypter.Key = GetValidKey();
                
                // создал дешифратор
                using (var encrypter = _crypter.CreateEncryptor())
                {
                    // создал поток дешифратора
                    using (var cryptoStream = new CryptoStream(memoryStream, encrypter, CryptoStreamMode.Write))
                    {
                        
                        cryptoStream.Write(bytes, 0, bytes.Length);
                    }
                }

                return Encoding.Unicode.GetString(memoryStream.GetBuffer());
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

    [Flags]
    public enum EncryptionType
    {
        Base64 = 1,
        Password = 2,
        ForMachine = 4,
    }
}
