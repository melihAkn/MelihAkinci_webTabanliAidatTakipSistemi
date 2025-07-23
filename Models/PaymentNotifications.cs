using static MelihAkıncı_webTabanliAidatTakipSistemi.classes.PaymentStatusEnum;

namespace MelihAkıncı_webTabanliAidatTakipSistemi.Models {
    public class PaymentNotifications {
        public int Id { get; set; }
        public int? MaintenanceFeeId { get; set; } // eğer 0 ise ya hatalı işlemdir ya da aidat ücreti değil özel ücrettir
        public int? SpecialFeeId { get; set; } // eğer 0 ise ya hatalı işlemdir ya da özel ücret değil aidat ücretidir
        public required int ApartmentId { get; set; }
        public required int ResidentId { get; set; }
        public required int ManagerId { get; set; }
        public required decimal Amount { get; set; }
        public required DateTime DueDate { get; set; }
        public DateTime? PaymentDate { get; set; } = DateTime.UtcNow;
        public PaymentStatus Status { get; set; } = PaymentStatus.Beklemede;
        public required string NotificationMessage { get; set; } // kat malikinin ödemeyi yaparken doldurması gereken mesaj yani yönetici görecek bunu
        public string? Message { get; set; } = string.Empty; //ödeme reddedilirse bu alan dolu olmak zorunda ama eğer onaylanırsa dolu olması şart değil
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? ReceiptUrl { get; set; } = null; // dekont için url opsiyonel olabilir gibi ama o zaman da farklı sorunlar çıkabilir mentordan cevap gelsin bakacam. bunu boyle yukleme githuba

        // navigation properties
        public ResidentsSpecificFee? ResidentsSpecificFee { get; set; }
        public MaintenanceFee? MaintenanceFee { get; set; }
        public required ApartmentResident ApartmentResident { get; set; }
        public required Apartment Apartment { get; set; }

    }
}
