using MelihAkıncı_webTabanliAidatTakipSistemi.Models;

namespace MelihAkıncı_webTabanliAidatTakipSistemi.DTOs {
    public class ApartmentManagerDto {
        public required string Name { get; set; }
        public required string Surname { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Email { get; set; }
        // şifre ve şifre tekrarı zorunlu değil
        public string? Password { get; set; }
        public string? NewPassword { get; set; }
        public string? NewPasswordAgain { get; set; }

        public required int RoleId = 2;
    }
}
