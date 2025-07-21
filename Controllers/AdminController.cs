using MelihAkıncı_webTabanliAidatTakipSistemi.classes;
using MelihAkıncı_webTabanliAidatTakipSistemi.Data;
using MelihAkıncı_webTabanliAidatTakipSistemi.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MelihAkıncı_webTabanliAidatTakipSistemi.Controllers {
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("[controller]")]
    public class AdminController(AppDbContext context) : Controller {
        private readonly AppDbContext _context = context;
        private readonly SanitizeAndValidate sanitizeAndValidate = new SanitizeAndValidate();
        private readonly EmailActions emailAction = new EmailActions();
        private readonly GetNthMonth28Day getNthMonth28Day = new GetNthMonth28Day();

        [HttpGet("notify-all-users")]
        public async Task<IActionResult> NotifyUsersForDueDate([FromBody] NotifyUserDto dto) {
            // ayın 21 inde ve 28 inde bildirim gönderilecek
            if(dto.dayofMonth == 21 || dto.dayofMonth == 27 || dto.dayofMonth == 28) {
                throw new ArgumentException("Gün numarası 21, 27 veya 28 olmalıdır");
            }
            var apartmentResidents = await _context.ApartmentResidents.ToListAsync();
            string mailSubject = string.Empty;
            string mailBody = string.Empty;

            foreach(var resident in apartmentResidents) {
                var maintenanceFee = await _context.MaintenanceFees
                    .Where(fee => fee.ResidentId == resident.Id && fee.DueDate == getNthMonth28Day.GetDueDate(0))
                    .FirstOrDefaultAsync();
                if(maintenanceFee == null) {
                    Console.WriteLine($"Aidat ücreti bulunamadı: {resident.Name} {resident.Surname}");
                }
                else {
                    mailSubject = $"Aidat Ödeme Bildirimi - {resident.Name} {resident.Surname}";
                    mailBody = $"""
                        Merhaba {resident.Name} {resident.Surname},
                        \n\nAidat ödemeniz için son tarih: {maintenanceFee.DueDate.ToString("dd/MM/yyyy") ?? "Bilinmiyor"}.
                        \n\n ödemeniz {maintenanceFee.Amount} TL dir.
                        \nLütfen ödemenizi zamanında yapınız.
                        \n\nTeşekkürler,Yönetim
                        """;

                    emailAction.SendEmail(resident.Email, mailSubject, mailBody);

                }

                var specialFee = await _context.ResidentsSpecificFees
                    .Where(fee => fee.ResidentId == resident.Id && fee.DueDate == getNthMonth28Day.GetDueDate(0))
                    .ToListAsync();
                if(specialFee.Count == 0) {
                    Console.WriteLine("Özel ücret bulunamadı: {resident.Name} {resident.Surname}");
                }
                else {
                    foreach(var fee in specialFee) {
                        mailSubject = $"Özel ÜCret Ödeme Bildirimi - {resident.Name} {resident.Surname}";
                        mailBody = $"""
                            Merhaba {resident.Name} {resident.Surname},
                            \n\nÖzel Ücret ödemeniz için son tarih: {fee.DueDate.ToString("dd/MM/yyyy") ?? "Bilinmiyor"}.
                            \n\nÖdemeniz {fee.Amount} TL dir.
                            \nLütfen ödemenizi zamanında yapınız.
                            \n\nTeşekkürler,\nYönetim
                            """;
                        emailAction.SendEmail(resident.Email, mailSubject, mailBody);
                    }
                }
            }
            return Ok("Tüm kullanıcılara ödeme bildirimi gönderildi");
        }
    }
}
