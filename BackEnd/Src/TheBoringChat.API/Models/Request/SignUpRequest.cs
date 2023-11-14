namespace TheBoringChat.Models.Request;

public class SignUpRequest
{
    [RegularExpression("^(?!0+$)(\\+\\d{1,3}[- ]?)?(?!0+$)\\d{10,15}$", ErrorMessage = "Please enter valid phone no.")]
    public required string Username { get; set; }
    public required string Password { get; set; }
    [EmailAddress]
    public required string Email { get; set; }
    public required string Fullname { get; set; }
}
