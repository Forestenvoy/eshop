using eshop.application.Enums;
using eshop.application.Models;

namespace eshop.application.Repositories.Admin.Interfaces
{
    /// <summary>
    /// 會員資料存取介面
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// 查詢會員(排除已軟刪除)
        /// </summary>
        Task<User?> GetAsync(long id);

        /// <summary>
        /// 依 Email 查詢會員(不過濾狀態,供登入/註冊查重使用)
        /// </summary>
        Task<User?> GetAsync(string email);

        /// <summary>
        /// 取得會員總數
        /// </summary>
        /// <param name="keyword">關鍵字(比對名稱、Email)</param>
        Task<int> GetCountAsync(string? keyword);

        /// <summary>
        /// 取得會員分頁列表
        /// </summary>
        Task<IEnumerable<User>> GetPagedListAsync(string? keyword, int pageIndex, int pageSize);

        /// <summary>
        /// 檢查 Email 是否已存在
        /// </summary>
        Task<bool> ExistsByEmailAsync(string email);

        /// <summary>
        /// 新增會員,回傳新增後的會員 ID
        /// </summary>
        Task<long> AddAsync(User user);

        /// <summary>
        /// 更新會員基本資料(名稱、性別、電話、生日、地址)
        /// </summary>
        Task<int> UpdateProfileAsync(User user);

        /// <summary>
        /// 更新密碼
        /// </summary>
        Task<int> UpdatePasswordAsync(long id, string hashedPassword);

        /// <summary>
        /// 切換啟用/停權狀態
        /// </summary>
        Task<int> ToggleStatusAsync(long id, UserStatus status);

        /// <summary>
        /// 批次軟刪除
        /// </summary>
        Task<int> SoftDeleteRangeAsync(List<long> ids);

        /// <summary>
        /// 更新最後登入時間
        /// </summary>
        Task<int> UpdateLastLoginAsync(long id);
    }
}
