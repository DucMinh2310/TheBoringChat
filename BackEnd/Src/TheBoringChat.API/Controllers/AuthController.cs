using TheBoringChat.Services;
using BCrypt.Net;

namespace TheBoringChat.Controllers;

public class Auth(
    IUserRepos service,
    ILogger<BaseController<IUserRepos>> logger,
    ApplicationUser applicationUser,
    IMapper mapper,
    TokenHandler tokenHandler,
    IMediator mediator,
    OTPHandler otp)
    : BaseController<IUserRepos>(service, logger, applicationUser, mapper)
{
    private readonly TokenHandler _tokenHandler = tokenHandler;
    private readonly OTPHandler _otp = otp;
    private readonly IMediator _mediator = mediator;

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

    [HttpPost(nameof(ForgotPassword))]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
    {
        var otpValid = _otp.ValidateOTP(request.Username, request.OTP);
        if (!otpValid)
        {
            return Ok(ResponseResult.Failure("Mã OTP không hợp lệ"));
        }

        var updateUser = _mapper.Map<UpdateUserDto>(request);
        var data = await _repos.UpdateUser(updateUser);
        if (data == -1)
        {
            return BadRequest("Tài khoản không tồn tại!");
        }
        var response = data == 1 ? ResponseResult.Success() : ResponseResult.Failure();
        return Ok(response);
    }

    [HttpPost(nameof(SendOTP))]
    public async Task<IActionResult> SendOTP(SendOTPRequest request)
    {
        var user = await _repos.GetByEmail(request.Email);
        if (user == null)
        {
            return BadRequest("Email không tồn tại!");
        }

        var otpResult = _otp.GenerateOTP(user.Username);
        await _mediator.Publish(new EmailSetting() { To = request.Email, Body = otpResult.OTPPassword, Subject = "Gửi từ TheBoringChat" });
        return Ok();
    }
}
