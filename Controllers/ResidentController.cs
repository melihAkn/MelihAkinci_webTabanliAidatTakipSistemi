using MelihAkıncı_webTabanliAidatTakipSistemi.classes;
using MelihAkıncı_webTabanliAidatTakipSistemi.Data;
using MelihAkıncı_webTabanliAidatTakipSistemi.DTOs;
using MelihAkıncı_webTabanliAidatTakipSistemi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MelihAkıncı_webTabanliAidatTakipSistemi.Controllers {

    [Authorize(Roles = "ApartmentResident")]
    [ApiController]
    [Route("[controller]")]
    public class ResidentController : Controller {

        private readonly AppDbContext _context;
        private readonly PasswordHash passwordHash = new PasswordHash();
        public ResidentController(AppDbContext context) {
            _context = context;
        }
        /*
         * bilgilerini guncelleyebilme / put --------------------------------------- done
         * kendi dairesini gorebilme / get --------------------------------------- done
         * aidat bilgilerini / get ---------------- done
         * özel ücretlerini gorebilme / get ---------------- done
         * 
         * aidat ödeyebilme  / post
         * özel ücretini ödeyebilme / post
         * yapmış olduğu ödemeleri ve durumlarını(red, onay vb.) goruntuleme / get
         * dekont yükleme ödeme yaptıktan sonra ödemeler panelin de dekont yükle butonu olmalı / post 
        */

        public IActionResult Index() {
            return View();
        }
        [HttpPut("updateResidentInfos")]
        public async Task<IActionResult> UpdateInfos([FromBody] ApartmentResidentDto dto) {
            if(string.IsNullOrWhiteSpace(dto.Name) ||
               string.IsNullOrWhiteSpace(dto.Surname) ||
               string.IsNullOrWhiteSpace(dto.PhoneNumber) ||
               string.IsNullOrWhiteSpace(dto.Email)) {
                throw new ArgumentException("Zorunlu alanlar boş bırakılamaz.");
            }
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
        [HttpGet("myApartmentUnitPayments")]
        public async Task<IActionResult> GetMyApartmentUnitPayments() {
            int apartmentResidentId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            var apartmentResidentMaintenanceFees = await _context.MaintenanceFees
                .Where(mf => mf.ApartmentResident.Id == apartmentResidentId)
                .ToListAsync();
            if(apartmentResidentMaintenanceFees == null) {
                throw new ArgumentException("Apartman sakini bulunamadı.");
            }
            return Ok(apartmentResidentMaintenanceFees);
        }

        [HttpGet("myApartmentUnitSpecialFees")]
        public async Task<IActionResult> GetMyApartmentUnitSpecialFees() {
            int apartmentResidentId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            var apartmentResidentSpecialFees = await _context.ResidentsSpecificDebts.Where(a => a.ResidentId == apartmentResidentId).ToListAsync();
            if(apartmentResidentSpecialFees == null) {
                throw new ArgumentException("Apartman sakini bulunamadı.");
            }
            return Ok(apartmentResidentSpecialFees);
        }
    }
}