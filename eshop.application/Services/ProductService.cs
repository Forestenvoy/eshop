using eshop.application.Common.Models;
using eshop.application.DTOs.Product.Request;
using eshop.application.DTOs.Product.Response;
using eshop.application.Models.Admin;
using eshop.application.Repositories.Admin.Interfaces;

namespace eshop.application.Services
{
    public class ProductService
    {
        private readonly ILogger<ProductService> _logger;
        private readonly IProductRepository _productRepository;

        public ProductService(ILogger<ProductService> logger, IProductRepository productRepository)
        {
            _logger = logger;
            _productRepository = productRepository;
        }

        /// <summary>
        /// 查詢商品分頁列表
        /// </summary>
        public async Task<ResponsePagingDataModel<ProductResponse>> GetListAsync(string? keyword, int pageIndex, int pageSize)
        {
            var count = await _productRepository.GetCountAsync(keyword);
            var list = await _productRepository.GetPagedListAsync(keyword, pageIndex, pageSize);

            var responses = list.Select(x => new ProductResponse
            {
                ProductId = x.Id,
                Name = x.Name,
                Description = x.Description,
                Price = x.Price,
                Stock = x.Stock,
                ImageUrl = x.ImageUrl,
                IsEnabled = x.IsEnabled,
                Sort = x.Sort,
                Modifier = x.Modifier,
                UpdatedAt = x.UpdatedAt,
            });

            return ApiResponse.Paging(count, responses);
        }

        /// <summary>
        /// 查詢單一商品
        /// </summary>
        public async Task<ResponseDataModel<ProductResponse>> GetAsync(long id)
        {
            var product = await _productRepository.GetAsync(id);
            if (product == null)
            {
                return ApiResponse.Fail<ProductResponse>(ResponseCode.PRODUCT_NOT_EXISTS);
            }

            var response = new ProductResponse
            {
                ProductId = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                ImageUrl = product.ImageUrl,
                IsEnabled = product.IsEnabled,
                Sort = product.Sort,
                Modifier = product.Modifier,
                UpdatedAt = product.UpdatedAt,
            };

            return ApiResponse.Success(response);
        }

        /// <summary>
        /// 新增商品
        /// </summary>
        public async Task<ResponseModel> AddAsync(string adminName, ProductAddRequest request)
        {
            // 排序不開放使用者填寫,新商品一律排在最前面(數值最大,搭配清單 sort DESC)
            var maxSort = await _productRepository.GetMaxSortAsync();

            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                Stock = request.Stock,
                ImageUrl = request.ImageUrl,
                IsEnabled = true,
                Sort = maxSort + 1,
                Modifier = adminName,
            };

            await _productRepository.AddAsync(product);

            _logger.LogInformation("商品新增成功");

            return ApiResponse.Success();
        }

        /// <summary>
        /// 編輯商品
        /// </summary>
        public async Task<ResponseModel> UpdateAsync(string adminName, ProductUpdateRequest request)
        {
            var existingProduct = await _productRepository.GetAsync(request.ProductId);
            if (existingProduct == null)
            {
                return ApiResponse.Fail(ResponseCode.PRODUCT_NOT_EXISTS);
            }

            var product = new Product
            {
                Id = request.ProductId,
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                Stock = request.Stock,
                ImageUrl = request.ImageUrl,
                // 上下架狀態由獨立的 ToggleAsync 負責,編輯不動用這個欄位
                IsEnabled = existingProduct.IsEnabled,
                // 排序改由拖曳排序 API(SaveSortAsync)專責調整,編輯不動用這個欄位
                Sort = existingProduct.Sort,
                Modifier = adminName,
            };

            await _productRepository.UpdateAsync(product);

            _logger.LogInformation("商品編輯成功");

            return ApiResponse.Success();
        }

        /// <summary>
        /// 上下架商品
        /// </summary>
        public async Task<ResponseModel> ToggleAsync(string adminName, ProductToggleRequest request)
        {
            var existingProduct = await _productRepository.GetAsync(request.ProductId);
            if (existingProduct == null)
            {
                return ApiResponse.Fail(ResponseCode.PRODUCT_NOT_EXISTS);
            }

            await _productRepository.ToggleAsync(request.ProductId, request.IsEnabled, adminName);

            _logger.LogInformation("商品上下架狀態更新成功");

            return ApiResponse.Success();
        }

        /// <summary>
        /// 查詢商品分頁列表(前台,僅顯示上架商品)
        /// </summary>
        public async Task<ResponsePagingDataModel<ProductPublicResponse>> GetPublicListAsync(string? keyword, int pageIndex, int pageSize)
        {
            var count = await _productRepository.GetEnabledCountAsync(keyword);
            var list = await _productRepository.GetEnabledPagedListAsync(keyword, pageIndex, pageSize);

            var responses = list.Select(x => new ProductPublicResponse
            {
                ProductId = x.Id,
                ImageUrl = x.ImageUrl,
                Name = x.Name,
                Price = x.Price,
                Stock = x.Stock,
            });

            return ApiResponse.Paging(count, responses);
        }

        /// <summary>
        /// 查詢單一商品(前台,下架商品視為不存在)
        /// </summary>
        public async Task<ResponseDataModel<ProductResponse>> GetPublicAsync(long id)
        {
            var product = await _productRepository.GetEnabledAsync(id);
            if (product == null)
            {
                return ApiResponse.Fail<ProductResponse>(ResponseCode.PRODUCT_NOT_EXISTS);
            }

            var response = new ProductResponse
            {
                ProductId = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                ImageUrl = product.ImageUrl,
                IsEnabled = product.IsEnabled,
                Sort = product.Sort,
                Modifier = product.Modifier,
                UpdatedAt = product.UpdatedAt,
            };

            return ApiResponse.Success(response);
        }

        /// <summary>
        /// 批次更新商品排序(拖曳排序用)
        /// </summary>
        public async Task<ResponseModel> SaveSortAsync(Dictionary<long, int> sort)
        {
            if (sort == null || sort.Count == 0)
            {
                return ApiResponse.Success();
            }

            await _productRepository.SaveSortAsync(sort);

            _logger.LogInformation("商品排序更新成功");

            return ApiResponse.Success();
        }
    }
}
