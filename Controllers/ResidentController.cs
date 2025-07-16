using MelihAkıncı_webTabanliAidatTakipSistemi.classes;
using MelihAkıncı_webTabanliAidatTakipSistemi.Data;
using MelihAkıncı_webTabanliAidatTakipSistemi.DTOs;
using MelihAkıncı_webTabanliAidatTakipSistemi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

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
         * aidat bilgilerini / get ---------------- done
         * özel ücretlerini gorebilme / get ---------------- done
         * 
         * aidat ödeyebilme  / post ---------------- done
         * özel ücretini ödeyebilme / post ---------------- done
         * yapmış olduğu ödemeleri ve durumlarını(red, beklemede, onay vb.) görüntüleme / get ---------------- done
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
                    Status = mf.Status
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
                    Status = spf.Status
                });
            }
            return Ok(mySpecialFees);
        }
        [HttpPost("payMyMaintenanceFee")]
        public async Task<IActionResult> PayMyApartmentUnitMaintenanceFee([FromBody] PayOrAllowOrDenyMaintenanceOrSpecialFeeDto dto) {
            // input validation
            sanitizeAndValidate.IsValidNumber(dto.MaintenanceFeeId);

            var myMaintenancefee = await _context.MaintenanceFees.Where(mf => mf.IsPaid == false && mf.Id == dto.MaintenanceFeeId).FirstOrDefaultAsync();
            if(myMaintenancefee == null) {
                throw new ArgumentException("Ödenmemiş aidat bulunamadı.");
            }
            myMaintenancefee!.IsPaid = true;
            myMaintenancefee!.PaymentDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return Ok(new SuccessResult {
                Message = "aidat başarılı bir şekilde ödendi"
            });
        }
        [HttpPost("payMySpecialFee")]
        public async Task<IActionResult> PayMyApartmentUnitSpecialFee([FromBody] PayOrAllowOrDenyMaintenanceOrSpecialFeeDto dto) {
            // input validation
            sanitizeAndValidate.IsValidNumber(dto.SpecialFeeId);

            var mySpecialFee = await _context.ResidentsSpecificFees.Where(mf => mf.IsPaid == false && mf.Id == dto.SpecialFeeId).FirstOrDefaultAsync();
            if(mySpecialFee == null) {
                throw new ArgumentException("Ödenmemiş özel ücret bulunamadı.");
            }
            mySpecialFee!.IsPaid = true;
            mySpecialFee!.PaymentDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new SuccessResult {
                Message = "özel ücret başarılı bir şekilde ödendi"
            });
        }
        public IActionResult CreatePaymentNotification() {
            // apartman sakini için ödeme bildirimi yapabileceği rota
            // ya ödeme yaptıktan sonra otomatik olarak ödeme bildirimi yapılacak ya da buradan manuel olarak yapılacak
            // ödeme bildirimi yapıldıktan sonra ödeme durumu beklemede olarak ayarlanacak




            // burada ödeme yapıldıktan sonra dekont yükleme işlemi yapılabilir


            return Ok();
        }





    }
}