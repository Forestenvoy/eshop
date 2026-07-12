using eshop.application.Common;
using eshop.application.Common.Models;
using eshop.application.Configurations;
using eshop.application.DTOs.Auth.Request;
using eshop.application.DTOs.Auth.Response;
using eshop.application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace eshop.application.Controllers.Web
{
    /// <summary>
    /// 會員(前台)
    /// </summary>
    [Route("web/auth")]
    [Authorize(Policy = AuthConstants.Policy.WebOnly)]
    [ApiExplorerSettings(GroupName = SwaggerConst.Front)]
    public class AuthController : BaseApiController
    {
        private readonly AuthService _authService;

        public AuthController(ILogger<AuthController> logger, AuthService authService)
            : base(logger)
        {
            _authService = authService;
        }

        /// <summary>
        /// 會員註冊
        /// </summary>
        [HttpPost("register")]
        [AllowAnonymous]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponseModel))]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest request)
        {
            return Ok(await _authService.RegisterAsync(request));
        }

        /// <summary>
        /// 會員登入
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponseDataModel<LoginResponse>))]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
        {
            return Ok(await _authService.LoginAsync(request));
        }

        /// <summary>
        /// 查看個人資料
        /// </summary>
        [HttpGet("info")]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponseDataModel<UserProfileResponse>))]
        public async Task<IActionResult> GetInfoAsync()
        {
            return Ok(await _authService.GetProfileAsync(GetUserId()));
        }

        /// <summary>
        /// 修改個人資料
        /// </summary>
        [HttpPost("update")]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponseModel))]
        public async Task<IActionResult> UpdateProfileAsync([FromBody] UpdateProfileRequest request)
        {
            return Ok(await _authService.UpdateProfileAsync(GetUserId(), request));
        }

        /// <summary>
        /// 修改密碼
        /// </summary>
        [HttpPost("password")]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponseModel))]
        public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordRequest request)
        {
            return Ok(await _authService.ChangePasswordAsync(GetUserId(), request));
        }
    }
}
