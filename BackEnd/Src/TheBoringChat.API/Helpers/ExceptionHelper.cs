namespace TheBoringChat.Helpers;
public class ExceptionHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ShowEx(Exception ex) => string.Concat(
        "Error: ", ex.Message, Environment.NewLine,
        "Inner: ", ex.InnerException, Environment.NewLine,
        "StackTrace: ", ex.StackTrace);

    public static Task HandleMiddlewareExceptionAsync(
            HttpContext context,
            Exception ex,
            ILogger logger,
            HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError)
    {
        string error = ShowEx(ex);
        logger.LogError("{Error}", ShowEx(ex));

        // Default
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)httpStatusCode;
        var response = new ResponseResult(false, error, null);

        var jsonResponse = JsonConvert.SerializeObject(response);
        return context.Response.WriteAsync(jsonResponse);
    }
}


