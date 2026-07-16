using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nozl;
using System.Text.RegularExpressions;
using Nozl.Model;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Nozl.DTOs.AuthDtos;


namespace Nozl.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly NozlDbContext _dbcontext;
        private readonly IConfiguration _configuration;
        public AuthController(NozlDbContext dbcontext, IConfiguration configuration)
        {
            _configuration = configuration;
            _dbcontext = dbcontext;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (_dbcontext.Users.Any(u => u.Email == dto.Email))
                return BadRequest(new { message = "Email already exists" });

            var user = new User
            {
                FullName = dto.Name,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Phone = dto.Phone,
                Role = dto.Role
            };
            _dbcontext.Users.Add(user);
            await _dbcontext.SaveChangesAsync();
            return Ok(new { message = "User registered successfully", userId = user.Id });



        }



        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            var user = _dbcontext.Users.FirstOrDefault(u => u.Email == dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized(new { message = "Invalid email or password" });
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = System.Text.Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Role, user.Role)
    }),

                Expires = DateTime.UtcNow.AddDays(7),

                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],

                SigningCredentials = new SigningCredentials(
        new SymmetricSecurityKey(key),
        SecurityAlgorithms.HmacSha256Signature
    )
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return Ok(new { Token = tokenString });
        }
    }
}


