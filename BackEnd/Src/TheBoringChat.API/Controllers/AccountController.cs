namespace TheBoringChat.Controllers;

[Authorize]
public class AccountController(IUserRepos service, ILogger<BaseController<IUserRepos>> logger, ApplicationUser applicationUser, IMapper mapper)
    : BaseController<IUserRepos>(service, logger, applicationUser, mapper)
{
    [HttpGet(nameof(GetUser))]
    public async Task<IActionResult> GetUser()
    {
        var user = await _repos.GetByUsernameOrId(_applicationUser.Username, -1);
        if (user == null) return BadRequest(ResponseResult.Failure("Tài khoản không tồn tại!"));

        var data = _mapper.Map<UserDto>(user);
        var response = ResponseResult.Success(data);
        return Ok(response);
    }

    [HttpPut(nameof(UpdateUser))]
    public async Task<IActionResult> UpdateUser(UpdateUserRequest request)
    {
        var updateUserDto = _mapper.Map<UpdateUserDto>(request);
        updateUserDto.Id = _applicationUser.UserId;
        var data = await _repos.UpdateUser(updateUserDto);
        if (data == -1)
        {
            return BadRequest(ResponseResult.Failure("Tài khoản không tồn tại!"));
        }

        var response = data >= 1 ? ResponseResult.Success() : ResponseResult.Failure();
        return Ok(response);
    }

    [HttpPut(nameof(ChangePassword))]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
    {
        var data = await _repos.UpdateUser(new UpdateUserDto()
        {
            Password = request.OldPw,
            NewPassword = request.NewPw.ToBCryptHash(),
            Username = _applicationUser.Username
        }, 1);

        if (data == -1)
        {
            return BadRequest(ResponseResult.Failure("Mật khẩu cũ không đúng!"));
        }

        var response = data >= 1 ? ResponseResult.Success() : ResponseResult.Failure();
        return Ok(response);
    }
}
