using System.Security.Cryptography;
using AesService.Abstractions;

namespace AesService;

public class AesService(string keyPath) : IAesService
{
    private readonly byte[] _key = File.ReadAllBytes(keyPath);
    
    public string EncryptString(string input, out byte[] iv)
    {
        var bytes = RandomNumberGenerator.GetBytes(16);
        iv = bytes;

        byte[] encrypted;
        
        using (var myAes = Aes.Create())
        {
            myAes.Key = _key;
            myAes.IV = bytes;

            var encryptor = myAes.CreateEncryptor();

            using (var memStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memStream,encryptor,CryptoStreamMode.Write))
                {
                    using (var writer = new StreamWriter(cryptoStream))
                    {
                        writer.Write(input);
                    }
                }

                encrypted = memStream.ToArray();
            }
        }

        return Convert.ToBase64String(encrypted);
    }

    public string DecryptString(string input, byte[] iv)
    {
        var bytes = Convert.FromBase64String(input);
        
        using (var myAes = Aes.Create())
        {
            myAes.Key = _key;
            myAes.IV = iv;

            var decryptor = myAes.CreateDecryptor();

            using (var memStream = new MemoryStream(bytes))
            {
                using (var cryptoStream = new CryptoStream(memStream,decryptor,CryptoStreamMode.Read))
                {
                    using (var reader = new StreamReader(cryptoStream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            
        }
    }
}