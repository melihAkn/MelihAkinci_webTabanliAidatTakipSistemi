using MelihAkıncı_webTabanliAidatTakipSistemi.Models;
using static MelihAkıncı_webTabanliAidatTakipSistemi.classes.PaymentStatusEnum;

namespace MelihAkıncı_webTabanliAidatTakipSistemi.Models {
    public class ResidentsSpecificFee {
        public int Id { get; set; }
        public required int ApartmentId { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required decimal Amount { get; set; }
        public required int ResidentId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? PaymentDate { get; set; } = null;
        public required DateTime DueDate { get; set; }
        public bool IsPaid { get; set; } = false;
        public required ApartmentResident ApartmentResident { get; set; } // Navigation Property

    }
}
