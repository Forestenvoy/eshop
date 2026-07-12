using eshop.application.Models.Admin;

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
        /// 查詢單一商品(僅限上架)
        /// </summary>
        Task<Product?> GetEnabledAsync(long id);

        /// <summary>
        /// 取得上架商品總數
        /// </summary>
        /// <param name="keyword">關鍵字</param>
        Task<int> GetEnabledCountAsync(string? keyword);

        /// <summary>
        /// 取得上架商品分頁列表
        /// </summary>
        /// <param name="keyword">關鍵字</param>
        /// <param name="pageIndex">頁碼</param>
        /// <param name="pageSize">筆數</param>
        Task<IEnumerable<Product>> GetEnabledPagedListAsync(string? keyword, int pageIndex, int pageSize);

        /// <summary>
        /// 取得目前最大排序權重
        /// </summary>
        Task<int> GetMaxSortAsync();

        /// <summary>
        /// 批次更新商品排序
        /// </summary>
        /// <param name="sortMap">Key 為商品 ID,Value 為排序權重</param>
        Task<int> SaveSortAsync(Dictionary<long, int> sortMap);
    }
}
