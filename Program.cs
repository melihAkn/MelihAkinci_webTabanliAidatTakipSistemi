using DotNetEnv;
using MelihAkıncı_webTabanliAidatTakipSistemi.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
var builder = WebApplication.CreateBuilder(args);

Env.Load();
var mysqlConnectionString = Env.GetString("MYSQL_CONNECTION_STRING");
var jwtSecret = Env.GetString("JWT_KEY");
var issuer = Env.GetString("JWT_ISSUER");
var audience = Env.GetString("JWT_AUDIENCE");
// add error handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        mysqlConnectionString,
        ServerVersion.AutoDetect(mysqlConnectionString)
    ));


// CORS ayarları
builder.Services.AddCors(options => {
    options.AddPolicy("AllowFrontend", policy => {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

/*
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            RoleClaimType = ClaimTypes.Role,
            IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

*/

builder.Services.AddAuthentication("Bearer")

    .AddJwtBearer("Bearer", options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSecret)),
            ValidateIssuer = false,
            ValidateAudience = false,
            RoleClaimType = ClaimTypes.Role
        };


        options.Events = new JwtBearerEvents {
            OnMessageReceived = context => {
                // Eğer header'dan geldiyse al
                var token = context.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();

                // Eğer header yoksa, cookie'den al
                if(string.IsNullOrEmpty(token)) {
                    token = context.Request.Cookies["accessToken"];
                }

                context.Token = token;
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();



// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddControllers()
    .AddJsonOptions(options => {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });


// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();
// using exception handler
app.UseExceptionHandler();
// Configure the HTTP request pipeline.
if(app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
