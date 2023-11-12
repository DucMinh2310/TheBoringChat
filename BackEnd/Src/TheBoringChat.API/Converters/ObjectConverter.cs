namespace TheBoringChat.Converters;

public static class ObjectConverter
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string DoSerialize(this object value) => JsonConvert.SerializeObject(value, new JsonSerializerSettings
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
    });
}
