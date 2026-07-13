using eshop.application.Common;
using eshop.application.Common.Models;
using eshop.application.Configurations;
using eshop.application.DTOs.File.Response;
using eshop.application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace eshop.application.Controllers.Web
{
    /// <summary>
    /// 檔案上傳(前台)
    /// </summary>
    [Route("file")]
    [Authorize(Policy = AuthConstants.Policy.WebOnly)]
    [ApiExplorerSettings(GroupName = SwaggerConst.Front)]
    public class FileController : BaseApiController
    {
        private readonly FileService _fileService;

        public FileController(ILogger<FileController> logger, FileService fileService)
            : base(logger)
        {
            _fileService = fileService;
        }

        /// <summary>
        /// 上傳大頭貼
        /// </summary>
        /// <param name="file">圖片檔案(webp/jpg/png,大小上限 1MB)</param>
        [HttpPost("upload/avatar")]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponseDataModel<FileUploadResponse>))]
        public async Task<IActionResult> UploadAvatarAsync(IFormFile file)
        {
            return Ok(await _fileService.UploadAvatarAsync(file));
        }
    }
}
