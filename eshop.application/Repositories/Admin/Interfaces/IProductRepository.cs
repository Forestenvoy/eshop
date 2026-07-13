using eshop.application.Models;

namespace eshop.application.Repositories.Admin.Interfaces
{
    /// <summary>
    /// 商品資料存取介面
    /// </summary>
    public interface IProductRepository
    {
        /// <summary>
        /// 查詢單一商品
        /// </summary>
        Task<Product?> GetAsync(long id);

        /// <summary>
        /// 取得商品總數
        /// </summary>
        /// <param name="keyword">關鍵字</param>
        Task<int> GetCountAsync(string? keyword);

        /// <summary>
        /// 取得商品分頁列表
        /// </summary>
        /// <param name="keyword">關鍵字</param>
        /// <param name="pageIndex">頁碼</param>
        /// <param name="pageSize">筆數</param>
        Task<IEnumerable<Product>> GetPagedListAsync(string? keyword, int pageIndex, int pageSize);

        /// <summary>
        /// 新增商品
        /// </summary>
        Task<int> AddAsync(Product product);

        /// <summary>
        /// 更新商品
        /// </summary>
        Task<int> UpdateAsync(Product product);

        /// <summary>
        /// 上下架商品
        /// </summary>
        Task<int> ToggleAsync(long id, bool isEnabled, string modifier);

        /// <summary>
        /// 取得目前最大排序權重
        /// </summary>
        Task<int> GetMaxSortAsync();

        /// <summary>
        /// 取得全部上架商品(不分頁,供快取重建使用)
        /// </summary>
        Task<IEnumerable<Product>> GetAllEnabledAsync();

        /// <summary>
        /// 扣減商品庫存(原子操作,庫存不足或商品已下架時不會扣減)
        /// </summary>
        /// <param name="productId">商品 ID</param>
        /// <param name="quantity">扣減數量</param>
        /// <returns>影響筆數,0 代表庫存不足或商品已下架</returns>
        Task<int> DecrementStockAsync(long productId, int quantity);

        /// <summary>
        /// 批次更新商品排序
        /// </summary>
        /// <param name="sortMap">Key 為商品 ID,Value 為排序權重</param>
        Task<int> SaveSortAsync(Dictionary<long, int> sortMap);
    }
}
