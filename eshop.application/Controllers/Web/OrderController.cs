using eshop.application.Common;
using eshop.application.Common.Models;
using eshop.application.Configurations;
using eshop.application.DTOs.Order.Request;
using eshop.application.DTOs.Order.Response;
using eshop.application.Enums;
using eshop.application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace eshop.application.Controllers.Web
{
    /// <summary>
    /// 訂單(前台)
    /// </summary>
    [Route("web/order")]
    [Authorize(Policy = AuthConstants.Policy.WebOnly)]
    [ApiExplorerSettings(GroupName = SwaggerConst.Front)]
    public class OrderController : BaseApiController
    {
        private readonly OrderService _orderService;

        public OrderController(ILogger<OrderController> logger, OrderService orderService)
            : base(logger)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// 查看我的訂單列表
        /// </summary>
        /// <param name="keyword">關鍵字(訂單編號、收件人姓名)</param>
        /// <param name="status">訂單狀態</param>
        /// <param name="pageIndex">頁碼</param>
        /// <param name="pageSize">筆數</param>
        [HttpGet("list")]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponsePagingDataModel<OrderResponse>))]
        public async Task<IActionResult> GetListAsync(
            [FromQuery] string? keyword,
            [FromQuery] OrderStatus? status,
            [Required][FromQuery][Range(1, int.MaxValue)] int pageIndex,
            [Required][FromQuery][Range(1, 100)] int pageSize)
        {
            return Ok(await _orderService.GetMemberListAsync(GetUserId(), keyword, status, pageIndex, pageSize));
        }

        /// <summary>
        /// 查看我的單一訂單(含商品明細)
        /// </summary>
        /// <param name="id">訂單 ID</param>
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponseDataModel<OrderDetailResponse>))]
        public async Task<IActionResult> GetAsync([Required][FromQuery] long id)
        {
            return Ok(await _orderService.GetMemberAsync(GetUserId(), id));
        }

        /// <summary>
        /// 送出訂單
        /// </summary>
        [HttpPost("submit")]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponseDataModel<OrderSubmitResponse>))]
        public async Task<IActionResult> SubmitAsync([FromBody] OrderSubmitRequest request)
        {
            return Ok(await _orderService.SubmitAsync(GetUserId(), request));
        }
    }
}
