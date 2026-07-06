using eshop.application.Common;
using eshop.application.Data.Repositories;
using eshop.application.DTO.Responses.Role;

namespace eshop.application.Services
{
    public class RoleService
    {
        private readonly ILogger<RoleService> _logger;
        private readonly RoleRepository _roleRepository;

        public RoleService(ILogger<RoleService> logger, RoleRepository roleRepository)
        {
            _logger = logger;
            _roleRepository = roleRepository;
        }

        public async Task<ResponsePagingDataModel<RoleListResponse>> GetListAsync(string? keyword, int pageIndex, int pageSize)
        {
            var roleList = await _roleRepository.GetRoleListAsync(keyword, pageIndex, pageSize);
            return new(roleList.count, roleList.list);
        }

        public async Task<ResponseDataModel<RoleResponse?>> GetDetailAsync(int roleId)
        {
            var roleDetail = await _roleRepository.GetRoleDetailAsync(roleId);
            return new(roleDetail);
        }

        public async Task<ResponseModel> Add(string? account, string roleName, List<int> permissionIds)
        {
            if (string.IsNullOrWhiteSpace(account))
            {
                return new(ResponseCode.ADMIN_NOT_EXISTS);
            }

            await _roleRepository.AddAsync(account, roleName, permissionIds);
            return new(ResponseCode.SUCCESS);
        }

        public async Task<ResponseModel> Update(string? account, int roleId, string roleName, List<int> permissionIds)
        {
            if (string.IsNullOrWhiteSpace(account))
            {
                return new(ResponseCode.ADMIN_NOT_EXISTS);
            }

            if (!await _roleRepository.CheckRoleExistsAsync(roleId))
            {
                return new(ResponseCode.ROLE_NOT_EXISTS);
            }

            await _roleRepository.UpdateAsync(account, roleId, roleName, permissionIds);
            return new(ResponseCode.SUCCESS);
        }

        public async Task<ResponseModel> DeleteRole(int roleId)
        {
            if (!await _roleRepository.CheckRoleExistsAsync(roleId))
            {
                return new(ResponseCode.SUCCESS);
            }

            await _roleRepository.DeleteRoleAsync(roleId);
            return new(ResponseCode.SUCCESS);
        }

        public async Task<ResponseDataModel<List<PermissionListResponse>>> GetPermissionListAsync()
        {
            var permissionList = await _roleRepository.GetPermissionListAsync();
            return new(permissionList);
        }
    }
}
