using eshop.application.Common;
using eshop.application.Common.Models;
using eshop.application.Configurations;
using eshop.application.DTOs.File.Response;
using eshop.application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace eshop.application.Controllers.Admin
{
    /// <summary>
    /// 檔案上傳
    /// </summary>
    [Route("upload")]
    [Authorize(Policy = AuthConstants.Policy.AdminOnly)]
    [ApiExplorerSettings(GroupName = SwaggerConst.Admin)]
    public class FileController : BaseApiController
    {
        private readonly FileService _fileService;

        public FileController(ILogger<FileController> logger, FileService fileService)
            : base(logger)
        {
            _fileService = fileService;
        }

        /// <summary>
        /// 上傳商品圖片
        /// </summary>
        /// <param name="file">圖片檔案(webp/jpg/png,大小上限 1MB)</param>
        [Authorize(Policy = AuthConstants.Permission.ProductEdit)]
        [HttpPost("product")]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponseDataModel<FileUploadResponse>))]
        public async Task<IActionResult> UploadProductImageAsync(IFormFile file)
        {
            return Ok(await _fileService.UploadProductImageAsync(file));
        }
    }
}
