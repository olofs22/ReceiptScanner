using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ReceiptProject1.Data;
using ReceiptProject1.Services;
using System.Text;
using ReceiptProject1.Middleware;
using FluentValidation.AspNetCore;
using FluentValidation;
using ReceiptProject1.Validators;
using ReceiptProject1.DTOs.ReceiptDTOs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT Key is not configured.");

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("http://192.168.1.185:5174")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});
builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddScoped<IValidator<CreateReceiptDTO>, CreateReceiptValidator>();
builder.Services.AddScoped<IValidator<UpdateReceiptDTO>, UpdateReceiptValidator>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ReceiptService>();


var app = builder.Build();
app.UseCors("AllowAll");
app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
