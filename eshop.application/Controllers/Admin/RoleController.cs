using eshop.application.Common;
using eshop.application.Common.Models;
using eshop.application.Configurations;
using eshop.application.DTOs.Role.Request;
using eshop.application.DTOs.Role.Response;
using eshop.application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace eshop.application.Controllers.Admin
{
    /// <summary>
    /// 角色權限管理
    /// </summary>
    [Route("role")]
    [Authorize(Policy = AuthConstants.Policy.AdminOnly)]
    [ApiExplorerSettings(GroupName = SwaggerConst.Admin)]
    public class RoleController : BaseApiController
    {
        private readonly RoleService _roleService;

        public RoleController(ILogger<RoleController> logger, RoleService roleService)
            : base(logger)
        {
            _roleService = roleService;
        }

        /// <summary>
        /// 角色清單（下拉式選單用）
        /// </summary>
        [Authorize(Policy = AuthConstants.Permission.AdminView)]
        [HttpGet("options")]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponseDataModel<List<SimpleResponse>>))]
        public async Task<IActionResult> GetOptionsAsync()
        {
            return Ok(await _roleService.GetOptionsAsync());
        }

        /// <summary>
        /// 所有權限列表
        /// </summary>
        [Authorize(Policy = AuthConstants.Permission.RoleView)]
        [HttpGet("permissions")]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponseDataModel<List<PermissionResponse>>))]
        public async Task<IActionResult> PermissionsAsync()
        {
            return Ok(await _roleService.GetPermissionsAsync());
        }

        /// <summary>
        /// 角色名稱與擁有權限
        /// </summary>
        [Authorize(Policy = AuthConstants.Permission.RoleView)]
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponseDataModel<DetailResponse>))]
        public async Task<IActionResult> GetAsync([FromQuery] int roleId)
        {
            return Ok(await _roleService.GetAsync(roleId));
        }

        /// <summary>
        /// 角色列表
        /// </summary>
        [Authorize(Policy = AuthConstants.Permission.RoleView)]
        [HttpGet("list")]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponsePagingDataModel<RoleResponse>))]
        public async Task<IActionResult> ListAsync(
            [FromQuery] string? keyword,
            [Required][FromQuery][Range(1, 999999)] int pageIndex,
            [Required][FromQuery][Range(1, 100)] int pageSize)
        {
            return Ok(await _roleService.GetListAsync(keyword, pageIndex, pageSize));
        }

        /// <summary>
        /// 新增角色
        /// </summary>
        [Authorize(Policy = AuthConstants.Permission.RoleEdit)]
        [HttpPost("create")]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponseModel))]
        public async Task<IActionResult> CreateAsync([FromBody] AddRequest request)
        {
            return Ok(await _roleService.CreateAsync(GetAdminName(), request));
        }

        /// <summary>
        /// 修改角色
        /// </summary>
        [Authorize(Policy = AuthConstants.Permission.RoleEdit)]
        [HttpPost("update")]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponseModel))]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateRequest request)
        {
            return Ok(await _roleService.UpdateAsync(GetAdminName(), request));
        }

        /// <summary>
        /// 刪除角色
        /// </summary>
        [Authorize(Policy = AuthConstants.Permission.RoleEdit)]
        [HttpPost("delete")]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponseModel))]
        public async Task<IActionResult> DeleteAsync([FromBody] DeleteRequest request)
        {
            return Ok(await _roleService.DeleteAsync(request));
        }
    }
}
