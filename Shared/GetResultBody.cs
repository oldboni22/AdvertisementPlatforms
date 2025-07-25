namespace Shared;

[System.Serializable]
public record struct GetResultBody
    (
        string EncryptedData, 
        string Iv
    );