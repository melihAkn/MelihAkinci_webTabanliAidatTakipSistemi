namespace MelihAkıncı_webTabanliAidatTakipSistemi.DTOs {
    public class SpecialFeeReport {
        public string ApartmentName { get; set; } = string.Empty;
        public string ManagerName { get; set; } = string.Empty;
        public object[] PaidSpecialFees { get; set; } = [];
        public object[] UnpaidSpecialFees { get; set; } = [];
        public int TotalSpecialFees { get; set; } = 0;
        public int TotalPaidSpecialFees { get; set; } = 0;
        public int TotalUnPaidSpecialFees { get; set; } = 0;
    }
}
