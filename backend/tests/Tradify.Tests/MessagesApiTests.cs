using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Tradify.Tests;

// Each test gets a fresh host, so in-memory state never leaks between tests.
public sealed class MessagesApiTests : IDisposable
{
    private static readonly string[] SeededJ1Bodies =
    [
        "Kitchen measurements are in the job notes.",
        "Thanks. Can you confirm the start date?",
        "Starting Monday 13 October.",
        "Great, I'll arrange access.",
    ];

    private readonly WebApplicationFactory<Program> _factory = new();
    private readonly HttpClient _client;

    public MessagesApiTests()
    {
        _client = _factory.CreateClient();
    }

    private static HttpRequestMessage Request(HttpMethod method, string url, string userId)
    {
        var request = new HttpRequestMessage(method, url);
        request.Headers.Add("X-User-Id", userId);
        return request;
    }

    [Fact]
    public async Task PostMessage_AssignedStaff_Returns201()
    {
        using var request = Request(HttpMethod.Post, "/jobs/J1/messages", "U1");
        request.Content = JsonContent.Create(new { body = "On site at 8am." });

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task PostMessage_UnassignedUser_Returns403()
    {
        using var request = Request(HttpMethod.Post, "/jobs/J1/messages", "U3");
        request.Content = JsonContent.Create(new { body = "Can I see this thread?" });

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task ListMessages_AssignedStaff_ReturnsPostOrder()
    {
        using var request = Request(HttpMethod.Get, "/jobs/J1/messages", "U1");

        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var bodies = document.RootElement
            .EnumerateArray()
            .Select(element => element.GetProperty("body").GetString())
            .ToArray();

        Assert.Equal(SeededJ1Bodies, bodies);
    }

    [Fact]
    public async Task ListMessages_UnassignedUser_Returns403()
    {
        using var request = Request(HttpMethod.Get, "/jobs/J1/messages", "U3");

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task ListMessages_EmptyThread_Returns200Empty()
    {
        using var request = Request(HttpMethod.Get, "/jobs/J2/messages", "U1");

        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(0, document.RootElement.GetArrayLength());
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}
