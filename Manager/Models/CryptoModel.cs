using System;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using Manager.Helper;
using Manager.ViewModels;

namespace Manager.Models
{
    public class CryptoModel
    {
        private readonly Encoding _baseEncoding = Encoding.Unicode;
        
        public string Encrypt(EncryptionType? type, string text, SecureString password)
        {
            switch (type)
            {
                    case EncryptionType.Base64:
                        return Convert.ToBase64String(_baseEncoding.GetBytes(text));
                    
                    case EncryptionType.Password:
                        return CryptoHelper.Encrypt<RijndaelManaged>(text, password.SecureStringToString());
                    
                    case EncryptionType.ForMachine:
                        return CryptoHelper.Encrypt<RijndaelManaged>(text, CryptoHelper.UniqueHardwareId());
                    
                    default:
                        return text;
            }
        }

        public string Decrypt(EncryptionType? type, string text, SecureString password)
        {
            switch (type)
            {
                case EncryptionType.Base64:
                    return _baseEncoding.GetString(Convert.FromBase64String(text));
                    
                case EncryptionType.Password:
                    return CryptoHelper.Decrypt<RijndaelManaged>(text, password.SecureStringToString());
                    
                case EncryptionType.ForMachine:
                    return CryptoHelper.Decrypt<RijndaelManaged>(text, CryptoHelper.UniqueHardwareId());
                    
                default:
                    return text;
            }
        }
    }
}