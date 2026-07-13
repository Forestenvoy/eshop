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

namespace eshop.application.Controllers.Admin
{
    /// <summary>
    /// 訂單管理
    /// </summary>
    [Route("order")]
    [Authorize(Policy = AuthConstants.Policy.AdminOnly)]
    [ApiExplorerSettings(GroupName = SwaggerConst.Admin)]
    public class OrderController : BaseApiController
    {
        private readonly OrderService _orderService;

        public OrderController(ILogger<OrderController> logger, OrderService orderService)
            : base(logger)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// 查看訂單列表
        /// </summary>
        /// <param name="keyword">關鍵字(訂單編號、收件人姓名)</param>
        /// <param name="status">訂單狀態</param>
        /// <param name="pageIndex">頁碼</param>
        /// <param name="pageSize">筆數</param>
        [HttpGet("list")]
        [Authorize(Policy = AuthConstants.Permission.OrderView)]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponsePagingDataModel<OrderResponse>))]
        public async Task<IActionResult> GetListAsync(
            [FromQuery] string? keyword,
            [FromQuery] OrderStatus? status,
            [Required][FromQuery][Range(1, int.MaxValue)] int pageIndex,
            [Required][FromQuery][Range(1, 100)] int pageSize)
        {
            return Ok(await _orderService.GetListAsync(keyword, status, pageIndex, pageSize));
        }

        /// <summary>
        /// 查看單一訂單(含商品明細)
        /// </summary>
        /// <param name="id">訂單 ID</param>
        [HttpGet]
        [Authorize(Policy = AuthConstants.Permission.OrderView)]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponseDataModel<OrderDetailResponse>))]
        public async Task<IActionResult> GetAsync([Required][FromQuery] long id)
        {
            return Ok(await _orderService.GetAsync(id));
        }

        /// <summary>
        /// 訂單出貨
        /// </summary>
        [Authorize(Policy = AuthConstants.Permission.OrderEdit)]
        [HttpPost("ship")]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponseModel))]
        public async Task<IActionResult> ShipAsync([FromBody] OrderStatusChangeRequest request)
        {
            return Ok(await _orderService.ShipAsync(request.OrderId));
        }

        /// <summary>
        /// 訂單完成
        /// </summary>
        [Authorize(Policy = AuthConstants.Permission.OrderEdit)]
        [HttpPost("complete")]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponseModel))]
        public async Task<IActionResult> CompleteAsync([FromBody] OrderStatusChangeRequest request)
        {
            return Ok(await _orderService.CompleteAsync(request.OrderId));
        }

        /// <summary>
        /// 取消訂單
        /// </summary>
        [Authorize(Policy = AuthConstants.Permission.OrderEdit)]
        [HttpPost("cancel")]
        [SwaggerResponse(StatusCodes.Status200OK, "成功", typeof(ResponseModel))]
        public async Task<IActionResult> CancelAsync([FromBody] OrderStatusChangeRequest request)
        {
            return Ok(await _orderService.CancelAsync(request.OrderId));
        }
    }
}
