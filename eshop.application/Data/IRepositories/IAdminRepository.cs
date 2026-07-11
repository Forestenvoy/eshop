using eshop.application.DTO.Responses.Admin;

namespace eshop.application.Data.IRepositories
{
    /// <summary>
    /// 後台帳號資料存取介面
    /// </summary>
    public interface IAdminRepository
    {
        /// <summary>
        /// 依帳號查詢登入所需資訊（密碼為雜湊值，驗證交給呼叫端）
        /// </summary>
        Task<(int? Id, string? Password, bool? IsEnable)> GetAdminByAccountAsync(string account);

        /// <summary>
        /// 依帳號查詢帳號 ID
        /// </summary>
        Task<int?> GetAdminIdAsync(string account);

        /// <summary>
        /// 依帳號 ID 查詢角色 ID
        /// </summary>
        Task<int> GetAdminRoleIdAsync(int adminId);

        /// <summary>
        /// 帳號管理列表（分頁）
        /// </summary>
        Task<(List<AdminListResponse> list, int count)> GetAllAsync(string? keyword, int pageIndex, int pageSize);

        /// <summary>
        /// 角色清單（下拉式選單用）
        /// </summary>
        Task<List<RoleIdNameResponse>> GetRoleListAsync();

        /// <summary>
        /// 取得單一帳號資料
        /// </summary>
        Task<AdminResponse?> GetAsync(int adminId);

        /// <summary>
        /// 新增帳號
        /// </summary>
        /// <param name="account"></param>
        /// <param name="roleId"></param>
        /// <param name="hashedPassword">已雜湊過的密碼</param>
        /// <param name="adminAccount"></param>
        Task AddAsync(string account, int roleId, string hashedPassword, string adminAccount);

        /// <summary>
        /// 檢查帳號是否已存在
        /// </summary>
        Task<bool> CheckAdminAccountExistsAsync(string account);

        /// <summary>
        /// 檢查帳號 ID 是否存在
        /// </summary>
        Task<bool> CheckAdminIdExistsAsync(int adminId);

        /// <summary>
        /// 更新帳號
        /// </summary>
        /// <param name="id"></param>
        /// <param name="account"></param>
        /// <param name="roleId"></param>
        /// <param name="hashedPassword">已雜湊過的密碼，null/空白表示不更新密碼</param>
        /// <param name="isEnabled"></param>
        /// <param name="adminAccount"></param>
        Task UpdateAsync(
            int id,
            string account,
            int? roleId,
            string? hashedPassword,
            bool? isEnabled,
            string adminAccount);

        /// <summary>
        /// 刪除帳號
        /// </summary>
        Task DeleteAsync(int adminId);
    }
}
