namespace TOTP.Common.Models.Settings;
public class OTPSettings : ISettings
{
    public OTPSettings()
    {
        try
        {
            IConfiguration configuration = new ConfigurationBuilder()
                           .AddJsonFile("appsettings.json")
                           .Build();
            var audienceConfig = configuration.GetSection("OTPSettings");

            TimeStep = Convert.ToInt16(audienceConfig["TimeStep"]);
            Tolerance = Convert.ToInt16(audienceConfig["Tolerance"]);
            DigitLength = Convert.ToInt16(audienceConfig["DigitLength"]);
        }
        catch (Exception)
        {

        }
    }

    public OTPSettings(int timeStep, int tolerance, int digitLength)
    {
        TimeStep = timeStep;
        Tolerance = tolerance;
        DigitLength = digitLength;
    }

    public int TimeStep { get; }
    public int Tolerance { get; }
    public int DigitLength { get; }
}
