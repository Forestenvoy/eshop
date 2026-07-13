using eshop.application.Models;

namespace eshop.application.Repositories.Admin.Interfaces
{
    /// <summary>
    /// 角色與權限資料存取介面
    /// </summary>
    public interface IRoleRepository
    {
        /// <summary>
        /// 角色清單（不分頁）
        /// </summary>
        Task<IEnumerable<Role>> GetAllAsync();

        /// <summary>
        /// 所有權限列表
        /// </summary>
        Task<IEnumerable<Permission>> GetAllPermissionsAsync();

        /// <summary>
        /// 依 ID 查詢角色
        /// </summary>
        Task<Role?> GetAsync(int roleId);

        /// <summary>
        /// 查詢角色目前擁有的權限 ID 清單
        /// </summary>
        Task<List<int>> GetPermissionIdsAsync(int roleId);

        /// <summary>
        /// 取得角色總數
        /// </summary>
        /// <param name="keyword">關鍵字</param>
        Task<int> GetCountAsync(string? keyword);

        /// <summary>
        /// 取得角色分頁列表
        /// </summary>
        /// <param name="keyword">關鍵字</param>
        /// <param name="pageIndex">頁碼</param>
        /// <param name="pageSize">筆數</param>
        Task<IEnumerable<Role>> GetPagedListAsync(string? keyword, int pageIndex, int pageSize);

        /// <summary>
        /// 檢查角色是否存在
        /// </summary>
        Task<bool> ExistsAsync(int roleId);

        /// <summary>
        /// 檢查角色名稱是否已存在
        /// </summary>
        Task<bool> ExistsByNameAsync(string roleName);

        /// <summary>
        /// 新增角色及其權限對應
        /// </summary>
        /// <returns>新角色 ID</returns>
        Task<int> AddAsync(Role role, List<int> permissionIds);

        /// <summary>
        /// 更新角色及其權限對應
        /// </summary>
        Task<int> UpdateAsync(Role role, List<int> permissionIds);

        /// <summary>
        /// 批次刪除角色
        /// </summary>
        Task<int> RemoveRangeAsync(List<int> ids);

        /// <summary>
        /// 依管理員 ID 查詢其角色所擁有的權限代碼清單
        /// </summary>
        Task<List<string>> GetPermissionCodesByAdminAsync(int adminId);
    }
}
