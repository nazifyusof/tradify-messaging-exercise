namespace Tradify.Core;

public interface IMessageRepository
{
    void Add(Message message);

    IReadOnlyList<Message> ListForJob(string jobId);
}
