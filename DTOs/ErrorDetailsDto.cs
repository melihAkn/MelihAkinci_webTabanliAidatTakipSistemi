namespace MelihAkıncı_webTabanliAidatTakipSistemi.DTOs {
    public class ErrorDetailsDto {
        public int StatusCode { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Detail { get; set; }
        public string? Path { get; set; }
        public string? TraceId { get; set; }
    }
}
