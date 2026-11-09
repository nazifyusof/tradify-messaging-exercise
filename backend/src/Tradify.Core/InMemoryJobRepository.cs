namespace Tradify.Core;

public sealed class InMemoryJobRepository : IJobRepository
{
    private readonly Dictionary<string, Job> _jobs;

    public InMemoryJobRepository(IEnumerable<Job> jobs)
    {
        _jobs = jobs.ToDictionary(j => j.Id);
    }

    public Job? Get(string jobId) => _jobs.GetValueOrDefault(jobId);
}
