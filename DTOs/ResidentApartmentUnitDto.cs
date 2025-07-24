namespace MelihAkıncı_webTabanliAidatTakipSistemi.DTOs {
    public class ResidentApartmentUnitDto {
        public required int Id { get; set; }

        public required int FloorNumber { get; set; }
        public required int ApartmentNumber { get; set; }
        public string? ApartmentType { get; set; }
        public int SquareMeters { get; set; }
        public string Address { get; set; } = string.Empty;
        public decimal MaintenanceFeeAmount { get; set; } = 0.0m;
        public string Iban { get; set; } = string.Empty;
        public string ManagerNameAndSurname { get; set; } = string.Empty;
        public string ApartmentName { get; set; } = string.Empty;

    }
}
