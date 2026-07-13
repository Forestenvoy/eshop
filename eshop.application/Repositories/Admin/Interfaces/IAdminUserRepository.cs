using eshop.application.Models;

namespace eshop.application.Repositories.Admin.Interfaces
{
    /// <summary>
    /// 後台帳號資料存取介面
    /// </summary>
    public interface IAdminUserRepository
    {
        /// <summary>
        /// 查詢管理員
        /// </summary>
        /// <param name="account">帳號</param>
        /// <returns></returns>
        Task<AdminUser?> GetAsync(string account);

        /// <summary>
        /// 查詢管理員
        /// </summary>
        /// <param name="adminId">帳號</param>
        /// <returns></returns>
        Task<AdminUser?> GetAsync(int adminId);

        /// <summary>
        /// 取得管理員總數
        /// </summary>
        /// <param name="keyword">關鍵字</param>
        /// <returns>管理員數量</returns>
        Task<int> GetCountAsync(string? keyword);

        /// <summary>
        /// 取得管理員分頁列表
        /// </summary>
        /// <param name="keyword">關鍵字</param>
        /// <param name="pageIndex">頁碼</param>
        /// <param name="pageSize">筆數</param>
        /// <returns>管理員列表</returns>
        Task<IEnumerable<AdminUser>> GetPagedListAsync(string? keyword, int pageIndex, int pageSize);

        /// <summary>
        /// 檢查帳號是否已存在
        /// </summary>
        Task<bool> ExistsByAccountAsync(string account);

        /// <summary>
        /// 新增帳號
        /// </summary>
        Task<int> AddAsync(AdminUser adminUser);

        /// <summary>
        /// 更新帳號
        /// </summary>
        Task<int> UpdateAsync(AdminUser adminUser);

        /// <summary>
        /// 刪除帳號
        /// </summary>
        Task<int> RemoveRangeAsync(List<int> ids);
    }
}
