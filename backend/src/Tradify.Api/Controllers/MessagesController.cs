using Microsoft.AspNetCore.Mvc;
using Tradify.Core;

namespace Tradify.Api.Controllers;

public sealed record CreateMessageRequest(string Body, string? SenderId);

[ApiController]
[Route("jobs/{jobId}/messages")]
public sealed class MessagesController(
    IMessageRepository messages,
    IJobAssignmentRepository assignments,
    IUserRepository users,
    ILogger<MessagesController> logger) : ControllerBase
{
    [HttpPost]
    public IActionResult Post(string jobId, [FromBody] CreateMessageRequest request)
    {
        var user = AuthStub.GetUser(Request, users);
        if (user is null)
        {
            return Unauthorized();
        }

        if (!assignments.IsAssigned(user.Id, jobId) ||
            (request.SenderId is not null && request.SenderId != user.Id))
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        var message = new Message(Guid.NewGuid(), jobId, user.Id, request.Body, DateTimeOffset.UtcNow);
        messages.Add(message);
        logger.LogInformation("Posted message {Body} to job {JobId}", request.Body, jobId);

        return StatusCode(StatusCodes.Status201Created, message);
    }

    [HttpGet]
    public IActionResult List(string jobId)
    {
        var user = AuthStub.GetUser(Request, users);
        if (user is null)
        {
            return Unauthorized();
        }

        if (!assignments.IsAssigned(user.Id, jobId))
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        return Ok(messages.ListForJob(jobId).Take(50));
    }
}
