using Microsoft.IdentityModel.Tokens;

namespace MelihAkıncı_webTabanliAidatTakipSistemi.classes {
    public class FileHandler {
        public async Task<bool> SaveFile(IFormFile file, string directoryPath, string uniqueFileName) {

            var filePath = Path.Combine(directoryPath, uniqueFileName);

            using(var stream = new FileStream(filePath, FileMode.Create)) {
                await file.CopyToAsync(stream);
            }
            return true;
        }
        private static bool ScanFileForMalware(IFormFile file) {
            return false;
        }
    }
}
