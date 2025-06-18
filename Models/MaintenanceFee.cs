namespace MelihAkıncı_webTabanliAidatTakipSistemi.Models {
    public class MaintenanceFee {
        public int Id { get; set; }
        public required int ResidentId { get; set; }
        public required decimal Amount { get; set; }
        public required DateTime DueDate { get; set; }
        public bool IsPaid { get; set; } = false;
        public DateTime? PaymentDate { get; set; } = null;

    }
}
