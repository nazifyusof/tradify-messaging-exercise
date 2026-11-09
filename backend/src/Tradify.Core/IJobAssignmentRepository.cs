namespace Tradify.Core;

public interface IJobAssignmentRepository
{
    bool IsAssigned(string userId, string jobId);

    IReadOnlyList<string> ListJobIdsForUser(string userId);

    IReadOnlyList<string> ListUserIdsForJob(string jobId);
}
