namespace TheBoringChat.Models.Request;

public class ForgotPasswordRequest
{
    public required string Username { get; set; }
    public required string NewPw { get; set; }
    public required string OTP { get; set; }
}
