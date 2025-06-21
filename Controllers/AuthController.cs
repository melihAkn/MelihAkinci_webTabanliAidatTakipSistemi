using DotNetEnv;
using MelihAkıncı_webTabanliAidatTakipSistemi.classes;
using MelihAkıncı_webTabanliAidatTakipSistemi.Data;
using MelihAkıncı_webTabanliAidatTakipSistemi.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace MelihAkıncı_webTabanliAidatTakipSistemi.Controllers {

    [Route("[controller]")]
    public class AuthController : Controller {
        private readonly AppDbContext _context;
        private readonly PasswordHash passwordHash = new PasswordHash();
        public AuthController(IConfiguration configuration, AppDbContext context) {
            _context = context;
        }
        public IActionResult Index() {
            return Content("auth coroller çalışıyor.");
        }
        [HttpPost("manager/login")]
        public async Task<IActionResult> ManagerLogin([FromBody] LoginDto dto) {
            //tum değerlerin doldurulması zorunlu
            if(string.IsNullOrWhiteSpace(dto.Username) ||
                string.IsNullOrWhiteSpace(dto.Password)) {
                return BadRequest("Zorunlu alanlar boş bırakılamaz.");
            }
            var apartmentManager = await _context.ApartmentManagers.FirstOrDefaultAsync(apartmentManager => apartmentManager.Username == dto.Username);
            if(apartmentManager is null) {
                return BadRequest("Kullanıcı adı veya şifre hatalı");
            }

            var verifyHashedPasswordForApartmentManager = passwordHash.VerifyPassword(apartmentManager.Password, dto.Password);

            if(apartmentManager != null && verifyHashedPasswordForApartmentManager == true) {
                // token'a eklemek için yöneticinin rol bilgisinin çekilmesi
                var apartmanManagerRole = await _context.UserRoles.FirstOrDefaultAsync(role => role.Id == apartmentManager.RoleId);
                if(apartmanManagerRole is null) {
                    return BadRequest("Rol bulunamadı.");
                }
                // tokenin sahip olacağı ve kontrol için eklenen değerler
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, dto.Username),
                    new Claim(ClaimTypes.Role, apartmanManagerRole.Role)
                };

                var jwtSecretKey = Env.GetString("JWT_KEY");
                var jwtIssuer = Env.GetString("JWT_ISSUER");
                var jwtAudience = Env.GetString("JWT_AUDIENCE");
                var jwtExpiresInMinutes = Env.GetString("JWT_EXPIRES_IN_MINUTES");


                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: jwtIssuer,
                    audience: jwtAudience,
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtExpiresInMinutes)),
                    signingCredentials: credentials
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
                return Ok(new { token = tokenString });
            }

            return BadRequest("kullanıcı adı veya şifre hatalı");
        }

        [HttpPost("resident/login")]
        public async Task<IActionResult> ResidentLogin([FromBody] LoginDto dto) {
            //tum değerlerin doldurulması zorunlu
            if(string.IsNullOrWhiteSpace(dto.Username) ||
                string.IsNullOrWhiteSpace(dto.Password)) {
                return BadRequest("Zorunlu alanlar boş bırakılamaz.");
            }
            var apartmentResident = await _context.ApartmentResidents.FirstOrDefaultAsync(apartmentResident => apartmentResident.Username == dto.Username);
            if(apartmentResident is null) {
                return BadRequest("Kullanıcı adı veya şifre hatalı");
            }

            var verifyHashedPasswordForApartmentManager = passwordHash.VerifyPassword(apartmentResident.Password, dto.Password);

            if(apartmentResident != null && verifyHashedPasswordForApartmentManager == true) {
                // token'a eklemek için yöneticinin rol bilgisinin çekilmesi
                var apartmentResidentRole = await _context.UserRoles.FirstOrDefaultAsync(role => role.Id == apartmentResident.RoleId);
                if(apartmentResidentRole is null) {
                    return BadRequest("Rol bulunamadı.");
                }

                // tokenin sahip olacağı ve kontrol için eklenen değerler
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, dto.Username),
                    new Claim(ClaimTypes.Role, apartmentResidentRole.Role)
                };

                var jwtSecretKey = Env.GetString("JWT_KEY");
                var jwtIssuer = Env.GetString("JWT_ISSUER");
                var jwtAudience = Env.GetString("JWT_AUDIENCE");
                var jwtExpiresInMinutes = Env.GetString("JWT_EXPIRES_IN_MINUTES");


                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: jwtIssuer,
                    audience: jwtAudience,
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtExpiresInMinutes)),
                    signingCredentials: credentials
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
                return Ok(new { token = tokenString });
                
            }

            return BadRequest("kullanıcı adı veya şifre hatalı");

        }
    }
}
