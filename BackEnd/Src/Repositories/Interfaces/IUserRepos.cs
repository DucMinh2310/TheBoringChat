namespace TheBoringChat.Repositories.Interfaces;

public interface IUserRepos
{
    Task<int> InsertUser(Users user);
    Task<Users?> GetByUsernameOrId(string username, long id);
    Task<Users?> GetByLogin(string username, string password);
    Task<Users?> GetByEmail(string email);
    Task<int> UpdateUser(UpdateUserDto user, int type = 0);
}
