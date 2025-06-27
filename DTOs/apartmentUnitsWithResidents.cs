namespace MelihAkıncı_webTabanliAidatTakipSistemi.DTOs {
    public class apartmentUnitsWithResidents {
        public int ApartmentUnitId { get; set; }
        public required int FloorNumber { get; set; }
        public required int ApartmentNumber { get; set; }
        public required string ApartmentType { get; set; }
        public required int SquareMeters { get; set; }
        public required bool isHaveResident { get; set; } = false;
        public List<ApartmentResidentDto> ApartmentResidents { get; set; } = new();
    }
}
