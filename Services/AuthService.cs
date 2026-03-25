using Microsoft.IdentityModel.Tokens;
using ReceiptProject1.Data;
using ReceiptProject1.Models;
using ReceiptProject1.DTOs.AuthDTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;

namespace ReceiptProject1.Services
{
    public class AuthService
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;

        public AuthService(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }
        public async Task<bool> Register(RegisterDTO rdto)
        {
            if ( await _db.Users.AnyAsync(u => u.EmailAdress == rdto.EmailAdress))
                return false;

            var user = new User
            {
                EmailAdress = rdto.EmailAdress,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(rdto.Password),
                CreatedAt = DateTime.UtcNow
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<string?> Login(LoginDTO ldto)
        {
            var user = _db.Users.SingleOrDefault(u => u.EmailAdress == ldto.EmailAdress);
            if (user == null || !BCrypt.Net.BCrypt.Verify(ldto.Password, user.PasswordHash))
                return null;

            return GenerateToken(user);
        }
        private string GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.EmailAdress)
            };
            var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
