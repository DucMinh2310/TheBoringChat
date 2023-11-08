namespace BackEnd.Middlewares;
public class JwtMiddleware(RequestDelegate next, ILogger<JwtMiddleware> logger, IConfiguration configuration)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<JwtMiddleware> _logger = logger;
    private readonly IConfiguration _configuration = configuration;

    public async Task Invoke(HttpContext context)
    {
        await LoggingRequest(context);
        try
        {
            var jwtEncodedString = context.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();
            _logger.LogInformation("Token: {jwtEncodedString}", jwtEncodedString);

            if (!string.IsNullOrEmpty(jwtEncodedString))
            {
                var token = new JwtSecurityToken(jwtEncodedString: jwtEncodedString);
                var username = token.Claims.First(c => c.Type == "Username").Value;
                var userId = token.Claims.First(c => c.Type == "UserId").Value;
                if (userId != null)
                {
                    var applicationUser = context.RequestServices.GetRequiredService<ApplicationUser>();
                    applicationUser.UserId = userId.ToLong();
                    applicationUser.Username = username;
                }
            }
        }
        catch (Exception ex)
        {
            await ExceptionHelper.HandleMiddlewareExceptionAsync(context, ex, _logger, HttpStatusCode.Unauthorized);
            return;
        }
        await _next(context);
    }

    private async Task LoggingRequest(HttpContext context)
    {
        var endpointIgnoreLog = _configuration.GetSection("LoggerSetting:EndpointIgnoreLoggingRequest").Get<List<string>>() ?? [];
        endpointIgnoreLog = endpointIgnoreLog.Select(e => e = e.ToLower()).ToList();
        var endpoint = context.Request.Path.ToString().Split("/").Last().ToLower();
        if (endpointIgnoreLog.Count == 0 || !endpointIgnoreLog.Contains(endpoint))
        {
            context.Request.EnableBuffering();
            using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true);
            var requestBody = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0; // Fix err 400
            var requestBodyJson = JsonConvert.DeserializeObject(requestBody);
            string body = JsonConvert.SerializeObject(requestBodyJson);
            _logger.LogInformation("Request => Method: {Method}, Path: {Path}, Body: {body}", context.Request.Method, context.Request.Path, body);
        }
    }
}