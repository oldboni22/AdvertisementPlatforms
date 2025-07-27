using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Domain;
using Microsoft.Extensions.Configuration;
using Shared;

namespace ConsoleProccessor;

class Program
{
    private const string JsonMediaType = "application/json";

    private static string GetAesKey()
    {
        var config = new ConfigurationBuilder().
            AddJsonFile("aes.json").
            Build();

        var key =  config.GetSection("Aes").Value!;
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException();

        return key;
    }
    
    private static (string senderAdress, string feedAdress) GetAddresses()
    {
        var config = new ConfigurationBuilder().
            AddJsonFile("addresses.json").
            Build();
        
        string sender = config.GetSection("Sender").Value!;
        string feed = config.GetSection("Feed").Value!;

        if (string.IsNullOrWhiteSpace(sender) || string.IsNullOrWhiteSpace(feed))
            throw new ArgumentException();

        return (sender,feed);
    }

    
    
    static async Task Main(string[] args)
    {
        HttpClient client = new HttpClient();
        
        var addresses = GetAddresses();
        var aes = new AesService.AesService(GetAesKey());
        
        #region LocalFunctions

            async Task WriteDataAsync(List<Platform> platforms)
            {
                var serialized =  JsonSerializer.Serialize(platforms);
                serialized = JsonSerializer.Serialize(serialized);
                
                var stringContent = new StringContent(serialized, Encoding.UTF8, JsonMediaType);

                var fullAddress = addresses.senderAdress + "/sender";
                var result = await client.PostAsync(fullAddress, stringContent);

                if (result.IsSuccessStatusCode)
                {
                    Console.WriteLine("Data was sent successfully.");
                }
                else
                {
                    var error = await result.Content.ReadAsStringAsync();
                    Console.WriteLine($"An error occured while sending data - {result.StatusCode}, {error}");
                }
            }

            async Task<List<string>?> GetPlatformsAsync(string query)
            {
                var encodedQuery = Uri.EscapeDataString(query);
                var fullAddress = addresses.feedAdress + "/feed?query=" + $"{encodedQuery}";

                var result = await client.GetAsync(fullAddress);

                if (result.IsSuccessStatusCode)
                {
                    Console.WriteLine("Data was fetched successfully.");
                }
                else
                {
                    var error = await result.Content.ReadAsStringAsync();
                    Console.WriteLine($"An error occured while fetching data - {result.StatusCode}, {error}");
                    
                    return null;
                }

                var responseBody = await result.Content.ReadFromJsonAsync<GetResultBody>();
                var decrypted = await aes.DecryptStringAsync(responseBody.EncryptedData, responseBody.Iv);

                var list = JsonSerializer.Deserialize<List<string>>(decrypted);

                return list;
            }
            
            async Task TestFetch(string query)
            {
                Console.WriteLine("Sending test query = " + query);
                var test = await GetPlatformsAsync(query);
        
                foreach (var str in test)
                {
                    Console.WriteLine(str);
                }
                Console.WriteLine("-----------------------------------------------");
            }

        #endregion

        var list1 = new List<Platform>
        {
            new Platform("/ru","Тест1"),
            new Platform("/ru/1","Тест2"),
            new Platform("/ru/1/2","Тест2"),
            new Platform("/ru/1/2/3","Тест3"),
            new Platform("/ru/1/2/3/4","Тест4"),
        };

        await WriteDataAsync(list1);

        await TestFetch("/ru");
        await Task.Delay(1500);
        
        await TestFetch("/ru/1");
        await Task.Delay(1500);
        
        await TestFetch("/ru/1/2");
        await Task.Delay(1500);
        
        await TestFetch("/ru/1/2/3");
        await Task.Delay(1500);
        
        await TestFetch("/ru/1/2/3/4");
        await Task.Delay(1500);
    }
}