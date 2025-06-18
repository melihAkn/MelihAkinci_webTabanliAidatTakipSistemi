namespace MelihAkıncı_webTabanliAidatTakipSistemi.Models {
    public class ApartmentUnit {
        public int Id { get; set; }
        public int ApartmentId { get; set; }
        public required int FloorNumber { get; set; }
        public required int ApartmentNumber { get; set; }
    }
}
