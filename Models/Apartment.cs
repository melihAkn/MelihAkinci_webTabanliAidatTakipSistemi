namespace MelihAkıncı_webTabanliAidatTakipSistemi.Models {
    public class Apartment {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required int ManagerId { get; set; }
        public int MaxAmountOfResidents { get; set; }
        public required string Address { get; set; }
        public decimal MaintenanceFeeAmount { get; set; } = 0.0m;
        public required int FloorCount { get; set; }
        public required int ApartmentUnitCountForEachFloor { get; set; }
        public required string Iban { get; set; } = string.Empty;
        public required ApartmentManager ApartmentManager { get; set; } // Navigation Property
        public ICollection<ApartmentUnit> ApartmentUnits { get; set; } = new List<ApartmentUnit>();

    }
}
