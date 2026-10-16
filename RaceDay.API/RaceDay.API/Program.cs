using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RaceDay.API.Data;
using RaceDay.API.Models;
using RaceDay.API.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddControllers();

builder.Services.AddScoped<JwtService>();

builder.Services.AddAuthentication(//tells the api how to handle authentication
                        JwtBearerDefaults.AuthenticationScheme)
                        .AddJwtBearer(options => //tells the api to expect jwt tokens
                         {
                             options.TokenValidationParameters =
                             new TokenValidationParameters
                             {
                                 //checking who issued tokens and for which user
                                 ValidateIssuer = true,
                                 ValidIssuer = "RaceDay.API",

                                 ValidateAudience = true,
                                 ValidAudience = "RaceDay.Client",

                                 ValidateIssuerSigningKey = true,
                                 IssuerSigningKey = new SymmetricSecurityKey(
                                             Encoding.UTF8.GetBytes(
                                             builder.Configuration["Jwt:Key"]!)),

                                 //checks if the token is expired
                                 ValidateLifetime = true,
                                 ClockSkew = TimeSpan.Zero
                             };

                         });

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services.AddAuthorization();

builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "RaceDay API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter your JWT token."
        });

    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
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
                Array.Empty<string>()
            }
        });
});

// Build the application AFTER registering services
var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{ 
    app.MapOpenApi();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Temporary database connection test
app.MapGet("/db-check", async (AppDbContext db) =>
{
    bool connected = await db.Database.CanConnectAsync();

    return connected
        ? Results.Ok("Connected to RaceDay Database!")
        : Results.Problem("Could not connect to the database.");
});

app.Run();

public partial class Program { }