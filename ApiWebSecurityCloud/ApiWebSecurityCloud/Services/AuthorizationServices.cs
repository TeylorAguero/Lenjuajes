using ApiWebSecurityCloud.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiWebSecurityCloud.Services
{
    public class AuthorizationServices : IAuthorizationServices
    {
        private readonly DbContextSecurity _context = null;
        private readonly IConfiguration _configuration = null;

        public AuthorizationServices(IConfiguration pConfiguration, DbContextSecurity pContext)
        {
            this._configuration = pConfiguration;
            this._context = pContext;
        }

        public AuthorizationResponse DevolverToken(LoginRequest authorization)
        {
            // Usar los nombres exactos de la BD: loginn, passwordd, estado
            Usuario temp = this._context.Usuarios.FirstOrDefault(
                u => u.loginn.Equals(authorization.loginn) &&
                     u.passwordd.Equals(authorization.passwordd) &&
                     u.estado == true);

            string token = "";

            if (temp != null)
            {
                token = this.GenerarToken(temp);

                return new AuthorizationResponse()
                {
                    Token = token,
                    Result = true,
                    Msj = "Autenticación exitosa"
                };
            }
            else
            {
                return new AuthorizationResponse()
                {
                    Token = "",
                    Result = false,
                    Msj = "Usuario o contraseña incorrectos, o usuario inactivo"
                };
            }
        }

        private string GenerarToken(Usuario usuario)
        {
            string tokenCreado = "";

            var key = this._configuration.GetValue<string>("JwtSettings:Key");
            var issuer = this._configuration.GetValue<string>("JwtSettings:Issuer");
            var audience = this._configuration.GetValue<string>("JwtSettings:Audience");
            var minutes = this._configuration.GetValue<int>("JwtSettings:MinutesToExpiration", 60);

            var keyBytes = Encoding.ASCII.GetBytes(key);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.loginn),
                new Claim("Login", usuario.loginn),
                new Claim("UsuarioId", usuario.Id.ToString())
            };

            var credencialesToken = new SigningCredentials(
                new SymmetricSecurityKey(keyBytes),
                SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(minutes),
                SigningCredentials = credencialesToken,
                Issuer = issuer,
                Audience = audience
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);
            tokenCreado = tokenHandler.WriteToken(tokenConfig);

            return tokenCreado;
        }
    }
}
