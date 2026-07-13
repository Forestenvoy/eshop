using Coravel.Invocable;
using eshop.application.Services;

namespace eshop.application.Tasks
{
    /// <summary>
    /// 定期刷新前台商品快取
    /// </summary>
    public class RefreshProductCacheTask : IInvocable
    {
        private readonly ILogger<RefreshProductCacheTask> _logger;
        private readonly ProductService _productService;

        public RefreshProductCacheTask(ILogger<RefreshProductCacheTask> logger, ProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }

        /// <inheritdoc />
        public async Task Invoke()
        {
            try
            {
                await _productService.RefreshCacheAsync();
                _logger.LogInformation("商品快取已刷新");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "商品快取刷新失敗: {ErrorMessage}", ex.Message);
            }
        }
    }
}
