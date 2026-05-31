using FinanceEdgeTrack.Application.Interfaces.Services.Auth;
using FinanceEdgeTrack.Application.Interfaces.Services.Cache;
using FinanceEdgeTrack.Application.Interfaces.Services.Carteira;
using FinanceEdgeTrack.Application.Interfaces.Services.Categories;
using FinanceEdgeTrack.Application.Interfaces.Services.Dashboard;
using FinanceEdgeTrack.Application.Interfaces.Services.Metrics;
using FinanceEdgeTrack.Application.Mappings;
using FinanceEdgeTrack.Application.Services.CarteiraService;
using FinanceEdgeTrack.Application.Services.Categories;
using FinanceEdgeTrack.Application.Services.Dashboard;
using FinanceEdgeTrack.Application.Services.Metrics;
using FinanceEdgeTrack.Domain.Interfaces;
using FinanceEdgeTrack.Domain.Interfaces.Repositories;
using FinanceEdgeTrack.Errors;
using FinanceEdgeTrack.Extensions;
using FinanceEdgeTrack.Filters;
using FinanceEdgeTrack.Infrastructure.Auth;
using FinanceEdgeTrack.Infrastructure.Cache;
using FinanceEdgeTrack.Infrastructure.Config;
using FinanceEdgeTrack.Infrastructure.Data;
using FinanceEdgeTrack.Infrastructure.Extensions;
using FinanceEdgeTrack.Infrastructure.Identity;
using FinanceEdgeTrack.Infrastructure.Logging;
using FinanceEdgeTrack.Infrastructure.Repositories;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
MapsterMappingConfig.ConfigurarMapeamento();
builder.Services.AddMapster();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiExceptionFilter>();
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
builder.Services.AddScoped<IAporteMetasRepository, AporteMetasRepository>();
builder.Services.AddScoped<IReceitaService, ReceitaService>();
builder.Services.AddScoped<IDespesaService, DespesaService>();
builder.Services.AddScoped<IMetaService, MetaService>();
builder.Services.AddScoped<IAporteMetasService, AporteMetasService>();
builder.Services.AddScoped<ICarteiraService, CarteiraService>();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthenticationService, AuthService>();
builder.Services.AddScoped<IRoleService, RoleSevice>();
builder.Services.AddScoped<IDespesaMetrics, DespesaMetricsService>();
builder.Services.AddScoped<IMetaMetrics, MetaMetricsService>();
builder.Services.AddScoped<IReceitaMetrics, ReceitaMetricsService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddSingleton<ICacheService, CacheService>();
builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();


// Add Logging provider
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
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "FinanceEdgeTrack API",
        Version = "v1",
        Description = """
            API de controle financeiro pessoal — despesas, receitas, metas e aportes.

            **Autenticação:** Todos os endpoints exigem JWT Bearer, exceto `/Login` e `/Register`.
            Informe o token no formato: `Bearer {seu_token}`.

            **Rate Limiting:** Requisições em excesso retornam `429 Too Many Requests`.
            Usuários autenticados têm limite por ID; requisições anônimas têm limite por IP.

            **Autorização:** Endpoints do Dashboard e administração exigem role `Admin`.
            Endpoints de CRUD (Despesas, Receitas, Metas, Aportes) exigem apenas autenticação.
            """
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Informe o token JWT no formato: **Bearer {token}**",
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

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
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
