using Infrastructure.Storage.Azure;

namespace Infrastructure.Storage;

public class StorageOptions
{
    public string Provider { get; set; }
    public string TempFolderPath { get; set; }
    public AzureBlobOption Azure { get; set; }

    public bool UseAzure()
    {
        return Provider == "Azure";
    }

    public bool UseFake()
    {
        return Provider == "Fake";
    }
}