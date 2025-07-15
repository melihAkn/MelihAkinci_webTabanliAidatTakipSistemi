using Sprache;
using System.Text.RegularExpressions;

namespace MelihAkıncı_webTabanliAidatTakipSistemi.classes {
    /// <summary>Input validator base class it contains a validator for every data in this project </summary>
    public class SanitizeAndValidate {
        public class InputSanitizer {
            public string SanitizeString(string unSanitizedText) {
                string sanitized = Regex.Replace(unSanitizedText, @"[^\u0000-\u007F]+", string.Empty); // ASCII dışı karakterleri sil
                sanitized = Regex.Replace(sanitized, @"<[^>]*>", string.Empty); // HTML tag'leri
                return sanitized.Trim();
            }
        }
        readonly InputSanitizer inputSanitizer = new InputSanitizer();
        /// <summary>
        /// it waits for string input and checks if it valid length and do the regex validation
        /// </summary>
        /// <param name="name"></param>
        /// <param name="maxLength"></param>
        /// <param name="minLength"> </param>
        /// <returns> bool </returns>
        public bool IsValidText(string text, int minLength = 2, int maxLength = 50) {
            if(string.IsNullOrWhiteSpace(text)) {
                throw new ArgumentException("boş ya da geçersiz metin");
            }
            // sanitize the text
            text = inputSanitizer.SanitizeString(text);
            if(text.Length < minLength || text.Length > maxLength) {
                throw new ArgumentException($"metin uzunluğu {minLength} ile {maxLength} arasında olmalıdır");
            }
            var regex = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z0-9\s]+$");
            if(!regex.IsMatch(text)) {
                throw new ArgumentException("metin sadece harf, rakam ve boşluk içerebilir");
            }
            return true;
        }
        public bool IsValidEmail(string email) {
            if(string.IsNullOrWhiteSpace(email)) return false;
            try {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch {
                return false;
            }
        }
        public bool IsValidPhoneNumber(string phoneNumber) {
            // regex den emin değilim o yuzden iyice öğrenmem lazım bu regex i
            if(string.IsNullOrWhiteSpace(phoneNumber)) return false;
            return System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, @"^(\+90|0)?5\d{9}$") ? true : throw new ArgumentException("apartman bulunamadı ve ya yöneticisi olduğunuz apartman yok");
        }
        public bool IsValidPassword(string password, int minLength = 6, int maxLength = 20) {
            if(string.IsNullOrWhiteSpace(password)) {
                throw new ArgumentException("şifre boş olamaz");
            }
            if(password.Length > 8 && password.Length < 256) {
                throw new ArgumentException($"şifre {minLength} ile {maxLength} arasında olmak zorunda");
            }
            return true;
        }
        public bool IsValidNumber(int number, int minValue = 0, int maxValue = 1000000) {

            if(number < minValue || number > maxValue) {
                throw new ArgumentException($"sayı {minValue} ile {maxValue} arasında olmalıdır");
            }
            return true;
        }
        public bool IsValidDecimal(decimal number, decimal minValue = 0, decimal maxValue = 1000000) {

            return number >= minValue && number <= maxValue ? true : throw new ArgumentException("apartman bulunamadı ve ya yöneticisi olduğunuz apartman yok");
        }
    }
}
