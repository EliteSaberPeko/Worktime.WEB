using System.Security.Cryptography;

namespace Worktime.WEB
{
    public class AES
    {
        public static byte[] Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes) => Execute(bytesToBeEncrypted, passwordBytes, isEncrypt: true);

        public static byte[] Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes) => Execute(bytesToBeDecrypted, passwordBytes, isEncrypt: false);

        private static byte[] Execute(byte[] bytesToBeEncrypted, byte[] passwordBytes, bool isEncrypt)
        {
            byte[] encryptedBytes;
            byte[] saltBytes = new byte[] { 9, 3, 8, 3, 1, 4, 7, 2 };
            using (MemoryStream ms = new())
            {
                using (Aes AES = Aes.Create())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);
                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms,
                        isEncrypt ? AES.CreateEncryptor() : AES.CreateDecryptor(),
                        CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }
            return encryptedBytes;
        }
    }
}
