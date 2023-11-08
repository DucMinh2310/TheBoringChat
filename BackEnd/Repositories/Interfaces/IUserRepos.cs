namespace BackEnd.Repositories.Interfaces;

public interface IUserRepos
{
    Task<int> InsertUser(Users users);
    Task<Users?> GetByUsernameOrId(string username, long id);
    Task<Users?> GetByLogin(string username, string password);
}
