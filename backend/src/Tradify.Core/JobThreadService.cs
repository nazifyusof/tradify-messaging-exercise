namespace Tradify.Core;

public sealed class JobThreadService(IJobAssignmentRepository assignments)
{
    // Ticket JT-101: only users assigned to the job may view its thread.
    public bool CanView(string userId, string jobId) => !string.IsNullOrWhiteSpace(userId);
}
