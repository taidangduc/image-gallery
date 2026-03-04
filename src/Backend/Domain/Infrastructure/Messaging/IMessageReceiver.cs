namespace Domain.Infrastructure.Messaging;

public interface IMessageReceiver<TConsumer, T>
{
    public Task ReceiveAsync(Func<T, MetaData, CancellationToken, Task> action, CancellationToken cancellationToken);
}
