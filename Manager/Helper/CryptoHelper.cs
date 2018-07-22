using System;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace Manager.Helper
{
    public static class CryptoHelper
    {
        /// <summary>
        ///     Шифруем пароль
        /// </summary>
        /// <typeparam name="T">
        ///     Любое симметричное шифрование,
        ///     я предпочитаю <see cref="AesManaged" />>
        /// </typeparam>
        /// <param name="value">Шифруемые данные</param>
        /// <param name="password">Ключ шифрования и зерно</param>
        /// <returns></returns>
        public static string Encrypt<T>(string value, string password)
            where T : SymmetricAlgorithm, new()
        {
            DeriveBytes rgb = new Rfc2898DeriveBytes(password, Encoding.Unicode.GetBytes(password));

            SymmetricAlgorithm algorithm = new T();

            var rgbKey = rgb.GetBytes(algorithm.KeySize >> 3);
            var rgbIV = rgb.GetBytes(algorithm.BlockSize >> 3);

            var transform = algorithm.CreateEncryptor(rgbKey, rgbIV);

            using (var buffer = new MemoryStream())
            {
                using (var stream = new CryptoStream(buffer, transform, CryptoStreamMode.Write))
                {
                    using (var writer = new StreamWriter(stream, Encoding.Unicode))
                    {
                        writer.Write(value);
                    }
                }

                return Convert.ToBase64String(buffer.ToArray());
            }
        }

        /// <summary>
        ///     Дешифруем пароль
        /// </summary>
        /// <typeparam name="T">
        ///     Любое симметричное шифрование,
        ///     я предпочитаю <see cref="AesManaged" />>
        /// </typeparam>
        /// <param name="text">Шифруемые данные</param>
        /// <param name="password">Ключ шифрования, и, соответственно, зерно</param>
        /// <returns></returns>
        public static string Decrypt<T>(string text, string password)
            where T : SymmetricAlgorithm, new()
        {
            DeriveBytes rgb = new Rfc2898DeriveBytes(password, Encoding.Unicode.GetBytes(password));

            SymmetricAlgorithm algorithm = new T();

            var rgbKey = rgb.GetBytes(algorithm.KeySize >> 3);
            var rgbIV = rgb.GetBytes(algorithm.BlockSize >> 3);

            var transform = algorithm.CreateDecryptor(rgbKey, rgbIV);

            using (var buffer = new MemoryStream(Convert.FromBase64String(text)))
            {
                using (var stream = new CryptoStream(buffer, transform, CryptoStreamMode.Read))
                {
                    using (var reader = new StreamReader(stream, Encoding.Unicode))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }

        //Return a hardware identifier
        public static string UniqueHardwareId()
        {
            // смотрим на эти поля
            var fields = new[] {"UniqueId", "ProcessorId", "SerialNumber", "Name", "Manufacturer", "MaxClockSpeed"};
            // смотрим на процессор
            var management = new ManagementClass("Win32_Processor");
            var collection = management.GetInstances();

            foreach (var managementObj in collection)
            {
                var proprties = managementObj.Properties.OfType<PropertyData>().ToList();

                foreach (var field in fields)
                {
                    var find = proprties.FirstOrDefault(x => string.Equals(x.Name, field));
                    if (!string.IsNullOrWhiteSpace(find?.Value?.ToString()))
                        return find.Value.ToString();
                }
            }

            return string.Empty;
        }
        
        public static String SecureStringToString(this SecureString value)
        {
            var bstr = Marshal.SecureStringToBSTR(value);

            try
            {
                return Marshal.PtrToStringBSTR(bstr);
            }
            finally
            {
                Marshal.FreeBSTR(bstr);
            }
        }
    }
}
