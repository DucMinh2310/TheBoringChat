using System.Security.Claims;
using BCrypt.Net;

namespace BackEnd.Controllers;

public class Auth(IUserRepos service, ILogger<BaseController<IUserRepos>> logger, ApplicationUser applicationUser, IMapper mapper, TokenHandler tokenHandler)
    : BaseController<IUserRepos>(service, logger, applicationUser, mapper)
{
    private readonly TokenHandler _tokenHandler = tokenHandler;

    [HttpPost(nameof(SignUp))]
    public async Task<IActionResult> SignUp(SignUpRequest request)
    {
        var user = await _repos.GetByUsernameOrId(request.Username, -1);
        if (user != null)
        {
            return Ok(ResponseResult.Failure("Tài khoản đã tồn tại!"));
        }
        var newUser = _mapper.Map<Users>(request);
        var userCreated = await _repos.InsertUser(newUser);
        var response = userCreated == 1 ? ResponseResult.Success() : ResponseResult.Failure();
        return Ok(response);
    }

    [HttpPost(nameof(SignIn))]
    public async Task<IActionResult> SignIn(SignInRequest request)
    {
        var user = await _repos.GetByUsernameOrId(request.Username, -1);
        if (user == null)
        {
            return Ok(ResponseResult.Failure("Tài khoản không tồn tại!"));
        }
        else if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
        {
            return Ok(ResponseResult.Failure("Mật khẩu không chính xác"));
        }
        else
        {
            var claims = new Claim[]
            {
                new ("Username", request.Username),
                new ("UserId", user.Id.ToString())
            };
            string accessToken = _tokenHandler.GenerateAccessToken(claims);
            return Ok(new
            {
                request.Username,
                user.FullName,
                accessToken
            });
        }
    }
}
