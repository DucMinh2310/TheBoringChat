namespace BackEnd.Middlewares;
public class GlobalInternalServerExceptionMiddleware(RequestDelegate next, ILogger<GlobalInternalServerExceptionMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<GlobalInternalServerExceptionMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await ExceptionHelper.HandleMiddlewareExceptionAsync(context, ex, _logger);
            return;
        }
    }
}
