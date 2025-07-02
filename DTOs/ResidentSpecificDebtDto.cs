using static MelihAkıncı_webTabanliAidatTakipSistemi.classes.PaymentStatusEnum;

namespace MelihAkıncı_webTabanliAidatTakipSistemi.DTOs {
    public class ResidentSpecificDebtDto {
        public required string Name { get;set;}
        public required string Description { get; set; }
        public required decimal Price { get; set; }
        public int ResidentId { get; set; } = 0;
        public DateTime DueDate { get; set; }
        public bool IsPaid { get; set; } = false;
        public DateTime? PaymentDate { get; set; } = null;
        public string ResidentName { get; set; } = string.Empty;
        public string ResidentSurname { get; set; } = string.Empty;
        public PaymentStatus Status { get; set; } = PaymentStatus.Beklemede;


    }
}
