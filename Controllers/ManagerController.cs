using MelihAkıncı_webTabanliAidatTakipSistemi.classes;
using MelihAkıncı_webTabanliAidatTakipSistemi.Data;
using MelihAkıncı_webTabanliAidatTakipSistemi.DTOs;
using MelihAkıncı_webTabanliAidatTakipSistemi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Net;
using System.Net.Mail;

namespace MelihAkıncı_webTabanliAidatTakipSistemi.Controllers {
    [Authorize(Roles = "ApartmentManager")]
    [ApiController]
    [Route("[controller]")]
    public class ManagerController(AppDbContext context) : Controller {
        private readonly AppDbContext _context = context;
        private readonly PasswordHash passwordHash = new PasswordHash();

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

        public IActionResult Index() {
            return Ok("rota çalışıyor");
        }

        [HttpGet("GetUserRole")]
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

        [HttpGet("getUserInfos")]
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

        [HttpGet("getApartments")]
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
                ApartmentUnitCountForEachFloor = apartment.ApartmentUnitCountForEachFloor,
            })
            .ToListAsync();
            if(managerApartments.Count == 0) {
                throw new ArgumentException("apartman bulunamadı ve ya yöneticisi olduğunuz apartman yok");


            }
            return Ok(managerApartments);
        }
        // yöneticinin apartmanına ait daireleri ve o dairelerde ki kat maliklerini getirme işlemi
        [HttpPost("getApartmentsUnits")]
        public async Task<IActionResult> GetApartmentUnitWithResidentsOrWithout([FromBody] ApartmentIdDto dto) {
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

        [HttpPut("updateManagerInfos")]
        public async Task<IActionResult> UpdateInfos([FromBody] ApartmentManagerDto dto) {

            //tum değerlerin doldurulması zorunlu
            if(string.IsNullOrWhiteSpace(dto.Name) ||
                string.IsNullOrWhiteSpace(dto.Surname) ||
                string.IsNullOrWhiteSpace(dto.PhoneNumber) ||
                string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Password)) {

                throw new ArgumentException("Zorunlu alanlar boş bırakılamaz.");
            }
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

            if(!passwordHash.VerifyPassword(apartmentManager.Password, dto.Password)) {
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

        [HttpPost("addApartment")]
        public async Task<IActionResult> AddApartment([FromBody] ApartmentDto dto) {
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

        [HttpPut("updateApartmentInfos")]
        //apartman bilgisi guncelleme işlemi
        public async Task<IActionResult> UpdateApartment([FromBody] ApartmentDto dto) {
            var apartment = await _context.Apartments.FindAsync(dto.ApartmentId);
            if(apartment == null) {
                throw new ArgumentException("Apartman bulunamadı.");
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
        [HttpPost("updateApartmentUnit")]
        public async Task<IActionResult> UpdateApartmentUnit([FromBody] ApartmentUnitDto dto) {
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

        [HttpPost("setApartmentResidentToAnUnit")]
        public async Task<IActionResult> SetApartmentResidentToAnUnit([FromBody] setResidentToApartmentUnitDto dto) {
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
                Amount = apartmentUnit.Apartment.MaintenanceFeeAmount,
                DueDate = GetNextMonth28thDay(), // bir sonraki ayın 28'i son ödeme tarihi
                IsPaid = false,
                ApartmentResident = apartmentResident
            };
            _context.MaintenanceFees.Add(apartmentMaintenanceFee);
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
        public static DateTime GetNextMonth28thDay() {
            // verilen tarihin bir sonraki ayının 28. ci gününü döndürür
            int year = DateTime.UtcNow.Year;
            int month = DateTime.UtcNow.AddMonths(1).Month;
            int day = 28;
            DateTime date = new DateTime(year, month, day);
            string fullTime = date.ToString("yyyy/MM/dd");
            return DateTime.Parse(fullTime);
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
        [HttpPost("setMaintenanceFeeToAllResidents")]
        public async Task<IActionResult> SetMaintenanceFeeToApartment([FromBody] ApartmentIdDto dto) {
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
                var findApartmentResidentMaintenanceFee = await _context.MaintenanceFees.Where(x => x.ResidentId == resident.Id && x.DueDate == GetNextMonth28thDay()).FirstOrDefaultAsync();
                // eğer veritabanında birdaha ki ayın 28 ini içeren kayıt yoksa yeni aidat kaydı yapılır tum kat maliklerine
                if(findApartmentResidentMaintenanceFee == null) {
                    var maintenanceFee = new MaintenanceFee {
                        ResidentId = resident.Id,
                        Amount = apartment.MaintenanceFeeAmount,
                        DueDate = GetNextMonth28thDay(), // bir sonraki ayın 28'i son ödeme tarihi
                        IsPaid = false,
                        Status = PaymentStatusEnum.PaymentStatus.Beklemede,
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

        [HttpPost("setSpecificFeeToApartmentResident")]
        public async Task<IActionResult> SetSpecificDebtToAnResident([FromBody] ResidentSpecificFeeDto dto) {
            var apartmentResident = _context.ApartmentResidents.Find(dto.ResidentId);
            if(apartmentResident == null) {
                throw new ArgumentException("kat maliki bulunamadı");
            }

            var residentSpecificFee = new ResidentsSpecificFee {
                Name = dto.Name,// borç adı
                Description = dto.Description,
                Amount = dto.Amount,
                ResidentId = apartmentResident!.Id,
                DueDate = GetNextMonth28thDay(),
                ApartmentResident = apartmentResident!
            };

            _context.ResidentsSpecificFees.Add(residentSpecificFee);
            await _context.SaveChangesAsync();
            return Ok(new SuccessResult {
                Message = "kat malikine özel borç eklendi"
            });
        }

        [HttpPost("getUnPaidMaintenanceFees")]
        public async Task<IActionResult> GetUnPaidMaintenanceFees([FromBody] ApartmentIdDto dto) {
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
                    Status = fee.Status
                })
                .ToListAsync();

            return Ok(getUnPaidMantenanceFees);

        }

        [HttpPost("getUnPaidSpecialFees")]
        public async Task<IActionResult> GetUnPaidSpecialFees([FromBody] ApartmentIdDto dto) {
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
                    Status = fee.Status
                })
                .ToListAsync();

            return Ok(getUnPaidSpecialFees);
        }

        [HttpPost("getAllPaidMaintenanceFees")]
        public async Task<IActionResult> GetAllPaidMaintenanceFees([FromBody] ApartmentIdDto dto) {
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
                    Status = fee.Status
                })
                .ToListAsync();

            return Ok(getPaidMaintenanceFees);
        }
        [HttpPost("getAllPaidSpecialFees")]
        public async Task<IActionResult> GetAllPaidSpecificFees([FromBody] ApartmentIdDto dto) {
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
                    Status = fee.Status
                })
                .ToListAsync();

            return Ok(getPaidSpecialFees);
        }

        [HttpPost("allowMaintenanceFeePayment")]
        public async Task<IActionResult> AllowMaintenanceFeePayment([FromBody] PayOrAllowOrDenyMaintenanceOrSpecialFeeDto dto) {
            // yöneticinin apartmanına ait tüm ödenmiş aidatları getirme işlemi
            int apartmentManagerId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            var maintenanceFees = await _context.MaintenanceFees.FindAsync(dto.MaintenanceFeeId);
            if(maintenanceFees == null) {
                throw new ArgumentException("aidat bulunamadı");
            }
            maintenanceFees.Status = PaymentStatusEnum.PaymentStatus.Onaylandi;
            await _context.SaveChangesAsync();

            return Ok(new SuccessResult {
                Message = "aidat ödemesi başarılı bir şekilde onaylandı"
            });


        }
        [HttpPost("DenyMaintenanceFeePayment")]
        public async Task<IActionResult> DenyMaintenanceFeePayment([FromBody] PayOrAllowOrDenyMaintenanceOrSpecialFeeDto dto) {
            // yöneticinin apartmanına ait tüm ödenmiş aidatları getirme işlemi
            int apartmentManagerId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            var maintenanceFees = await _context.MaintenanceFees.FindAsync(dto.MaintenanceFeeId);
            if(maintenanceFees == null) {
                throw new ArgumentException("aidat bulunamadı");
            }
            maintenanceFees.Status = PaymentStatusEnum.PaymentStatus.Reddedildi;
            await _context.SaveChangesAsync();

            return Ok(new SuccessResult {
                Message = "aidat ödemesi başarılı bir şekilde reddedildi"
            });


        }


        [HttpPost("allowSpecificFeePayment")]
        public async Task<IActionResult> AllowSpecificFeePayment([FromBody] PayOrAllowOrDenyMaintenanceOrSpecialFeeDto dto) {
            // yöneticinin apartmanına ait tüm ödenmiş aidatları getirme işlemi
            int apartmentManagerId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            var specificFees = await _context.ResidentsSpecificFees.FindAsync(dto.SpecialFeeId);
            if(specificFees == null) {
                throw new ArgumentException("özel ücret bulunamadı");
            }
            specificFees.Status = PaymentStatusEnum.PaymentStatus.Onaylandi;
            await _context.SaveChangesAsync();

            return Ok(new SuccessResult {
                Message = "özel ücret ödemesi başarılı bir şekilde onaylandı"
            });


        }
        [HttpPost("DenySpecificFeePayment")]
        public async Task<IActionResult> DenySpecificFeePayment([FromBody] PayOrAllowOrDenyMaintenanceOrSpecialFeeDto dto) {
            // yöneticinin apartmanına ait tüm ödenmiş aidatları getirme işlemi
            int apartmentManagerId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            var specificFees = await _context.ResidentsSpecificFees.FindAsync(dto.SpecialFeeId);
            if(specificFees == null) {
                throw new ArgumentException("özel ücret bulunamadı");
            }
            specificFees.Status = PaymentStatusEnum.PaymentStatus.Reddedildi;
            await _context.SaveChangesAsync();

            return Ok(new SuccessResult {
                Message = "özel ücret ödemesi başarılı bir şekilde reddedildi"
            });


        }

    }
}
