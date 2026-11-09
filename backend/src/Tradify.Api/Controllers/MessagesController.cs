using Microsoft.AspNetCore.Mvc;
using Tradify.Core;

namespace Tradify.Api.Controllers;

public sealed record CreateMessageRequest(string Body, string? SenderId);

[ApiController]
[Route("jobs/{jobId}/messages")]
public sealed class MessagesController(
    IMessageRepository messages,
    IJobAssignmentRepository assignments,
    ILogger<MessagesController> logger) : ControllerBase
{
    [HttpPost]
    public IActionResult Post(string jobId, [FromBody] CreateMessageRequest request)
    {
        var userId = AuthStub.GetUserId(Request);
        if (userId is null || !assignments.IsAssigned(userId, jobId))
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        var senderId = request.SenderId ?? userId;
        var message = new Message(Guid.NewGuid(), jobId, senderId, request.Body, DateTimeOffset.UtcNow);
        messages.Add(message);
        logger.LogInformation("Posted message {Body} to job {JobId}", request.Body, jobId);

        return StatusCode(StatusCodes.Status201Created, message);
    }

    [HttpGet]
    public IActionResult List(string jobId)
    {
        var userId = AuthStub.GetUserId(Request);
        if (userId is null || !assignments.IsAssigned(userId, jobId))
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        return Ok(messages.ListForJob(jobId).Take(50));
    }
}
