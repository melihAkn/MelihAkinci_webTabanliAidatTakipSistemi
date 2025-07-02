using MelihAkıncı_webTabanliAidatTakipSistemi.classes;
using static MelihAkıncı_webTabanliAidatTakipSistemi.classes.PaymentStatusEnum;

namespace MelihAkıncı_webTabanliAidatTakipSistemi.DTOs {
    public class GetMyApartmentUnitPaymentsDto {
        public required int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? PaymentDate { get; set; } = null;
        public PaymentStatus Status { get; set; } = PaymentStatus.Beklemede;

    }
}
