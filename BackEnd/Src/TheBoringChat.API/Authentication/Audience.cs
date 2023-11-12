namespace TheBoringChat.Authentication;
public class Audience
{
    public Audience(IConfiguration configuration)
    {
        var audienceConfig = configuration.GetSection("Audience");
        Secret = audienceConfig["Secret"] ?? string.Empty;
        Iss = audienceConfig["Iss"] ?? string.Empty;
        Aud = audienceConfig["Aud"] ?? string.Empty;
        Expires = Convert.ToInt16(audienceConfig["Expires"]);
    }

    public string Secret { get; set; } = null!;
    public string Iss { get; set; } = null!;
    public string Aud { get; set; } = null!;
    public int Expires { get; set; }
}
