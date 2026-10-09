using Interbank.Productos.Comercial.Fcd.Application.Contracts.Services;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace Interbank.Productos.Comercial.Fcd.Infrastructure.Services.Encrypter
{
    public class EncrypterService : IEncrypterService
    {
        private readonly IConfiguration _configuration;

        public EncrypterService(IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);

            _configuration = configuration;
        }

        public string Encrypt(string plainText)
        {
            string key = _configuration["EncryptionKey"] ?? "";
            return EncryptInternal(plainText, key);
        }

        public string EncryptKey(string plainText, string Key)
        {
            return EncryptInternal(plainText, Key);
        }

        public string Decrypt(string cipherText)
        {
            string key = _configuration["EncryptionKey"] ?? "";
            return DecryptInternal(cipherText, key);
        }

        public string DecryptKey(string cipherText, string Key)
        {
            return DecryptInternal(cipherText, Key);
        }

        private static string EncryptInternal(string plainText, string key)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.GenerateIV();
                byte[] iv = aesAlg.IV; // Guardar el IV generado

                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, iv);

                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }

                    // Combinar IV y datos cifrados en una sola matriz de bytes
                    byte[] encryptedBytes = msEncrypt.ToArray();
                    byte[] encryptedDataWithIV = new byte[iv.Length + encryptedBytes.Length];
                    Buffer.BlockCopy(iv, 0, encryptedDataWithIV, 0, iv.Length);
                    Buffer.BlockCopy(encryptedBytes, 0, encryptedDataWithIV, iv.Length, encryptedBytes.Length);

                    return Convert.ToBase64String(encryptedDataWithIV);
                }
            }
        }

        private static string DecryptInternal(string cipherText, string key)
        {
            byte[] cipherBytes = Convert.FromBase64String(cipherText);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);

                // Extraer el IV del inicio de los datos cifrados
                byte[] iv = new byte[aesAlg.BlockSize / 8];
                Array.Copy(cipherBytes, 0, iv, 0, iv.Length);
                aesAlg.IV = iv;

                // Crear el descifrador con la clave y el IV
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Descifrar los datos
                using (var msDecrypt = new MemoryStream(cipherBytes, iv.Length, cipherBytes.Length - iv.Length))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
