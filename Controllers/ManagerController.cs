using MelihAkıncı_webTabanliAidatTakipSistemi.classes;
using MelihAkıncı_webTabanliAidatTakipSistemi.Data;
using MelihAkıncı_webTabanliAidatTakipSistemi.DTOs;
using MelihAkıncı_webTabanliAidatTakipSistemi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Drawing;

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
         * bilgilerini guncelleyebilme / put
         * yöneticisi oldugu apartmanları ve o apartman da oturan kat maliklerini gorebilme / get
         * apartman ekleme / post
         * apartmana daire ekleme / post
         * apartman bilgilerini guncelleme / put
         * apartmana aidat bilgisi ekleyebilmesi ve guncelleyebilmesi / put
         * apartmanda ki daireleri göruntuleme ve dairelere kat maliki ataması, kat maliki atandığı zaman kullanıcının mail adresine giriş bilgileri gönderimi / post
         * istediği daireye özel ek ücret ekleyebilme / post
         * gelen ödemeleri goruntuleyebilme / get
         * gelen ödemeleri reddetme ve onaylama ki bu durumda iade işlemi olması gerekir gibi? / post
         */
        /*
         * bugun yapılacak 
         * dünün planı: bilgilerini guncelleyebilme / put done
         * apartman ekleme / post
         * apartmana daire ekleme / post
         * apartman bilgilerini guncelleme / put
         * apartmana aidat bilgisi ekleyebilmesi ve guncelleyebilmesi / put
         * 
         * 
         * 
         * bugunun planı
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
            var apartment = new Apartments {
                Name = dto.Name,
                ManagerId = apartmentManagerId,
                MaxAmountOfResidents = dto.MaxAmountOfResidents,
                Address = dto.Address,
                ApartmentManager = apartmentManager!,
            };
            _context.Apartments.Add(apartment);
            await _context.SaveChangesAsync();
            returnText = "apartman başarılı şekilde eklendi. apartman dairelerini elle eklemeniz gerekecek.";
            // apartman eklendikten sonra daire ekleme işlemi eğer seçildiyse
            // apartman daire ekleme hem elle hem otomatik olacak
            // elle girme seçilirse tek tek eklemeleri gerekecek
            // otomatik olursa her kat ve daire sayısına göre otomatik eklenecek
            if(dto.IsWantedToAutoFillApartmentUnits) {
                List<ApartmentUnit> apartmentUnits = FillAllApartmentUnits(dto.FloorCount, dto.ApartmentUnitCountForEachFloor,apartment);
                // async olarak bir fonksiyon ve geriye apartman listesi dönmeli
                // toplam kat sayısı ve her katta kaç daire oldugu gönderilecek
                // burada guncelleme işlemi lazım
                apartment.ApartmentUnits = apartmentUnits;
                await _context.SaveChangesAsync();
                returnText = "apartman ve  daireler başarılı bir şekilde eklendi.";
            }

            return Ok(returnText);
        }
        private static List<ApartmentUnit> FillAllApartmentUnits(int floorCount,int unitCount,Apartments apartment) {
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
                        Apartments = apartment
                    });
                }
            }
            return apartmentUnits;


        }


        //bu fonksiyon olabilir gibi çünkü apartman eklendiği zaman maxamount alanına göre apartman ekleyebiliriz tabi boş olarak sonra da ek rota olarak kullanıcı eklemesi yaparız
        // bu tek apartman dairesi ekleme için
        public async Task<IActionResult> AddApartmentUnit([FromBody] ApartmentUnitDto dto) {




            return Ok();
        }

    }
}
