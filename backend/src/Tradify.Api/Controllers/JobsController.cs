using Microsoft.AspNetCore.Mvc;
using Tradify.Core;

namespace Tradify.Api.Controllers;

public sealed record Participant(string Id, string Name);

public sealed record JobSummary(
    string Id,
    string Title,
    IReadOnlyList<Participant> Participants,
    Message? LastMessage);

[ApiController]
[Route("jobs")]
public sealed class JobsController(
    IJobRepository jobs,
    IUserRepository users,
    IJobAssignmentRepository assignments,
    IMessageRepository messages) : ControllerBase
{
    [HttpGet]
    public IActionResult List()
    {
        var userId = AuthStub.GetUserId(Request);
        if (userId is null || users.Get(userId) is null)
        {
            return Unauthorized();
        }

        var summaries = assignments.ListJobIdsForUser(userId)
            .Select(jobs.Get)
            .OfType<Job>()
            .Select(job => new JobSummary(
                job.Id,
                job.Title,
                assignments.ListUserIdsForJob(job.Id)
                    .Select(id => new Participant(id, users.Get(id)?.Name ?? id))
                    .OrderBy(p => p.Name)
                    .ToList(),
                messages.ListForJob(job.Id).LastOrDefault()))
            .OrderByDescending(s => s.LastMessage?.CreatedAt ?? DateTimeOffset.MinValue)
            .ThenBy(s => s.Title)
            .ToList();

        return Ok(summaries);
    }
}
