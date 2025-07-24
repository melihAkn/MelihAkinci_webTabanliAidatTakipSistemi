using DotNetEnv;
using MelihAkıncı_webTabanliAidatTakipSistemi.classes;
using MelihAkıncı_webTabanliAidatTakipSistemi.Data;
using MelihAkıncı_webTabanliAidatTakipSistemi.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;


namespace MelihAkıncı_webTabanliAidatTakipSistemi.Controllers {

    [Route("[controller]")]
    public class AuthController : Controller {
        private readonly AppDbContext _context;
        private readonly PasswordHash passwordHash = new PasswordHash();
        private readonly JwtToken jwtToken = new JwtToken();
        private readonly SanitizeAndValidate sanitizeAndValidate = new SanitizeAndValidate();
        private readonly EmailActions emailAction = new EmailActions();
        public AuthController(AppDbContext context) {
            _context = context;
        }
        [HttpPost("manager/login")]
        public async Task<IActionResult> ManagerLogin([FromBody] LoginDto dto) {
            //tum değerlerin doldurulması zorunlu
            if(string.IsNullOrWhiteSpace(dto.Username) ||
                string.IsNullOrWhiteSpace(dto.Password)) {
                throw new ArgumentException("Zorunlu alanlar boş bırakılamaz.");
            }
            var apartmentManager = await _context.ApartmentManagers.FirstOrDefaultAsync(apartmentManager => apartmentManager.Username == dto.Username);
            if(apartmentManager is null) {
                throw new ArgumentException("Kullanıcı adı veya şifre hatalı");
            }

            var verifyHashedPasswordForApartmentManager = passwordHash.VerifyPassword(apartmentManager.Password, dto.Password);
            if(verifyHashedPasswordForApartmentManager == false) {
                throw new ArgumentException("Kullanıcı adı veya şifre hatalı");
            }
            if(apartmentManager != null) {
                // apartmentMananagerToken'a eklemek için yöneticinin rol bilgisinin çekilmesi
                var apartmanManagerRole = await _context.UserRoles.FirstOrDefaultAsync(role => role.Id == apartmentManager.RoleId);
                if(apartmanManagerRole is null) {
                    throw new ArgumentException("Rol bulunamadı.");
                }

                string apartmentMananagerToken = jwtToken.GenerateJwtToken(dto.Username, apartmanManagerRole.Role, apartmentManager.Id);
                Response.Cookies.Append("accessToken", apartmentMananagerToken, new CookieOptions {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict,
                });
            }
            return Ok(new SuccessResult {
                Message = "giriş başarılı"
            });
        }

        [HttpPost("resident/login")]
        public async Task<IActionResult> ResidentLogin([FromBody] LoginDto dto) {
            //tum değerlerin doldurulması zorunlu
            if(string.IsNullOrWhiteSpace(dto.Username) ||
                string.IsNullOrWhiteSpace(dto.Password)) {
                throw new ArgumentException("Zorunlu alanlar boş bırakılamaz.");
            }
            var apartmentResident = await _context.ApartmentResidents.FirstOrDefaultAsync(apartmentResident => apartmentResident.Username == dto.Username);
            if(apartmentResident is null) {
                throw new ArgumentException("Kullanıcı adı veya şifre hatalı");
            }

            var verifyHashedPasswordForApartmentManager = passwordHash.VerifyPassword(apartmentResident.Password, dto.Password);

            if(apartmentResident != null && verifyHashedPasswordForApartmentManager == true) {
                // apartmentMananagerToken'a eklemek için kat malikinin rol bilgisinin çekilmesi
                var apartmentResidentRole = await _context.UserRoles.FirstOrDefaultAsync(role => role.Id == apartmentResident.RoleId);
                if(apartmentResidentRole is null) {
                    throw new ArgumentException("Rol bulunamadı.");
                }
                
                string apartmentResidentToken = jwtToken.GenerateJwtToken(dto.Username, apartmentResidentRole.Role, apartmentResident.Id);
                Response.Cookies.Append("accessToken", apartmentResidentToken, new CookieOptions {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict
                });
                Console.WriteLine(apartmentResidentToken);

                // async gerekebilir

                //return Ok(new { apartmentMananagerToken = apartmentResidentToken });
            }
            return Ok(new SuccessResult {
                Message = "giriş başarılı"
            });
        }
        [HttpGet("manager/logout")]
        public IActionResult ManagerLogout() {
            // tokenin son geçerlilik tarihini ayarlayarak logout işlemi
            Response.Cookies.Append("accessToken", "", new CookieOptions {
                Expires = DateTimeOffset.UtcNow.AddDays(-1),
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                Path = "/"
            });
            return Ok("başarılı bir şekilde çıkış yapıldı");
        }
        [HttpGet("resident/logout")]
        public IActionResult ResidentLogout() {

            Response.Cookies.Append("accessToken", "", new CookieOptions {
                Expires = DateTimeOffset.UtcNow.AddDays(-1),
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                Path = "/"
            });
            return Ok("başarılı bir şekilde çıkış yapıldı");
        }

        [HttpPost("manager-forgot-password")]
        public async Task<IActionResult> ManagerForgotPassword([FromBody] ForgotPasswordDto dto) {
            // mail gönderme işlemi olacak yani burada bir token oluşturucalacak client tan gelen mail adresi ile 
            // sonra bu token ile birlikte mail gönderilecek
            var apartmentManager = await _context.ApartmentManagers
                .Include(x => x.UserRoles)
                .FirstOrDefaultAsync(x => x.Email == dto.Email);
            if(apartmentManager == null) {
                throw new ArgumentException("Bu e-posta adresi ile kayıtlı bir kullanıcı bulunamadı.");
            }
            var passwordResetToken = jwtToken.GenerateJwtToken(apartmentManager.Username, apartmentManager.UserRoles.Role, apartmentManager.Id, 15);
            // dto.Username, apartmentResidentRole.Role, apartmentResident.I
            string mailSubject = "Şifre Sıfırlama Talebi";
            string mailBody = $"""
                Merhaba
                    şifrenizi sıfırlamak için lütfen aşağıdaki bağlantıya tıklayın:
                    
                    <a>http://localhost:5263/ManagerResetPassword?token={passwordResetToken}</a>
                    

                    bu link 15 dakika boyunca geçerli olacaktır.
                """;

            emailAction.SendEmail(dto.Email, mailSubject, mailBody);


            return Ok(new SuccessResult { Message = "Şifre sıfırlama işlemi başarılı" });
        }
        [HttpPost("manager-reset-password")]
        [Authorize(Roles = "ApartmentManager")]
        public async Task<IActionResult> MananagerResetPassword([FromBody] ResetPasswordDto dto) {
            // clienta gönderilmiş olan token doğrulaması yapılacak ve client tan gelen şifre ile kullanıcının şifresi guncellenecek
            if(dto.Password == null) {
                throw new ArgumentException("şifre boş olamaz");
            }
            sanitizeAndValidate.IsValidPassword(dto.Password);
            int apartmentManagerId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            var apartmentManager = _context.ApartmentManagers.FirstOrDefault(x => x.Id == apartmentManagerId);
            if(apartmentManager == null) {
                throw new ArgumentException("Kullanıcı bulunamadı.");
            }
            string hashedPassword = passwordHash.HashPassword(dto.Password);
            apartmentManager.Password = hashedPassword;
            await _context.SaveChangesAsync();
            return Ok(new SuccessResult { Message = "Şifre sıfırlama işlemi başarılı" });
        }


        [HttpPost("resident-forgot-password")]
        public async Task<IActionResult> ResidentForgotPassword([FromBody] ForgotPasswordDto dto) {
            // mail gönderme işlemi olacak yani burada bir token oluşturucalacak client tan gelen mail adresi ile 
            // sonra bu token ile birlikte mail gönderilecek
            var apartmentResident = await _context.ApartmentManagers
                .Include(x => x.UserRoles)
                .FirstOrDefaultAsync(x => x.Email == dto.Email);
            if(apartmentResident == null) {
                throw new ArgumentException("Bu e-posta adresi ile kayıtlı bir kullanıcı bulunamadı.");
            }
            var passwordResetToken = jwtToken.GenerateJwtToken(apartmentResident.Username, apartmentResident.UserRoles.Role, apartmentResident.Id, 15);
            string mailSubject = "Şifre Sıfırlama Talebi";
            // bu link henüz çalışmıyor bu frontend'in sorunu sanırım?
            string mailBody = $"""
                Merhaba
                    şifrenizi sıfırlamak için lütfen aşağıdaki bağlantıya tıklayın:
                    
                    <a>http://localhost:5263/ManagerResetPassword?token={passwordResetToken}</a>
                    

                    bu link 15 dakika boyunca geçerli olacaktır.
                """;

            emailAction.SendEmail(dto.Email, mailSubject, mailBody);


            return Ok(new SuccessResult { Message = "Şifre sıfırlama işlemi başarılı" });
        }
        [HttpPost("resident-resetpassword")]
        [Authorize(Roles = "ApartmentResident")]
        public async Task<IActionResult> ResidentResetPassword([FromBody] ResetPasswordDto dto) {
            // clienta gönderilmiş olan token doğrulaması yapılacak ve client tan gelen şifre ile kullanıcının şifresi guncellenecek
            if(dto.Password == null) {
                throw new ArgumentException("şifre boş olamaz");
            }
            sanitizeAndValidate.IsValidPassword(dto.Password);
            int ApartmentResidentId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            var ApartmentResident = _context.ApartmentResidents.FirstOrDefault(x => x.Id == ApartmentResidentId);
            if(ApartmentResident == null) {
                throw new ArgumentException("Kullanıcı bulunamadı.");
            }
            string hashedPassword = passwordHash.HashPassword(dto.Password);
            ApartmentResident.Password = hashedPassword;
            await _context.SaveChangesAsync();
            return Ok(new SuccessResult { Message = "Şifre sıfırlama işlemi başarılı" });

        }
    }
}
