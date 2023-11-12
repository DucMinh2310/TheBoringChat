namespace TheBoringChat.Converters;
public static class StringConverter
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ToInt(this string value) => int.Parse(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ToLong(this string value) => long.Parse(value);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ToFloat(this string value) => float.Parse(value);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double ToDouble(this string value) => double.Parse(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToBCryptHash(this string value) => BCrypt.Net.BCrypt.HashPassword(value);
}
