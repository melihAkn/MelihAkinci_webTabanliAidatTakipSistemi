using static MelihAkıncı_webTabanliAidatTakipSistemi.classes.PaymentStatusEnum;

namespace MelihAkıncı_webTabanliAidatTakipSistemi.DTOs {
    public class AllowOrDenyPaymentNotificationDto {
        public int PaymentNotificationId { get; set; }
        public string? Message { get; set; } = string.Empty; //ödeme reddedilirse bu alan dolu olmak zorunda
    }
}
