using DotNetEnv;
using MelihAkıncı_webTabanliAidatTakipSistemi.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
var builder = WebApplication.CreateBuilder(args);

Env.Load();
var mysqlConnectionString = Env.GetString("MYSQL_CONNECTION_STRING");
var jwtSecret = Env.GetString("JWT_KEY");
var issuer = Env.GetString("JWT_ISSUER");
var audience = Env.GetString("JWT_AUDIENCE");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        mysqlConnectionString,
        ServerVersion.AutoDetect(mysqlConnectionString)
    ));

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSecret))

        };
    });

builder.Services.AddAuthorization();



// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if(app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
