namespace BackEnd.Controllers;

[Authorize]
public class AccountController(IUserRepos service, ILogger<BaseController<IUserRepos>> logger, ApplicationUser applicationUser, IMapper mapper)
    : BaseController<IUserRepos>(service, logger, applicationUser, mapper)
{
    [HttpGet(nameof(GetUser))]
    public async Task<IActionResult> GetUser()
    {
        var user = await _repos.GetByUsernameOrId(_applicationUser.Username, -1);
        if (user == null) return NotFound();

        var response = ResponseResult.Success<object>(new
        {
            user.Username,
            user.FullName,
            user.Avatar,
            user.Birthday,
            user.Gender,
            user.Address,
            user.Email,
        });
        return Ok(response);
    }
}
