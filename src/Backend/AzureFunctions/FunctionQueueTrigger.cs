using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzureFunctions;

public class FunctionQueueTrigger
{
    private readonly ILogger<FunctionQueueTrigger> _logger;

    public FunctionQueueTrigger(ILogger<FunctionQueueTrigger> logger)
    {
        _logger = logger;
    }

    [Function(nameof(FunctionQueueTrigger))]
    public void Run([QueueTrigger("image-processing-queue", Connection = "QueueTriggerConnection")] QueueMessage message)
    {
        _logger.LogInformation("C# Queue trigger function processed: {messageText}", message.MessageText);
    }
}