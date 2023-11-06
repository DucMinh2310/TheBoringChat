namespace BackEnd.Domains.Entities;
public class Users
{
    public long Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public DateTime? Birthday { get; set; }
    public int? Gender { get; set; }
    public string Address { get; set; }
    public string Avatar { get; set; }
    public int? Status { get; set; }
    public DateTime? SysD { get; set; }
    public long? SysU { get; set; }
}