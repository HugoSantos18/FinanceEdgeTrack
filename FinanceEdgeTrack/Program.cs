using FinanceEdgeTrack.Application.Common.Pagination.Filters;
using FinanceEdgeTrack.Domain.Interfaces;
using FinanceEdgeTrack.Domain.Interfaces.Repositories;
using FinanceEdgeTrack.Domain.Models;
using FinanceEdgeTrack.Infrastructure.Extensions;
using FinanceEdgeTrack.Infrastructure.Config;
using FinanceEdgeTrack.Infrastructure.Data;
using FinanceEdgeTrack.Infrastructure.Repositories;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;
using FinanceEdgeTrack.Domain.Interfaces.Services.Auth;
using FinanceEdgeTrack.Domain.Interfaces.Services.Categories;
using FinanceEdgeTrack.Application.Services.Auth;
using FinanceEdgeTrack.Application.Services.Categories;
using Microsoft.AspNetCore.Authorization;
using FinanceEdgeTrack.Logging;
using FinanceEdgeTrack.Application.Services.CarteiraService;
using FinanceEdgeTrack.Domain.Interfaces.Services.CarteiraService;
using FinanceEdgeTrack.Application.Services.Metrics;
using FinanceEdgeTrack.Domain.Interfaces.Metrics;
using FinanceEdgeTrack.Domain.Interfaces.Services.Dashboard;
using FinanceEdgeTrack.Application.Services.Dashboard;
using FinanceEdgeTrack.Application.Services.Cache;
using FinanceEdgeTrack.Domain.Interfaces.Services.Cache;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMapster();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddControllers(options =>
{
    options.Filters.Add(new ApiExceptionFilter());
    options.Filters.Add<PaginationHeaderFilter>();
})
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });


// UserSecrets configuration
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>();

// DataBase configuration
builder.Services.AddDatabaseConfiguration(builder.Configuration);


// Bind strongly-typed JWT settings
var jwtSection = builder.Configuration.GetSection("JWT");
builder.Services.Configure<JwtSettings>(jwtSection);
var jwtSettings = jwtSection.Get<JwtSettings>() ?? throw new ArgumentException("JWT configuration missing");
builder.Services.AddSingleton(jwtSettings);

// Cors config
builder.Services.AddCorsConfiguration(builder.Configuration);

// Rate Limiting config
builder.Services.AddApiRateLimiting(builder.Configuration);

// Versioning config
builder.Services.AddApiVersionConfig();

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();


// Dependency Injections (Services, Mapper, etc...)
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IMetaRepository, MetaRepository>();
builder.Services.AddScoped<IReceitaService, ReceitaService>();
builder.Services.AddScoped<IDespesaService, DespesaService>();
builder.Services.AddScoped<IMetaService, MetaService>();
builder.Services.AddScoped<ICarteiraService, CarteiraService>();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthenticationService, AuthService>();
builder.Services.AddScoped<IRoleService, RoleSevice>();
builder.Services.AddScoped<ICarteiraMetrics, CarteiraMetricsService>();
builder.Services.AddScoped<IDespesaMetrics, DespesaMetricsService>();
builder.Services.AddScoped<IMetaMetrics, MetaMetricsService>();
builder.Services.AddScoped<IReceitaMetrics, ReceitaMetricsService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();


// Add Loging provider
builder.Logging.AddProvider(new CustomerLoggerProvider(new CustomerLoggerProviderConfig
{
    LogLevel = LogLevel.Information
}));


// Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    var secretKey = jwtSettings.SecretKey ?? throw new ArgumentException("Invalid Key");

    if (secretKey.IsNullOrEmpty())
        throw new Exception("JWT secretKey não configurado!");

    if (jwtSettings.SecretKey.Length < 32)
        throw new Exception("JWT secretKey deve ter no mínimo 32 caracteres!");

    options.SaveToken = true;
    options.RequireHttpsMetadata = true;

    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        ValidAudience = jwtSettings.ValidAudience,
        ValidIssuer = jwtSettings.ValidIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});


// JWT Global e Roles policy
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build();

}).AddRolesPolicy();


// Swagger configuration
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "FinanceEdgeTrackApi", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Bearer JWT",
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{ }
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseCors("DefaultAllowedCors");

app.UseRateLimiter();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
