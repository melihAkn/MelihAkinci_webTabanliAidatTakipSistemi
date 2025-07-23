using Microsoft.IdentityModel.Tokens;

namespace MelihAkıncı_webTabanliAidatTakipSistemi.classes {
    public class FileHandler {

        public async Task<string> GetFilePath(string directoryPath, string extension, string? existingFilePath = null) {
            // guncelleme işlemleri için halihazırda olan dosya yolunu atayarak öncekinin üstüe yazılması
            var uniqueFileName = !string.IsNullOrEmpty(existingFilePath)
            ? existingFilePath
            : $"{Guid.NewGuid()}{extension}";

            var filePath = Path.Combine(directoryPath, uniqueFileName);
            return filePath;
        }
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
