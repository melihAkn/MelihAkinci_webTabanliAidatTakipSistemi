using MelihAkıncı_webTabanliAidatTakipSistemi.classes;
using static MelihAkıncı_webTabanliAidatTakipSistemi.classes.PaymentStatusEnum;

namespace MelihAkıncı_webTabanliAidatTakipSistemi.DTOs {
    public class MaintenanceFeeDto {
        public int Id { get; set; }
        public required decimal Amount { get; set; }
        public required DateTime DueDate { get; set; } // son ödeme tarihi
        public bool IsPaid { get; set; } = false;
        public DateTime? PaymentDate { get; set; } = null;
        public string ResidentName { get; set; } = string.Empty; // Apartman sakininin adı
        public string ResidentSurname { get; set; } = string.Empty; // Apartman sakininin soyadı

        public PaymentStatus Status { get; set; } = PaymentStatus.Beklemede; // Ödeme durumu
    }
}
