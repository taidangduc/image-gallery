using Domain.Infrastructure.Messaging;

namespace Infrastructure.Messaging.Fake;

public class FakeReceiver<TConsumer, T> : IMessageReceiver<TConsumer, T>
{
    public Task ReceiveAsync(Func<T, MetaData, CancellationToken, Task> action, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
