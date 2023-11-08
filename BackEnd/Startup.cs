namespace BackEnd;

public static class Startup
{
    public static IApplicationBuilder UseApplicationConfigure(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.UseMiddlewares();
        app.MapGet("/", async context => await context.Response.WriteAsync($"Hello, I love this world"));
        return app;
    }

    public static WebApplication BuildApplication(this WebApplicationBuilder builder, IConfiguration configuration)
    {
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        builder.Services.AddSingleton<ApplicationUser>();
        builder.Services.AddCustomSwagger();
        builder.Services.AddControllers()
                        .AddCustomBadRequest();
        builder.Services.AddEndpointsApiExplorer(); // Config minimal api
        builder.Host.AddSerilog(configuration);
        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        builder.Services.AddDbContext<EFContext>(options =>
                    options.UseSqlServer(
                        configuration.GetConnectionString("SqlConnection"),
                        providerOptions => providerOptions.EnableRetryOnFailure()));

        // Đăng ký dịch vụ mặc định cho repos
        builder.Services
                .Scan(scan => scan.FromCallingAssembly()
                .AddClasses(classes => classes.InNamespaces("BackEnd.Repositories.Interfaces", "BackEnd.Repositories"))
                .UsingRegistrationStrategy(RegistrationStrategy.Replace(ReplacementBehavior.ServiceType))
                .AsMatchingInterface()
                .WithScopedLifetime());

        return builder.Build();
    }

    public static IServiceCollection AddFolderScopedServices(this IServiceCollection services, params string[] namespaces)
    {
        services.Scan(scan => scan.FromCallingAssembly()
                .AddClasses(classes => classes.InNamespaces(namespaces))
                .UsingRegistrationStrategy(RegistrationStrategy.Replace(ReplacementBehavior.ServiceType)));
        return services;
    }

    private static WebApplication UseMiddlewares(this WebApplication app)
    {
        app.UseMiddleware<GlobalInternalServerExceptionMiddleware>();
        app.UseMiddleware<JwtMiddleware>();
        return app;
    }

    private static IHostBuilder AddSerilog(this ConfigureHostBuilder hostBuilder, IConfiguration configuration)
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

    private static IMvcBuilder AddCustomBadRequest(this IMvcBuilder controllers)
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

    private static IServiceCollection AddCustomSwagger(this IServiceCollection services)
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
