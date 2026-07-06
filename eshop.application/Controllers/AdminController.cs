using eshop.application.Common;
using eshop.application.Configurations;
using eshop.application.DTO.Requests.Admin;
using eshop.application.DTO.Responses.Admin;
using eshop.application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace eshop.application.Controllers
{
    /// <summary>
    /// 後台帳號管理
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
        /// 後台登入
        /// </summary>
        [AllowAnonymous]
        [HttpPost("login")]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponseDataModel<string>))]
        public async Task<IActionResult> LoginAsync([FromBody] AdminLoginRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Account) || string.IsNullOrWhiteSpace(req.Password))
            {
                return Ok(new ResponseDataModel<string>(ResponseCode.BAD_PARAMS));
            }

            return Ok(await _adminService.AdminLoginAsync(req.Account, req.Password));
        }

        /// <summary>
        /// 目前登入者的管理員資訊
        /// </summary>
        [HttpGet("info")]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponseDataModel<AdminInfoResponse>))]
        public async Task<IActionResult> InfoAsync()
        {
            return Ok(await _adminService.AdminInfoAsync(GetAdminAccount()));
        }

        /// <summary>
        /// 帳號管理列表
        /// </summary>
        /// <param name="keyword">帳號 關鍵字</param>
        /// <param name="pageIndex">頁碼</param>
        /// <param name="pageSize">筆數</param>
        [Authorize(Policy = AuthConstants.PermissionClaim.AdminView)]
        [HttpGet("list")]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponsePagingDataModel<AdminListResponse>))]
        public async Task<IActionResult> ListAsync(
            [FromQuery] string? keyword,
            [Required][FromQuery][Range(1, 999999)] int pageIndex,
            [Required][FromQuery][Range(1, 100)] int pageSize)
        {
            return Ok(await _adminService.GetAllAsync(keyword, pageIndex, pageSize));
        }

        /// <summary>
        /// 取得單一帳號資料
        /// </summary>
        /// <param name="adminId">帳號 ID</param>
        [Authorize(Policy = AuthConstants.PermissionClaim.AdminView)]
        [HttpGet("detail")]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponseDataModel<AdminResponse>))]
        public async Task<IActionResult> DetailAsync([Required][FromQuery] int adminId)
        {
            return Ok(await _adminService.GetAsync(adminId));
        }

        /// <summary>
        /// 角色清單（下拉式選單用）
        /// </summary>
        [Authorize(Policy = AuthConstants.PermissionClaim.AdminView)]
        [HttpGet("roles")]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponseDataModel<List<RoleIdNameResponse>>))]
        public async Task<IActionResult> RolesAsync()
        {
            return Ok(await _adminService.GetRoleListAsync());
        }

        /// <summary>
        /// 新增帳號
        /// </summary>
        [Authorize(Policy = AuthConstants.PermissionClaim.AdminEdit)]
        [HttpPost("create")]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponseModel))]
        public async Task<IActionResult> CreateAsync([FromBody] AdminAddRequest request)
        {
            return Ok(await _adminService.AddAsync(request, GetAdminAccount()));
        }

        /// <summary>
        /// 編輯帳號
        /// </summary>
        [Authorize(Policy = AuthConstants.PermissionClaim.AdminEdit)]
        [HttpPost("update")]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponseModel))]
        public async Task<IActionResult> UpdateAsync([FromBody] AdminUpdateRequest request)
        {
            return Ok(await _adminService.UpdateAsync(request, GetAdminAccount()));
        }

        /// <summary>
        /// 刪除帳號
        /// </summary>
        [Authorize(Policy = AuthConstants.PermissionClaim.AdminEdit)]
        [HttpPost("delete")]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponseModel))]
        public async Task<IActionResult> DeleteAsync([Required][FromBody] int adminId)
        {
            return Ok(await _adminService.DeleteAsync(adminId));
        }
    }
}
