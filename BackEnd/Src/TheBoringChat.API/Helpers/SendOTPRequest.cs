namespace TheBoringChat.Helpers;

public class SendOTPRequest
{
    [EmailAddress]
    public required string Email { get; set; }
}
