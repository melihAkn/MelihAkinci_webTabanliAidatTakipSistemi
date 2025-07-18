using MelihAkıncı_webTabanliAidatTakipSistemi.classes;
using MelihAkıncı_webTabanliAidatTakipSistemi.Data;
using MelihAkıncı_webTabanliAidatTakipSistemi.DTOs;
using MelihAkıncı_webTabanliAidatTakipSistemi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using static MelihAkıncı_webTabanliAidatTakipSistemi.classes.PaymentStatusEnum;

namespace MelihAkıncı_webTabanliAidatTakipSistemi.Controllers {

    [Authorize(Roles = "ApartmentResident")]
    [ApiController]
    [Route("[controller]")]
    public class ResidentController(AppDbContext context) : Controller {

        private readonly AppDbContext _context = context;
        private readonly PasswordHash passwordHash = new PasswordHash();
        private readonly SanitizeAndValidate sanitizeAndValidate = new SanitizeAndValidate();

        /*
         * bilgilerini guncelleyebilme / put --------------------------------------- done
         * kendi dairesini gorebilme / get --------------------------------------- done
         * aidat bilgilerini görebilme / get ---------------- done
         * özel ücretlerini gorebilme / get ---------------- done
         * 
         * aidat ödeyebilme  / post ---------------- done
         * özel ücretini ödeyebilme / post ---------------- done
         * yapmış olduğu ödemeleri ve durumlarını(red, beklemede, onay vb.) görüntüleme / get ---------------- done
         * ödeme yaptıktan sonra ödeme bildirimi yapabilme ve dekont yukleyebilme
         * dekont yükleme ödeme yaptıktan sonra ödemeler panelin de dekont yükle butonu olmalı /  post 
        */

        public IActionResult Index() {
            return View();
        }

        [HttpGet("GetUserRoleId")]
        public IActionResult GetUserRoleId() {
            // yöneticinin bilgilerini getirme işlemi
            var token = Request.Cookies["accessToken"];
            Console.WriteLine(token);
            int apartmentResidentId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            var apartmentResident = _context.ApartmentManagers
                .Include(e => e.UserRoles)
                .FirstOrDefault();
            Console.WriteLine(apartmentResidentId);
            if(apartmentResident == null) {
                throw new ArgumentException("apartman sakini bulunamadı");
            }
            // apartman sakininin rol bilgisini de döndürmek lazım ki frontend de apartman sakininin rolüne göre yönlendirme yapabilsin
            return Ok(new returnRoleIdforNavigationsDto {
                userRole = apartmentResident.UserRoles.Role
            });
        }

        [HttpPut("updateResidentInfos")]
        public async Task<IActionResult> UpdateInfos([FromBody] ApartmentResidentDto dto) {
            // input validation
            sanitizeAndValidate.IsValidText(dto.Name);
            sanitizeAndValidate.IsValidText(dto.Surname);
            sanitizeAndValidate.IsValidPhoneNumber(dto.PhoneNumber);
            sanitizeAndValidate.IsValidEmail(dto.Email);
            sanitizeAndValidate.IsValidPassword(dto.Password!);

            int apartmentResidentId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            var apartmentResident = await _context.ApartmentResidents.FindAsync(apartmentResidentId);
            if(apartmentResident == null) {
                throw new ArgumentException("Apartman sakini bulunamadı.");
            }
            apartmentResident.Name = dto.Name;
            apartmentResident.Surname = dto.Surname;
            apartmentResident.PhoneNumber = dto.PhoneNumber;
            apartmentResident.Email = dto.Email;

            if(dto.Password != null && dto.NewPassword != null && dto.NewPasswordAgain != null) {
                if(dto.NewPassword != dto.NewPasswordAgain) {
                    throw new ArgumentException("Yeni şifreler eşleşmiyor.");
                }
                if(!passwordHash.VerifyPassword(apartmentResident.Password, dto.Password)) {
                    throw new ArgumentException("Eski şifre yanlış.");
                }
                apartmentResident.Password = passwordHash.HashPassword(dto.NewPassword);
            }
            await _context.SaveChangesAsync();
            return Ok(new SuccessResult {
                Message = "Bilgiler güncellendi."
            });
        }
        [HttpGet("myApartmentUnit")]
        public async Task<IActionResult> GetMyApartmentUnit() {
            int apartmentResidentId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            var apartmentResident = await _context.ApartmentResidents.FindAsync(apartmentResidentId);
            if(apartmentResident == null) {
                throw new ArgumentException("apartman sakini bulunamadı");
            }
            var residentApartmentUnit = await _context.ApartmentUnits.FindAsync(apartmentResident.ApartmentUnitId);

            if(apartmentResident == null) {
                throw new ArgumentException("Apartman sakini bulunamadı.");
            }
            return Ok(residentApartmentUnit);

        }
        [HttpGet("myApartmentUnitMaintenanceFees")]
        //tum apartman iadatlarını getirir burada da sayfalama gerekiyor
        public async Task<IActionResult> GetMyApartmentUnitPayments() {
            List<GetMyApartmentUnitPaymentsDto> myMaintenanceFees = new List<GetMyApartmentUnitPaymentsDto>();
            int apartmentResidentId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            var apartmentResidentMaintenanceFees = await _context.MaintenanceFees
                .Where(mf => mf.ApartmentResident.Id == apartmentResidentId)
                .ToListAsync();
            if(apartmentResidentMaintenanceFees == null) {
                throw new ArgumentException("herhangi bir aidat bilgisi bulunamadı.");
            }
            foreach(var mf in apartmentResidentMaintenanceFees) {
                myMaintenanceFees.Add(new GetMyApartmentUnitPaymentsDto {
                    Id = mf.Id,
                    Amount = mf.Amount,
                    DueDate = mf.DueDate,
                    IsPaid = mf.IsPaid,
                    PaymentDate = mf.PaymentDate,
                });
            }

            return Ok(myMaintenanceFees);
        }

        [HttpGet("myApartmentUnitSpecialFees")]
        public async Task<IActionResult> GetMyApartmentUnitSpecialFees() {
            List<GetMyApartmentUnitSpecialFeesDto> mySpecialFees = new List<GetMyApartmentUnitSpecialFeesDto>();
            int apartmentResidentId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            var apartmentResidentSpecialFees = await _context.ResidentsSpecificFees.Where(a => a.ResidentId == apartmentResidentId).ToListAsync();
            if(apartmentResidentSpecialFees == null) {
                throw new ArgumentException("herhangi bir özel ücret bulunamadı.");
            }
            foreach(var spf in apartmentResidentSpecialFees) {
                mySpecialFees.Add(new GetMyApartmentUnitSpecialFeesDto {
                    Id = spf.Id,
                    Name = spf.Name,
                    Description = spf.Description,
                    Amount = spf.Amount,
                    CreatedAt = spf.CreatedAt,
                    PaymentDate = spf.PaymentDate,
                    DueDate = spf.DueDate,
                    IsPaid = spf.IsPaid,
                });
            }
            return Ok(mySpecialFees);
        }
        [HttpPost("CreatePaymentNotification")]
        public async Task<IActionResult> CreatePaymentNotification([FromBody] CreatePaymentNotificationDto dto) {
            // ödeme bildirimi yapıldıktan sonra ödeme durumu beklemede olarak ayarlanacak

            // burada ödeme yapıldıktan sonra dekont yükleme işlemi yapılabilir
            // input validation
            sanitizeAndValidate.IsValidNumber(dto.MaintenanceFeeId);
            sanitizeAndValidate.IsValidNumber(dto.SpecialFeeId);
            sanitizeAndValidate.IsValidText(dto.NotificationMessage, 5, 100);

            // koşula bağlı dönen mesaj belirli olacağı için önceden sınıfı oluşturma(initialize)
            var successResult = new SuccessResult {
                Message = ""
            };

            if(dto.MaintenanceFeeId == 0 && dto.SpecialFeeId == 0) {
                throw new ArgumentException("hem aidat hem de özel ücret aynı anda ödenemez");
            }
            int apartmentResidentId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            if(dto.MaintenanceFeeId != 0) {
                var maintenanceFee = _context.MaintenanceFees.FirstOrDefault(mf => mf.Id == dto.MaintenanceFeeId && mf.ResidentId == apartmentResidentId && mf.IsPaid == false);
                if(maintenanceFee == null) {
                    throw new ArgumentException("Ödenmemiş aidat bulunamadı.");
                }
                var apartmentResident = _context.ApartmentResidents.FirstOrDefault(ar => ar.Id == apartmentResidentId);
                var apartment = _context.Apartments
                    .Include(x => x.ApartmentManager)
                    .FirstOrDefault(x => x.Id == maintenanceFee.ApartmentId);

                if(apartmentResident == null) {
                    throw new ArgumentException("Apartman sakini bulunamadı.");
                }
                if(apartment == null) {
                    throw new ArgumentException("Apartman bulunamadı.");
                }
                var paymentNotification = new PaymentNotifications {
                    MaintenanceFeeId = dto.MaintenanceFeeId,
                    SpecialFeeId = null,
                    ApartmentId = maintenanceFee.ApartmentId,
                    ResidentId = apartmentResidentId,
                    ManagerId = apartment.ManagerId,
                    Amount = maintenanceFee.Amount,
                    DueDate = maintenanceFee.DueDate,
                    NotificationMessage = dto.NotificationMessage,
                    ReceiptUrl = dto.ReceiptUrl,
                    // navigation properties
                    ResidentsSpecificFee = null, // çünkü bu bir aidat ödemesi
                    MaintenanceFee = maintenanceFee,
                    Apartment = apartment,
                    ApartmentResident = apartmentResident

                };
                maintenanceFee.PaymentDate = DateTime.UtcNow;
                maintenanceFee.IsPaid = true;

                _context.PaymentNotifications.Add(paymentNotification);
                _context.MaintenanceFees.Update(maintenanceFee);
                await _context.SaveChangesAsync();
                successResult.Message = "aidat ödemesi için ödeme bildirimi oluşturuldu";
            }
            else if(dto.SpecialFeeId != 0) {
                var specialFee = _context.ResidentsSpecificFees.FirstOrDefault(mf => mf.Id == dto.SpecialFeeId && mf.ResidentId == apartmentResidentId && mf.IsPaid == false);
                if(specialFee == null) {
                    throw new ArgumentException("Ödenmemiş aidat bulunamadı.");
                }
                var apartmentResident = _context.ApartmentResidents.FirstOrDefault(ar => ar.Id == apartmentResidentId);
                var apartment = _context.Apartments.FirstOrDefault(x => x.Id == specialFee.ApartmentId);

                if(apartmentResident == null) {
                    throw new ArgumentException("Apartman sakini bulunamadı.");
                }
                if(apartment == null) {
                    throw new ArgumentException("Apartman bulunamadı.");
                }
                var paymentNotification = new PaymentNotifications {
                    MaintenanceFeeId = null,
                    SpecialFeeId = dto.SpecialFeeId,
                    ApartmentId = specialFee.ApartmentId,
                    ResidentId = apartmentResidentId,
                    ManagerId = apartment.ManagerId,
                    Amount = specialFee.Amount,
                    DueDate = specialFee.DueDate,
                    NotificationMessage = dto.NotificationMessage,
                    ReceiptUrl = dto.ReceiptUrl,
                    // navigation properties
                    ResidentsSpecificFee = specialFee,
                    MaintenanceFee = null, // çünkü bu bir özel ücret ödemesi
                    Apartment = apartment,
                    ApartmentResident = apartmentResident

                };
                _context.PaymentNotifications.Add(paymentNotification);
                _context.ResidentsSpecificFees.Update(specialFee);
                await _context.SaveChangesAsync();
                successResult.Message = "özel ücret ödemesi için ödeme bildirimi oluşturuldu";
            }
            else {
                throw new ArgumentException("Ödeme bildirimi oluşturulamadı. Lütfen geçerli bir aidat ya da özel ücret ID'si girin.");
            }

            return Ok(successResult);
        }
        [HttpPut("UpdatePaymentNotification")]
        public async Task<IActionResult> UpdateMyPaymentNotification([FromBody] CreatePaymentNotificationDto dto) {
            var paymentNotification = await _context.PaymentNotifications.FindAsync(dto.PaymentNotificationId);
            if(paymentNotification == null) {
                throw new ArgumentException("Ödeme bildirimi bulunamadı.");
            }
            if(paymentNotification.Status != PaymentStatus.Beklemede) {
                throw new ArgumentException("Ödeme bildirimi zaten onaylandı ya da reddedildi.");
            }
            paymentNotification.NotificationMessage = dto.NotificationMessage;
            paymentNotification.ReceiptUrl = dto.ReceiptUrl;

            _context.PaymentNotifications.Update(paymentNotification);
            await _context.SaveChangesAsync();

            return Ok(new SuccessResult {
                Message = "Ödeme bildirimi başarılı bir şekilde güncellendi."
            });
        }
        [HttpGet("getMyPaymentNotification")]
        public async Task<IActionResult> GetMyPaymentNotification() {
            int apartmentResidentId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            var myPaymentNotifications = await _context.PaymentNotifications.Where(x => x.ResidentId == apartmentResidentId).ToListAsync();
            return Ok(myPaymentNotifications);
        }



    }
}