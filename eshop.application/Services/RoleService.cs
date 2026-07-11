using eshop.application.Common.Models;
using eshop.application.DTOs.Role.Request;
using eshop.application.DTOs.Role.Response;
using eshop.application.Models.Admin;
using eshop.application.Repositories.Admin.Interfaces;

namespace eshop.application.Services
{
    public class RoleService
    {
        private readonly ILogger<RoleService> _logger;
        private readonly IRoleRepository _roleRepository;

        public RoleService(ILogger<RoleService> logger, IRoleRepository roleRepository)
        {
            _logger = logger;
            _roleRepository = roleRepository;
        }

        /// <summary>
        /// 角色清單（下拉式選單用）
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseDataModel<List<SimpleResponse>>> GetOptionsAsync()
        {
            var roles = await _roleRepository.GetAllAsync();

            var responses = roles.Select(role => new SimpleResponse
            {
                RoleId = role.Id,
                Name = role.Name
            }).ToList();

            return ApiResponse.Success(responses);
        }

        /// <summary>
        /// 所有權限列表
        /// </summary>
        public async Task<ResponseDataModel<List<PermissionResponse>>> GetPermissionsAsync()
        {
            var permissions = await _roleRepository.GetAllPermissionsAsync();

            var responses = permissions.Select(permission => new PermissionResponse
            {
                Id = permission.Id,
                Code = permission.Code
            }).ToList();

            return ApiResponse.Success(responses);
        }

        /// <summary>
        /// 角色名稱與擁有權限
        /// </summary>
        public async Task<ResponseDataModel<DetailResponse>> GetAsync(int roleId)
        {
            var role = await _roleRepository.GetAsync(roleId);
            if (role == null)
            {
                return ApiResponse.Fail<DetailResponse>(ResponseCode.ROLE_NOT_EXISTS);
            }

            var permissionIds = await _roleRepository.GetPermissionIdsAsync(roleId);

            var response = new DetailResponse
            {
                RoleName = role.Name,
                PermissionIds = permissionIds
            };

            return ApiResponse.Success(response);
        }

        /// <summary>
        /// 角色列表（分頁）
        /// </summary>
        public async Task<ResponsePagingDataModel<RoleResponse>> GetListAsync(string? keyword, int pageIndex, int pageSize)
        {
            var count = await _roleRepository.GetCountAsync(keyword);
            var list = await _roleRepository.GetPagedListAsync(keyword, pageIndex, pageSize);

            var responses = list.Select(role => new RoleResponse
            {
                RoleId = role.Id,
                Name = role.Name,
                Modifier = role.Modifier,
                UpdatedAt = role.UpdatedAt
            });

            return ApiResponse.Paging(count, responses);
        }

        /// <summary>
        /// 新增角色
        /// </summary>
        public async Task<ResponseModel> CreateAsync(string adminName, AddRequest request)
        {
            if (await _roleRepository.ExistsByNameAsync(request.RoleName))
            {
                return ApiResponse.Fail(ResponseCode.ROLE_EXISTS);
            }

            var role = new Role
            {
                Name = request.RoleName,
                Modifier = adminName
            };

            await _roleRepository.AddAsync(role, request.PermissionIds.Distinct().ToList());

            // 管理日誌
            _logger.LogInformation("角色新增成功");

            return ApiResponse.Success();
        }

        /// <summary>
        /// 修改角色
        /// </summary>
        public async Task<ResponseModel> UpdateAsync(string adminName, UpdateRequest request)
        {
            var existingRole = await _roleRepository.GetAsync(request.RoleId);
            if (existingRole == null)
            {
                return ApiResponse.Fail(ResponseCode.ROLE_NOT_EXISTS);
            }

            var role = new Role
            {
                Id = request.RoleId,
                Name = string.IsNullOrWhiteSpace(request.RoleName) ? existingRole.Name : request.RoleName,
                Modifier = adminName
            };

            await _roleRepository.UpdateAsync(role, request.PermissionIds.Distinct().ToList());

            // 管理日誌
            _logger.LogInformation("角色編輯成功");

            return ApiResponse.Success();
        }

        /// <summary>
        /// 刪除角色
        /// </summary>
        public async Task<ResponseModel> DeleteAsync(DeleteRequest request)
        {
            var ids = request.Ids;

            if (ids == null || ids.Count == 0)
            {
                return ApiResponse.Success();
            }

            await _roleRepository.RemoveRangeAsync(ids);

            // 管理日誌
            _logger.LogInformation("角色刪除成功");

            return ApiResponse.Success();
        }
    }
}
