using eshop.application.Common;
using eshop.application.Common.Models;
using eshop.application.Configurations;
using eshop.application.DTOs.Product.Request;
using eshop.application.DTOs.Product.Response;
using eshop.application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace eshop.application.Controllers.Admin
{
    /// <summary>
    /// 商品管理
    /// </summary>
    [Route("product")]
    [Authorize(Policy = AuthConstants.Policy.AdminOnly)]
    [ApiExplorerSettings(GroupName = SwaggerConst.Admin)]
    public class ProductController : BaseApiController
    {
        private readonly ProductService _productService;

        public ProductController(ILogger<ProductController> logger, ProductService productService)
            : base(logger)
        {
            _productService = productService;
        }

        /// <summary>
        /// 查看商品列表
        /// </summary>
        /// <param name="keyword">關鍵字</param>
        /// <param name="pageIndex">頁碼</param>
        /// <param name="pageSize">筆數</param>
        [HttpGet("list")]
        [Authorize(Policy = AuthConstants.Permission.ProductView)]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponsePagingDataModel<ProductResponse>))]
        public async Task<IActionResult> GetListAsync(
            [FromQuery] string? keyword,
            [Required][FromQuery][Range(1, int.MaxValue)] int pageIndex,
            [Required][FromQuery][Range(1, 100)] int pageSize)
        {
            return Ok(await _productService.GetListAsync(keyword, pageIndex, pageSize));
        }

        /// <summary>
        /// 查看單一商品
        /// </summary>
        /// <param name="id">商品 ID</param>
        [HttpGet]
        [Authorize(Policy = AuthConstants.Permission.ProductView)]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponseDataModel<ProductResponse>))]
        public async Task<IActionResult> GetAsync([Required][FromQuery] long id)
        {
            return Ok(await _productService.GetAsync(id));
        }

        /// <summary>
        /// 新增商品
        /// </summary>
        [Authorize(Policy = AuthConstants.Permission.ProductEdit)]
        [HttpPost("create")]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponseModel))]
        public async Task<IActionResult> CreateAsync([FromBody] ProductAddRequest request)
        {
            return Ok(await _productService.AddAsync(GetAdminName(), request));
        }

        /// <summary>
        /// 編輯商品
        /// </summary>
        [Authorize(Policy = AuthConstants.Permission.ProductEdit)]
        [HttpPost("update")]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponseModel))]
        public async Task<IActionResult> UpdateAsync([FromBody] ProductUpdateRequest request)
        {
            return Ok(await _productService.UpdateAsync(GetAdminName(), request));
        }

        /// <summary>
        /// 上下架商品
        /// </summary>
        [Authorize(Policy = AuthConstants.Permission.ProductEdit)]
        [HttpPost("toggle")]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponseModel))]
        public async Task<IActionResult> ToggleAsync([FromBody] ProductToggleRequest request)
        {
            return Ok(await _productService.ToggleAsync(GetAdminName(), request));
        }

        /// <summary>
        /// 批次更新商品排序(拖曳排序 + 保存用)
        /// </summary>
        /// <param name="sort">Key 為商品 ID,Value 為排序權重</param>
        [Authorize(Policy = AuthConstants.Permission.ProductEdit)]
        [HttpPost("sort")]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponseModel))]
        public async Task<IActionResult> SaveSortAsync([FromBody] Dictionary<long, int> sort)
        {
            return Ok(await _productService.SaveSortAsync(sort));
        }
    }
}
