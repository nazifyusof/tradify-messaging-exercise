namespace Tradify.Core;

public sealed class InMemoryUserRepository : IUserRepository
{
    private readonly Dictionary<string, User> _users;

    public InMemoryUserRepository(IEnumerable<User> users)
    {
        _users = users.ToDictionary(u => u.Id);
    }

    public User? Get(string userId) => _users.GetValueOrDefault(userId);

    public User? FindByUsername(string username) =>
        _users.Values.FirstOrDefault(u => string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase));
}
