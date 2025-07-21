using MelihAkıncı_webTabanliAidatTakipSistemi.classes;
using MelihAkıncı_webTabanliAidatTakipSistemi.Data;
using MelihAkıncı_webTabanliAidatTakipSistemi.DTOs;
using MelihAkıncı_webTabanliAidatTakipSistemi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace MelihAkıncı_webTabanliAidatTakipSistemi.Controllers {
    [Route("[controller]")]
    public class HomeController : Controller {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;


        public HomeController(IConfiguration configuration, AppDbContext context) {
            _configuration = configuration;
            _context = context;
        }
        //sadece apartman yoneticileri sisteme kayıt olabilir
        //kullanıcılar için yonetici o daireye kullanıcı eklediği zaman e posta ile kullanıcı adı ve şifre bilgileri gonderilecek ve ilk giriş yaptığında şifresini ve kullanıcı bilgilerini güncellemesi istenecek
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto) {

            var role = await _context.UserRoles.FirstOrDefaultAsync(rol => rol.Id == 2);
            if(role is null) {
                return BadRequest("Rol bulunamadı.");
            }
            // mail adresi geçerli mi kontrol etmeliyiz

            //tum değerlerin doldurulması zorunlu
            if(string.IsNullOrWhiteSpace(dto.Name) ||
                string.IsNullOrWhiteSpace(dto.Surname) ||
                string.IsNullOrWhiteSpace(dto.PhoneNumber) ||
                string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Username) ||
                string.IsNullOrWhiteSpace(dto.Password)) {
                return BadRequest("Zorunlu alanlar boş bırakılamaz.");
            }

            // değerlerin veritabanına eklenmeden önce trim ile başında ki ve sonunda ki boşlukların silinmesi
            dto.Name = dto.Name.Trim();
            dto.Surname = dto.Surname.Trim();
            dto.PhoneNumber = dto.PhoneNumber.Trim();
            dto.Email = dto.Email.Trim();
            dto.Username = dto.Username.Trim();
            dto.Password = dto.Password.Trim();
            dto.Email = dto.Email.Trim();
            dto.Username = dto.Username.Trim();
            dto.Password = dto.Password.Trim();

            //ve en az bir büyük harf, bir küçük harf, bir rakam ve bir özel karakter içermeli
            // şifre en az 8 karakter olmalı 
            if(dto.Password.Length < 8) {
                return BadRequest("şifre en az 8 karakter olmalı");
            }
            //password hash işlemi
            var service = new PasswordHash();
            var hashedPassword = service.HashPassword(dto.Password);

            var apartmentManager = new ApartmentManager {
                Name = dto.Name!,
                Surname = dto.Surname!,
                PhoneNumber = dto.PhoneNumber!,
                Email = dto.Email!,
                Username = dto.Username!,
                Password = hashedPassword,
                RoleId = 2,
                UserRoles = role
            };

            _context.ApartmentManagers.Add(apartmentManager);
            await _context.SaveChangesAsync();

            return Content("Kayıt Başarılı!");
        }





    }
}
