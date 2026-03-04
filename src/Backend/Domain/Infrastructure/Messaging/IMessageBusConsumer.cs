namespace Domain.Infrastructure.Messaging;

public interface IMessageBusConsumer<TConsumer, T>
{
    public Task HandleAsync(T message, MetaData metaData, CancellationToken cancellationToken = default);
}
