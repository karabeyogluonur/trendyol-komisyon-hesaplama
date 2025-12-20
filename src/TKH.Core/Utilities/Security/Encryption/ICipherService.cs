namespace TKH.Core.Utilities.Security.Encryption
{
    public interface ICipherService
    {
        string Encrypt(string plainText);
        string Decrypt(string cipherText);
    }
}
