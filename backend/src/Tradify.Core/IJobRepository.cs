namespace Tradify.Core;

public interface IJobRepository
{
    Job? Get(string jobId);
}
