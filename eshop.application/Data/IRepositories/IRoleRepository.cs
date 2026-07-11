using eshop.application.DTO.Responses.Role;

namespace eshop.application.Data.IRepositories
{
    /// <summary>
    /// 角色與權限資料存取介面
    /// </summary>
    public interface IRoleRepository
    {
        /// <summary>
        /// 角色列表（分頁）
        /// </summary>
        Task<(List<RoleListResponse> list, int count)> GetRoleListAsync(string? keyword, int pageIndex, int pageSize);

        /// <summary>
        /// 角色名稱與擁有權限
        /// </summary>
        Task<RoleResponse?> GetRoleDetailAsync(int roleId);

        /// <summary>
        /// 新增角色
        /// </summary>
        Task AddAsync(string? account, string roleName, List<int> permissionIds);

        /// <summary>
        /// 所有權限列表
        /// </summary>
        Task<List<PermissionListResponse>> GetPermissionListAsync();

        /// <summary>
        /// 依帳號 ID 查詢其角色所擁有的權限代碼清單
        /// </summary>
        Task<List<string>> GetAdminIdRolePermissionListAsync(int adminId);

        /// <summary>
        /// 修改角色
        /// </summary>
        Task UpdateAsync(string account, int roleId, string roleName, List<int> permissionIds);

        /// <summary>
        /// 刪除角色
        /// </summary>
        Task DeleteRoleAsync(int roleId);

        /// <summary>
        /// 檢查角色是否存在
        /// </summary>
        Task<bool> CheckRoleExistsAsync(int roleId);
    }
}
