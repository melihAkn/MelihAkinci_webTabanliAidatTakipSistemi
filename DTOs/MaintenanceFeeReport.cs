namespace MelihAkıncı_webTabanliAidatTakipSistemi.DTOs {
    public class MaintenanceFeeReport {
        public string ApartmentName { get; set; } = string.Empty;
        public string ManagerName { get; set; } = string.Empty;
        public object[] PaidMaintenanceFees { get; set; } = [];
        public object[] UnPaidMaintenanceFees { get; set; } = [];
        public int TotalMaintenanceFees { get; set; } = 0;
        public int TotalPaidMaintenanceFees { get; set; } = 0;
        public int TotalUnPaidMaintenanceFees { get; set; } = 0;

    }
}
