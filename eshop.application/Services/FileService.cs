using eshop.application.Common.Models;
using eshop.application.DTOs.File.Response;

namespace eshop.application.Services
{
    public class FileService
    {
        private const long MaxFileSizeBytes = 1024 * 1024; // 1MB
        private static readonly string[] AllowedExtensions = [".webp", ".jpg", ".jpeg", ".png"];

        private readonly ILogger<FileService> _logger;
        private readonly IWebHostEnvironment _environment;

        public FileService(ILogger<FileService> logger, IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        /// <summary>
        /// 上傳商品圖片
        /// </summary>
        public async Task<ResponseDataModel<FileUploadResponse>> UploadProductImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return ApiResponse.Fail<FileUploadResponse>(ResponseCode.INVALID_PARAMS, "請選擇檔案");
            }

            if (file.Length > MaxFileSizeBytes)
            {
                return ApiResponse.Fail<FileUploadResponse>(ResponseCode.INVALID_PARAMS, "檔案大小不可超過 1MB");
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
            {
                return ApiResponse.Fail<FileUploadResponse>(ResponseCode.INVALID_PARAMS, "檔案格式僅支援 webp、jpg、png");
            }

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var bytes = memoryStream.ToArray();

            // 副檔名可被使用者端偽造,實際內容再比對簽章位元組(magic bytes)確認真的是圖片,
            // 避免有心人把惡意內容改副檔名塞進 wwwroot 這個對外直接提供靜態檔案的目錄
            if (!IsValidImageContent(bytes, extension))
            {
                return ApiResponse.Fail<FileUploadResponse>(ResponseCode.INVALID_PARAMS, "檔案內容與副檔名不符");
            }

            var fileName = $"{Guid.NewGuid()}{extension}";
            var folderPath = Path.Combine(_environment.WebRootPath, "images", "product");
            Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, fileName);
            await System.IO.File.WriteAllBytesAsync(filePath, bytes);

            _logger.LogInformation("商品圖片上傳成功:{FileName}", fileName);

            var url = $"/images/product/{fileName}";
            return ApiResponse.Success(new FileUploadResponse { Url = url });
        }

        private static bool IsValidImageContent(byte[] bytes, string extension)
        {
            return extension switch
            {
                ".png" => bytes.Length >= 4 && bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47,
                ".jpg" or ".jpeg" => bytes.Length >= 3 && bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF,
                ".webp" => bytes.Length >= 12
                    && bytes[0] == 0x52 && bytes[1] == 0x49 && bytes[2] == 0x46 && bytes[3] == 0x46 // "RIFF"
                    && bytes[8] == 0x57 && bytes[9] == 0x45 && bytes[10] == 0x42 && bytes[11] == 0x50, // "WEBP"
                _ => false,
            };
        }
    }
}
