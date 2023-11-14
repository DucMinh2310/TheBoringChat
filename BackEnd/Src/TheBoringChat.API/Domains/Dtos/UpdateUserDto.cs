namespace TheBoringChat.Domains.Dtos;

public class UpdateUserDto
{
    public long Id { get; set; }
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public DateTime? Birthday { get; set; }
    public int? Gender { get; set; }
    public string? Address { get; set; }
    public string? Avatar { get; set; }
}
