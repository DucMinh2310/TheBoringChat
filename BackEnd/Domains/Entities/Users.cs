namespace BackEnd.Domains.Entities;

[Table("Users", Schema = "dbo")]
public class Users : BaseEntity
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public DateTime? Birthday { get; set; }
    public int? Gender { get; set; }
    public string? Address { get; set; }
    public string? Avatar { get; set; }
    public int Status { get; set; }
}