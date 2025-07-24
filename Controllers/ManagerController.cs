using MelihAkıncı_webTabanliAidatTakipSistemi.classes;
using MelihAkıncı_webTabanliAidatTakipSistemi.Data;
using MelihAkıncı_webTabanliAidatTakipSistemi.DTOs;
using MelihAkıncı_webTabanliAidatTakipSistemi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using static MelihAkıncı_webTabanliAidatTakipSistemi.classes.PaymentStatusEnum;

namespace MelihAkıncı_webTabanliAidatTakipSistemi.Controllers {
    [Authorize(Roles = "ApartmentManager")]
    [ApiController]
    [Route("[controller]")]
    public class ManagerController(AppDbContext context) : Controller {
        private readonly AppDbContext _context = context;
        private readonly PasswordHash passwordHash = new PasswordHash();
        private readonly SanitizeAndValidate sanitizeAndValidate = new SanitizeAndValidate();
        private readonly EmailActions emailAction = new EmailActions();
        private readonly GetNthMonth28Day getNthMonth28Day = new GetNthMonth28Day();
        /*
         * bilgilerini guncelleyebilme / put -------------------------------------------------- done
         * apartman ekleme / post -------------------------------------------------- done
         * apartmana daire ekleme / post -------------------------------------------------- done
         * apartman bilgilerini guncelleme(aidat dahil) / put -------------------------------------------------- done
         * yöneticisi oldugu apartmanları ve o apartman da oturan kat maliklerini gorebilme / get
         *  dairelere kat maliki ataması, kat maliki atandığı zaman kullanıcının mail adresine giriş bilgileri gönderimi / post -------------------------------------------------- done
         * istediği dairede ki kullanıcıya özel ek ücret ekleyebilme / post -------------------------------------------------- done
         * apartmanda ki daireleri göruntuleme / get -------------------------------------------------- done
         * 
         * gelen ödemeleri goruntuleyebilme / get -------------------------------------------------- done
         * gelen ödemeleri reddetme ve onaylama / post -------------------------------------------------- done
         */


        [HttpGet("get-user-role")]
        public IActionResult GetUserRole() {
            // yöneticinin bilgilerini getirme işlemi
            var token = Request.Cookies["accessToken"];
            Console.WriteLine(token);
            int apartmentManagerId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            var apartmentManager = _context.ApartmentManagers
                .Include(e => e.UserRoles)
                .FirstOrDefault();
            Console.WriteLine(apartmentManagerId);
            if(apartmentManager == null) {
                throw new ArgumentException("apartman sakini bulunamadı");
            }
            // apartman sakininin rol bilgisini de döndürmek lazım ki frontend de apartman sakininin rolüne göre yönlendirme yapabilsin
            return Ok(new returnRoleIdforNavigationsDto {
                userRole = apartmentManager.UserRoles.Role
            });
        }

        [HttpGet("get-user-infos")]
        public async Task<IActionResult> GetUserInfos() {
            int apartmentManagerId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            var apartmentManager = await _context.ApartmentManagers.FindAsync(apartmentManagerId);
            if(apartmentManager == null) {
                throw new ArgumentException("apartman yöneticisi bulunamadı.");
            }
            var apartmanManagerInfos = new ApartmentManagerDto {
                Name = apartmentManager.Name,
                Surname = apartmentManager.Surname,
                PhoneNumber = apartmentManager.PhoneNumber,
                Email = apartmentManager.Email
            };

            return Ok(apartmanManagerInfos);
        }

        [HttpGet("get-apartments")]
        public async Task<IActionResult> GetApartments() {
            int apartmentManagerId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            var managerApartments = await _context.Apartments
            .Where(x => x.ManagerId == apartmentManagerId)
            .Select(apartment => new ApartmentDto {
                ApartmentId = apartment.Id,
                Name = apartment.Name,
                MaxAmountOfResidents = apartment.MaxAmountOfResidents,
                Address = apartment.Address,
                MaintenanceFeeAmount = apartment.MaintenanceFeeAmount,
                FloorCount = apartment.FloorCount,
                Iban = apartment.Iban,
                ApartmentUnitCountForEachFloor = apartment.ApartmentUnitCountForEachFloor,
            })
            .ToListAsync();
            if(managerApartments.Count == 0) {
                throw new ArgumentException("apartman bulunamadı ve ya yöneticisi olduğunuz apartman yok");


            }
            return Ok(managerApartments);
        }
        // yöneticinin apartmanına ait daireleri ve o dairelerde ki kat maliklerini getirme işlemi
        [HttpPost("get-apartment-units")]
        public async Task<IActionResult> GetApartmentUnitWithResidentsOrWithout([FromBody] ApartmentIdDto dto) {
            // client'dan gelen apartman id değerinin input validator sınıfı ile belirlenen aralıkta olup olmadıgının kontrolü varsayılan olarak 1 ile 1 milyon arasında
            // geriye false değer dönmez o yuzden ek bir kontrole gerek yok gibi?
            bool validated = sanitizeAndValidate.IsValidNumber(dto.ApartmentId);

            // yöneticinin apartmanına ait kat maliklerini getirme işlemi
            // burada custom bir list ya da dto lazım gibi çünkü aparmant dairelerini
            var apartmentUnits = await _context.ApartmentUnits.Where(x => x.ApartmentId == dto.ApartmentId).ToListAsync();
            List<apartmentUnitsWithResidents> apartmentUnitsWithResidents = new List<apartmentUnitsWithResidents>();
            // bize ek bilgi gerekeceği için yani koşula göre ek bilgi gerekeceği için
            // foreach ile her daireyi listeye ekleyeceğiz
            // verimli bir yaklaşım olmayabilir
            foreach(var unit in apartmentUnits) {
                if(unit.IsOccupied) {
                    var resident = await _context.ApartmentResidents
                        .Where(x => x.ApartmentUnitId == unit.Id)
                        .Select(resident => new ApartmentResidentDto {
                            Id = resident.Id,
                            Name = resident.Name,
                            Surname = resident.Surname,
                            PhoneNumber = resident.PhoneNumber,
                            Email = resident.Email,
                            ApartmentUnitId = resident.ApartmentUnitId
                        })
                        .ToListAsync();
                    apartmentUnitsWithResidents.Add(new apartmentUnitsWithResidents {
                        ApartmentUnitId = unit.Id,
                        FloorNumber = unit.FloorNumber,
                        ApartmentNumber = unit.ApartmentNumber,
                        ApartmentType = unit.ApartmentType,
                        SquareMeters = unit.SquareMeters,
                        IsHaveResident = unit.IsOccupied,
                        ApartmentResidents = resident
                    });
                }
                else {
                    // eğer daire boş ise o daireye ait kat maliki yoktur
                    // o yüzden isHaveResident false olacak
                    apartmentUnitsWithResidents.Add(new apartmentUnitsWithResidents {
                        ApartmentUnitId = unit.Id,
                        FloorNumber = unit.FloorNumber,
                        ApartmentNumber = unit.ApartmentNumber,
                        ApartmentType = unit.ApartmentType,
                        SquareMeters = unit.SquareMeters,
                        IsHaveResident = unit.IsOccupied

                    });
                }
            }
            return Ok(apartmentUnitsWithResidents);
        }

        [HttpPut("update-manager-infos")]
        public async Task<IActionResult> UpdateInfos([FromBody] ApartmentManagerDto dto) {
            // dto deperleri için input validator
            sanitizeAndValidate.IsValidText(dto.Name);
            sanitizeAndValidate.IsValidText(dto.Surname);
            sanitizeAndValidate.IsValidPhoneNumber(dto.PhoneNumber);
            sanitizeAndValidate.IsValidEmail(dto.Email);
            sanitizeAndValidate.IsValidPassword(dto.Password!);

            //apartmentMananagerToken'dan gelen yöneticinin id değeri
            int apartmentManagerId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            var apartmentManager = await _context.ApartmentManagers.FindAsync(apartmentManagerId);
            if(apartmentManager == null) {
                throw new ArgumentException("Apartman yöneticisi bulunamadı.");
            }
            apartmentManager.Name = dto.Name;
            apartmentManager.Surname = dto.Surname;
            apartmentManager.PhoneNumber = dto.PhoneNumber;
            apartmentManager.Email = dto.Email;

            if(!passwordHash.VerifyPassword(apartmentManager.Password, dto.Password!)) {
                throw new ArgumentException("Eski şifre yanlış.");
            }
            //şifre guncellemesi
            if(dto.NewPassword != null) {

                apartmentManager.Password = passwordHash.HashPassword(dto.NewPassword);
            }
            await _context.SaveChangesAsync();
            return Ok(new SuccessResult {
                Message = "Yönetici bilgileri güncellendi."
            });
        }

        [HttpPost("add-apartment")]
        public async Task<IActionResult> AddApartment([FromBody] ApartmentDto dto) {

            // tum dto elemanları için input validator
            sanitizeAndValidate.IsValidNumber(dto.ApartmentId);
            sanitizeAndValidate.IsValidText(dto.Name);
            sanitizeAndValidate.IsValidNumber(dto.MaxAmountOfResidents);
            sanitizeAndValidate.IsValidText(dto.Address);
            sanitizeAndValidate.IsValidDecimal(dto.MaintenanceFeeAmount);
            sanitizeAndValidate.IsValidNumber(dto.FloorCount);
            sanitizeAndValidate.IsValidNumber(dto.ApartmentUnitCountForEachFloor);

            SuccessResult result = new SuccessResult();
            int apartmentManagerId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            // burada password da geliyormuş bunun gelmemesi gerekiyor.
            var apartmentManager = await _context.ApartmentManagers.FindAsync(apartmentManagerId);
            if(apartmentManager == null) {
                throw new ArgumentException("kullanıcı bulunamadı");
            }
            var apartment = new Apartment {
                Name = dto.Name,
                ManagerId = apartmentManagerId,
                MaxAmountOfResidents = dto.MaxAmountOfResidents,
                Address = dto.Address,
                MaintenanceFeeAmount = dto.MaintenanceFeeAmount,
                FloorCount = dto.FloorCount,
                ApartmentUnitCountForEachFloor = dto.ApartmentUnitCountForEachFloor,
                Iban = dto.Iban,
                ApartmentManager = apartmentManager!
            };
            _context.Apartments.Add(apartment);
            await _context.SaveChangesAsync();
            result.Message = "apartman başarılı şekilde eklendi. apartman dairelerini elle eklemeniz gerekecek.";
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
                result.Message = "apartman ve  daireler başarılı bir şekilde eklendi.";
            }

            return Ok(result);
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
                        Apartment = apartment,
                    });
                }
            }
            return apartmentUnits;

        }

        [HttpPut("update-apartment-infos")]
        //apartman bilgisi guncelleme işlemi
        public async Task<IActionResult> UpdateApartment([FromBody] ApartmentDto dto) {
            //input validator
            sanitizeAndValidate.IsValidText(dto.Name);
            sanitizeAndValidate.IsValidNumber(dto.MaxAmountOfResidents);
            sanitizeAndValidate.IsValidDecimal(dto.MaintenanceFeeAmount);



            var apartment = await _context.Apartments.FindAsync(dto.ApartmentId);
            if(apartment == null) {
                throw new ArgumentException("Apartman bulunamadı.");
            }

            apartment.Name = dto.Name;
            apartment.MaxAmountOfResidents = dto.MaxAmountOfResidents;
            apartment.Address = dto.Address;
            apartment.MaintenanceFeeAmount = dto.MaintenanceFeeAmount;
            apartment.Iban = dto.Iban;
            _context.Apartments.Update(apartment);
            await _context.SaveChangesAsync();
            return Ok("Apartman bilgileri güncellendi.");
        }

        // bu tek apartman dairesi ekleme için
        [HttpPost("add-an-apartment-unit")]
        public async Task<IActionResult> AddApartmentUnit([FromBody] ApartmentUnitDto dto) {
            // input validator
            sanitizeAndValidate.IsValidNumber(dto.ApartmentId);
            sanitizeAndValidate.IsValidNumber(dto.FloorNumber);
            sanitizeAndValidate.IsValidNumber(dto.ApartmentNumber);
            sanitizeAndValidate.IsValidText(dto.ApartmentType);
            sanitizeAndValidate.IsValidNumber(dto.SquareMeters);




            // apartman da ki kat sayısı ve daire sayısı kontrol edilip ona göre eklenmeli
            // o değerleri aşıyorsa eklenmemeli

            var apartment = await _context.Apartments.FindAsync(dto.ApartmentId);
            if(apartment == null) {
                throw new ArgumentException("Apartman bulunamadı.");
            }

            if(dto.FloorNumber >= apartment.FloorCount || dto.ApartmentNumber > apartment.ApartmentUnitCountForEachFloor) {
                throw new ArgumentException("Kat sayısı veya daire sayısı aşıldı.");
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
            return Ok(new SuccessResult {
                Message = "apartman dairesi başarılı şekilde eklendi"
            });
        }
        [HttpPost("update-apartment-unit")]
        public async Task<IActionResult> UpdateApartmentUnit([FromBody] ApartmentUnitDto dto) {
            // input validator
            sanitizeAndValidate.IsValidNumber(dto.ApartmentId);
            sanitizeAndValidate.IsValidNumber(dto.FloorNumber);
            sanitizeAndValidate.IsValidNumber(dto.ApartmentNumber);
            sanitizeAndValidate.IsValidText(dto.ApartmentType);
            sanitizeAndValidate.IsValidNumber(dto.SquareMeters);
            sanitizeAndValidate.IsValidNumber(dto.ApartmentUnitId);
            // apartmentUnitId ile daireyi bulma işlemi
            var apartmentUnit = await _context.ApartmentUnits.FindAsync(dto.ApartmentUnitId);
            if(apartmentUnit == null) {
                throw new ArgumentException("Daire bulunamadı.");
            }
            apartmentUnit.ApartmentType = dto.ApartmentType;
            apartmentUnit.SquareMeters = dto.SquareMeters;
            _context.ApartmentUnits.Update(apartmentUnit);
            await _context.SaveChangesAsync();
            return Ok(new SuccessResult {
                Message = "apartman dairesi guncellendi"
            });
        }

        [HttpPost("set-apartment-resident-to-an-unit")]
        public async Task<IActionResult> SetApartmentResidentToAnUnit([FromBody] setResidentToApartmentUnitDto dto) {
            // input validator
            sanitizeAndValidate.IsValidText(dto.Name);
            sanitizeAndValidate.IsValidText(dto.Surname);
            sanitizeAndValidate.IsValidPhoneNumber(dto.PhoneNumber);
            sanitizeAndValidate.IsValidEmail(dto.Email);
            sanitizeAndValidate.IsValidNumber(dto.ApartmentUnitId);
            // apartment resident tablosuna ekleme işlemi
            // apartment unit tablosuna da ekleme işlemi
            // apartment unit tablosunda ki isOccupied değerini true yapma işlemi

            var apartmentUnit = await _context.ApartmentUnits
                .Include(p => p.Apartment)
                .FirstOrDefaultAsync(p => p.Id == dto.ApartmentUnitId);

            if(apartmentUnit == null) {
                throw new ArgumentException("Daire bulunamadı.");
            }
            if(apartmentUnit.IsOccupied) {
                throw new ArgumentException("Bu daire zaten dolu.");
            }
            // oluşabilecek çakışmaların önüne geçmek için rastgele bir kaç harfle kullanıcı adı oluşturulacak
            // şimdilik sayı ekleyerek oluşturulacak
            int randomNumberForUsername = new Random().Next(1000, 9999);
            string createdUsername = $"{dto.Name.ToLower()}.{dto.Surname.ToLower()}" + randomNumberForUsername.ToString();

            // kullanıcı adı ve mail unique oldugundan dolayı kontrol edilecek
            var existingResident = _context.ApartmentResidents
                .FirstOrDefault(resident => resident.Email == dto.Email || resident.Username == createdUsername);
            if(existingResident != null) {
                throw new ArgumentException("Bu e-posta veya kullanıcı adı zaten kullanılıyor.");
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
            _context.ApartmentResidents.Add(apartmentResident);




            var apartmentMaintenanceFee = new MaintenanceFee {
                ResidentId = apartmentResident.Id,
                ApartmentId = apartmentUnit.ApartmentId,
                Amount = apartmentUnit.Apartment.MaintenanceFeeAmount,
                DueDate = getNthMonth28Day.GetDueDate(1), // bir sonraki ayın 28'i son ödeme tarihi
                IsPaid = false,
                ApartmentResident = apartmentResident
            };
            _context.MaintenanceFees.Add(apartmentMaintenanceFee);
            // veri eklendikten sonra mail adresine giriş bilgileri gönderilecek
            var apartmentManager = _context.ApartmentManagers.Find(int.Parse(User.FindFirst("id")?.Value ?? "0"));

            string mailSubject = "Web Tabanlı Aidat Takip Sistemi - Kat Maliki Ekleme";
            string mailBody = $"""
                            Merhaba, {apartmentManager!.Name} sizi web tabanlı aidat takip sistemin de şu adres {apartmentUnit.Apartment.Address} de ki şu kat {apartmentUnit.FloorNumber + 1} şu daire nolu {apartmentUnit.ApartmentNumber} daireye kat maliki olarak ekledi.
                            Sisteme giriş yapablimeniz için giriş bilgileriniz:
                            kullanıcı adınız : {createdUsername}
                            şifreniz : {randomlyCreatedPassword}
                            eğer bu işlem bilginiz dahilinde değilse bize bu mail adresinden bildirin
                        """;

            var sendEmail = emailAction.SendEmail(dto.Email, mailSubject, mailBody);
            if(!sendEmail) {
                throw new ArgumentException("E-posta gönderilirken bir hata oluştu. verileri tekrar eklemeniz gerekiyor");
            }
            // MelihAkıncı_webTabanliAidatTakipSistemi.Models.ApartmentUnit.Apartments.get returned null.

            apartmentUnit.IsOccupied = true;
            _context.ApartmentUnits.Update(apartmentUnit);

            await _context.SaveChangesAsync();
            return Ok(new SuccessResult {
                Message = "Kat maliki başarılı bir şekilde eklendi."
            });
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

        // apartmanın tamamına aidat ekleme yani toplu aidat ekleme
        // birde bunun aylık olarak otomatik yapılması lazım ama birden fazla olmaması için veritabanında isMaintenanceFeeAdded gibi bir alan eklenebilir
        // ama onun da her ay yeniden false yapılması lazım
        [HttpPost("set-maintenance-fee-to-all-residents")]
        public async Task<IActionResult> SetMaintenanceFeeToApartment([FromBody] ApartmentIdDto dto) {
            // input validator
            sanitizeAndValidate.IsValidNumber(dto.ApartmentId);

            // yöneticinin apartmanına ait aidat ekleme işlemi
            int apartmentManagerId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            var apartment = await _context.Apartments.FindAsync(dto.ApartmentId);
            if(apartment == null) {
                throw new ArgumentException("Yönetici olduğunuz apartman bulunamadı.");
            }
            // her kat maliki için aidat eklenmeli
            var apartmentResidents = await _context.ApartmentResidents
                .Include(x => x.ApartmentUnit)
                .Where(x => x.ApartmentUnit!.ApartmentId == dto.ApartmentId)
                .ToListAsync();
            if(apartmentResidents.Count == 0) {
                throw new ArgumentException("Bu apartmanda kat maliki bulunamadı.");
            }
            // maintenancefee ye aktif oluduğu ayı eklesek ve ona göre kontrol etsek şyle 2025/08 şeklinde ya da direk olarak 08 o da yıllar arasında sıkıntı yaratır
            // ya da apartman'a kat maliklerine aidat eklendi mi gibi bir alan ekleyebiliriz ama bunu her ay false olarak ayarlamamız gerekir

            foreach(var resident in apartmentResidents) {
                var findApartmentResidentMaintenanceFee = await _context.MaintenanceFees.Where(x => x.ResidentId == resident.Id && x.DueDate == getNthMonth28Day.GetDueDate(1)).FirstOrDefaultAsync();
                // eğer veritabanında birdaha ki ayın 28 ini içeren kayıt yoksa yeni aidat kaydı yapılır tum kat maliklerine
                if(findApartmentResidentMaintenanceFee == null) {
                    var maintenanceFee = new MaintenanceFee {
                        ResidentId = resident.Id,
                        ApartmentId = apartment.Id,
                        Amount = apartment.MaintenanceFeeAmount,
                        DueDate = getNthMonth28Day.GetDueDate(1), // bir sonraki ayın 28'i son ödeme tarihi
                        IsPaid = false,
                        ApartmentResident = resident
                    };
                    _context.MaintenanceFees.Add(maintenanceFee);
                }
            }
            await _context.SaveChangesAsync();
            return Ok(new SuccessResult {
                Message = "aidatlar başarılı bir şekilde eklendi"
            });
        }

        [HttpPost("set-specific-fee-to-apartment-resident")]
        public async Task<IActionResult> SetSpecificDebtToAnResident([FromBody] ResidentSpecificFeeDto dto) {
            // input validator
            sanitizeAndValidate.IsValidText(dto.Name);
            sanitizeAndValidate.IsValidText(dto.Description);
            sanitizeAndValidate.IsValidDecimal(dto.Amount);
            sanitizeAndValidate.IsValidNumber(dto.ResidentId);
            var apartmentResident = _context.ApartmentResidents
                .Include(x => x.ApartmentUnit)
                .FirstOrDefault(x => x.Id == dto.ResidentId);
            if(apartmentResident == null) {
                throw new ArgumentException("kat maliki bulunamadı");
            }

            var residentSpecificFee = new ResidentsSpecificFee {
                Name = dto.Name,// borç adı
                Description = dto.Description,
                Amount = dto.Amount,
                ResidentId = apartmentResident!.Id,
                ApartmentId = apartmentResident.ApartmentUnit!.ApartmentId,
                DueDate = getNthMonth28Day.GetDueDate(1),
                ApartmentResident = apartmentResident!
            };

            _context.ResidentsSpecificFees.Add(residentSpecificFee);
            await _context.SaveChangesAsync();
            return Ok(new SuccessResult {
                Message = "kat malikine özel borç eklendi"
            });
        }

        [HttpPost("get-un-paid-maintenance-fees")]
        public async Task<IActionResult> GetUnPaidMaintenanceFees([FromBody] ApartmentIdDto dto) {
            // input validator
            sanitizeAndValidate.IsValidNumber(dto.ApartmentId);

            int apartmentManagerId = int.Parse(User.FindFirst("id")?.Value ?? "0");

            // yöneticinin o apartman da yönetici olup olamdığının kontrolü
            var apartments = await _context.Apartments
                .Include(a => a.ApartmentManager)
                .Where(x => x.ApartmentManager.Id == apartmentManagerId && x.Id == dto.ApartmentId)
                .FirstOrDefaultAsync();

            if(apartments == null) {
                throw new ArgumentException("Yönetici olduğunuz apartman bulunamadı.");
            }
            var getUnPaidMantenanceFees = await _context.MaintenanceFees
                .Include(x => x.ApartmentResident)
                .Where(x => x.ApartmentResident!.ApartmentUnit!.ApartmentId == dto.ApartmentId && x.IsPaid == false)
                .Select(fee => new MaintenanceFeeDto {
                    Id = fee.Id,
                    Amount = fee.Amount,
                    DueDate = fee.DueDate,
                    IsPaid = fee.IsPaid,
                    PaymentDate = fee.PaymentDate,
                    ResidentName = fee.ApartmentResident!.Name,
                    ResidentSurname = fee.ApartmentResident!.Surname,
                })
                .ToListAsync();

            return Ok(getUnPaidMantenanceFees);

        }

        [HttpPost("get-un-paid-special-fees")]
        public async Task<IActionResult> GetUnPaidSpecialFees([FromBody] ApartmentIdDto dto) {
            // input validator
            sanitizeAndValidate.IsValidNumber(dto.ApartmentId);
            int apartmentManagerId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            var apartments = await _context.Apartments
                .Include(a => a.ApartmentManager)
                .Where(x => x.ApartmentManager.Id == apartmentManagerId && x.Id == dto.ApartmentId)
                .FirstOrDefaultAsync();
            if(apartments == null) {
                throw new ArgumentException("Yönetici olduğunuz apartman bulunamadı ve ya bu apartman da yönetici değilsiniz");
            }
            var getUnPaidSpecialFees = await _context.ResidentsSpecificFees
                .Include(x => x.ApartmentResident)
                .Where(x => x.ApartmentResident!.ApartmentUnit!.ApartmentId == dto.ApartmentId && x.IsPaid == false)
                .Select(fee => new ResidentSpecificFeeDto {
                    Id = fee.Id,
                    Name = fee.Name,
                    Description = fee.Description,
                    Amount = fee.Amount,
                    DueDate = fee.DueDate,
                    IsPaid = fee.IsPaid,
                    PaymentDate = fee.PaymentDate,
                    ResidentName = fee.ApartmentResident!.Name,
                    ResidentSurname = fee.ApartmentResident!.Surname,
                })
                .ToListAsync();

            return Ok(getUnPaidSpecialFees);
        }

        [HttpPost("get-all-paid-maintenance-fees")]
        public async Task<IActionResult> GetAllPaidMaintenanceFees([FromBody] ApartmentIdDto dto) {
            // input validator
            sanitizeAndValidate.IsValidNumber(dto.ApartmentId);

            // yöneticinin apartmanına ait tüm ödenmiş aidatları getirme işlemi
            int apartmentManagerId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            var apartments = await _context.Apartments.Include(a => a.ApartmentManager)
                .Where(x => x.ApartmentManager.Id == apartmentManagerId && x.Id == dto.ApartmentId)
                .FirstOrDefaultAsync();
            if(apartments == null) {
                throw new ArgumentException("Yönetici olduğunuz apartman bulunamadı ve ya bu apartman da yönetici değilsiniz");
            }
            var getPaidMaintenanceFees = await _context.MaintenanceFees
                .Include(x => x.ApartmentResident)
                .Where(x => x.ApartmentResident!.ApartmentUnit!.ApartmentId == dto.ApartmentId && x.IsPaid == true)
                .Select(fee => new MaintenanceFeeDto {
                    Id = fee.Id,
                    Amount = fee.Amount,
                    DueDate = fee.DueDate,
                    IsPaid = fee.IsPaid,
                    PaymentDate = fee.PaymentDate,
                    ResidentName = fee.ApartmentResident!.Name,
                    ResidentSurname = fee.ApartmentResident!.Surname,
                })
                .ToListAsync();

            return Ok(getPaidMaintenanceFees);
        }
        [HttpPost("get-all-paid-special-fees")]
        public async Task<IActionResult> GetAllPaidSpecificFees([FromBody] ApartmentIdDto dto) {
            // input validator
            sanitizeAndValidate.IsValidNumber(dto.ApartmentId);
            // yöneticinin apartmanına ait tüm ödenmiş aidatları getirme işlemi
            int apartmentManagerId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            var apartments = await _context.Apartments.Include(a => a.ApartmentManager)
                .Where(x => x.ApartmentManager.Id == apartmentManagerId && x.Id == dto.ApartmentId)
                .FirstOrDefaultAsync();
            if(apartments == null) {
                throw new ArgumentException("Yönetici olduğunuz apartman bulunamadı ve ya bu apartman da yönetici değilsiniz");
            }
            var getPaidSpecialFees = await _context.ResidentsSpecificFees
                .Include(x => x.ApartmentResident)
                .Where(x => x.ApartmentResident!.ApartmentUnit!.ApartmentId == dto.ApartmentId && x.IsPaid == true)
                .Select(fee => new ResidentSpecificFeeDto {
                    Id = fee.Id,
                    Name = fee.Name,
                    Description = fee.Description,
                    Amount = fee.Amount,
                    DueDate = fee.DueDate,
                    IsPaid = fee.IsPaid,
                    PaymentDate = fee.PaymentDate,
                    ResidentName = fee.ApartmentResident!.Name,
                    ResidentSurname = fee.ApartmentResident!.Surname,
                })
                .ToListAsync();

            return Ok(getPaidSpecialFees);
        }
        //
        public static List<PaymentNotificationDto> GetPaymentNotifications(List<PaymentNotifications> paymentNotifications, List<PaymentNotificationDto> paymentNotification) {
            // yöneticinin apartmanına ait ödeme bildirimlerini getirme işlemi
            paymentNotifications.ForEach(notification => {
                paymentNotification.Add(new PaymentNotificationDto {
                    // payment notification id
                    NotificationId = notification.Id,
                    // apartment infos
                    ApartmentName = notification.Apartment!.Name,
                    ApartmentAddress = notification.Apartment!.Address,
                    // resident infos
                    ResidentName = notification.ApartmentResident!.Name,
                    ResidentSurname = notification.ApartmentResident!.Surname,
                    ResidentPhoneNumber = notification.ApartmentResident!.PhoneNumber,
                    ResidentEmail = notification.ApartmentResident!.Email,
                    // payment infos
                    PaymentName = "aidat" ?? notification.ResidentsSpecificFee?.Name ?? "Ödeme Bilgisi Yok",
                    PaymentDescription = "aidat ödemesi" ?? notification.ResidentsSpecificFee?.Description ?? "Ödeme Bilgisi Yok",
                    PaymentDate = notification.PaymentDate ?? DateTime.UtcNow,
                    Amount = notification.MaintenanceFee?.Amount ?? notification.ResidentsSpecificFee?.Amount ?? 0,
                    DueDate = notification.MaintenanceFee?.DueDate ?? notification.ResidentsSpecificFee?.DueDate ?? DateTime.MinValue,
                    Status = notification.Status,
                    NotificationMessage = notification.Message,
                    ReceiptUrl = notification.ReceiptUrl,
                });
            });
            return paymentNotification;


        }
        [HttpPost("get-payment-notifications")]
        public IActionResult PaymentNotifications([FromBody] ApartmentIdDto dto) {
            // aidat ya da özel ücret ödemesi yapıldıktan sonra yöneticinin onayına sunulacak
            // yani yönetici yapılan ödemeleri göreblicek ve ilgili rotalardan onaylayabilecek ya da reddedebilecek
            // yönetici ödeme bildirimlerini iki yolla da görebilmeli hem apartman id sine göre hem de bir bildirim gibi ayrı bir yerde
            // yeni bir dto lazım gibi çünkü hem apartman ismi, kat maliki ismi soyismi gibi ek bilgiler lazım
            // gereken şema  bu şekilde
            /*
             * ------------- apartman bilgileri -------------
             * apartman ismi
             * apartman adresi
             * 
             * ------------- kat maliki bilgileri -------------
             * kat maliki ismi
             * kat maliki soyismi
             * kat maliki telefon numarası
             * kat maliki e-posta adresi
             * 
             * 
             * 
             * ------------- ödeme(aidat, özel ücret) bilgileri -------------
             * tutar
             * ödeme tarihi
             * son ödeme tarihi
             * ödeme durumu (beklemede, onaylandı, reddedildi)
             * dekont url
             * mesaj (yöneticiye iletilen mesaj)
             * 
             */
            sanitizeAndValidate.IsValidNumber(dto.ApartmentId);
            int apartmentManagerId = int.Parse(User.FindFirst("id")?.Value ?? "0");

            List<PaymentNotificationDto> paymentNotification = new List<PaymentNotificationDto>();

            if(dto.ApartmentId == 0) {
                var paymentNotifications = _context.PaymentNotifications
                    .Include(x => x.Apartment)
                    .Include(x => x.ApartmentResident)
                    .Include(x => x.MaintenanceFee)
                    .Include(x => x.ResidentsSpecificFee)
                    .Where(x => x.Status == PaymentStatusEnum.PaymentStatus.Beklemede && x.ManagerId == apartmentManagerId)
                    .ToList();
                paymentNotification = GetPaymentNotifications(paymentNotifications, paymentNotification);
            }
            else if(dto.ApartmentId > 0) {
                var paymentNotifications = _context.PaymentNotifications
                    .Include(x => x.Apartment)
                    .Include(x => x.ApartmentResident)
                    .Include(x => x.MaintenanceFee)
                    .Include(x => x.ResidentsSpecificFee)
                    .Where(x => x.Status == PaymentStatusEnum.PaymentStatus.Beklemede && x.ApartmentId == dto.ApartmentId)
                    .ToList();
                paymentNotification = GetPaymentNotifications(paymentNotifications, paymentNotification);
            }
            else {
                throw new ArgumentException("Apartman id değeri geçersiz.");
            }

            return Ok(paymentNotification);
        }
        [HttpPost("allow-payment")]
        public async Task<IActionResult> AllowPayment([FromBody] AllowOrDenyPaymentNotificationDto dto) {
            // gelen mesaj onay durumun da boş gelebilir ama sadece burada
            sanitizeAndValidate.IsValidNumber(dto.PaymentNotificationId);
            var paymentNotification = await _context.PaymentNotifications.FindAsync(dto.PaymentNotificationId);
            if(paymentNotification == null) {
                throw new ArgumentException("Ödeme bildirimi bulunamadı.");
            }
            paymentNotification.Message = dto.Message;
            paymentNotification.Status = PaymentStatus.onaylandi;
            _context.PaymentNotifications.Update(paymentNotification);
            await _context.SaveChangesAsync();

            return Ok(new SuccessResult { Message = "ödeme başarılı bir şekilde onaylandı" });
        }
        [HttpPost("deny-payment")]
        public async Task<IActionResult> DenyPayment([FromBody] AllowOrDenyPaymentNotificationDto dto) {
            if(string.IsNullOrWhiteSpace(dto.Message)) {
                throw new ArgumentException("reddedilen ödeme bildirimlerinde reddetme sebebi olarak mesaj girilmesi zorunlu");
            }
            sanitizeAndValidate.IsValidNumber(dto.PaymentNotificationId);
            sanitizeAndValidate.IsValidText(dto.Message);
            var paymentNotification = await _context.PaymentNotifications.FindAsync(dto.PaymentNotificationId);
            if(paymentNotification == null) {
                throw new ArgumentException("Ödeme bildirimi bulunamadı.");
            }
            paymentNotification.Message = dto.Message;
            paymentNotification.Status = PaymentStatus.Reddedildi;
            _context.PaymentNotifications.Update(paymentNotification);
            await _context.SaveChangesAsync();
            return Ok(new SuccessResult { Message = "ödeme başarılı bir şekilde reddedildi" });
        }


        [HttpPost("get-monthly-maintenance-fee-report")]
        public async Task<IActionResult> GetMonthlyMaintenanceFeeReport([FromBody] MontlyMaintenanceOrSpecialfeeReportDto dto) {

            // input validate
            sanitizeAndValidate.IsValidNumber(dto.ApartmentId);
            // rapor için gereken bilgilerin çekilmesi burada select ile sadece gereken bilgiler çekilmeli
            int apartmentManagerId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            var managerInfos = await _context.ApartmentManagers.FindAsync(apartmentManagerId);

            var apartmentInfos = await _context.Apartments
                .Where(x => x.ManagerId == apartmentManagerId)
                .ToListAsync();

            // oluşturmuş olduğum sınıf yapısı ile rapor için bir liste oluşturulması
            List<MaintenanceFeeReport> maintenanceFeeReport = new List<MaintenanceFeeReport>();
            foreach(var apartment in apartmentInfos) {

                Console.WriteLine(apartmentInfos.Count);
                maintenanceFeeReport.Add(new MaintenanceFeeReport {
                    ApartmentName = apartment.Name,
                    ManagerName = managerInfos!.Name,
                    PaidMaintenanceFees = [],
                    UnPaidMaintenanceFees = [],
                    TotalMaintenanceFees = 0,
                    TotalPaidMaintenanceFees = 0,
                    TotalUnPaidMaintenanceFees = 0

                });
                int currentIndex = maintenanceFeeReport.Count - 1;
                var maintenanceFees = await _context.MaintenanceFees
                    .Include(x => x.ApartmentResident)
                    .Where(x => x.ApartmentId == apartment.Id && x.DueDate == dto.ReportDate)
                    .ToListAsync();

                foreach(var fee in maintenanceFees) {
                    maintenanceFeeReport[currentIndex].TotalMaintenanceFees = maintenanceFees.Count;
                    maintenanceFeeReport[currentIndex].TotalPaidMaintenanceFees = maintenanceFees.Count(x => x.IsPaid);
                    maintenanceFeeReport[currentIndex].TotalUnPaidMaintenanceFees = maintenanceFees.Count - maintenanceFeeReport[currentIndex].TotalPaidMaintenanceFees;

                    if(fee.IsPaid) {
                        maintenanceFeeReport[currentIndex].PaidMaintenanceFees = maintenanceFeeReport[currentIndex].PaidMaintenanceFees.Append(new {
                            ResidentName = fee.ApartmentResident!.Name,
                            ResidentSurname = fee.ApartmentResident!.Surname,
                            ResidentPhoneNumber = fee.ApartmentResident!.PhoneNumber,
                            ResidentEmail = fee.ApartmentResident!.Email,
                            fee.Amount,
                            fee.DueDate,
                            fee.PaymentDate,
                            fee.IsPaid
                        }).ToArray();
                    }
                    else {

                        maintenanceFeeReport[currentIndex].UnPaidMaintenanceFees = maintenanceFeeReport[currentIndex].UnPaidMaintenanceFees.Append(new {
                            ResidentName = fee.ApartmentResident!.Name,
                            ResidentSurname = fee.ApartmentResident!.Surname,
                            ResidentPhoneNumber = fee.ApartmentResident!.PhoneNumber,
                            ResidentEmail = fee.ApartmentResident!.Email,
                            fee.Amount,
                            fee.DueDate,
                            fee.IsPaid
                        }).ToArray();
                    }
                }
            }
            return Ok(maintenanceFeeReport);

        }
        [HttpPost("get-monthly-special-fee-report")]
        public async Task<IActionResult> GetMonthlySpecialFeeReport([FromBody] MontlyMaintenanceOrSpecialfeeReportDto dto) {
            // input validate
            sanitizeAndValidate.IsValidNumber(dto.ApartmentId);
            // rapor için gereken bilgilerin çekilmesi burada select ile sadece gereken bilgiler çekilmeli
            int apartmentManagerId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            var managerInfos = await _context.ApartmentManagers.FindAsync(apartmentManagerId);

            var apartmentInfos = await _context.Apartments
                .Where(x => x.ManagerId == apartmentManagerId)
                .ToListAsync();

            // oluşturmuş olduğum sınıf yapısı ile rapor için bir liste oluşturulması
            List<SpecialFeeReport> specialFeeReport = new List<SpecialFeeReport>();
            foreach(var apartment in apartmentInfos) {

                Console.WriteLine(apartmentInfos.Count);
                specialFeeReport.Add(new SpecialFeeReport {
                    ApartmentName = apartment.Name,
                    ManagerName = managerInfos!.Name,
                    PaidSpecialFees = [],
                    UnpaidSpecialFees = [],
                    TotalSpecialFees = 0,
                    TotalPaidSpecialFees = 0,
                    TotalUnPaidSpecialFees = 0

                });
                int currentIndex = specialFeeReport.Count - 1;
                var specialFees = await _context.MaintenanceFees
                    .Include(x => x.ApartmentResident)
                    .Where(x => x.ApartmentId == apartment.Id && x.DueDate == dto.ReportDate)
                    .ToListAsync();

                foreach(var fee in specialFees) {
                    specialFeeReport[currentIndex].TotalSpecialFees = specialFees.Count;
                    specialFeeReport[currentIndex].TotalPaidSpecialFees = specialFees.Count(x => x.IsPaid);
                    specialFeeReport[currentIndex].TotalUnPaidSpecialFees = specialFees.Count - specialFeeReport[currentIndex].TotalPaidSpecialFees;

                    if(fee.IsPaid) {
                        specialFeeReport[currentIndex].PaidSpecialFees = specialFeeReport[currentIndex].PaidSpecialFees.Append(new {
                            ResidentName = fee.ApartmentResident!.Name,
                            ResidentSurname = fee.ApartmentResident!.Surname,
                            ResidentPhoneNumber = fee.ApartmentResident!.PhoneNumber,
                            ResidentEmail = fee.ApartmentResident!.Email,
                            fee.Amount,
                            fee.DueDate,
                            fee.PaymentDate,
                            fee.IsPaid
                        }).ToArray();
                    }
                    else {

                        specialFeeReport[currentIndex].UnpaidSpecialFees = specialFeeReport[currentIndex].UnpaidSpecialFees.Append(new {
                            ResidentName = fee.ApartmentResident!.Name,
                            ResidentSurname = fee.ApartmentResident!.Surname,
                            ResidentPhoneNumber = fee.ApartmentResident!.PhoneNumber,
                            ResidentEmail = fee.ApartmentResident!.Email,
                            fee.Amount,
                            fee.DueDate,
                            fee.IsPaid
                        }).ToArray();
                    }
                }
            }
            return Ok(specialFeeReport);
        }


        [HttpPost("get-monthly-maintenance-fee-report-for-apartment")]
        public async Task<IActionResult> GetMonthlyMaintenanceFeeReportForApartment([FromBody] MontlyMaintenanceOrSpecialfeeReportDto dto) {
            // veritabanın da ki alanın değeri bu 2025-08-28 00:00:00.000000 arama bu şekilde düzgün geliyor SELECT * FROM webtabanliaidatyonetimsistemi.maintenancefees where DueDate = '2025-08-28'
            // input validate
            sanitizeAndValidate.IsValidNumber(dto.ApartmentId);
            // rapor için gereken bilgilerin çekilmesi burada select ile sadece gereken bilgiler çekilmeli
            int apartmentManagerId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            var managerInfos = await _context.ApartmentManagers.FindAsync(apartmentManagerId);
            var apartmentInfos = await _context.Apartments.FindAsync(dto.ApartmentId);
            // oluşturmuş olduğum sınıf yapısı ile rapor için bir liste oluşturulması
            List<MaintenanceFeeReport> maintenanceFeeReport = new List<MaintenanceFeeReport> {
                new MaintenanceFeeReport {
                    ApartmentName = apartmentInfos!.Name,
                    ManagerName = managerInfos!.Name,
                    PaidMaintenanceFees = [],
                    UnPaidMaintenanceFees = [],
                    TotalMaintenanceFees = 0,
                    TotalPaidMaintenanceFees = 0,
                    TotalUnPaidMaintenanceFees = 0
                }
            };

            var maintenanceFees = await _context.MaintenanceFees
                .Include(x => x.ApartmentResident)
                .Where(x => x.ApartmentId == dto.ApartmentId && x.DueDate == dto.ReportDate)
                .ToListAsync();

            foreach(var fee in maintenanceFees) {
                maintenanceFeeReport.First().TotalMaintenanceFees = maintenanceFees.Count;
                maintenanceFeeReport.First().TotalPaidMaintenanceFees = maintenanceFees.Count(x => x.IsPaid);
                maintenanceFeeReport.First().TotalUnPaidMaintenanceFees = maintenanceFees.Count - maintenanceFeeReport.First().TotalPaidMaintenanceFees;

                if(fee.IsPaid) {
                    maintenanceFeeReport[0].PaidMaintenanceFees = maintenanceFeeReport[0].PaidMaintenanceFees.Append(new {
                        ResidentName = fee.ApartmentResident!.Name,
                        ResidentSurname = fee.ApartmentResident!.Surname,
                        ResidentPhoneNumber = fee.ApartmentResident!.PhoneNumber,
                        ResidentEmail = fee.ApartmentResident!.Email,
                        fee.Amount,
                        fee.DueDate,
                        fee.PaymentDate,
                        fee.IsPaid
                    }).ToArray();
                }
                else {
                    maintenanceFeeReport[0].UnPaidMaintenanceFees = maintenanceFeeReport[0].UnPaidMaintenanceFees.Append(new {
                        ResidentName = fee.ApartmentResident!.Name,
                        ResidentSurname = fee.ApartmentResident!.Surname,
                        ResidentPhoneNumber = fee.ApartmentResident!.PhoneNumber,
                        ResidentEmail = fee.ApartmentResident!.Email,
                        fee.Amount,
                        fee.DueDate,
                        fee.IsPaid
                    }).ToArray();
                }
            }

            return Ok(maintenanceFeeReport);
        }

        [HttpPost("get-monthly-special-fee-report-for-apartment")]
        public async Task<IActionResult> GetMontlySpecialFeeReports([FromBody] MontlyMaintenanceOrSpecialfeeReportDto dto) {
            sanitizeAndValidate.IsValidNumber(dto.ApartmentId);
            // rapor için gereken bilgilerin çekilmesi burada select ile sadece gereken bilgiler çekilmeli
            int apartmentManagerId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            var managerInfos = await _context.ApartmentManagers.FindAsync(apartmentManagerId);
            var apartmentInfos = await _context.Apartments.FindAsync(dto.ApartmentId);
            // oluşturmuş olduğum sınıf yapısı ile rapor için bir liste oluşturulması
            List<SpecialFeeReport> specialFeeReport = new List<SpecialFeeReport> {
                new SpecialFeeReport {
                    ApartmentName = apartmentInfos!.Name,
                    ManagerName = managerInfos!.Name,
                    PaidSpecialFees = [],
                    UnpaidSpecialFees = [],
                    TotalSpecialFees = 0,
                    TotalPaidSpecialFees = 0,
                    TotalUnPaidSpecialFees = 0
                }
            };

            var specialFees = await _context.ResidentsSpecificFees
                .Include(x => x.ApartmentResident)
                .Where(x => x.ApartmentId == dto.ApartmentId && x.DueDate == dto.ReportDate)
                .ToListAsync();

            foreach(var fee in specialFees) {
                specialFeeReport.First().TotalSpecialFees = specialFees.Count;
                specialFeeReport.First().TotalPaidSpecialFees = specialFees.Count(x => x.IsPaid);
                specialFeeReport.First().TotalUnPaidSpecialFees = specialFees.Count - specialFeeReport.First().TotalPaidSpecialFees;

                if(fee.IsPaid) {
                    specialFeeReport[0].PaidSpecialFees = specialFeeReport[0].PaidSpecialFees.Append(new {
                        ResidentName = fee.ApartmentResident!.Name,
                        ResidentSurname = fee.ApartmentResident!.Surname,
                        ResidentPhoneNumber = fee.ApartmentResident!.PhoneNumber,
                        ResidentEmail = fee.ApartmentResident!.Email,
                        fee.Amount,
                        fee.DueDate,
                        fee.PaymentDate,
                        fee.IsPaid
                    }).ToArray();
                }
                else {
                    specialFeeReport[0].UnpaidSpecialFees = specialFeeReport[0].UnpaidSpecialFees.Append(new {
                        ResidentName = fee.ApartmentResident!.Name,
                        ResidentSurname = fee.ApartmentResident!.Surname,
                        ResidentPhoneNumber = fee.ApartmentResident!.PhoneNumber,
                        ResidentEmail = fee.ApartmentResident!.Email,
                        fee.Amount,
                        fee.DueDate,
                        fee.IsPaid
                    }).ToArray();
                }
            }
            return Ok(specialFeeReport);
        }




    }
}
