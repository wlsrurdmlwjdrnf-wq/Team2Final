using Microsoft.EntityFrameworkCore;
using TeamProjectServer.Models;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using TeamProjectServer.Data;
using TeamProjectServer.Services;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Npgsql;


var builder = WebApplication.CreateBuilder(args);

//appsetting.json 연결 문자열을 읽어서 AppDbContext 설정(DB 연결설정)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
dataSourceBuilder.EnableDynamicJson();
var dataSource = dataSourceBuilder.Build();

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(dataSource));

// jwt 인증 토큰
var jwtSetting = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSetting["Key"];
var key = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSetting["Issuer"],
        ValidAudience = jwtSetting["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});



// Add services to the container.
builder.Services.AddScoped<JwtService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//===========================================================================================================
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
DataManager.Initialize();



app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();



app.MapControllers();
app.Run();
