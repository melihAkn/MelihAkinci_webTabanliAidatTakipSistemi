using static MelihAkıncı_webTabanliAidatTakipSistemi.classes.PaymentStatusEnum;

namespace MelihAkıncı_webTabanliAidatTakipSistemi.DTOs {
    public class GetMyApartmentUnitSpecialFeesDto {
        public required int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public required decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PaymentDate { get; set; } = null;
        public required DateTime DueDate { get; set; }
        public bool IsPaid { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Beklemede;




    }
}
