var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddScoped<Audience>();
builder.Services.AddTransient<TokenHandler>();
var app = builder.BuildApplication(builder.Configuration);
app.UseApplicationConfigure();
app.Run();