using MelihAkıncı_webTabanliAidatTakipSistemi.classes;

namespace MelihAkıncı_webTabanliAidatTakipSistemi.DTOs {
    public class PaymentNotificationDto {
        public required int paymentNotificationId { get; set; }
        // apartman bilgileri
        public required string ApartmentName { get; set; }
        public required string ApartmentAdress { get; set; } = string.Empty;

        // kat malki bilgileri
        public required string ResidentName { get; set; } = string.Empty;
        public required string ResidentSurname { get; set; } = string.Empty;
        public required string ResidentPhoneNumber { get; set; } = string.Empty;
        public required string ResidentEmail { get; set; } = string.Empty;
        // aidat ve ya özel ücret bilgileri
        public string PaymentName { get; set; } = "aidat"; // aidat ücreti ya da özel ücret adı
        public string PaymentDescription { get; set; } = "aidat ödemesi"; // aidat ücreti ya da özel ücret açıklaması
        public required decimal Amount { get; set; }
        public required DateTime? PaymentDate { get; set; }
        public required DateTime DueDate { get; set; }
        public required PaymentStatusEnum.PaymentStatus Status { get; set; }
        public string ReceiptUrl { get; set; } = string.Empty; // dekont için url opsiyonel olabilir gibi ama o zaman da farklı sorunlar çıkabilir mentordan cevap gelsin bakacam. bunu boyle yukleme githuba

        public required string NotificationMessage { get; set; } // kat malikinin ödemeyi yaparken doldurması gereken mesaj yani yönetici görecek bunu
    }
}
