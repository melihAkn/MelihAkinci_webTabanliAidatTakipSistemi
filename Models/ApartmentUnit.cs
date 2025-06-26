namespace MelihAkıncı_webTabanliAidatTakipSistemi.Models {
    public class ApartmentUnit {
        public int Id { get; set; }
        public int ApartmentId { get; set; }
        public required int FloorNumber { get; set; }
        public required int ApartmentNumber { get; set; }
        public string ApartmentType { get; set; } = "2+1"; // 1+1, 2+1, etc.
        public int SquareMeters { get; set; } = 90; // Square meters of the apartment unit default 90 m²
        public required Boolean IsOccupied { get; set; } = false; // Default is not occupied
        public required Apartment Apartment { get; set; } // Navigation Property
    }
}
