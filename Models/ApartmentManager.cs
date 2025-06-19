
using System.Data;

namespace MelihAkıncı_webTabanliAidatTakipSistemi.Models {
    public class ApartmentManager {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Surname { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Email { get; set; }
        public required  string Username { get; set; }
        public required string Password { get; set; }
        public required int RoleId { get; set; }
        public required UserRoles UserRoles { get; set; } // Navigation Property

    }
}
