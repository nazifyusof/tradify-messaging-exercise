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

    [Fact]
    public async Task PostMessage_SpoofedSenderId_Returns403AndNotStored()
    {
        using var request = Request(HttpMethod.Post, "/jobs/J1/messages", "U1");
        request.Content = JsonContent.Create(new { body = "Spoofed", senderId = "U4" });

        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        using var list = Request(HttpMethod.Get, "/jobs/J1/messages", "U1");
        var listResponse = await _client.SendAsync(list);
        using var document = JsonDocument.Parse(await listResponse.Content.ReadAsStringAsync());
        Assert.Equal(SeededJ1Bodies.Length, document.RootElement.GetArrayLength());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("U1")]
    public async Task PostMessage_OwnOrOmittedSenderId_Returns201WithCallerAsSender(string? senderId)
    {
        using var request = Request(HttpMethod.Post, "/jobs/J1/messages", "U1");
        request.Content = JsonContent.Create(new { body = "Hello", senderId });

        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal("U1", document.RootElement.GetProperty("senderId").GetString());
    }

    [Theory]
    [InlineData("GET", null)]
    [InlineData("GET", "U999")]
    [InlineData("POST", null)]
    [InlineData("POST", "U999")]
    public async Task MessagesEndpoints_MissingOrUnknownUser_Returns401(string method, string? userId)
    {
        using var request = new HttpRequestMessage(new HttpMethod(method), "/jobs/J1/messages");
        if (userId is not null)
        {
            request.Headers.Add("X-User-Id", userId);
        }

        if (method == "POST")
        {
            request.Content = JsonContent.Create(new { body = "Hi" });
        }

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}
