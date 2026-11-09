using Microsoft.AspNetCore.Http;

namespace Tradify.Api;

// Simulates authentication for the exercise. Not real auth.
public static class AuthStub
{
    public const string UserHeader = "X-User-Id";

    public static string? GetUserId(HttpRequest request) =>
        request.Headers.TryGetValue(UserHeader, out var value) ? value.ToString() : null;
}
