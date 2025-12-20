using Microsoft.AspNetCore.DataProtection;

namespace TKH.Core.Utilities.Security.Encryption
{
    public class CipherService : ICipherService
    {
        private readonly IDataProtector _protector;

        public CipherService(IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector("TKH.Marketplace.Security.V1");
        }

        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return plainText;
            return _protector.Protect(plainText);
        }

        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText)) return cipherText;
            return _protector.Unprotect(cipherText);
        }
    }
}
