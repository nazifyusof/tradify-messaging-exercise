using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Tradify.Tests;

public sealed class AuthAndJobsApiTests : IDisposable
{
    private readonly WebApplicationFactory<Program> _factory = new();
    private readonly HttpClient _client;

    public AuthAndJobsApiTests()
    {
        _client = _factory.CreateClient();
    }

    [Theory]
    [InlineData("alex", "anything")]
    [InlineData("ALEX", "")]
    public async Task Login_KnownUsername_AnyPassword_ReturnsUser(string username, string password)
    {
        var response = await _client.PostAsJsonAsync("/auth/login", new { username, password });
        response.EnsureSuccessStatusCode();

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal("U1", document.RootElement.GetProperty("id").GetString());
        Assert.Equal("Alex Morgan", document.RootElement.GetProperty("name").GetString());
    }

    [Theory]
    [InlineData("alex", "Admin")]
    [InlineData("sam", "FieldWorker")]
    [InlineData("chris", "Customer")]
    public async Task Login_ReturnsRoleAndCompany(string username, string role)
    {
        var response = await _client.PostAsJsonAsync("/auth/login", new { username, password = "x" });
        response.EnsureSuccessStatusCode();

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(role, document.RootElement.GetProperty("role").GetString());
        Assert.Equal("CO1", document.RootElement.GetProperty("companyId").GetString());
    }

    [Fact]
    public async Task Login_UnknownUsername_Returns401()
    {
        var response = await _client.PostAsJsonAsync("/auth/login", new { username = "nobody", password = "x" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ListJobs_ReturnsOnlyAssignedJobs()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/jobs");
        request.Headers.Add("X-User-Id", "U2");

        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var job = Assert.Single(document.RootElement.EnumerateArray());
        Assert.Equal("J2", job.GetProperty("id").GetString());
        Assert.Equal("Boiler service", job.GetProperty("title").GetString());
        Assert.Equal(JsonValueKind.Null, job.GetProperty("lastMessage").ValueKind);
    }

    [Fact]
    public async Task ListJobs_IncludesParticipantNamesAndLastMessage()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/jobs");
        request.Headers.Add("X-User-Id", "U1");

        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var jobs = document.RootElement.EnumerateArray().ToArray();
        Assert.Equal(["J1", "J2"], jobs.Select(j => j.GetProperty("id").GetString()).ToArray());

        var j1 = jobs[0];
        var names = j1.GetProperty("participants").EnumerateArray()
            .Select(p => p.GetProperty("name").GetString())
            .ToArray();
        Assert.Equal(["Alex Morgan", "Chris Walker"], names);
        Assert.Equal(JsonValueKind.Object, j1.GetProperty("lastMessage").ValueKind);
    }

    [Fact]
    public async Task ListJobs_UserWithNoAssignments_ReturnsEmpty()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/jobs");
        request.Headers.Add("X-User-Id", "U3");

        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(0, document.RootElement.GetArrayLength());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("U999")]
    public async Task ListJobs_MissingOrUnknownUser_Returns401(string? userId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/jobs");
        if (userId is not null)
        {
            request.Headers.Add("X-User-Id", userId);
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
