namespace MelihAkıncı_webTabanliAidatTakipSistemi.DTOs {
    public class ApartmentResidentDto {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Surname { get; set; }
        public required string PhoneNumber { get; set; }
        public string? Password { get; set; }
        public string? NewPassword { get; set; }
        public string? NewPasswordAgain { get; set; }
        public required string Email { get; set; }
        public int ApartmentUnitId { get; set; } = 0;


    }
}
