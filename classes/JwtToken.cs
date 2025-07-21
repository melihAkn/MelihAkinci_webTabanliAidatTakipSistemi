using DotNetEnv;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MelihAkıncı_webTabanliAidatTakipSistemi.classes {
    public class JwtToken {
        public string GenerateJwtToken(string username, string role, int id, int expireMinute = 0) {
            // apartmentMananagerTokenin sahip olacağı ve kontrol için eklenen değerler
            var claims = new[]
              {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, role),
                    new Claim("id",id.ToString()),
                };
            var jwtSecretKey = Env.GetString("JWT_KEY");
            var jwtIssuer = Env.GetString("JWT_ISSUER");
            var jwtAudience = Env.GetString("JWT_AUDIENCE");
            var jwtExpiresInMinutes = Env.GetDouble("JWT_EXPIRES_MINUTES");


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            double expirationMinutes = expireMinute > 0
            ? expireMinute
            : jwtExpiresInMinutes;

            var apartmentMananagerToken = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),//240.0
                signingCredentials: credentials

            );

            return new JwtSecurityTokenHandler().WriteToken(apartmentMananagerToken);
        }
    }
}
