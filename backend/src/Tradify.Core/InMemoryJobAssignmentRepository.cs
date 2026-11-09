namespace Tradify.Core;

public sealed class InMemoryJobAssignmentRepository : IJobAssignmentRepository
{
    private readonly HashSet<(string UserId, string JobId)> _assignments;

    public InMemoryJobAssignmentRepository(IEnumerable<(string UserId, string JobId)> assignments)
    {
        _assignments = new HashSet<(string UserId, string JobId)>(assignments);
    }

    public bool IsAssigned(string userId, string jobId) =>
        _assignments.Contains((userId, jobId));

    public IReadOnlyList<string> ListJobIdsForUser(string userId) =>
       _assignments.Select(a => a.JobId).Distinct().ToList();

    public IReadOnlyList<string> ListUserIdsForJob(string jobId) =>
        _assignments.Where(a => a.JobId == jobId).Select(a => a.UserId).ToList();
}
