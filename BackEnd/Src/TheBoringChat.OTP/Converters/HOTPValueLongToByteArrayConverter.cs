namespace MinhLD_OTP.Converters;

public class HOTPValueLongToByteArrayConverter : IOneWayConverter<long, byte[]>
{
    public byte[] Convert(long input)
    {
        var bytes = BitConverter.GetBytes(input);

        // See more at https://tools.ietf.org/html/rfc4226#section-5.3
        // and https://tools.ietf.org/html/rfc4226#section-5.4
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }
        return bytes;
    }
}
