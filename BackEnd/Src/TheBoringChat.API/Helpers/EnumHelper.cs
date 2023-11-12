namespace TheBoringChat.Helpers;

public static class EnumHelper
{
    public static string GetDescription(this Enum value)
    {
        var fi = value.GetType().GetField(value.ToString()) ?? throw new InvalidDataException("Invalid data");
        var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
        return attributes is { Length: > 0 } ? attributes[0].Description : value.ToString();
    }
}
