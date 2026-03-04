using Domain.Entities;
using Domain.Infrastructure.Messaging;

namespace Application.FileEntries.MessageBusEvents;

public class FileDeletedEvent : IMessageBusEvent
{
    public FileEntry FileEntry { get; set; }
}
