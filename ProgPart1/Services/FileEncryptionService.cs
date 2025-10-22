using System.Security.Cryptography;
using System.Text;

namespace LibraryManagementSystem.Services
{
    public class FileEncryptionService
    {
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("MyWayIsTheBestMyWayIsTheBest");
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("MyInitVector16!!"); 


        public async Task EncryptFileAsync( Stream input, string outputPath )
        {
            using ( Aes aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (FileStream fileStream = new FileStream(outputPath, FileMode.Create))
                using (CryptoStream cryptoStream = new CryptoStream(fileStream, encryptor, CryptoStreamMode.Write) )
                {
                    await input.CopyToAsync(cryptoStream);
                }
            }
        }

        public async Task<MemoryStream> DecryptFileAsync(string encryptedFilePath)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key,aes.IV);

                using (FileStream fileStream = new FileStream(encryptedFilePath, FileMode.Open))
                using (CryptoStream cryptoStream = new CryptoStream(fileStream, decryptor, CryptoStreamMode.Read))
                {
                    MemoryStream decryptStream = new MemoryStream();
                    await cryptoStream.CopyToAsync(decryptStream);
                    decryptStream.Position = 0;
                    return decryptStream;
                }
            }
        }
    }
}
