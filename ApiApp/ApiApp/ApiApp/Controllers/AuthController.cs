using ApiApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("UserAuthentication")]
        public IActionResult UserAuthentication([FromBody] LoginRequest request)
        {
            if (request.Email == "admin" && request.Password == "admin123")
            {
                var token = GenerateToken(request.Email);

                return Ok(new
                {
                    mensaje = "Autenticación correcta",
                    token = token
                });
            }

            return Unauthorized(new
            {
                mensaje = "Correo o contraseña incorrectos"
            });
        }

        private string GenerateToken(string email)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Name, "Usuario BigFOOD")
            };

            var jwtKey = _configuration["JwtSettings:Key"];

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey!)
            );

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}