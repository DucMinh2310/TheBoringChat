namespace TheBoringChat.Models.Request;

public class ChangePasswordRequest
{
    public required string OldPw { get; set; }
    public required string NewPw { get; set; }
}