using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
namespace Domain.Infrastructure.Messaging;

public class MessageBus : IMessageBus
{
    private readonly IServiceProvider _serviceProvider;
    private static List<Type> _consumers = new List<Type>();

    public MessageBus(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    internal static void AddConsumers(Assembly assembly, IServiceCollection services)
    {
        var types = assembly.GetTypes()
                           .Where(x => x.GetInterfaces().Any(y => y.IsGenericType && y.GetGenericTypeDefinition() == typeof(IMessageBusConsumer<,>)))
                           .ToList();

        foreach (var type in types)
        {
            services.AddTransient(type);
        }

        _consumers.AddRange(types);
    }

    public async Task SendAsync<T>(T message, MetaData metaData, CancellationToken cancellationToken = default) where T : IMessageBusMessage
    {
        await _serviceProvider.GetRequiredService<IMessageSender<T>>().SendAsync(message, metaData, cancellationToken);
    }

    public async Task ReceiveAsync<TConsumer, T>(Func<T, MetaData, CancellationToken, Task> action, CancellationToken cancellationToken = default) where T : IMessageBusMessage
    {
        await _serviceProvider.GetRequiredService<IMessageReceiver<TConsumer, T>>().ReceiveAsync(action, cancellationToken);
    }

    public async Task ReceiveAsync<TConsumer, T>(CancellationToken cancellationToken = default) where T : IMessageBusMessage
    {
        await _serviceProvider.GetRequiredService<IMessageReceiver<TConsumer, T>>().ReceiveAsync(async(data, metaData, cancellationToken) =>
        {
            using var scope = _serviceProvider.CreateScope();

            foreach (Type handlerType in _consumers)
            {
                bool hasHandlerEvent = handlerType.GetInterfaces()
                    .Any(x => x.IsGenericType 
                        && x.GetGenericTypeDefinition() == typeof(IMessageBusConsumer<,>) 
                        && x.GetGenericArguments()[1] == typeof(T));

                if(hasHandlerEvent)
                {
                    dynamic handler = scope.ServiceProvider.GetService(handlerType);
                    await handler.HandleAsync((dynamic)data, metaData, cancellationToken);
                }
            }

        }, cancellationToken);
    }
}

public static class MessageBusExtensions
{
    public static void AddMessageBusConsumers(this IServiceCollection services, Assembly assembly)
    {
        MessageBus.AddConsumers(assembly, services);
    }

    public static void AddMessageBus(this IServiceCollection services, Assembly assembly)
    {
        services.AddTransient<IMessageBus, MessageBus>();
        services.AddMessageBusConsumers(assembly);
    }
}
