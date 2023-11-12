namespace TheBoringChat.Models.Request;

public class UpdateUserRequest
{
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public DateTime? Birthday { get; set; }
    public int? Gender { get; set; }
    public string? Address { get; set; }
    public string? Avatar { get; set; }
}
