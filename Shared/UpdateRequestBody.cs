namespace Shared;

[System.Serializable]
public record struct UpdateRequestBody
    (
        string EncryptedData, 
        string Iv
    );