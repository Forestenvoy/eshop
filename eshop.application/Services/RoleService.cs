using eshop.application.Common;
using eshop.application.Data.IRepositories;
using eshop.application.DTO.Responses.Role;

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
        /// 角色列表（分頁）
        /// </summary>
        public async Task<ResponsePagingDataModel<RoleListResponse>> GetListAsync(string? keyword, int pageIndex, int pageSize)
        {
            var roleList = await _roleRepository.GetRoleListAsync(keyword, pageIndex, pageSize);
            return ApiResponse.Paging(roleList.count, roleList.list);
        }

        /// <summary>
        /// 角色名稱與擁有權限
        /// </summary>
        public async Task<ResponseDataModel<RoleResponse?>> GetDetailAsync(int roleId)
        {
            var roleDetail = await _roleRepository.GetRoleDetailAsync(roleId);
            return ApiResponse.Success(roleDetail);
        }

        /// <summary>
        /// 新增角色
        /// </summary>
        public async Task<ResponseModel> Add(string? account, string roleName, List<int> permissionIds)
        {
            if (string.IsNullOrWhiteSpace(account))
            {
                return ApiResponse.Fail(ResponseCode.ADMIN_NOT_EXISTS);
            }

            await _roleRepository.AddAsync(account, roleName, permissionIds);
            return ApiResponse.Success();
        }

        /// <summary>
        /// 修改角色
        /// </summary>
        public async Task<ResponseModel> Update(string? account, int roleId, string roleName, List<int> permissionIds)
        {
            if (string.IsNullOrWhiteSpace(account))
            {
                return ApiResponse.Fail(ResponseCode.ADMIN_NOT_EXISTS);
            }

            if (!await _roleRepository.CheckRoleExistsAsync(roleId))
            {
                return ApiResponse.Fail(ResponseCode.ROLE_NOT_EXISTS);
            }

            await _roleRepository.UpdateAsync(account, roleId, roleName, permissionIds);
            return ApiResponse.Success();
        }

        /// <summary>
        /// 刪除角色
        /// </summary>
        public async Task<ResponseModel> DeleteRole(int roleId)
        {
            if (!await _roleRepository.CheckRoleExistsAsync(roleId))
            {
                return ApiResponse.Success();
            }

            await _roleRepository.DeleteRoleAsync(roleId);
            return ApiResponse.Success();
        }

        /// <summary>
        /// 所有權限列表
        /// </summary>
        public async Task<ResponseDataModel<List<PermissionListResponse>>> GetPermissionListAsync()
        {
            var permissionList = await _roleRepository.GetPermissionListAsync();
            return ApiResponse.Success(permissionList);
        }
    }
}
