namespace MelihAkıncı_webTabanliAidatTakipSistemi.Models {
    public class ApartmentResident {

        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Surname { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Email { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string RoleId { get; set; }
        public required int ApartmentUnitId { get; set; }
    }
}
