    using System.Security.Cryptography;
    using AesService.Abstractions;

    namespace AesService;

    public class AesService(string stringKey) : IAesService
    {
        private readonly byte[] _key = Convert.FromBase64String(stringKey);
        
        public async Task<(string encrypted,string iv)> EncryptStringAsync(string input)
        {
            var bytes = RandomNumberGenerator.GetBytes(16);
            var iv = bytes;

            byte[] encrypted;
            
            using (var myAes = Aes.Create())
            {
                myAes.Key = _key;
                myAes.IV = bytes;

                var encryptor = myAes.CreateEncryptor();

                using (var memStream = new MemoryStream())
                {
                    await using (var cryptoStream = new CryptoStream(memStream,encryptor,CryptoStreamMode.Write))
                    {
                        await using (var writer = new StreamWriter(cryptoStream))
                        {
                            await writer.WriteAsync(input);
                        }
                    }

                    encrypted = memStream.ToArray();
                }
            }

            return (Convert.ToBase64String(encrypted), Convert.ToBase64String(iv));
        }

        public async Task<string> DecryptStringAsync(string input, string iv)
        {
            var bytes = Convert.FromBase64String(input);
            
            using (var myAes = Aes.Create())
            {
                myAes.Key = _key;
                myAes.IV = Convert.FromBase64String(iv);

                var decryptor = myAes.CreateDecryptor();

                using (var memStream = new MemoryStream(bytes))
                {
                    await using (var cryptoStream = new CryptoStream(memStream,decryptor,CryptoStreamMode.Read))
                    {
                        using (var reader = new StreamReader(cryptoStream))
                        {
                            return await reader.ReadToEndAsync();
                        }
                    }
                }
                
            }
        }
    }