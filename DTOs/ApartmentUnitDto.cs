using MelihAkıncı_webTabanliAidatTakipSistemi.Models;

namespace MelihAkıncı_webTabanliAidatTakipSistemi.DTOs {
    public class ApartmentUnitDto {
        public int ApartmentId { get; set; } = 0;
        public int ApartmentUnitId { get; set; }
        public required int FloorNumber { get; set; }
        public required int ApartmentNumber { get; set; }
        public required string ApartmentType { get; set; }
        public required int SquareMeters { get; set; }
    }
}
