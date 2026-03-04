using Domain.Infrastructure.Messaging;
using Infrastructure.Messaging.AzureQueueStorage;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Messaging;

public static class MessagingCollectionExtensions
{
    public static IServiceCollection AddMessageBusSender<T>(this IServiceCollection services, MessagingOptions options)
    {
        if (options.UseAzureQueue())
        {
            services.AddAzureQueueSender<T>(options.AzureQueue);
        }

        return services;
    }

    public static IServiceCollection AddAzureQueueSender<T>(this IServiceCollection services, AzureQueueOptions options)
    {
        var queueOptions = new AzureQueueStorageOptions
        {
            ConnectionString = options.ConnectionString,
            QueueName = options.QueueNames[typeof(T).Name],
            QueueClientOptions = options.QueueClientOptions
        };

        services.AddSingleton<IMessageSender<T>>(new AzureQueueStorageSender<T>(queueOptions));
        return services;
    }
}