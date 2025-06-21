namespace MelihAkıncı_webTabanliAidatTakipSistemi.Models {
    public class ApartmentResident {

        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Surname { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Email { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required int RoleId { get; set; }
        public required int ApartmentUnitId { get; set; }
        public Boolean IsFirstLogin { get; set; } = true;
        public required ApartmentUnit ApartmentUnit { get; set; } // Navigation Property
        public required UserRoles UserRoles { get; set; } // Navigation Property

    }
}
