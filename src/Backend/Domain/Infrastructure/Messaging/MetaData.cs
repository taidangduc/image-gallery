namespace Domain.Infrastructure.Messaging;

public class MetaData
{
    public string MessageId { get; set; }
    public string Version { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? EnqueuedAt { get; set; }
}
