using FinanceEdgeTrack.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FinanceEdgeTrack.Error.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// UserSecrets configuration
builder.Configuration
    .AddJsonFile("appsettings.json", optional: true)
    .AddUserSecrets<Program>();

// DataBase configuration
builder.Services.AddDbContext<AppDbContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


// Dependency Injections (Services, Mapper, etc...)

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();


// Swagger configuration

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
    ApiExtensionMiddleware.ConfigureExceptionHandler(app);
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
