using Infrastructure.Messaging.AzureQueueStorage;

namespace Infrastructure.Messaging;

public class MessagingOptions
{
    public string Provider {get; set; }
    public AzureQueueOptions AzureQueue { get; set; }

    public bool UseAzureQueue()
    {
        return Provider == "AzureQueue";
    }

    public bool UseFake()
    { 
        return Provider == "Fake";
    }
}