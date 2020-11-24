using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Curse.Data
{
    public class DataScrambler
    {
        //public (string Key, string IVBase64) InitSymmetricEncryptionKeyIV()
        //{
        //    var key = GetEncodedRandomString(32); // 256
        //    Aes cipher = CreateCipher(key);
        //    var IVBase64 = Convert.ToBase64String(cipher.IV);
        //    return (key, IVBase64);
        //}

        //private string GetEncodedRandomString(int length)
        //{
        //    var base64 = Convert.ToBase64String(GenerateRandomBytes(length));
        //    return base64;
        //}

        //private Aes CreateCipher(string keyBase64)
        //{
        //    // Default values: Keysize 256, Padding PKC27
        //    Aes cipher = Aes.Create();
        //    cipher.Mode = CipherMode.CBC;  // Ensure the integrity of the ciphertext if using CBC

        //    cipher.Padding = PaddingMode.ISO10126;
        //    cipher.Key = Convert.FromBase64String(keyBase64);

        //    return cipher;
        //}

        //private byte[] GenerateRandomBytes(int length)
        //{
        //    var byteArray = new byte[length];
        //    RandomNumberGenerator.Fill(byteArray);
        //    return byteArray;
        //}
        //public string Encrypt(string text, string IV, string key)
        //{
        //    Aes cipher = CreateCipher(key);
        //    cipher.IV = Convert.FromBase64String(IV);

        //    ICryptoTransform cryptTransform = cipher.CreateEncryptor();
        //    byte[] plaintext = Encoding.UTF8.GetBytes(text);
        //    byte[] cipherText = cryptTransform.TransformFinalBlock(plaintext, 0, plaintext.Length);

        //    return Convert.ToBase64String(cipherText);
        //}

        //public string Decrypt(string encryptedText, string IV, string key)
        //{
        //    Aes cipher = CreateCipher(key);
        //    cipher.IV = Convert.FromBase64String(IV);

        //    ICryptoTransform cryptTransform = cipher.CreateDecryptor();
        //    byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
        //    byte[] plainBytes = cryptTransform.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

        //    return Encoding.UTF8.GetString(plainBytes);
        //}
        public byte[] Encrypt(string plainText, byte[] Key, byte[] IV)
        {
            byte[] encrypted;
            // Create a new AesManaged.    
            using (AesManaged aes = new AesManaged())
            {
                // Create encryptor    
                ICryptoTransform encryptor = aes.CreateEncryptor(Key, IV);
                // Create MemoryStream    
                using (MemoryStream ms = new MemoryStream())
                {
                    // Create crypto stream using the CryptoStream class. This class is the key to encryption    
                    // and encrypts and decrypts data from any given stream. In this case, we will pass a memory stream    
                    // to encrypt    
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        // Create StreamWriter and write data to a stream    
                        using (StreamWriter sw = new StreamWriter(cs))
                            sw.Write(plainText);
                        encrypted = ms.ToArray();
                    }
                }
            }
            // Return encrypted data    
            return encrypted;
        }
        public string Decrypt(byte[] cipherText, byte[] Key, byte[] IV)
        {
            string plaintext = null;
            // Create AesManaged    
            using (AesManaged aes = new AesManaged())
            {
                // Create a decryptor    
                ICryptoTransform decryptor = aes.CreateDecryptor(Key, IV);
                // Create the streams used for decryption.    
                using (MemoryStream ms = new MemoryStream(cipherText))
                {
                    // Create crypto stream    
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        // Read crypto stream    
                        using (StreamReader reader = new StreamReader(cs))
                            plaintext = reader.ReadToEnd();
                    }
                }
            }
            return plaintext;
        }
    }
}
