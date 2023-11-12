namespace Startup.BackEnd;

public static class Startup
{
    public static IServiceCollection AddFolderScopedServices(this IServiceCollection services, params string[] namespaces)
    {
        services.Scan(scan => scan.FromCallingAssembly()
                .AddClasses(classes => classes.InNamespaces(namespaces))
                .UsingRegistrationStrategy(RegistrationStrategy.Replace(ReplacementBehavior.ServiceType)));
        return services;
    }

    public static IServiceCollection AddOTP(this IServiceCollection services, params string[] namespaces)
    {
        services.AddTransient<OTPHandler>();
        services.AddSingleton<ISettings, OTPSettings>();
        services.AddSingleton<IOneWayConverter<long, byte[]>, HOTPValueLongToByteArrayConverter>();
        services.AddTransient<ISecretGeneratorProvider, HMAC256UserIdSecretGeneratorProvider>();
        return services;
    }

    public static WebApplication UseMiddlewares(this WebApplication app)
    {
        app.UseMiddleware<GlobalInternalServerExceptionMiddleware>();
        app.UseMiddleware<JwtMiddleware>();
        return app;
    }

    public static IApplicationBuilder UseServiceDefaults(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        return app;
    }

    public static IHostBuilder AddSerilog(this ConfigureHostBuilder hostBuilder, IConfiguration configuration)
    {
        string outputTemplate = "[{Level:u3}] ==> [{Timestamp:dd/MM/yyyy HH:mm:ss}] {Message:lj}{NewLine}{Exception}";
        hostBuilder.UseSerilog((context, configuaration) =>
        {
            configuaration
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: outputTemplate)
                .WriteTo.File(
                    path: "./Logs/log-.txt",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: outputTemplate,
                    rollOnFileSizeLimit: true,
                    fileSizeLimitBytes: 5 * 1024 * 1024,
                    retainedFileCountLimit: 30)
                .ReadFrom
                .Configuration(configuration);
        });
        return hostBuilder;
    }

    public static IMvcBuilder AddCustomBadRequest(this IMvcBuilder controllers)
    {
        controllers.ConfigureApiBehaviorOptions(options =>
        {
            options.InvalidModelStateResponseFactory = actionContext =>
            {
                var modelState = actionContext.ModelState;
                var errors = modelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();

                return new BadRequestObjectResult(new ResponseResult(false, "Bad request", errors));
            };
        });

        return controllers;
    }

    public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(option =>
        {
            option.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "The boring chat",
                Version = "1.0",
                Description = $"Service for the boring chat",
            });
            option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });
            option.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
             {
                 new OpenApiSecurityScheme
                 {
                     Reference = new OpenApiReference
                     {
                         Type=ReferenceType.SecurityScheme,
                         Id="Bearer"
                     }
                 },
                 Array.Empty<string>()
             }
            });
        });
        return services;
    }
}
