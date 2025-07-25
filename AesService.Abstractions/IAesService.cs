namespace AesService.Abstractions;

public interface IAesService
{
    Task<(string encrypted,string iv)> EncryptStringAsync(string input);
    Task<string> DecryptStringAsync(string input, string iv);
}