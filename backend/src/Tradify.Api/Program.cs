using System.Text.Json.Serialization;
using Tradify.Core;
using Tradify.Seed;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddSingleton<IMessageRepository>(_ =>
{
    var repository = new InMemoryMessageRepository();
    SeedData.ApplyMessages(repository);
    return repository;
});

builder.Services.AddSingleton<IJobAssignmentRepository>(_ =>
    new InMemoryJobAssignmentRepository(SeedData.Assignments));

builder.Services.AddSingleton<IUserRepository>(_ =>
    new InMemoryUserRepository(SeedData.Users));

builder.Services.AddSingleton<IJobRepository>(_ =>
    new InMemoryJobRepository(SeedData.Jobs));

builder.Services.AddSingleton<JobThreadService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

app.UseCors();

app.MapControllers();

app.Run();

// Exposed for integration tests (WebApplicationFactory<Program>).
public partial class Program { }
