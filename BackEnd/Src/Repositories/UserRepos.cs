using BCrypt.Net;

namespace TheBoringChat.Repositories;

public class UserRepos(EFContext context, ILogger<BaseRepos> logger) : BaseRepos(context, logger), IUserRepos
{
    public async Task<Users?> GetByLogin(string username, string password)
    {
        var user = await _ef.Users.SingleOrDefaultAsync(x => x.Username == username);
        if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password)) return user;
        else return null;
    }

    public async Task<Users?> GetByUsernameOrId(string username, long id)
    {
        var user = await _ef.Users.SingleOrDefaultAsync(x => (x.Username == username || x.Id == id) && x.Status == 1);
        return user;
    }

    public async Task<Users?> GetByEmail(string email)
    {
        var user = await _ef.Users.SingleOrDefaultAsync(x => x.Email == email);
        return user;
    }

    public async Task<int> InsertUser(Users user)
    {
        await _ef.Users.AddAsync(user);
        return await _ef.SaveChangesAsync();
    }

    public async Task<int> UpdateUser(UpdateUserDto request, int type = 0)
    {
        var user = type switch
        {
            0 => await GetByUsernameOrId(request.Username, request.Id),
            1 => await GetByLogin(request.Username, request.Password),
            _ => throw new Exception("UpdateUser: Type is not exist")
        };
        if (user == null) return -1;

        user.Address = request.Address ?? user.Address;
        user.FullName = request.FullName ?? user.FullName;
        user.Email = request.Email ?? user.Email;
        user.Gender = request.Gender ?? user.Gender;
        user.Birthday = request.Birthday ?? user.Birthday;
        user.Avatar = request.Avatar ?? user.Avatar;
        user.Password = request.NewPassword ?? user.Password;
        user.UpdatedAt = DateTime.Now;
        return await _ef.SaveChangesAsync();
    }
}
