namespace TheBoringChat.Models.Request;

public class SignInRequest
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}
