using Microsoft.AspNetCore.Http;
using Tradify.Core;

namespace Tradify.Api;

// Simulates authentication for the exercise. Not real auth.
public static class AuthStub
{
    public const string UserHeader = "X-User-Id";

    public static string? GetUserId(HttpRequest request) =>
        request.Headers.TryGetValue(UserHeader, out var value) ? value.ToString() : null;

    public static User? GetUser(HttpRequest request, IUserRepository users)
    {
        var userId = GetUserId(request);
        return string.IsNullOrWhiteSpace(userId) ? null : users.Get(userId);
    }
}
