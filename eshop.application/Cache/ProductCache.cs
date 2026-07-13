using eshop.application.Models;
using Newtonsoft.Json;
using RedLockNet;
using StackExchange.Redis;

namespace eshop.application.Cache
{
    /// <summary>
    /// 前台商品快取(僅快取上架商品)
    /// </summary>
    public class ProductCache
    {
        private readonly IDatabase _redis;
        private readonly IDistributedLockFactory _lockFactory;

        private const string CacheKey = "PRODUCT:ENABLED_LIST";
        private const string LockKey = "PRODUCT:ENABLED_LIST:LOCK";

        private static readonly TimeSpan Expiry = TimeSpan.FromMinutes(15);

        public ProductCache(IDatabase redis, IDistributedLockFactory lockFactory)
        {
            _redis = redis;
            _lockFactory = lockFactory;
        }

        /// <summary>
        /// 取得快取的上架商品清單,cache miss 時用分散式鎖確保同一時間只有一個請求查 DB 重建快取,
        /// 避免多個請求同時 cache miss 而一起打 DB(cache 擊穿)
        /// </summary>
        public async Task<List<Product>> GetOrSetEnabledListAsync(Func<Task<List<Product>>> factory)
        {
            var cached = await GetEnabledListAsync();
            if (cached != null)
            {
                return cached;
            }

            await using var redLock = await _lockFactory.CreateLockAsync(
                LockKey,
                expiryTime: TimeSpan.FromSeconds(10),
                waitTime: TimeSpan.FromSeconds(3),
                retryTime: TimeSpan.FromMilliseconds(100));

            if (!redLock.IsAcquired)
            {
                // 沒搶到鎖:代表已經有其他請求在重建快取,等待逾時就直接查一次 DB 當保底,
                // 不寫入快取(避免跟鎖持有者互搶寫入),犧牲極少數請求多查一次 DB,換取不讓整批請求都卡住等待
                return await factory();
            }

            // 拿到鎖:double-check,可能等鎖的這段時間內,前一個持有鎖的請求已經把快取建好了
            cached = await GetEnabledListAsync();
            if (cached != null)
            {
                return cached;
            }

            var products = await factory();
            await SetEnabledListAsync(products);

            return products;
        }

        /// <summary>
        /// 取得快取的上架商品清單,快取不存在時回傳 null
        /// </summary>
        public async Task<List<Product>?> GetEnabledListAsync()
        {
            string? json = await _redis.StringGetAsync(CacheKey);
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            return JsonConvert.DeserializeObject<List<Product>>(json);
        }

        /// <summary>
        /// 寫入/覆寫上架商品清單快取
        /// </summary>
        public async Task SetEnabledListAsync(List<Product> products)
        {
            string json = JsonConvert.SerializeObject(products);
            await _redis.StringSetAsync(CacheKey, json, Expiry);
        }

        /// <summary>
        /// 移除上架商品清單快取
        /// </summary>
        public async Task RemoveAsync()
        {
            await _redis.KeyDeleteAsync(CacheKey);
        }
    }
}
