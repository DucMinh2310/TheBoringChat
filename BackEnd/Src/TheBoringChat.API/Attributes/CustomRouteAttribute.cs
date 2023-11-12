namespace TheBoringChat.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class CustomRouteAttribute : Attribute, IRouteTemplateProvider
{
    private int? _order;
    public string Template { get; init; } = "api/[controller]";
    public int Order
    {
        get { return _order ?? 0; }
        set { _order = value; }
    }
    int? IRouteTemplateProvider.Order => _order;
    public string? Name { get; set; }
}