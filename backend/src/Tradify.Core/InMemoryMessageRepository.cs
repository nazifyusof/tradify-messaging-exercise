namespace Tradify.Core;

public sealed class InMemoryMessageRepository : IMessageRepository
{
    private readonly List<Message> _messages = [];

    public void Add(Message message)
    {
        lock (_messages)
        {
            var index = _messages.FindIndex(m => m.Id == message.Id);
            if (index >= 0) _messages[index] = message;
            else _messages.Add(message);
        }
    }

    public IReadOnlyList<Message> ListForJob(string jobId)
    {
        lock (_messages)
        {
            return _messages
                .Where(m => m.JobId == jobId)
                .OrderBy(m => m.CreatedAt)
                .ToList();
        }
    }
}
