using Microsoft.AspNetCore.Identity;

namespace MelihAkıncı_webTabanliAidatTakipSistemi.classes {
    public class PasswordHash {

        private readonly PasswordHasher<object> _hasher = new PasswordHasher<object>();
        // şimdilik boş bir obje geçiyorum.
        public string HashPassword(string plainPassword) {
            return _hasher.HashPassword(new object(), plainPassword);
        }

        public bool VerifyPassword(string hashedPassword, string plainPassword) {
            var result = _hasher.VerifyHashedPassword(new object(), hashedPassword, plainPassword);
            return result == PasswordVerificationResult.Success;
        }


    }
}
