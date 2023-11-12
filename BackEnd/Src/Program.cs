var builder = WebApplication.CreateBuilder(args);

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

// ORM
builder.Services.AddDbContext<EFContext>(options =>
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("SqlConnection"),
                providerOptions => providerOptions.EnableRetryOnFailure()));

// Scrutor
builder.Services
        .Scan(scan => scan.FromCallingAssembly()
        .AddClasses(classes => classes.InNamespaces("TheBoringChat.Repositories.Interfaces", "TheBoringChat.Repositories"))
        .UsingRegistrationStrategy(RegistrationStrategy.Replace(ReplacementBehavior.ServiceType))
        .AsMatchingInterface()
        .WithScopedLifetime());

// Authentication
builder.Services.AddSingleton<ApplicationUser>();
builder.Services.AddScoped<Audience>();
builder.Services.AddTransient<TokenHandler>();
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Audience:Secret"] ?? string.Empty)),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Audience:Iss"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Audience:Aud"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            RequireExpirationTime = true
        };
    });

// OTP
builder.Services.AddOTP();

// Orthers
builder.Services.AddCustomSwagger();
builder.Services.AddControllers()
                .AddCustomBadRequest();
builder.Services.AddEndpointsApiExplorer(); // Config minimal api
builder.Host.AddSerilog(builder.Configuration);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());

// ===========================================================
var app = builder.Build();
app.UseServiceDefaults();
app.UseMiddlewares();
app.UseFileServer(new FileServerOptions
{
    FileProvider = new PhysicalFileProvider(
            Path.Combine(Directory.GetCurrentDirectory(), "StaticFiles")),
    RequestPath = "/StaticFiles",
    EnableDefaultFiles = true
});
app.MapGet("/", async context => await context.Response.WriteAsync($"Hello, I love this world"));
app.Run();