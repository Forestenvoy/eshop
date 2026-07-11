using eshop.application.Common;
using eshop.application.Common.Models;
using eshop.application.Configurations;
using eshop.application.DTOs.Admin.Request;
using eshop.application.DTOs.Admin.Response;
using eshop.application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace eshop.application.Controllers.Admin
{
    /// <summary>
    /// 管理員管理
    /// </summary>
    [Route("admin")]
    [Authorize(Policy = AuthConstants.Policy.AdminOnly)]
    [ApiExplorerSettings(GroupName = SwaggerConst.Admin)]
    public class AdminController : BaseApiController
    {
        private readonly AdminService _adminService;

        public AdminController(ILogger<AdminController> logger, AdminService adminService)
            : base(logger)
        {
            _adminService = adminService;
        }

        /// <summary>
        /// 管理員登入
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponseDataModel<LoginResponse>))]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
        {
            return Ok(await _adminService.LoginAsync(request));
        }

        /// <summary>
        /// 查詢已登入管理員資訊
        /// </summary>
        [HttpGet("info")]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponseDataModel<InfoResponse>))]
        public async Task<IActionResult> GetInfoAsync()
        {
            return Ok(await _adminService.GetInfoAsync(GetAdminId()));
        }

        /// <summary>
        /// 查詢單一管理員資料
        /// </summary>
        /// <param name="id">管理員 ID</param>
        [HttpGet]
        [Authorize(Policy = AuthConstants.Permission.AdminView)]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponseDataModel<AdminUserResponse>))]
        public async Task<IActionResult> GetAsync([Required][FromQuery] int id)
        {
            return Ok(await _adminService.GetAsync(id));
        }

        /// <summary>
        /// 查詢管理員分頁列表
        /// </summary>
        /// <param name="keyword">關鍵字</param>
        /// <param name="pageIndex">頁碼</param>
        /// <param name="pageSize">筆數</param>
        [HttpGet("list")]
        [Authorize(Policy = AuthConstants.Permission.AdminView)]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponsePagingDataModel<AdminUserResponse>))]
        public async Task<IActionResult> GetListAsync(
            [FromQuery] string? keyword,
            [Required][FromQuery][Range(1, int.MaxValue)] int pageIndex,
            [Required][FromQuery][Range(1, 100)] int pageSize)
        {
            return Ok(await _adminService.GetListAsync(keyword, pageIndex, pageSize));
        }

        /// <summary>
        /// 新增帳號
        /// </summary>
        [Authorize(Policy = AuthConstants.Permission.AdminEdit)]
        [HttpPost("create")]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponseModel))]
        public async Task<IActionResult> CreateAsync([FromBody] AddRequest request)
        {
            return Ok(await _adminService.AddAsync(GetAdminName(), request));
        }

        /// <summary>
        /// 編輯帳號
        /// </summary>
        [Authorize(Policy = AuthConstants.Permission.AdminEdit)]
        [HttpPost("update")]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponseModel))]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateRequest request)
        {
            return Ok(await _adminService.UpdateAsync(GetAdminName(), request));
        }

        /// <summary>
        /// 刪除帳號
        /// </summary>
        [Authorize(Policy = AuthConstants.Permission.AdminEdit)]
        [HttpPost("delete")]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponseModel))]
        public async Task<IActionResult> DeleteAsync([FromBody] DeleteRequest request)
        {
            return Ok(await _adminService.DeleteAsync(request));
        }
    }
}
