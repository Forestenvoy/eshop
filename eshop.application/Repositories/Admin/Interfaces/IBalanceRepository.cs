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

        /// <summary>
        /// 查詢會員餘額
        /// </summary>
        Task<Balance?> GetAsync(long userId);

        /// <summary>
        /// 增加餘額(充值)
        /// </summary>
        Task<int> IncreaseAsync(long userId, decimal amount);

        /// <summary>
        /// 扣減餘額(原子操作,餘額不足時不會扣減)
        /// </summary>
        /// <returns>影響筆數,0 代表餘額不足</returns>
        Task<int> DecreaseIfSufficientAsync(long userId, decimal amount);
    }
}
