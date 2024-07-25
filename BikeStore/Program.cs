using BikeStore.Data;
using BikeStore.Extensions;
using BikeStore.Middlewares;
using BikeStore.Models.Options;
using BikeStore.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<ISalesRepository, SalesRepository>();

builder.Services.AddOptions<UserOptions>()
    .BindConfiguration(UserOptions.User)
    // Validate Options before runtime
    .ValidateOnStart()
    // Validate Options at runtime
    .ValidateDataAnnotations()
    // Custom Validations
    .Validate(options =>
    {
        if (options.IPAddress is null)
        {
            return false;
        }

        return true;
    });

builder.Services.Configure<UserOptions>(
    builder.Configuration.GetSection(UserOptions.User)); 

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(config =>
{
    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    config.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(configOptions =>
{
    configOptions.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["Authentication:Schemes:Bearer:ValidIssuer"],
        ValidAudience = builder.Configuration["Authentication:Schemes:Bearer:ValidAudiences"],
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = false
    };
});

builder.Services.AddAuthorization();

builder.Services.AddDbContext<BikeStoresContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("BikeStoreSQLServer"));
});

builder.Services.AddLogging();
builder.Services.AddTransient<GlobalExceptionHandlerMiddleware>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.MapControllers();

app.Run();
