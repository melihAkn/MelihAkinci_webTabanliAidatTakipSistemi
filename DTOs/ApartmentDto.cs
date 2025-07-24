using MelihAkıncı_webTabanliAidatTakipSistemi.Models;
using System.ComponentModel.DataAnnotations;

namespace MelihAkıncı_webTabanliAidatTakipSistemi.DTOs {
    public class ApartmentDto {
        public int ApartmentId { get; set; } = 0;
        public required string Name { get; set; }
        public required int MaxAmountOfResidents { get; set; }
        public required string Address { get; set; }
        public decimal MaintenanceFeeAmount { get; set; } = 0.0m;
        public required int FloorCount { get; set; }
        public required int ApartmentUnitCountForEachFloor { get; set; }
        public required string Iban { get; set; } = string.Empty;
        public bool IsWantedToAutoFillApartmentUnits { get; set; } = false;
    }
}
