namespace MelihAkıncı_webTabanliAidatTakipSistemi.DTOs {
    public class CreatePaymentNotificationDto {
        public int PaymentNotificationId { get; set; } = 0; // guncelleme işlemi için 0 sa ödeme bildirimidir.
        public int SpecialFeeId { get; set; } = 0;
        public int MaintenanceFeeId { get; set; } = 0;
        public required string NotificationMessage { get; set; } = string.Empty;
        public required IFormFile ReceiptFile { get; set; } // dekont
        public string? ReceiptUrl { get; set; } = string.Empty;
    }
}
