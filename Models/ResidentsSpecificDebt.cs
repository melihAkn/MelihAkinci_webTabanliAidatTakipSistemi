namespace MelihAkıncı_webTabanliAidatTakipSistemi.Models {
    public class ResidentsSpecificDebt {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required decimal Price { get; set; }
        public required int ResidentId { get; set; }
        public required ApartmentResident ApartmentResident { get; set; } // Navigation Property

    }
}
