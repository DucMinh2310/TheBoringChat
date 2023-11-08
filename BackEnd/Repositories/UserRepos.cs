namespace BackEnd.Repositories;

public class UserRepos(EFContext context, ILogger<BaseRepos> logger) : BaseRepos(context, logger), IUserRepos
{
    public async Task<Users?> GetByLogin(string username, string password)
    {
        var user = await _ef.Users.SingleOrDefaultAsync(x => x.Username == username && x.Password == password);
        return user;
    }

    public async Task<Users?> GetByUsernameOrId(string username, long id)
    {
        var user = await _ef.Users.SingleOrDefaultAsync(x => x.Username == username || x.Id == id);
        return user;
    }

    public async Task<int> InsertUser(Users users)
    {
        await _ef.Users.AddAsync(users);
        var rowAffected = await _ef.SaveChangesAsync();
        return rowAffected;
    }
}
