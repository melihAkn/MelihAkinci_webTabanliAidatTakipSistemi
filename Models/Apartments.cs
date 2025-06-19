namespace MelihAkıncı_webTabanliAidatTakipSistemi.Models {
    public class Apartments {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required int ManagerId { get; set; }
        public int MaxAmountOfResidents { get; set; }
        public required string Address { get; set; }
        public required ApartmentManager ApartmentManager { get; set; } // Navigation Property
        public ICollection<ApartmentUnit> ApartmentUnits { get; set; } = new List<ApartmentUnit>();






    }
}
