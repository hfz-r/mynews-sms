namespace StockManagementSystem.Services.Security
{
    public interface IEncryptionService
    {
        string CreateHash(byte[] data, string hashAlgorithm);
        string CreatePasswordHash(string password, string saltkey, string passwordFormat);
        string CreateSaltKey(int size);
        string DecryptText(string cipherText, string encryptionPrivateKey = "");
        string EncryptText(string plainText, string encryptionPrivateKey = "");
    }
}