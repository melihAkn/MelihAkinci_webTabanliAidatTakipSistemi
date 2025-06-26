using MelihAkıncı_webTabanliAidatTakipSistemi.classes;
using MelihAkıncı_webTabanliAidatTakipSistemi.Data;
using MelihAkıncı_webTabanliAidatTakipSistemi.DTOs;
using MelihAkıncı_webTabanliAidatTakipSistemi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Drawing;
using System.Net;
using System.Net.Mail;

namespace MelihAkıncı_webTabanliAidatTakipSistemi.Controllers {

    [Authorize(Roles = "ApartmentManager")]
    [ApiController]
    [Route("[controller]")]
    public class ManagerController : Controller {
        private readonly AppDbContext _context;
        private readonly PasswordHash passwordHash = new PasswordHash();
        public ManagerController(AppDbContext context) {
            _context = context;
        }

        /*
         * bilgilerini guncelleyebilme / put -------------------------------------------------- done
         * apartman ekleme / post -------------------------------------------------- done
         * apartmana daire ekleme / post -------------------------------------------------- done
         * apartman bilgilerini guncelleme(aidat dahil) / put -------------------------------------------------- done
         * yöneticisi oldugu apartmanları ve o apartman da oturan kat maliklerini gorebilme / get
         *  dairelere kat maliki ataması, kat maliki atandığı zaman kullanıcının mail adresine giriş bilgileri gönderimi / post -------------------------------------------------- done
         * istediği dairede ki kullanıcıya özel ek ücret ekleyebilme / post -------------------------------------------------- done
         
         * henuz bitmemiş olanlar         
         * apartmanda ki daireleri göruntuleme / get
         * gelen ödemeleri goruntuleyebilme / get
         * gelen ödemeleri reddetme ve onaylama ki bu durumda iade işlemi olması gerekir gibi? / post
         */

        public IActionResult Index() {
            return Ok("rota çalışıyor");
        }


        [HttpPut("updateManagerInfos")]
        public async Task<IActionResult> UpdateInfos([FromBody] ApartmentManagerDto dto) {

            //tum değerlerin doldurulması zorunlu
            if(string.IsNullOrWhiteSpace(dto.Name) ||
                string.IsNullOrWhiteSpace(dto.Surname) ||
                string.IsNullOrWhiteSpace(dto.PhoneNumber) ||
                string.IsNullOrWhiteSpace(dto.Email)) {
                return BadRequest("Zorunlu alanlar boş bırakılamaz.");
            }
            //token'dan gelen yöneticinin id değeri
            int apartmentManagerId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            var apartmentManager = await _context.ApartmentManagers.FindAsync(apartmentManagerId);
            if(apartmentManager == null) {
                return NotFound("Apartman yöneticisi bulunamadı.");
            }
            apartmentManager.Name = dto.Name;
            apartmentManager.Surname = dto.Surname;
            apartmentManager.PhoneNumber = dto.PhoneNumber;
            apartmentManager.Email = dto.Email;
            //şifre guncellemesi
            if(dto.Password != null && dto.NewPassword != null && dto.NewPasswordAgain != null) {
                if(dto.NewPassword != dto.NewPasswordAgain) {
                    return BadRequest("Yeni şifreler eşleşmiyor.");
                }
                if(!passwordHash.VerifyPassword(apartmentManager.Password, dto.Password)) {
                    return BadRequest("Eski şifre yanlış.");
                }
                apartmentManager.Password = passwordHash.HashPassword(dto.NewPassword);
            }
            await _context.SaveChangesAsync();
            return Ok("Bilgiler güncellendi.");
        }

        [HttpPost("addApartment")]
        public async Task<IActionResult> AddApartment([FromBody] ApartmentDto dto) {
            string returnText = "";
            int apartmentManagerId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            // burada password da geliyormuş bunun gelmemesi gerekiyor.
            var apartmentManager = await _context.ApartmentManagers.FindAsync(apartmentManagerId);
            if(apartmentManager == null) {
                return BadRequest("kullanıcı bulunamadı");
            }
            var apartment = new Apartment {
                Name = dto.Name,
                ManagerId = apartmentManagerId,
                MaxAmountOfResidents = dto.MaxAmountOfResidents,
                Address = dto.Address,
                MaintenanceFeeAmount = dto.MaintenanceFeeAmount,
                FloorCount = dto.FloorCount,
                ApartmentUnitCountForEachFloor = dto.ApartmentUnitCountForEachFloor,
                ApartmentManager = apartmentManager!
            };
            _context.Apartments.Add(apartment);
            await _context.SaveChangesAsync();
            returnText = "apartman başarılı şekilde eklendi. apartman dairelerini elle eklemeniz gerekecek.";
            // apartman eklendikten sonra daire ekleme işlemi eğer seçildiyse
            // apartman daire ekleme hem elle hem otomatik olacak
            // elle girme seçilirse tek tek eklemeleri gerekecek
            // otomatik olursa her kat ve daire sayısına göre otomatik eklenecek
            if(dto.IsWantedToAutoFillApartmentUnits) {
                List<ApartmentUnit> apartmentUnits = FillAllApartmentUnits(dto.FloorCount, dto.ApartmentUnitCountForEachFloor, apartment!);
                // async olarak bir fonksiyon ve geriye apartman listesi dönmeli
                // toplam kat sayısı ve her katta kaç daire oldugu gönderilecek
                // burada guncelleme işlemi lazım
                apartment.ApartmentUnits = apartmentUnits;
                await _context.SaveChangesAsync();
                returnText = "apartman ve  daireler başarılı bir şekilde eklendi.";
            }

            return Ok(returnText);
        }
        private static List<ApartmentUnit> FillAllApartmentUnits(int floorCount, int unitCount, Apartment apartment) {
            List<ApartmentUnit> apartmentUnits = new List<ApartmentUnit>();
            // iç içe for dongusu o da bilgileri doldurucak sonra guncelleme işlemi yapacak apartment tablosuna
            for(int i = 1; i <= floorCount; i++) {
                for(int k = 1; k <= unitCount; k++) {

                    apartmentUnits.Add(new ApartmentUnit {
                        ApartmentId = apartment.Id,
                        FloorNumber = i - 1,// zemin kat için 0 dan başlangıç
                        ApartmentNumber = (i * 10) + k,
                        ApartmentType = "2+1",
                        SquareMeters = 90,
                        IsOccupied = false,
                        Apartment = apartment
                    });
                }
            }
            return apartmentUnits;

        }

        [HttpPut("updateApartmentInfos")]
        //apartman bilgisi guncelleme işlemi
        public async Task<IActionResult> UpdateApartment([FromBody] ApartmentDto dto) {
            var apartment = await _context.Apartments.FindAsync(dto.ApartmentId);
            if(apartment == null) {
                return NotFound("Apartman bulunamadı.");
            }

            apartment.Name = dto.Name;
            apartment.MaxAmountOfResidents = dto.MaxAmountOfResidents;
            apartment.Address = dto.Address;
            apartment.MaintenanceFeeAmount = dto.MaintenanceFeeAmount;
            _context.Apartments.Update(apartment);
            await _context.SaveChangesAsync();
            return Ok("Apartman bilgileri güncellendi.");
        }

        // bu tek apartman dairesi ekleme için
        [HttpPost("addAnApartmentUnit")]
        public async Task<IActionResult> AddApartmentUnit([FromBody] ApartmentUnitDto dto) {
            // apartman da ki kat sayısı ve daire sayısı kontrol edilip ona göre eklenmeli
            // o değerleri aşıyorsa eklenmemeli

            var apartment = await _context.Apartments.FindAsync(dto.ApartmentId);
            if(apartment == null) {
                return NotFound("Apartman bulunamadı.");
            }

            if(dto.FloorNumber >= apartment.FloorCount || dto.ApartmentNumber > apartment.ApartmentUnitCountForEachFloor) {
                return BadRequest("Kat sayısı veya daire sayısı aşıldı.");
            }
            var apartmentUnit = new ApartmentUnit {
                ApartmentId = dto.ApartmentId,
                FloorNumber = dto.FloorNumber,
                ApartmentNumber = dto.ApartmentNumber,
                ApartmentType = dto.ApartmentType,
                SquareMeters = dto.SquareMeters,
                IsOccupied = false,
                Apartment = apartment
            };
            _context.ApartmentUnits.Add(apartmentUnit);
            await _context.SaveChangesAsync();

            return Ok("apartman dairesi başarılı şekilde eklendi");
        }
        [HttpPost("updateApartmentUnit")]
        public async Task<IActionResult> UpdateApartmentUnit([FromBody] ApartmentUnitDto dto) {
            var apartmentUnit = await _context.ApartmentUnits.FindAsync(dto.ApartmentUnitId);
            if(apartmentUnit == null) {
                return NotFound("Daire bulunamadı.");
            }
            apartmentUnit.ApartmentType = dto.ApartmentType;
            apartmentUnit.SquareMeters = dto.SquareMeters;
            _context.ApartmentUnits.Update(apartmentUnit);
            await _context.SaveChangesAsync();
            return Ok("apartman dairesi guncellendi");
        }

        [HttpPost("setApartmentResidentToAnUnit")]
        public IActionResult SetApartmentResidentToAnUnit([FromBody] setResidentToApartmentUnitDto dto) {
            // apartment resident tablosuna ekleme işlemi
            // apartment unit tablosuna da ekleme işlemi
            // apartment unit tablosunda ki isOccupied değerini true yapma işlemi

            var apartmentUnit = _context.ApartmentUnits.Find(dto.ApartmentUnitId);
            if(apartmentUnit == null) {
                return NotFound("Daire bulunamadı.");
            }
            if(apartmentUnit.IsOccupied) {
                return BadRequest("Bu daire zaten dolu.");
            }
            // oluşabilecek çakışmaların önüne geçmek için rastgele bir kaç harfle kullanıcı adı oluşturulacak
            // şimdilik sayı ekleyerek oluşturulacak
            int randomNumberForUsername = new Random().Next(1000, 9999);
            string createdUsername = $"{dto.Name.ToLower()}.{dto.Surname.ToLower()}" + randomNumberForUsername.ToString();

            // kullanıcı adı ve mail unique oldugundan dolayı kontrol edilecek
            var existingResident = _context.ApartmentResidents
                .FirstOrDefault(resident => resident.Email == dto.Email || resident.Username == createdUsername);
            if(existingResident != null) {
                return BadRequest("Bu e-posta veya kullanıcı adı zaten kullanılıyor.");
            }
            // apartment resident tablosuna ekleme işlemi
            var apartmentResidentRole = _context.UserRoles.FirstOrDefault(role => role.Role == "ApartmentResident");
            // kullanıcı bu şifreyi değiştirmeli ilk girişte ama bu şifre yeterince güçlü
            string randomlyCreatedPassword = GenerateRandomPassword(12);
            var apartmentResident = new ApartmentResident {
                Name = dto.Name,
                Surname = dto.Surname,
                PhoneNumber = dto.PhoneNumber,
                Email = dto.Email,
                ApartmentUnitId = dto.ApartmentUnitId,
                IsFirstLogin = true,
                Username = createdUsername,
                Password = passwordHash.HashPassword(randomlyCreatedPassword),
                RoleId = 3,
                UserRoles = apartmentResidentRole!,
                ApartmentUnit = apartmentUnit
            };

            // include diye birşey buldum
            _context.ApartmentUnits
            .Include(p => p.Apartment);
            //
            // veri eklendikten sonra mail adresine giriş bilgileri gönderilecek
            var apartmentManager = _context.ApartmentManagers.Find(int.Parse(User.FindFirst("id")?.Value ?? "0"));
            var emailAction = new EmailActions();
            var sendEmail = emailAction.SendEmail(apartmentManagerName: $"{apartmentManager!.Name}",
                targetMailAddress: dto.Email,
                address: apartmentUnit.Apartment.Address,
                floor: apartmentUnit.FloorNumber + 1,
                unitNumber: apartmentUnit.ApartmentNumber,
                username: createdUsername,
                password: randomlyCreatedPassword);
            if(!sendEmail) {
                return BadRequest("E-posta gönderilirken bir hata oluştu. verileri tekrar eklemeniz gerekiyor");
            }
            // MelihAkıncı_webTabanliAidatTakipSistemi.Models.ApartmentUnit.Apartments.get returned null.
            _context.ApartmentResidents.Add(apartmentResident);
            apartmentUnit.IsOccupied = true;
            _context.ApartmentUnits.Update(apartmentUnit);
            _context.SaveChanges();
            return Ok("Kat maliki başarılı bir şekilde eklendi.");
        }
        public static string GenerateRandomPassword(int length = 12) {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()_+[]{}|;:,.<>?";
            Random random = new Random();
            char[] chars = new char[length];
            for(int i = 0; i < length; i++) {
                chars[i] = validChars[random.Next(validChars.Length)];
            }
            return new string(chars);

        }

        [HttpPost("setSpecificDebtToApartmentResident")]
        public async Task<IActionResult> SetSpecificDebtToAnResident([FromBody] SetResidentSpecificDebtDto dto) {
            var apartmentResident = _context.ApartmentResidents.Find(dto.ResidentId);
            if(apartmentResident == null) {
                return BadRequest("kat maliki bulunamadı");
            }
            var residentSpecificDebt = new ResidentsSpecificDebt {
                Name = dto.Name,// borç adı
                Description = dto.Description,
                Price = dto.Price,
                ResidentId = apartmentResident!.Id,
                ApartmentResident = apartmentResident!
            };

            _context.ResidentsSpecificDebts.Add(residentSpecificDebt);
            await _context.SaveChangesAsync();
            return Ok("kat malikine özel borç eklendi");
        }




    }
}
