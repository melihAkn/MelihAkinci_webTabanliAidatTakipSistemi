namespace MelihAkıncı_webTabanliAidatTakipSistemi.DTOs {
    public class ApartmentResidentDto {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Surname { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Email { get; set; }
        public string Username { get; set; } = string.Empty;
        public int ApartmentUnitId { get; set; } = 0;


    }
}
