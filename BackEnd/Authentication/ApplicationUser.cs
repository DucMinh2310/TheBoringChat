namespace BackEnd.Authentication
{
    public class ApplicationUser
    {
        public long UserId { get; set; }
        public required string Username { get; set; }
    }
}