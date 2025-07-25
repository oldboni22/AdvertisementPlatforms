namespace AesService.Abstractions;

public interface IAesService
{
    string EncryptString(string input, out byte[] iv);
    string DecryptString(string input, byte[] iv);
}