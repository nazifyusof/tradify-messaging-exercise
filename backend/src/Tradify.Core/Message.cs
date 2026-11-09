namespace Tradify.Core;

public sealed record Message(
    Guid Id,
    string JobId,
    string SenderId,
    string Body,
    DateTimeOffset CreatedAt);
