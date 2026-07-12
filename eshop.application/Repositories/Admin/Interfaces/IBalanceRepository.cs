using eshop.application.Models;

namespace eshop.application.Repositories.Admin.Interfaces
{
    /// <summary>
    /// 會員餘額資料存取介面
    /// </summary>
    public interface IBalanceRepository
    {
        /// <summary>
        /// 新增餘額紀錄
        /// </summary>
        Task<int> AddAsync(Balance balance);
    }
}
