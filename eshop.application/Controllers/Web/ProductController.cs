using eshop.application.Common;
using eshop.application.Common.Models;
using eshop.application.Configurations;
using eshop.application.DTOs.Product.Response;
using eshop.application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace eshop.application.Controllers.Web
{
    /// <summary>
    /// 商品瀏覽(前台)
    /// </summary>
    [Route("web/product")]
    [AllowAnonymous]
    [ApiExplorerSettings(GroupName = SwaggerConst.Front)]
    public class ProductController : BaseApiController
    {
        private readonly ProductService _productService;

        public ProductController(ILogger<ProductController> logger, ProductService productService)
            : base(logger)
        {
            _productService = productService;
        }

        /// <summary>
        /// 查詢商品列表(僅上架商品)
        /// </summary>
        /// <param name="keyword">關鍵字</param>
        /// <param name="pageIndex">頁碼</param>
        /// <param name="pageSize">筆數</param>
        [HttpGet("list")]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponsePagingDataModel<ProductPublicResponse>))]
        public async Task<IActionResult> GetListAsync(
            [FromQuery] string? keyword,
            [Required][FromQuery][Range(1, int.MaxValue)] int pageIndex,
            [Required][FromQuery][Range(1, 100)] int pageSize)
        {
            return Ok(await _productService.GetPublicListAsync(keyword, pageIndex, pageSize));
        }

        /// <summary>
        /// 查詢單一商品
        /// </summary>
        /// <param name="id">商品 ID</param>
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponseDataModel<ProductResponse>))]
        public async Task<IActionResult> GetAsync([Required][FromQuery] long id)
        {
            return Ok(await _productService.GetPublicAsync(id));
        }
    }
}
