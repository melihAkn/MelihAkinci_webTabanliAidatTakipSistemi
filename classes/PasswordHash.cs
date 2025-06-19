using Microsoft.AspNetCore.Identity;

namespace MelihAkıncı_webTabanliAidatTakipSistemi.classes {
    public class PasswordHash {

        private readonly PasswordHasher<object> _hasher = new PasswordHasher<object>();

        public string HashPassword(string plainPassword) {
            return _hasher.HashPassword(null, plainPassword);
        }

        public bool VerifyPassword(string hashedPassword, string plainPassword) {
            var result = _hasher.VerifyHashedPassword(null, hashedPassword, plainPassword);
            return result == PasswordVerificationResult.Success;
        }


    }
}
