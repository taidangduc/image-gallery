using Azure.Storage.Queues;

namespace Infrastructure.Messaging.AzureQueueStorage;

public class AzureQueueOptions
{
    public string ConnectionString { get; set; }
    public string AccountName { get; set; }
    public Dictionary<string, string> QueueNames { get; set; }
    public QueueClientOptions QueueClientOptions { get; set; }
}

public class AzureQueueStorageOptions
{
    public string ConnectionString { get; set; }
    public string QueueName { get; set; }
    public QueueClientOptions QueueClientOptions { get; set; }

    public QueueClient CreateQueueClient()
    {
        var options = new QueueClientOptions();

        return options == null ?
            new QueueClient(ConnectionString, QueueName) :
            new QueueClient(ConnectionString, QueueName, options);
    }

   public QueueClientOptions GetQueueClientOptions()
   {
       if (QueueClientOptions is null)
       {
            return null;
       }

       return new QueueClientOptions
       {
            MessageEncoding = QueueClientOptions.MessageEncoding
       };
    }
}
