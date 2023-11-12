using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace MinhLD_OTP;
public class OTPHandler
{
    private const byte MaxOffsetLength = 0xf;
    private static readonly DateTime UnixEpoch = new(1970, 1, 1, 0, 0, 0);
    private readonly IOneWayConverter<long, byte[]> _hotpToByteConverter;
    private readonly ILogger<OTPHandler> _logger;
    private readonly ISecretGeneratorProvider _secretGeneratorProvider;
    private readonly ISettings _OtpSettings;
    private readonly long _digitModulo;


    public OTPHandler(
        ISettings OtpSettings,
        ILogger<OTPHandler> logger,
        IOneWayConverter<long, byte[]> hotpToByteConverter,
        ISecretGeneratorProvider secretGeneratorProvider)
    {
        _OtpSettings = OtpSettings;
        _digitModulo = Convert.ToInt64(Math.Pow(10, OtpSettings.DigitLength));
        _hotpToByteConverter = hotpToByteConverter;
        _logger = logger;
        _secretGeneratorProvider = secretGeneratorProvider;
    }

    /// <summary>
    ///     Conveys of the specifications of https://tools.ietf.org/html/rfc4226#section-5.3
    ///     and https://tools.ietf.org/id/draft-mraihi-totp-timebased-06.html#Section-Time-based-Variant
    /// </summary>
    /// <returns></returns>
    public OTPResult GenerateOTP(string userId, DateTime? utcDateTime = null)
    {
        var dateTime = utcDateTime ?? DateTime.UtcNow;
        var secret = _secretGeneratorProvider.GenerateSecret(userId);
        var hotpValue = ComputeHOTPValue(dateTime);
        var hotpBytes = _hotpToByteConverter.Convert(hotpValue);
        var hashBytes = GenerateHMACSHA1Hash(hotpBytes, secret);
        var binaryCode = GenerateBinaryCode(hashBytes);

        var otp = binaryCode % _digitModulo;

        var otpString = otp.ToString().PadLeft(_OtpSettings.DigitLength, '0');
        var otpResult = ComputeOTPResult(otpString, dateTime);

        return otpResult;
    }

    public bool ValidateOTP(string userId, string otp)
    {
        var utcNow = DateTime.UtcNow;

        for (var i = 0; i < _OtpSettings.Tolerance; i++)
        {
            var date = utcNow.AddSeconds(-(_OtpSettings.TimeStep * i));
            var generatedOtp = GenerateOTP(userId, date);
            if (generatedOtp.OTPPassword == otp) return true;
        }

        return false;
    }
    /// <summary>
    ///     See more at https://tools.ietf.org/id/draft-mraihi-totp-timebased-06.html#anchor3
    ///     Basically, we define TOTP as TOTP = HOTP(K, T) where T is an integer and represents the number
    ///     of time steps between the initial counter time T0 and the current Unix time (i.e. the number of seconds
    ///     elapsed since midnight UTC of January 1, 1970).
    ///     More specifically T = (Current Unix time - T0) / X where:
    ///     - X represents the time step in seconds (default value X = 30 seconds) and is a system parameter;
    ///     - T0 is the Unix time to start counting time steps (default value is 0, Unix epoch) and is also a system parameter;
    ///     - The default floor function is used in the computation. For example, with T0 = 0 and time step X = 30, T = 1
    ///     if the current Unix time is 59 seconds and T = 2 if the current Unix time is 60 seconds.
    /// </summary>
    /// <param name="utcDateTime">the input utc time</param>
    /// <returns>TOTP value</returns>
    // ReSharper disable once InconsistentNaming
    private long ComputeHOTPValue(DateTime utcDateTime)
    {
        var totpValue = (utcDateTime - UnixEpoch).TotalSeconds / _OtpSettings.TimeStep;
        _logger.LogInformation("{_OtpSettings}", JsonSerializer.Serialize(_OtpSettings));
        var totpLongValue = Convert.ToInt64(totpValue);

        return totpLongValue;
    }

    private OTPResult ComputeOTPResult(string otpString, DateTime utcDateTime)
    {
        var dateTime = utcDateTime;

        var second = dateTime.Second;
        if (second >= 15 && second < 45)
        {
            dateTime = dateTime.AddSeconds(-(second - 15));
        }
        else if (second >= 45 && second <= 59)
        {
            dateTime = dateTime.AddSeconds(-(second - 45));
        }
        else if (second >= 0 && second < 15)
        {
            dateTime = dateTime.AddSeconds(-second - 15);
        }

        var validUntil = dateTime.AddSeconds(_OtpSettings.TimeStep);
        var secondsLeft = (int)(validUntil - utcDateTime).TotalSeconds;

        var otpResult = new OTPResult
        {
            OTPPassword = otpString,
            ValidFrom = dateTime,
            ValidUntil = dateTime.AddSeconds(_OtpSettings.TimeStep),
            ValidSecondsLeft = secondsLeft
        };

        return otpResult;
    }
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once IdentifierTypo
    /// <summary>
    ///     Generating a HMACSHA1 value
    ///     for more information: https://tools.ietf.org/html/rfc4226#section-5.3
    ///     the inner contains an implementation of the https://tools.ietf.org/html/rfc4226#section-5.4
    /// </summary>
    private static byte[] GenerateHMACSHA1Hash(byte[] hotpBytes, string secret)
    {
        var secretBytes = Convert.FromBase64String(secret);

        var hmacSHA1 = new HMACSHA1(secretBytes);
        var hash = hmacSHA1.ComputeHash(hotpBytes);

        return hash;
    }

    /// <summary>
    ///     Generates the offset for the hash
    ///     Part of the https://tools.ietf.org/html/rfc4226#section-5.3
    /// </summary>
    /// <param name="hash">input hash bytes</param>
    /// <returns>the byte offset</returns>
    private static int GenerateHMACOffset(IEnumerable<byte> hash)
    {
        var offset = hash.Last();
        offset &= MaxOffsetLength;

        return offset;
    }

    /// <summary>
    ///     The method that generates the actual OTP based on the given hash
    ///     Taken from https://tools.ietf.org/html/rfc4226#section-5.3
    ///     DT(String) // String = String[0]...String[19]
    ///     Let OffsetBits be the low-order 4 bits of String[19]
    ///     Offset = StToNum(OffsetBits) // 0 &lt;= OffSet &lt;= 15
    ///     Let P = String[OffSet]...String[OffSet+3]
    ///     Return the Last 31 bits of P
    ///     Implemented based on the https://tools.ietf.org/html/rfc4226#section-5.4 suggestions
    /// </summary>
    /// <param name="hashBytes">the input hash bytes</param>
    /// <returns>the generated binary code</returns>
    // ReSharper disable once InconsistentNaming
    private static int GenerateBinaryCode(IReadOnlyList<byte> hashBytes)
    {
        var offset = GenerateHMACOffset(hashBytes);

        var binaryCode =
            (hashBytes[offset] & 0x7f) << 24
            | (hashBytes[offset + 1] & 0xff) << 16
            | (hashBytes[offset + 2] & 0xff) << 8
            | hashBytes[offset + 3] & 0xff;

        return binaryCode;
    }
}
