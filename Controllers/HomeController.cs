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
        public HomeController(AppDbContext context) {
            _context = context;
        }
        [HttpGet]
        public IActionResult Index() {
            // Şimdilik sadece test amaçlı
            return Content("HomeController çalışıyor.");
        }
        // tum kullanıcıların sisteme girş yapabilmesi için
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto) {
            //bunu context gibi global bir değişken olarak tanımlayabiliriz
            var service = new PasswordHash();
            //tum değerlerin doldurulması zorunlu
            if(string.IsNullOrWhiteSpace(dto.Username) ||
                string.IsNullOrWhiteSpace(dto.Password)) {
                return BadRequest("Zorunlu alanlar boş bırakılamaz.");
            }
            //apartman yöneticisi giriş işlemleri
            var apartmentManager = await _context.ApartmentManagers.FirstOrDefaultAsync(apartmentManager => apartmentManager.Username == dto.Username);
            if(apartmentManager is null) {
                return BadRequest("Kullanıcı adı veya şifre hatalı");
            }
            
            var verifyHashedPasswordForApartmentManager = service.VerifyPassword(apartmentManager.Password,dto.Password);
            Console.WriteLine(verifyHashedPasswordForApartmentManager);

            if(apartmentManager != null) {
                //token işlemleri burada


                return Ok("apartman yöneticisi girişi");
            }

            // apartman sakini giriş işlemleri
            var apartmentResident = await _context.ApartmentResidents.FirstOrDefaultAsync(apartmentResident => apartmentResident.Username == dto.Username);
            if(apartmentResident is null) {
                return BadRequest("Kullanıcı adı veya şifre hatalı");
            }
            var verifyHashedPasswordForApartmentResident = service.VerifyPassword(apartmentResident.Password, dto.Password);
            Console.WriteLine(verifyHashedPasswordForApartmentResident);
            if(apartmentResident != null) {
                //token işlemleri burada



                return Ok("apartman sahibi girişi");
            }
            
       
            

            return BadRequest("Kullanıcı adı veya şifre hatalı");
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


            //kullanıcı adı ve email eşşiz olmalı

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
