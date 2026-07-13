using eshop.application.Enums;
using eshop.application.Models;

namespace eshop.application.Repositories.Admin.Interfaces
{
    /// <summary>
    /// 訂單資料存取介面
    /// </summary>
    public interface IOrderRepository
    {
        /// <summary>
        /// 查詢單一訂單
        /// </summary>
        Task<Order?> GetAsync(long id);

        /// <summary>
        /// 取得訂單總數
        /// </summary>
        /// <param name="keyword">關鍵字(比對訂單編號、收件人姓名)</param>
        /// <param name="status">訂單狀態</param>
        Task<int> GetCountAsync(string? keyword, OrderStatus? status);

        /// <summary>
        /// 取得訂單分頁列表
        /// </summary>
        /// <param name="keyword">關鍵字(比對訂單編號、收件人姓名)</param>
        /// <param name="status">訂單狀態</param>
        /// <param name="pageIndex">頁碼</param>
        /// <param name="pageSize">筆數</param>
        Task<IEnumerable<Order>> GetPagedListAsync(string? keyword, OrderStatus? status, int pageIndex, int pageSize);

        /// <summary>
        /// 查詢訂單商品明細
        /// </summary>
        /// <param name="orderId">訂單 Id</param>
        Task<IEnumerable<OrderItem>> GetItemsAsync(long orderId);

        /// <summary>
        /// 更新訂單狀態
        /// </summary>
        /// <param name="id">訂單 Id</param>
        /// <param name="status">訂單狀態</param>
        /// <param name="paymentStatus">付款狀態,為 null 時不更新此欄位</param>
        Task<int> UpdateStatusAsync(long id, OrderStatus status, PaymentStatus? paymentStatus);

        /// <summary>
        /// 新增訂單
        /// </summary>
        /// <returns>新增訂單的 Id</returns>
        Task<long> AddAsync(Order order);

        /// <summary>
        /// 新增訂單商品明細(批次)
        /// </summary>
        Task<int> AddItemsAsync(IEnumerable<OrderItem> items);

        /// <summary>
        /// 取得會員本人的訂單總數
        /// </summary>
        /// <param name="userId">會員 Id</param>
        /// <param name="keyword">關鍵字(比對訂單編號、收件人姓名)</param>
        /// <param name="status">訂單狀態</param>
        Task<int> GetMemberCountAsync(long userId, string? keyword, OrderStatus? status);

        /// <summary>
        /// 取得會員本人的訂單分頁列表
        /// </summary>
        /// <param name="userId">會員 Id</param>
        /// <param name="keyword">關鍵字(比對訂單編號、收件人姓名)</param>
        /// <param name="status">訂單狀態</param>
        /// <param name="pageIndex">頁碼</param>
        /// <param name="pageSize">筆數</param>
        Task<IEnumerable<Order>> GetMemberPagedListAsync(long userId, string? keyword, OrderStatus? status, int pageIndex, int pageSize);
    }
}
