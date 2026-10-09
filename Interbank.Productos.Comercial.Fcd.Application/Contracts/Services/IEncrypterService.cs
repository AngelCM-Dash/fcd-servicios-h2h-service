namespace Interbank.Productos.Comercial.Fcd.Application.Contracts.Services
{
    public interface IEncrypterService
    {
        string Encrypt(string plainText);
        string Decrypt(string cipherText);

        string EncryptKey(string plainText, string Key);
        string DecryptKey(string cipherText, string Key);
    }
}
