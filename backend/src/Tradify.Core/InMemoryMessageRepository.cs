using System.Collections.Concurrent;

namespace Tradify.Core;

public sealed class InMemoryMessageRepository : IMessageRepository
{
    private readonly ConcurrentDictionary<Guid, Message> _messages = new();

    public void Add(Message message) => _messages[message.Id] = message;

    public IReadOnlyList<Message> ListForJob(string jobId) =>
        _messages.Values
            .Where(m => m.JobId == jobId)
            .OrderBy(m => m.CreatedAt)
            .ToList();
}
