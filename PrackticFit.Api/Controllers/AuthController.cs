using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PrackticFit.Api.Data;
using PrackticFit.Api.Models;
using PrackticFit.Api.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PrackticFit.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly NutritionCalculatorService _calculatorService;

        public AuthController(AppDbContext context, NutritionCalculatorService calculatorService)
        {
            _context = context;
            _calculatorService = calculatorService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
                return BadRequest("Користувач з таким Email вже існує.");

            // У реальному додатку: хешуємо пароль
            // user.PasswordHash = HashPassword(user.PasswordHash);

            // Розраховуємо персональні цілі
            _calculatorService.CalculateUserGoals(user);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Реєстрація успішна!" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == request.Email);
            
            // Заглушка перевірки пароля (у реальності BCrypt)
            if (user == null || user.PasswordHash != request.Password)
                return Unauthorized("Невірний Email або пароль.");

            var token = GenerateJwtToken(user);
            return Ok(new { Token = token, User = user });
        }

        private string GenerateJwtToken(User user)
        {
            var key = "super-secret-key-that-should-be-very-long-and-secure";
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("id", user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
