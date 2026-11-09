namespace Tradify.Core;

public interface IUserRepository
{
    User? Get(string userId);

    User? FindByUsername(string username);
}
