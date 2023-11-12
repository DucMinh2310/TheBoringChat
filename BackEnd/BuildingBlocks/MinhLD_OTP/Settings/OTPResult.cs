namespace MinhLD_OTP.Settings;
public class OTPResult
{
    public string OTPPassword { get; set; } = null!;
    public DateTime ValidFrom { get; set; }
    public DateTime ValidUntil { get; set; }
    public int ValidSecondsLeft { get; set; }
}
