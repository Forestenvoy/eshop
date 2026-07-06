using eshop.application.Common;
using eshop.application.Configurations;
using eshop.application.DTO.Requests.Role;
using eshop.application.DTO.Responses.Role;
using eshop.application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace eshop.application.Controllers
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
        /// 角色列表
        /// </summary>
        [Authorize(Policy = AuthConstants.PermissionClaim.RoleView)]
        [HttpGet("list")]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponsePagingDataModel<RoleListResponse>))]
        public async Task<IActionResult> ListAsync(
            [FromQuery] string? keyword,
            [Required][FromQuery][Range(1, 999999)] int pageIndex,
            [Required][FromQuery][Range(1, 100)] int pageSize)
        {
            return Ok(await _roleService.GetListAsync(keyword, pageIndex, pageSize));
        }

        /// <summary>
        /// 所有權限列表
        /// </summary>
        [Authorize(Policy = AuthConstants.PermissionClaim.RoleView)]
        [HttpGet("permissions")]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponseDataModel<List<PermissionListResponse>>))]
        public async Task<IActionResult> PermissionsAsync()
        {
            return Ok(await _roleService.GetPermissionListAsync());
        }

        /// <summary>
        /// 角色名稱與擁有權限
        /// </summary>
        [Authorize(Policy = AuthConstants.PermissionClaim.RoleView)]
        [HttpGet("detail")]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponseDataModel<RoleResponse>))]
        public async Task<IActionResult> DetailAsync([FromQuery] int roleId)
        {
            return Ok(await _roleService.GetDetailAsync(roleId));
        }

        /// <summary>
        /// 新增角色
        /// </summary>
        [Authorize(Policy = AuthConstants.PermissionClaim.RoleEdit)]
        [HttpPost("create")]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponseModel))]
        public async Task<IActionResult> CreateAsync([FromBody] RoleAddRequest request)
        {
            return Ok(await _roleService.Add(GetAdminAccount(), request.RoleName, request.PermissionIds));
        }

        /// <summary>
        /// 修改角色
        /// </summary>
        [Authorize(Policy = AuthConstants.PermissionClaim.RoleEdit)]
        [HttpPost("update")]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponseModel))]
        public async Task<IActionResult> UpdateAsync([FromBody] RoleUpdateRequest request)
        {
            return Ok(await _roleService.Update(GetAdminAccount(), request.RoleId, request.RoleName, request.PermissionIds));
        }

        /// <summary>
        /// 刪除角色
        /// </summary>
        [Authorize(Policy = AuthConstants.PermissionClaim.RoleEdit)]
        [HttpPost("delete")]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponseModel))]
        public async Task<IActionResult> DeleteAsync([Required][FromBody] int roleId)
        {
            return Ok(await _roleService.DeleteRole(roleId));
        }
    }
}
