using eshop.application.Common;
using eshop.application.Common.Models;
using eshop.application.Configurations;
using eshop.application.DTOs.User.Request;
using eshop.application.DTOs.User.Response;
using eshop.application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace eshop.application.Controllers.Admin
{
    /// <summary>
    /// 會員管理
    /// </summary>
    [Route("auth")]
    [Authorize(Policy = AuthConstants.Policy.AdminOnly)]
    [ApiExplorerSettings(GroupName = SwaggerConst.Admin)]
    public class AuthController : BaseApiController
    {
        private readonly UserService _userService;

        public AuthController(ILogger<AuthController> logger, UserService userService)
            : base(logger)
        {
            _userService = userService;
        }

        /// <summary>
        /// 查詢會員分頁列表
        /// </summary>
        /// <param name="keyword">關鍵字(比對名稱、Email)</param>
        /// <param name="pageIndex">頁碼</param>
        /// <param name="pageSize">筆數</param>
        [HttpGet("list")]
        [Authorize(Policy = AuthConstants.Permission.UserView)]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponsePagingDataModel<UserResponse>))]
        public async Task<IActionResult> GetListAsync(
            [FromQuery] string? keyword,
            [Required][FromQuery][Range(1, int.MaxValue)] int pageIndex,
            [Required][FromQuery][Range(1, 100)] int pageSize)
        {
            return Ok(await _userService.GetListAsync(keyword, pageIndex, pageSize));
        }

        /// <summary>
        /// 查詢單一會員資料
        /// </summary>
        /// <param name="id">會員 ID</param>
        [HttpGet]
        [Authorize(Policy = AuthConstants.Permission.UserView)]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponseDataModel<UserResponse>))]
        public async Task<IActionResult> GetAsync([Required][FromQuery] long id)
        {
            return Ok(await _userService.GetAsync(id));
        }

        /// <summary>
        /// 新增會員
        /// </summary>
        [Authorize(Policy = AuthConstants.Permission.UserEdit)]
        [HttpPost("create")]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponseModel))]
        public async Task<IActionResult> CreateAsync([FromBody] UserAddRequest request)
        {
            return Ok(await _userService.AddAsync(request));
        }

        /// <summary>
        /// 編輯會員
        /// </summary>
        [Authorize(Policy = AuthConstants.Permission.UserEdit)]
        [HttpPost("update")]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponseModel))]
        public async Task<IActionResult> UpdateAsync([FromBody] UserUpdateRequest request)
        {
            return Ok(await _userService.UpdateAsync(request));
        }

        /// <summary>
        /// 啟用/停權會員
        /// </summary>
        [Authorize(Policy = AuthConstants.Permission.UserEdit)]
        [HttpPost("toggle")]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponseModel))]
        public async Task<IActionResult> ToggleAsync([FromBody] UserToggleRequest request)
        {
            return Ok(await _userService.ToggleAsync(request));
        }

        /// <summary>
        /// 刪除會員(軟刪除)
        /// </summary>
        [Authorize(Policy = AuthConstants.Permission.UserEdit)]
        [HttpPost("delete")]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponseModel))]
        public async Task<IActionResult> DeleteAsync([FromBody] UserDeleteRequest request)
        {
            return Ok(await _userService.DeleteAsync(request));
        }
    }
}
